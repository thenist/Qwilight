using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Qwilight.ViewModel
{
    public sealed partial class ModifyDefaultAudioFilePathViewModel : BaseViewModel
    {
        DefaultAudioFilePathItem? _defaultAudioFilePathItem;

        public override double TargetLength => 0.6;

        public override double TargetHeight => 0.4;

        public ObservableCollection<DefaultAudioFilePathItem> DefaultAudioFilePathItemCollection { get; } = new();

        public DefaultAudioFilePathItem? DefaultAudioFilePathItemValue
        {
            get => _defaultAudioFilePathItem;

            set
            {
                if (SetProperty(ref _defaultAudioFilePathItem, value, nameof(DefaultAudioFilePathItemValue)) && value.HasValue)
                {
                    ViewModels.Instance.MainValue.DefaultAudioSalt = DefaultAudioFilePathItemCollection.IndexOf(value.Value);
                    ViewModels.Instance.MainValue.CloseAutoComputer("Default");
                }
            }
        }

        public void OnInputLower(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete when DefaultAudioFilePathItemValue.HasValue:
                    var defaultAudioFilePathItemValue = DefaultAudioFilePathItemValue.Value;
                    var i = DefaultAudioFilePathItemCollection.IndexOf(defaultAudioFilePathItemValue);
                    DefaultAudioFilePathItemCollection.RemoveAt(i);
                    Configure.Instance.DefaultAudioFilePathItems = DefaultAudioFilePathItemCollection.ToArray();
                    AudioSystem.Instance.WipeDefaultAudioItem(defaultAudioFilePathItemValue.Value);
                    if (i < DefaultAudioFilePathItemCollection.Count)
                    {
                        DefaultAudioFilePathItemValue = DefaultAudioFilePathItemCollection[i];
                    }
                    StrongReferenceMessenger.Default.Send(new MoveDefaultAudioFilePathView
                    {
                        Target = DefaultAudioFilePathItemValue
                    });
                    break;
            }
        }

        [RelayCommand]
        async Task OnNewDefaultAudioFilePath()
        {
            var fileName = await StrongReferenceMessenger.Default.Send(new ViewFileWindow { Filters = QwilightComponent.AudioFileFormats });
            var defaultAudioFilePathItem = new DefaultAudioFilePathItem
            {
                Value = fileName
            };
            if (!string.IsNullOrEmpty(fileName) && !DefaultAudioFilePathItemCollection.Contains(defaultAudioFilePathItem))
            {
                AudioSystem.Instance.LoadDefaultAudioItem(fileName);
                DefaultAudioFilePathItemCollection.Add(defaultAudioFilePathItem);
                Configure.Instance.DefaultAudioFilePathItems = DefaultAudioFilePathItemCollection.ToArray();
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();
            Configure.Instance.DefaultAudioVarietyValue = Configure.DefaultAudioVariety.Favor;
            DefaultAudioFilePathItemCollection.Clear();
            foreach (var defaultAudioFilePathItem in Configure.Instance.DefaultAudioFilePathItems)
            {
                DefaultAudioFilePathItemCollection.Add(defaultAudioFilePathItem);
            }
            var pausableAudioFileName = ViewModels.Instance.MainValue.PausableAudioFileName;
            DefaultAudioFilePathItemValue = DefaultAudioFilePathItemCollection.Where(defaultAudioFilePathItem => $"{nameof(AudioSystem)}://{defaultAudioFilePathItem.Value}" == pausableAudioFileName).ToArray().GetSafely();
        }
    }
}