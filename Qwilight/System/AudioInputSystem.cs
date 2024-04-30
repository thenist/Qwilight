using Concentus;
using Concentus.Enums;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Qwilight.UIComponent;
using Qwilight.ViewModel;
using System.Buffers;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;

namespace Qwilight
{
    public sealed class AudioInputSystem : Model, IDisposable
    {
        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(AudioInputSystem));

        public static readonly string LoopWaveInID = Guid.NewGuid().ToString();

        public static readonly AudioInputSystem Instance = QwilightComponent.GetBuiltInData<AudioInputSystem>(nameof(AudioInputSystem));

        readonly WaveFormat _waveFormat = new(48000, 1);
        readonly Dictionary<string, BufferedWaveProvider> _waveData = new();
        readonly MixingWaveProvider32 _mwp32 = new();
        readonly IOpusEncoder _zipper;
        readonly IOpusDecoder _rawer;
        readonly byte[] _zippingData;
        readonly int _frameLength;
        int _zippingPosition;
        WasapiOut _wave;
        WasapiCapture _waveIn;
        bool _loopWaveIn;
        WaveValue? _waveInValue;
        WaveValue? _waveValue;

        public AudioInputSystem()
        {
            _frameLength = _waveFormat.SampleRate / 1000 * 20;
            _zippingData = new byte[2 * _frameLength];
            _zipper = OpusCodecFactory.CreateEncoder(_waveFormat.SampleRate, _waveFormat.Channels, OpusApplication.OPUS_APPLICATION_VOIP);
            _rawer = OpusCodecFactory.CreateDecoder(_waveFormat.SampleRate, _waveFormat.Channels);
            GetWaveInValues();
            GetWaveValues();
        }

        public double AudioInputValue { get; set; }

        public ObservableCollection<WaveValue> WaveInValues { get; } = new();

        public ObservableCollection<WaveValue> WaveValues { get; } = new();

        public bool LoopWaveIn
        {
            get => _loopWaveIn;

            set
            {
                if (SetProperty(ref _loopWaveIn, value, nameof(LoopWaveIn)))
                {
                    OnPropertyChanged(nameof(LoopWaveInText));
                    OnPropertyChanged(nameof(LoopWaveInPaint));
                }
            }
        }

        public string LoopWaveInText => LoopWaveIn ? LanguageSystem.Instance.LoopWaveInText : LanguageSystem.Instance.NotLoopWaveInText;

        public Brush LoopWaveInPaint => Paints.PointPaints[LoopWaveIn ? 1 : 0];

        public WaveValue? WaveInValue
        {
            get => _waveInValue;

            set
            {
                if (SetProperty(ref _waveInValue, value, nameof(WaveInValue)) && Configure.Instance.AudioInput && value.HasValue)
                {
                    SetWaveIn();
                }
            }
        }

        public WaveValue? WaveValue
        {
            get => _waveValue;

            set
            {
                if (SetProperty(ref _waveValue, value, nameof(WaveValue)) && Configure.Instance.AudioInput && value.HasValue)
                {
                    SetWave();
                }
            }
        }

