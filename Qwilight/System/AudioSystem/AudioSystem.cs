using FMOD;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Qwilight
{
    public class AudioSystem : Model
    {
        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(AudioSystem));

        static readonly Dictionary<string, AudioConfigure> _audioConfigureValues = new()
        {
            {
                "Beats Flex",
                new()
                {
                    AudioWait = -173.0,
                    HandleInputAudio = false
                }
            },
            {
                "Buds2 Pro",
                new()
                {
                    AudioWait = -250.0,
                    HandleInputAudio = false
                }
            },
            {
                "Buds2 Stereo",
                new()
                {
                    AudioWait = -250.0,
                    HandleInputAudio = false
                }
            },
            {
                "direm W1",
                new()
                {
                    AudioWait = -48.0,
                    HandleInputAudio = false
                }
            },
            {
                "Galaxy Buds Pro",
                new()
                {
                    AudioWait = -337.0,
                    HandleInputAudio = false
                }
            },
            {
                "MOMENTUM 4",
                new()
                {
                    AudioWait = -249.0,
                    HandleInputAudio = false
                }
            },
            {
                "Razer Hammerhead TWS (2nd Gen)",
                new()
                {
                    AudioWait = -275.0,
                    HandleInputAudio = false
                }
            },
            {
                "Razer Hammerhead TWS Pro",
                new()
                {
                    AudioWait = -259.0,
                    HandleInputAudio = false
                }
            },
            {
                "WF-1000XM5",
                new()
                {
                    AudioWait = -216.0,
                    HandleInputAudio = false
                }
            },
            {
                "WH-1000XM5",
                new()
                {
                    AudioWait = -216.0,
                    HandleInputAudio = false
                }
            }
        };

        public static readonly AudioSystem Instance = QwilightComponent.GetBuiltInData<AudioSystem>(nameof(AudioSystem));

        const int Channel = 4093;
        const MODE LoadingAudioModes = MODE.DEFAULT | MODE._2D | MODE._3D_WORLDRELATIVE | MODE._3D_INVERSEROLLOFF | MODE.OPENMEMORY | MODE.ACCURATETIME | MODE.MPEGSEARCH | MODE.IGNORETAGS | MODE.LOWMEM;

        public const int MainAudio = 0;
        public const int InputAudio = 1;
        public const int SEAudio = 2;
        public const int TotalAudio = 3;

        readonly ReaderWriterLockSlim _audioCSX = new();
        readonly Timer _audioHandler;
        readonly SYSTEM_CALLBACK _onModified;
        readonly float[] _audioVolumes = new float[4];
        readonly ChannelGroup[] _audioGroups = new ChannelGroup[3];
        readonly ConcurrentDictionary<IAudioContainer, ConcurrentDictionary<string, AudioItem>> _audioMap = new();
        readonly ConcurrentDictionary<IAudioHandler, ConcurrentBag<AudioHandlerItem>> _audioHandlerMap = new();
        readonly DSP[] _audioVisualizerComputers = new DSP[2];
        readonly DSP[] _equalizerComputers = new DSP[2];
        readonly DSP[] _tubeComputers = new DSP[2];
        readonly DSP[] _valueSFXComputers = new DSP[2];
        readonly DSP[] _flangeComputers = new DSP[2];
        readonly DSP[] _averagerComputers = new DSP[2];
        readonly DSP[] _audioMultiplierAtoneComputers = new DSP[2];
        readonly double[][] _audioVisualizerValue = new double[2][];
        FMOD.System _targetSystem;
        int _rate;
        bool _isAvailable;
        float _audioInputVolume = 1F;
        AudioValue? _audioValue;

        public AudioItem? BanalAudio { get; set; }

        public void LoadBanalAudio()
        {
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    BanalAudio?.Dispose();
                }
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
            try
            {
                var filePath = Configure.Instance.BanalAudioFilePath;
                if (File.Exists(filePath))
                {
                    BanalAudio = Load(filePath, null, 1F);
                }
                else
                {
                    BanalAudio = null;
                }
            }
            catch
            {
                BanalAudio = null;
            }
        }

        public ConcurrentDictionary<string, AudioItem> DefaultAudioItemMap { get; } = new();

        public string GetDefaultAudioFileName(long randomMillis)
        {
            var defaultAudioFilePathItems = Configure.Instance.DefaultAudioFilePathItems;
            var defaultAudioFilePathItemsLength = defaultAudioFilePathItems.Length;
            return defaultAudioFilePathItems.Length > 0 ? $"{nameof(AudioSystem)}://{defaultAudioFilePathItems[randomMillis < defaultAudioFilePathItemsLength ? randomMillis : randomMillis % defaultAudioFilePathItemsLength].Value}" : null;
        }

        public void WipeDefaultAudioItem(string filePath)
        {
            if (DefaultAudioItemMap.TryRemove($"{nameof(AudioSystem)}://{filePath}", out var defaultAudioItem))
            {
                _audioCSX.EnterWriteLock();
                try
                {
                    if (_isAvailable)
                    {
                        defaultAudioItem.Dispose();
                    }
                }
                finally
                {
                    _audioCSX.ExitWriteLock();
                }
            }
        }

        public void LoadDefaultAudioItem(string filePath)
        {
            WipeDefaultAudioItem(filePath);
            try
            {
                if (File.Exists(filePath))
                {
                    DefaultAudioItemMap[$"{nameof(AudioSystem)}://{filePath}"] = Load(filePath, null, 1F, null, true);
                }
            }
            catch
            {
            }
        }

        public void LoadDefaultAudioItems()
        {
            foreach (var defaultAudioFilePathItem in Configure.Instance.DefaultAudioFilePathItems)
            {
                LoadDefaultAudioItem(defaultAudioFilePathItem.Value);
            }
        }

        public Dictionary<PostableItem, AudioItem> PostableItemAudioMap { get; } = new();

        public AudioItem PostableItemAudio { get; set; }

        public AudioItem PostedItemAudio { get; set; }

        public long LastHandledAudioInputMillis { get; set; }

        public string AudioDate { get; set; }

        public string AudioDateHTML { get; set; }

        public ObservableCollection<AudioValue> AudioValues { get; } = new();

        public int AudioItemCount => _audioMap.Values.Sum(audioItems => audioItems.Count);

        public int AudioHandlerItemCount => _audioHandlerMap.Values.Sum(audioHandlerItems => audioHandlerItems.Count);

        public AudioValue? AudioValue
        {
            get => _audioValue;

            set
            {
                if (SetProperty(ref _audioValue, value, nameof(AudioValue)) && value.HasValue)
                {
                    var audioValue = value.Value;
                    var audioValueID = audioValue.ID;
                    _targetSystem.setDriver(audioValueID);
                    switch (Configure.Instance.AudioVariety)
                    {
                        case OUTPUTTYPE.WASAPI:
                            Configure.Instance.LastWASAPIAudioValueID = audioValueID;
                            break;
                        case OUTPUTTYPE.ASIO:
                            Configure.Instance.LastASIOAudioValueID = audioValueID;
                            break;
                    }
                    var audioValueName = audioValue.Name;
                    if (Configure.Instance.AudioConfigureValues.TryGetValue(audioValueName, out var audioConfigureValue))
                    {
                        Configure.Instance.BanalAudioWait = audioConfigureValue.AudioWait;
                        Configure.Instance.HandleInputAudio = audioConfigureValue.HandleInputAudio;
                    }
                    else
                    {
                        Configure.Instance.AudioConfigureValues[audioValueName] = _audioConfigureValues.FirstOrDefault(audioConfigureValue => audioValueName.Contains(audioConfigureValue.Key)).Value ?? new()
                        {
                            HandleInputAudio = true
                        };
                    }
                }
            }
        }

        public AudioSystem()
        {
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "fmod.dll"), Path.Combine(AppContext.BaseDirectory, "fmod.dll"));
            _onModified = (system, type, commanddata1, commanddata2, userdata) =>
            {
                _targetSystem.getNumDrivers(out var audioValueCount);
                var audioValues = Enumerable.Range(0, audioValueCount).Select(i =>
                {
                    _targetSystem.getDriverInfo(i, out var audioValueName, 128, out _, out _, out _, out _);
                    return new AudioValue
                    {
                        ID = i,
                        Name = audioValueName
                    };
                }).ToArray();
                UIHandler.Instance.HandleParallel(() =>
                {
                    AudioValues.Clear();
                    foreach (var audioValue in audioValues)
                    {
                        AudioValues.Add(audioValue);
                    }
                    AudioValue = AudioValues.GetSafely(Configure.Instance.AudioVariety switch
                    {
                        OUTPUTTYPE.WASAPI => Configure.Instance.LastWASAPIAudioValueID,
                        OUTPUTTYPE.ASIO => Configure.Instance.LastASIOAudioValueID,
                        _ => default
                    });
                });
                return RESULT.OK;
            };

            var targetAudioVisualizerValues = new double[2][];
            var targetAudioHeights = new double[2][];
            float[][] audioVisualizerItems = null;
            var lastLength = 0;
            for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
            {
                targetAudioVisualizerValues[audioVariety] = new double[256];
                targetAudioHeights[audioVariety] = new double[256];
                _audioVisualizerValue[audioVariety] = new double[256];
            }
            _audioHandler = new(state =>
            {
                _audioCSX.EnterWriteLock();
                try
                {
                    if (_isAvailable)
                    {
                        if (Configure.Instance.AudioVisualizer)
                        {
                            var audioVisualizerCount = Configure.Instance.AudioVisualizerCount;
                            for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                            {
                                Array.Clear(targetAudioHeights[audioVariety], 0, audioVisualizerCount);
                                if (_audioGroups[audioVariety].getNumChannels(out var available) == RESULT.OK && available > 0)
                                {
                                    if (_audioVisualizerComputers[audioVariety].getParameterData((int)DSP_FFT.SPECTRUMDATA, out var data, out _) == RESULT.OK)
                                    {
                                        var dataFFT = Marshal.PtrToStructure<DSP_PARAMETER_FFT>(data);
                                        if (audioVisualizerItems == null || audioVisualizerItems.GetLength(0) != dataFFT.numchannels || lastLength != dataFFT.length)
                                        {
                                            audioVisualizerItems = new float[dataFFT.numchannels][];
                                            lastLength = dataFFT.length;
                                            for (var i = audioVisualizerItems.Length - 1; i >= 0; --i)
                                            {
                                                audioVisualizerItems[i] = new float[lastLength];
                                            }
                                        }
                                        dataFFT.getSpectrum(ref audioVisualizerItems);
                                        if (audioVisualizerItems.Length > 0)
                                        {
                                            foreach (var audioVisualizerItem in audioVisualizerItems)
                                            {
                                                for (var j = audioVisualizerItem.Length - 1; j >= 0; --j)
                                                {
                                                    var m = (int)(j / ((double)audioVisualizerItem.Length / audioVisualizerCount));
                                                    targetAudioHeights[audioVariety][m] += audioVisualizerItem[m];
                                                }
                                            }
                                            for (var i = audioVisualizerCount - 1; i >= 0; --i)
                                            {
                                                targetAudioHeights[audioVariety][i] = Math.Clamp(targetAudioHeights[audioVariety][i], 0.0, 1.0);
                                            }
                                            targetAudioVisualizerValues[audioVariety] = targetAudioHeights[audioVariety];
                                        }
                                    }
                                }
                                Array.Copy(targetAudioHeights[audioVariety], 0, targetAudioVisualizerValues[audioVariety], 0, audioVisualizerCount);
                                for (var i = audioVisualizerCount - 1; i >= 0; --i)
                                {
                                    _audioVisualizerValue[audioVariety][i] += Utility.GetMove(targetAudioVisualizerValues[audioVariety][i], _audioVisualizerValue[audioVariety][i], 1000.0 / 60);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _audioCSX.ExitWriteLock();
                }

                _audioInputVolume += (float)Utility.GetMove(LastHandledAudioInputMillis + QwilightComponent.StandardWaitMillis >= Environment.TickCount64 ? (float)(Configure.Instance.WaveFadeVolume / 100.0) : 1F, _audioInputVolume);
                var totalAudioVolume = _audioVolumes[TotalAudio] * _audioInputVolume * (!Configure.Instance.LostPointAudio && !ViewModels.Instance.MainValue.HasPoint ? 0F : 1F);

                for (var audioVariety = SEAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            var targetAudioVolume = (float)(1 - Math.Sqrt(1 - Math.Pow(totalAudioVolume * _audioVolumes[audioVariety], 2)));
                            if (_audioGroups[audioVariety].getVolume(out var audioVolume) == RESULT.OK && audioVolume != targetAudioVolume)
                            {
                                _audioGroups[audioVariety].setVolume(targetAudioVolume);
                            }
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }

                _audioCSX.EnterWriteLock();
                try
                {
                    if (_isAvailable)
                    {
                        _targetSystem.update();
                    }
                }
                finally
                {
                    _audioCSX.ExitWriteLock();
                }
            }, null, TimeSpan.Zero, QwilightComponent.StandardFrametime);
        }

        public virtual void Init()
        {
            _audioCSX.EnterWriteLock();
            try
            {
                if (!_isAvailable)
                {
                    Factory.System_Create(out _targetSystem);
                    _targetSystem.getVersion(out var audioDate);
                    AudioDate = $"v{(audioDate >> 16)}.{((audioDate >> 8) & 255).ToString("00")}.{Convert.ToString(audioDate & 255, 16)}";
                    AudioDateHTML = $"https://fmod.com/resources/documentation-api?version={(audioDate >> 16)}.{((audioDate >> 8) & 255)}&page=welcome-revision-history.html";
                    _targetSystem.setSoftwareChannels(Channel);
                    _targetSystem.getSoftwareFormat(out var rate, out _, out _);
                    _rate = (int)(rate / 1000.0);
                    _targetSystem.setDSPBufferSize(Configure.Instance.AudioDataLength, 4);
                    _targetSystem.init(Channel, INITFLAGS.NORMAL, nint.Zero);

                    for (var audioVariety = SEAudio; audioVariety >= MainAudio; --audioVariety)
                    {
                        _targetSystem.createChannelGroup(null, out _audioGroups[audioVariety]);
                        _audioGroups[audioVariety].setVolumeRamp(false);
                    }

                    _targetSystem.setCallback(_onModified, SYSTEM_CALLBACK_TYPE.DEVICELISTCHANGED | SYSTEM_CALLBACK_TYPE.DEVICELOST);
                    _isAvailable = true;
                }
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }

            for (var i = PostableItem.Values.Length - 1; i >= 0; --i)
            {
                var postableItem = PostableItem.Values[i];
                PostableItemAudioMap.GetValueOrDefault(postableItem).Dispose();
                var audioFilePath = Utility.GetFilePath(Path.Combine(QwilightComponent.AssetsEntryPath, "Audio", "Postable Item", postableItem.VarietyValue.ToString()), Utility.FileFormatFlag.Audio);
                if (File.Exists(audioFilePath))
                {
                    PostableItemAudioMap[postableItem] = Load(audioFilePath, null, 1F);
                }
            }
            PostableItemAudio = Load(Utility.GetFilePath(Path.Combine(QwilightComponent.AssetsEntryPath, "Audio", "Postable Item", "Postable"), Utility.FileFormatFlag.Audio), null, 1F);
            PostedItemAudio = Load(Utility.GetFilePath(Path.Combine(QwilightComponent.AssetsEntryPath, "Audio", "Postable Item", "Posted"), Utility.FileFormatFlag.Audio), null, 1F);

            SetAudioVariety(Configure.Instance.AudioVariety);
            SetVolume(MainAudio, (float)Configure.Instance.MainAudioVolume);
            SetVolume(InputAudio, (float)Configure.Instance.InputAudioVolume);
            SetVolume(SEAudio, (float)Configure.Instance.SEAudioVolume);
            SetVolume(TotalAudio, (float)Configure.Instance.TotalAudioVolume);

            if (Configure.Instance.AudioVisualizer)
            {
                SetAudioVisualizer(true);
            }
            if (Configure.Instance.Equalizer)
            {
                SetEqualizer(true);
            }
            if (Configure.Instance.Tube)
            {
                SetTube(true);
            }
            if (Configure.Instance.SFX)
            {
                SetSFX(true);
            }
            if (Configure.Instance.Flange)
            {
                SetFlange(true);
            }
            if (Configure.Instance.Averager)
            {
                SetAverager(true);
            }
        }

        public void SetAudioVariety(OUTPUTTYPE audioValueVariety)
        {
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    _targetSystem.setOutput(audioValueVariety);
                }
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    _onModified(_targetSystem.handle, SYSTEM_CALLBACK_TYPE.DEVICELISTCHANGED, nint.Zero, nint.Zero, nint.Zero);
                }
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
        }

        public void SetVolume(int audioVariety, float audioVolume) => _audioVolumes[audioVariety] = audioVolume;

        public double GetAudioVisualizerValue(int audioVariety, int audioPosition) => _audioVisualizerValue[audioVariety][audioPosition];

        public void SetAudioMultiplierAtone(bool isAudioMultiplierAtoneSet, double audioMultiplier)
        {
            if (isAudioMultiplierAtoneSet)
            {
                _audioCSX.EnterWriteLock();
                try
                {
                    if (_isAvailable)
                    {
                        for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                        {
                            _targetSystem.createDSPByType(DSP_TYPE.PITCHSHIFT, out _audioMultiplierAtoneComputers[audioVariety]);
                            _audioGroups[audioVariety].addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, _audioMultiplierAtoneComputers[audioVariety]);
                            _audioMultiplierAtoneComputers[audioVariety].setParameterFloat((int)DSP_PITCHSHIFT.PITCH, (float)(1 / audioMultiplier));
                        }
                    }
                }
                finally
                {
                    _audioCSX.ExitWriteLock();
                }
            }
            else
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _audioGroups[audioVariety].removeDSP(_audioMultiplierAtoneComputers[audioVariety]);
                            _audioMultiplierAtoneComputers[audioVariety].release();
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
        }

        public int GetHandlingAudioCount()
        {
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    _targetSystem.getChannelsPlaying(out _, out var handlingAudioCount);
                    return handlingAudioCount;
                }
                return default;
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
        }

        public float GetAudioUnitStatus()
        {
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    _targetSystem.getCPUUsage(out var audioUnitStatus);
                    return audioUnitStatus.dsp;
                }
                return default;
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
        }

        public void SetEqualizer(bool isEqualizerSet)
        {
            if (isEqualizerSet)
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _targetSystem.createDSPByType(DSP_TYPE.MULTIBAND_EQ, out _equalizerComputers[audioVariety]);
                            _equalizerComputers[audioVariety].setParameterInt((int)DSP_MULTIBAND_EQ.A_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
                            _equalizerComputers[audioVariety].setParameterInt((int)DSP_MULTIBAND_EQ.B_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
                            _equalizerComputers[audioVariety].setParameterInt((int)DSP_MULTIBAND_EQ.C_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
                            _equalizerComputers[audioVariety].setParameterInt((int)DSP_MULTIBAND_EQ.D_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
                            _equalizerComputers[audioVariety].setParameterInt((int)DSP_MULTIBAND_EQ.E_FILTER, (int)DSP_MULTIBAND_EQ_FILTER_TYPE.PEAKING);
                            _audioGroups[audioVariety].addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, _equalizerComputers[audioVariety]);
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
                SetEqualizerHz(0, (float)Configure.Instance.EqualizerHz0);
                SetEqualizerHz(1, (float)Configure.Instance.EqualizerHz1);
                SetEqualizerHz(2, (float)Configure.Instance.EqualizerHz2);
                SetEqualizerHz(3, (float)Configure.Instance.EqualizerHz3);
                SetEqualizerHz(4, (float)Configure.Instance.EqualizerHz4);
                SetEqualizer(0, (float)Configure.Instance.Equalizer0);
                SetEqualizer(1, (float)Configure.Instance.Equalizer1);
                SetEqualizer(2, (float)Configure.Instance.Equalizer2);
                SetEqualizer(3, (float)Configure.Instance.Equalizer3);
                SetEqualizer(4, (float)Configure.Instance.Equalizer4);
            }
            else
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _audioGroups[audioVariety].removeDSP(_equalizerComputers[audioVariety]);
                            _equalizerComputers[audioVariety].release();
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
        }

        public void SetEqualizerHz(int equalizerHzPosition, float equalizerHzValue)
        {
            for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
            {
                _audioCSX.EnterWriteLock();
                try
                {
                    if (_isAvailable)
                    {
                        switch (equalizerHzPosition)
                        {
                            case 0:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.A_FREQUENCY, equalizerHzValue);
                                break;
                            case 1:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.B_FREQUENCY, equalizerHzValue);
                                break;
                            case 2:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.C_FREQUENCY, equalizerHzValue);
                                break;
                            case 3:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.D_FREQUENCY, equalizerHzValue);
                                break;
                            case 4:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.E_FREQUENCY, equalizerHzValue);
                                break;
                        }
                    }
                }
                finally
                {
                    _audioCSX.ExitWriteLock();
                }
            }
        }

        public void SetEqualizer(int equalizerPosition, float equalizerValue)
        {
            for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
            {
                _audioCSX.EnterWriteLock();
                try
                {
                    if (_isAvailable)
                    {
                        switch (equalizerPosition)
                        {
                            case 0:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.A_GAIN, equalizerValue);
                                break;
                            case 1:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.B_GAIN, equalizerValue);
                                break;
                            case 2:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.C_GAIN, equalizerValue);
                                break;
                            case 3:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.D_GAIN, equalizerValue);
                                break;
                            case 4:
                                _equalizerComputers[audioVariety].setParameterFloat((int)DSP_MULTIBAND_EQ.E_GAIN, equalizerValue);
                                break;
                        }
                    }
                }
                finally
                {
                    _audioCSX.ExitWriteLock();
                }
            }
        }

        public void SetTube(bool isTubeSet)
        {
            if (isTubeSet)
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _targetSystem.createDSPByType(DSP_TYPE.COMPRESSOR, out _tubeComputers[audioVariety]);
                            _audioGroups[audioVariety].addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, _tubeComputers[audioVariety]);
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
            else
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _audioGroups[audioVariety].removeDSP(_tubeComputers[audioVariety]);
                            _tubeComputers[audioVariety].release();
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
        }

        public void SetAudioVisualizer(bool isAudioVisualizerSet)
        {
            if (isAudioVisualizerSet)
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _targetSystem.createDSPByType(DSP_TYPE.FFT, out _audioVisualizerComputers[audioVariety]);
                            _audioGroups[audioVariety].addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, _audioVisualizerComputers[audioVariety]);
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
            else
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _audioGroups[audioVariety].removeDSP(_audioVisualizerComputers[audioVariety]);
                            _audioVisualizerComputers[audioVariety].release();
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
        }

        public void SetSFX(bool isSFX)
        {
            if (isSFX)
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _targetSystem.createDSPByType(DSP_TYPE.SFXREVERB, out _valueSFXComputers[audioVariety]);
                            _audioGroups[audioVariety].addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, _valueSFXComputers[audioVariety]);
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
            else
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _audioGroups[audioVariety].removeDSP(_valueSFXComputers[audioVariety]);
                            _valueSFXComputers[audioVariety].release();
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
        }

        public void SetFlange(bool isFlange)
        {
            if (isFlange)
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _targetSystem.createDSPByType(DSP_TYPE.FLANGE, out _flangeComputers[audioVariety]);
                            _audioGroups[audioVariety].addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, _flangeComputers[audioVariety]);
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
            else
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _audioGroups[audioVariety].removeDSP(_flangeComputers[audioVariety]);
                            _flangeComputers[audioVariety].release();
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
        }

        public void SetAverager(bool isAverager)
        {
            if (isAverager)
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _targetSystem.createDSPByType(DSP_TYPE.NORMALIZE, out _averagerComputers[audioVariety]);
                            _audioGroups[audioVariety].addDSP(CHANNELCONTROL_DSP_INDEX.HEAD, _averagerComputers[audioVariety]);
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
            else
            {
                for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            _audioGroups[audioVariety].removeDSP(_averagerComputers[audioVariety]);
                            _averagerComputers[audioVariety].release();
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
        }

        public AudioItem Load(string audioFilePath, IAudioContainer audioContainer, float audioVolume, string bmsID = null, bool isLooping = false)
        {
            using var rms = PoolSystem.Instance.GetDataFlow(File.ReadAllBytes(audioFilePath));
            return Load(rms, audioContainer, audioVolume, bmsID, isLooping);
        }

        public AudioItem Load(Stream s, IAudioContainer audioContainer, float audioVolume, string bmsID = null, bool isLooping = false)
        {
            var hash = Utility.GetID128(s);
            var audioItem = new AudioItem();
            if (audioContainer != null && _audioMap.TryGetValue(audioContainer, out var audioItems) && audioItems.TryGetValue(hash, out audioItem))
            {
                return audioItem;
            }
            s.Position = 0;
            var audioInfo = new CREATESOUNDEXINFO
            {
                length = (uint)s.Length,
                cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>()
            };
            var ms = s as MemoryStream;
            using var rms = ms == null ? PoolSystem.Instance.GetDataFlow((int)audioInfo.length) : null;
            if (ms == null)
            {
                s.CopyTo(rms);
                ms = rms;
            }
            _audioCSX.EnterReadLock();
            try
            {
                if (_isAvailable)
                {
                    if (_targetSystem.createSound(ms.GetBuffer(), LoadingAudioModes | (isLooping ? MODE.LOOP_NORMAL : MODE.LOOP_OFF), ref audioInfo, out var audioData) == RESULT.OK)
                    {
                        audioItem.BMSID = bmsID;
                        audioItem.System = _targetSystem.handle;
                        audioItem.AudioData = audioData;
                        audioItem.AudioVolume = audioVolume;
                    }
                }
            }
            finally
            {
                _audioCSX.ExitReadLock();
            }
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    audioItem.AudioData.getLength(out var audioLength, TIMEUNIT.MS);
                    audioItem.Length = audioLength;
                }
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
            if (audioContainer != null)
            {
                _audioMap.AddOrUpdate(audioContainer, (audioContainer, audioItem) => new([KeyValuePair.Create(hash, audioItem)]), (audioContainer, audioItems, audioItem) =>
                {
                    audioItems[hash] = audioItem;
                    return audioItems;
                }, audioItem);
            }
            return audioItem;
        }

        public void Stop(IAudioHandler audioHandler)
        {
            if (_audioHandlerMap.TryRemove(audioHandler, out var audioHandlerItems))
            {
                foreach (var audioHandlerItem in audioHandlerItems)
                {
                    Stop(audioHandlerItem.Channel);
                }
            }
        }

        public void Stop(Channel audioChannel)
        {
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    audioChannel.stop();
                }
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
        }

        public double Fade(IAudioHandler audioHandler, double fadeMillis)
        {
            if (_audioHandlerMap.TryRemove(audioHandler, out var audioHandlerItems))
            {
                foreach (var audioHandlerItem in audioHandlerItems)
                {
                    var audioChannel = audioHandlerItem.Channel;
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            audioChannel.getPosition(out var audioPosition, TIMEUNIT.MS);
                            audioChannel.getDSPClock(out _, out var audioStandardUnit);
                            audioChannel.addFadePoint(audioStandardUnit, 1F);
                            audioChannel.addFadePoint(audioStandardUnit + (ulong)(_rate * fadeMillis), 0F);
                            audioChannel.setDelay(0UL, audioStandardUnit + (ulong)(_rate * fadeMillis));
                            return audioPosition + fadeMillis;
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }

            return default;
        }

        public void Close(IAudioContainer audioContainer, IAudioHandler audioHandler = null)
        {
            if (_audioMap.TryRemove(audioContainer, out var audioItems))
            {
                foreach (var audioItem in audioItems.Values)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            audioItem.Dispose();
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
            if (audioHandler != null)
            {
                _audioHandlerMap.TryRemove(audioHandler, out var audioHandlerItems);
            }
        }

        public void Migrate(IAudioContainer src, IAudioContainer target)
        {
            if (_audioMap.TryRemove(src, out var audioItems))
            {
                _audioMap[target] = audioItems;
            }
        }

        public void Migrate(IAudioHandler src, IAudioHandler target)
        {
            if (_audioHandlerMap.TryRemove(src, out var audioItems))
            {
                _audioHandlerMap[target] = audioItems;
            }
        }

        public void Pause(IAudioHandler audioHandler, bool isPaused)
        {
            if (_audioHandlerMap.TryGetValue(audioHandler, out var audioHandlerItems))
            {
                if (isPaused)
                {
                    foreach (var audioHandlerItem in audioHandlerItems)
                    {
                        var audioChannel = audioHandlerItem.Channel;
                        _audioCSX.EnterWriteLock();
                        try
                        {
                            if (_isAvailable)
                            {
                                audioChannel.isPlaying(out var isHandling);
                                if (isHandling)
                                {
                                    audioChannel.getPosition(out var audioPosition, TIMEUNIT.MS);
                                    audioChannel.setPaused(true);
                                    if (audioHandlerItem.LevyingPosition.HasValue)
                                    {
                                        audioHandlerItem.Position = audioPosition - audioHandlerItem.LevyingPosition.Value;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            _audioCSX.ExitWriteLock();
                        }
                    }
                }
                else
                {
                    foreach (var audioHandlerItem in audioHandlerItems)
                    {
                        var audioChannel = audioHandlerItem.Channel;
                        _audioCSX.EnterWriteLock();
                        try
                        {
                            if (_isAvailable)
                            {
                                if (audioHandlerItem.Length.HasValue)
                                {
                                    audioChannel.setDelay(0UL, audioHandlerItem.AudioStandardUnit + (ulong)(_rate * (audioHandlerItem.Length.Value - audioHandlerItem.Position)));
                                }
                                audioChannel.setPaused(false);
                            }
                        }
                        finally
                        {
                            _audioCSX.ExitWriteLock();
                        }
                    }
                }
            }
        }

        public void SetAudioMultiplier(IAudioHandler audioHandler, double audioMultiplier)
        {
            if (_audioHandlerMap.TryGetValue(audioHandler, out var audioHandlerItems))
            {
                foreach (var audioHandlerItem in audioHandlerItems)
                {
                    _audioCSX.EnterWriteLock();
                    try
                    {
                        if (_isAvailable)
                        {
                            audioHandlerItem.Channel.setPitch((float)audioMultiplier);
                        }
                    }
                    finally
                    {
                        _audioCSX.ExitWriteLock();
                    }
                }
            }
            for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
            {
                _audioCSX.EnterWriteLock();
                try
                {
                    if (_isAvailable)
                    {
                        _audioMultiplierAtoneComputers[audioVariety].setParameterFloat((int)DSP_PITCHSHIFT.PITCH, (float)(1 / audioMultiplier));
                    }
                }
                finally
                {
                    _audioCSX.ExitWriteLock();
                }
            }
        }

        public Channel Handle(AudioNote? audioNote, int audioVariety, double audioMultiplier = 1.0, bool isCounterWave = false, IAudioHandler audioHandler = null, double fadeInLength = 0.0, byte inputPower = byte.MaxValue)
        {
            if (audioNote.HasValue)
            {
                var audioNoteValue = audioNote.Value;
                var audioItem = audioNoteValue.AudioItem;
                if (audioItem.HasValue)
                {
                    var audioItemValue = audioItem.Value;
                    var audioLevyingPosition = audioNoteValue.AudioLevyingPosition;
                    if (audioItemValue.Length > audioLevyingPosition)
                    {
                        _audioCSX.EnterWriteLock();
                        try
                        {
                            if (_isAvailable)
                            {
                                if (audioItemValue.System == _targetSystem.handle)
                                {
                                    _targetSystem.playSound(audioItemValue.AudioData, _audioGroups[audioVariety], false, out var audioChannel);
                                    audioChannel.getDSPClock(out _, out var audioStandardUnit);
                                    var targetVolume = audioItemValue.AudioVolume * inputPower / byte.MaxValue;
                                    if (fadeInLength > 0.0)
                                    {
                                        audioChannel.addFadePoint(audioStandardUnit, 0F);
                                        audioChannel.addFadePoint(audioStandardUnit + (ulong)(_rate * fadeInLength), targetVolume);
                                    }
                                    else
                                    {
                                        audioChannel.setVolume(targetVolume);
                                    }
                                    if (audioMultiplier != 1.0)
                                    {
                                        audioChannel.setPitch((float)audioMultiplier);
                                    }
                                    if (isCounterWave)
                                    {
                                        audioChannel.setPosition(audioItemValue.Length - audioLevyingPosition - 1, TIMEUNIT.MS);
                                        audioChannel.getFrequency(out var audioWave);
                                        audioChannel.setFrequency((float)-audioWave);
                                    }
                                    else
                                    {
                                        audioChannel.setPosition(audioLevyingPosition, TIMEUNIT.MS);
                                    }
                                    var audioLength = audioNoteValue.Length;
                                    if (audioLength.HasValue)
                                    {
                                        audioChannel.getDSPClock(out _, out audioStandardUnit);
                                        audioChannel.setDelay(0UL, audioStandardUnit + (ulong)(_rate * audioLength.Value));
                                    }
                                    if (audioHandler != null)
                                    {
                                        var audioHandlerItem = new AudioHandlerItem
                                        {
                                            Channel = audioChannel,
                                            LevyingPosition = audioLevyingPosition,
                                            Length = audioLength,
                                            AudioStandardUnit = audioStandardUnit
                                        };
                                        _audioHandlerMap.AddOrUpdate(audioHandler, (audioHandler, audioHandlerItem) => new()
                                        {
                                            audioHandlerItem
                                        }, (audioHandler, audioHandlerItems, audioHandlerItem) =>
                                        {
                                            audioHandlerItems.Add(audioHandlerItem);
                                            return audioHandlerItems;
                                        }, audioHandlerItem);
                                    }
                                    return audioChannel;
                                }
                            }
                        }
                        finally
                        {
                            _audioCSX.ExitWriteLock();
                        }
                    }
                }
            }
            return default;
        }

        public void HandleImmediately(string audioFilePath, IAudioContainer audioContainer, IAudioHandler audioHandler, bool isLooping)
        {
            var rms = PoolSystem.Instance.GetDataFlow(File.ReadAllBytes(audioFilePath));
            var hash = Utility.GetID128(rms);
            rms.Position = 0;
            var audioInfo = new CREATESOUNDEXINFO
            {
                length = (uint)rms.Length,
                cbsize = Marshal.SizeOf<CREATESOUNDEXINFO>()
            };
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    if (_targetSystem.createSound(rms.GetBuffer(), LoadingAudioModes | (isLooping ? MODE.LOOP_NORMAL : MODE.LOOP_OFF), ref audioInfo, out var audioData) == RESULT.OK && audioData.getLength(out var audioLength, TIMEUNIT.MS) == RESULT.OK)
                    {
                        _audioMap.AddOrUpdate(audioContainer, (audioContainer, audioItem) => new([KeyValuePair.Create(hash, audioItem)]), (audioContainer, audioItems, audioItem) =>
                        {
                            audioItems[hash] = audioItem;
                            return audioItems;
                        }, new AudioItem
                        {
                            System = _targetSystem.handle,
                            AudioData = audioData,
                            AudioVolume = 1F,
                            Length = audioLength
                        });
                        _targetSystem.playSound(audioData, _audioGroups[MainAudio], false, out var audioChannel);
                        audioChannel.getDSPClock(out _, out var audioStandardUnit);
                        var audioHandlerItem = new AudioHandlerItem
                        {
                            Channel = audioChannel,
                            AudioStandardUnit = audioStandardUnit
                        };
                        _audioHandlerMap.AddOrUpdate(audioHandler, (audioHandler, audioHandlerItem) => new()
                        {
                            audioHandlerItem
                        }, (audioHandler, audioHandlerItems, audioHandlerItem) =>
                        {
                            audioHandlerItems.Add(audioHandlerItem);
                            return audioHandlerItems;
                        }, audioHandlerItem);
                    }
                }
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            _audioCSX.EnterWriteLock();
            try
            {
                if (_isAvailable)
                {
                    _isAvailable = false;
                    foreach (var audioVisualizerComputer in _audioVisualizerComputers)
                    {
                        audioVisualizerComputer.release();
                    }
                    for (var audioVariety = InputAudio; audioVariety >= MainAudio; --audioVariety)
                    {
                        _audioGroups[audioVariety].removeDSP(_equalizerComputers[audioVariety]);
                        _equalizerComputers[audioVariety].release();
                        _audioGroups[audioVariety].removeDSP(_tubeComputers[audioVariety]);
                        _tubeComputers[audioVariety].release();
                        _audioGroups[audioVariety].removeDSP(_audioMultiplierAtoneComputers[audioVariety]);
                        _audioMultiplierAtoneComputers[audioVariety].release();
                        _audioGroups[audioVariety].removeDSP(_valueSFXComputers[audioVariety]);
                        _valueSFXComputers[audioVariety].release();
                        _audioGroups[audioVariety].removeDSP(_flangeComputers[audioVariety]);
                        _flangeComputers[audioVariety].release();
                        _audioGroups[audioVariety].removeDSP(_averagerComputers[audioVariety]);
                        _averagerComputers[audioVariety].release();
                    }
                    foreach (var audioContainer in _audioMap.Keys)
                    {
                        if (_audioMap.TryRemove(audioContainer, out var audioItems))
                        {
                            foreach (var audioItem in audioItems.Values)
                            {
                                audioItem.Dispose();
                            }
                        }
                    }
                    _audioHandlerMap.Clear();
                    foreach (var audioGroup in _audioGroups)
                    {
                        audioGroup.release();
                    }
                    _targetSystem.release();
                    _targetSystem.close();
                }
            }
            finally
            {
                _audioCSX.ExitWriteLock();
            }
        }
    }
}