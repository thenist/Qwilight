using Qwilight.Compute;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Buffers;
using System.IO;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.Storage.Streams;

namespace Qwilight
{
    public sealed class MIDISystem : Model
    {
        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(MIDISystem));

        public static readonly MIDISystem Instance = QwilightComponent.GetBuiltInData<MIDISystem>(nameof(MIDISystem));

        readonly List<MidiInPort> _rawMIDIControllers = new();
        readonly HandlingController<MIDI> _handlingController = new(rawInput => ViewModels.Instance.InputValue.OnMIDILower(rawInput), rawInput => ViewModels.Instance.InputStandardControllerValue.OnMIDILower(rawInput), DefaultCompute.InputFlag.MIDI);

        public string LastMIDI { get; set; }

        public string[] MIDIs { get; set; } = Array.Empty<string>();

        public string MIDIContents => string.Join(", ", MIDIs);

        public string MIDICountContents => string.Format(LanguageSystem.Instance.MIDICountContents, MIDIs.Length);

        public void Init()
        {
            _handlingController.Init();
        }

        void Handle(object sender, MidiMessageReceivedEventArgs args)
        {
            var input = args.Message;
            var inputVariety = input.Type;
            var rawInput = input.RawData;
            var data = ArrayPool<byte>.Shared.Rent((int)rawInput.Length);
            try
            {
                var dataLength = 0;
                using (var r = DataReader.FromBuffer(rawInput))
                {
                    while (dataLength < rawInput.Length)
                    {
                        data[dataLength++] = r.ReadByte();
                    }
                }
                if (ViewModels.Instance.ConfigureValue.IsOpened)
                {
                    LastMIDI = $"{nameof(input.RawData)}: [{string.Join(", ", data.Take(dataLength))}], {nameof(input.Type)}: {inputVariety}";
                    OnPropertyChanged(nameof(LastMIDI));
                }
                switch (inputVariety)
                {
                    case MidiMessageType.NoteOn:
                        _handlingController.Input(new MIDI
                        {
                            Data = MidiMessageType.NoteOn,
                            Value = data[1]
                        }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, data[2] > 0, data[2]);
                        break;
                    case MidiMessageType.NoteOff:
                        _handlingController.Input(new MIDI
                        {
                            Data = MidiMessageType.NoteOn,
                            Value = data[1]
                        }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, false, 0);
                        break;
                    case MidiMessageType.ControlChange:
                        _handlingController.Input(new MIDI
                        {
                            Data = MidiMessageType.ControlChange,
                            Value = data[1]
                        }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, data[2] > 0, data[2]);
                        break;
                    case MidiMessageType.ProgramChange:
                        _handlingController.Input(new MIDI
                        {
                            Data = MidiMessageType.ProgramChange,
                            Value = data[1]
                        }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, true, byte.MaxValue);
                        break;
                    case MidiMessageType.PitchBendChange:
                        var value = data[2];
                        var mMIDIPBCSensitivity = 63 * Configure.Instance.MIDIPBCSensitivity / 100.0;
                        if (value > 127 - mMIDIPBCSensitivity)
                        {
                            _handlingController.Input(new MIDI
                            {
                                Data = MidiMessageType.PitchBendChange,
                                Value = 0
                            }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, false, byte.MaxValue);
                            _handlingController.Input(new MIDI
                            {
                                Data = MidiMessageType.PitchBendChange,
                                Value = 1
                            }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, true, byte.MaxValue);
                        }
                        else if (value < mMIDIPBCSensitivity + 1)
                        {
                            _handlingController.Input(new MIDI
                            {
                                Data = MidiMessageType.PitchBendChange,
                                Value = 1
                            }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, false, byte.MaxValue);
                            _handlingController.Input(new MIDI
                            {
                                Data = MidiMessageType.PitchBendChange,
                                Value = 0
                            }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, true, byte.MaxValue);
                        }
                        else
                        {
                            _handlingController.Input(new MIDI
                            {
                                Data = MidiMessageType.PitchBendChange,
                                Value = 0
                            }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, false, byte.MaxValue);
                            _handlingController.Input(new MIDI
                            {
                                Data = MidiMessageType.PitchBendChange,
                                Value = 1
                            }, Configure.Instance.MIDIBundlesV4.StandardInputs, Configure.Instance.MIDIBundlesV4.Inputs, false, byte.MaxValue);
                        }
                        break;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }

        public async void GetMIDIs()
        {
            var rawMIDIControllers = (await Task.WhenAll((await DeviceInformation.FindAllAsync(MidiInPort.GetDeviceSelector())).Select(async data => (data.Name, await MidiInPort.FromIdAsync(data.Id))))).Where(inputController => inputController.Item2 != null).ToArray();
            Utility.SetUICollection(_rawMIDIControllers, rawMIDIControllers.Select(rawMIDIController => rawMIDIController.Item2).ToArray(), rawMIDIController =>
            {
                try
                {
                    using (rawMIDIController)
                    {
                        rawMIDIController.MessageReceived -= Handle;
                    }
                    _rawMIDIControllers.Remove(rawMIDIController);
                }
                catch
                {
                }
            }, rawMIDIController =>
            {
                rawMIDIController.MessageReceived += Handle;
                _rawMIDIControllers.Add(rawMIDIController);
            });
            MIDIs = rawMIDIControllers.Select(rawMIDIController => rawMIDIController.Name).ToArray();
            OnPropertyChanged(nameof(MIDIs));
            OnPropertyChanged(nameof(MIDIContents));
            OnPropertyChanged(nameof(MIDICountContents));
        }

        public void HandleSystem()
        {
            GetMIDIs();
            var w = DeviceInformation.CreateWatcher(MidiInPort.GetDeviceSelector());
            w.Added += (sender, args) => GetMIDIs();
            w.Removed += (sender, args) => GetMIDIs();
            w.Updated += (sender, args) => GetMIDIs();
            w.EnumerationCompleted += (sender, args) => GetMIDIs();
            w.Start();
        }
    }
}