        void SetWaveIn()
        {
            var waveInSystem = WaveInValue?.System;
            if (waveInSystem != null)
            {
                UIHandler.Instance.HandleParallel(() =>
                {
                    try
                    {
                        _waveIn = new(waveInSystem, false, 60)
                        {
                            WaveFormat = _waveFormat
                        };
                        _waveIn.DataAvailable += OnWaveIn;
                        _waveIn.StartRecording();
                    }
                    catch (Exception e)
                    {
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.WaveInFault, e.Message));
                    }
                });
            }
        }

        public void CloseWaveIn()
        {
            if (_waveIn != null)
            {
                using (_waveIn)
                {
                    _waveIn.DataAvailable -= OnWaveIn;
                    _waveIn.StopRecording();
                }
            }
        }

        void SetWave()
        {
            var waveSystem = WaveValue?.System;
            if (waveSystem != null)
            {
                UIHandler.Instance.HandleParallel(() =>
                {
                    try
                    {
                        _wave = new(waveSystem, AudioClientShareMode.Shared, true, 200);
                        _wave.Init(_mwp32);
                        _wave.Play();
                    }
                    catch (Exception e)
                    {
                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Fault, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.WaveFault, e.Message));
                    }
                });
            }
        }

        public void CloseWave()
        {
            if (_wave != null)
            {
                using (_wave)
                {
                    _wave.Stop();
                }
                _wave = null;
            }
        }

        public void GetWaveInValues()
        {
            if (Configure.Instance.AudioInput && Configure.Instance.WaveIn)
            {
                UIHandler.Instance.HandleParallel(() =>
                {
                    var waveInValues = new List<WaveValue>();
                    foreach (var targetSystem in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
                    {
                        waveInValues.Add(new WaveValue
                        {
                            System = targetSystem,
                            Name = targetSystem.FriendlyName
                        });
                    }
                    WaveInValues.Clear();
                    foreach (var waveInValue in waveInValues)
                    {
                        WaveInValues.Add(waveInValue);
                    }
                    WaveInValue = WaveInValues.Count > 0 ? WaveInValues.First() : null;
                });
            }
        }

        public void GetWaveValues()
        {
            if (Configure.Instance.AudioInput && Configure.Instance.Wave)
            {
                UIHandler.Instance.HandleParallel(() =>
                {
                    var waveValues = new List<WaveValue>();
                    foreach (var targetSystem in new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                    {
                        waveValues.Add(new WaveValue
                        {
                            System = targetSystem,
                            Name = targetSystem.FriendlyName
                        });
                    }
                    WaveValues.Clear();
                    foreach (var waveValue in waveValues)
                    {
                        WaveValues.Add(waveValue);
                    }
                    WaveValue = WaveValues.Count > 0 ? WaveValues.First() as WaveValue? : null;
                });
            }
        }

        void OnWaveIn(object sender, WaveInEventArgs e)
        {
            var rawData = e.Buffer;
            var rawDataLength = rawData.Length;
            if (rawDataLength > 0)
            {
                var audioInputValue = 0;
                for (var i = 0; i < rawDataLength; ++i)
                {
                    audioInputValue += rawData[i];
                }
                AudioInputValue = audioInputValue / rawDataLength;
            }
            else
            {
                AudioInputValue = 0;
            }
            if (AudioInputValue >= Configure.Instance.AudioInputValue)
            {
                for (var i = 0; i < e.BytesRecorded; ++i)
                {
                    _zippingData[_zippingPosition++] = rawData[i];
                    if (_zippingPosition == _zippingData.Length)
                    {
                        _zippingPosition = 0;

                        var zippingPosition = _zippingData.Length / 2;
                        var zippingData = ArrayPool<short>.Shared.Rent(zippingPosition);
                        try
                        {
                            for (var j = 0; j < zippingPosition; ++j)
                            {
                                zippingData[j] = (short)(_zippingData[j * 2] + (((int)_zippingData[(j * 2) + 1]) << 8));
                            }

                            var zippedData = ArrayPool<byte>.Shared.Rent(zippingPosition);
                            try
                            {
                                var zippedLength = _zipper.Encode(zippingData.AsSpan(0, zippingPosition), _frameLength, zippedData.AsSpan(0, zippedData.Length), zippedData.Length);
                                if (zippedLength > 0)
                                {
                                    ViewModels.Instance.SiteContainerValue.AudioInput(zippedData, zippedLength);
                                    if (LoopWaveIn)
                                    {
                                        Handle(LoopWaveInID, zippedData, zippedLength);
                                    }
                                }
                            }
                            finally
                            {
                                ArrayPool<byte>.Shared.Return(zippedData);
                            }
                        }
                        finally
                        {
                            ArrayPool<short>.Shared.Return(zippingData);
                        }
                    }
                }
            }
        }

        public void Handle(string avatarID, byte[] zippedData, int zippedLength)
        {
            if (Configure.Instance.AudioInput && Configure.Instance.Wave)
            {
                if (_wave != null)
                {
                    AudioSystem.Instance.LastHandledAudioInputMillis = Environment.TickCount64;
                    var rawData16 = ArrayPool<short>.Shared.Rent(16 * zippedLength);
                    var rawLength16 = _rawer.Decode(zippedData.AsSpan(0, zippedLength), rawData16.AsSpan(0, rawData16.Length), _frameLength);
                    try
                    {
                        if (!_waveData.TryGetValue(avatarID, out var waveData))
                        {
                            waveData = new(_waveFormat)
                            {
                                DiscardOnBufferOverflow = true
                            };
                            _mwp32.AddInputStream(new Wave16ToFloatProvider(waveData));
                            CloseWave();
                            SetWave();
                            _waveData[avatarID] = waveData;
                        }

                        var rawLength8 = rawLength16 * 2;
                        var rawData8 = ArrayPool<byte>.Shared.Rent(rawLength8);
                        try
                        {
                            for (var i = rawLength16 - 1; i >= 0; --i)
                            {
                                rawData8[i * 2] = (byte)(rawData16[i] & 255);
                                rawData8[i * 2 + 1] = (byte)((rawData16[i] >> 8) & 255);
                            }

                            waveData.AddSamples(rawData8, 0, rawLength8);
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(rawData8);
                        }
                    }
                    finally
                    {
                        ArrayPool<short>.Shared.Return(rawData16);
                    }
                }
            }
        }

        public void Dispose()
        {
            CloseWave();
            CloseWaveIn();
        }
    }
}