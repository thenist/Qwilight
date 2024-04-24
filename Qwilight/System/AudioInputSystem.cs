using NAudio.CoreAudioApi;
using NAudio.Wave;
#if X64
using OpusDotNet;
#endif
using Qwilight.UIComponent;
#if X64
using Qwilight.ViewModel;
using System.Buffers;
#endif
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

        readonly WaveFormat _waveFormat = new(48000, 2);
        readonly Dictionary<string, BufferedWaveProvider> _waveData = new();
        readonly MixingWaveProvider32 _m = new();
#if X64
        readonly OpusEncoder _zipComputer;
        readonly OpusDecoder _rawComputer;
        readonly byte[] _zippingData = new byte[11520];
        int _zippingPosition;
#endif
        WasapiOut _wave;
        WasapiCapture _waveIn;
        bool _loopWaveIn;
        WaveValue? _waveInValue;
        WaveValue? _waveValue;

        public AudioInputSystem()
        {
#if X64
            _zipComputer = new(Application.VoIP, _waveFormat.SampleRate, _waveFormat.Channels);
            _rawComputer = new(_waveFormat.SampleRate, _waveFormat.Channels);
#endif
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
                        _wave.Init(_m);
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
#if X64
                var zippedData = Array.Empty<byte>();
                var zippedLength = 0;
                for (var i = 0; i < e.BytesRecorded; ++i)
                {
                    _zippingData[_zippingPosition++] = rawData[i];
                    if (_zippingPosition == _zippingData.Length)
                    {
                        zippedData = ArrayPool<byte>.Shared.Rent(_zippingPosition);
                        zippedLength = _zipComputer.Encode(_zippingData, _zippingPosition, zippedData, zippedData.Length);
                        _zippingPosition = 0;
                    }
                }

                try
                {
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
#endif
            }
        }

        public void Handle(string avatarID, byte[] zippedData, int zippedLength)
        {
            if (Configure.Instance.AudioInput && Configure.Instance.Wave)
            {
                if (_wave != null)
                {
                    AudioSystem.Instance.LastHandledAudioInputMillis = Environment.TickCount64;
#if X64
                    var rawData = ArrayPool<byte>.Shared.Rent(8 * zippedLength);
                    var rawLength = _rawComputer.Decode(zippedData, zippedLength, rawData, rawData.Length);
                    try
                    {
                        if (!_waveData.TryGetValue(avatarID, out var waveData))
                        {
                            waveData = new(_waveFormat)
                            {
                                DiscardOnBufferOverflow = true
                            };
                            _m.AddInputStream(new Wave16ToFloatProvider(waveData));
                            CloseWave();
                            SetWave();
                            _waveData[avatarID] = waveData;
                        }
                        waveData.AddSamples(rawData, 0, rawLength);
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(rawData);
                    }
#endif
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