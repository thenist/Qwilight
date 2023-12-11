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

            set => SetProperty(ref _defaultAudioFilePathItem, value, nameof(DefaultAudioFilePathItemValue));
        }

        public void OnInputLower(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete when DefaultAudioFilePathItemValue.HasValue:
                    var i = DefaultAudioFilePathItemCollection.IndexOf(DefaultAudioFilePathItemValue.Value);
                    DefaultAudioFilePathItemCollection.RemoveAt(i);
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
            if (!string.IsNullOrEmpty(fileName) && !DefaultAudioFilePathItemCollection.Any(defaultAudioFilePathItem => defaultAudioFilePathItem.Value == fileName))
            {
                DefaultAudioFilePathItemCollection.Add(new() { Value = fileName });
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();
            DefaultAudioFilePathItemCollection.Clear();
            foreach (var defaultAudioFilePathItem in Configure.Instance.DefaultAudioFilePathItems)
            {
                DefaultAudioFilePathItemCollection.Add(defaultAudioFilePathItem);
            }
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            var defaultAudioFilePathItems = DefaultAudioFilePathItemCollection.ToArray();
            if (Utility.IsItemsEqual(Configure.Instance.DefaultAudioFilePathItems, defaultAudioFilePathItems) == false)
            {
                Configure.Instance.DefaultAudioFilePathItems = defaultAudioFilePathItems;
                AudioSystem.Instance.LoadDefaultAudioItems();
                if (defaultAudioFilePathItems.Length > 0)
                {
                    Configure.Instance.DefaultAudioVarietyValue = Configure.DefaultAudioVariety.Favor;
                }
                else if (Configure.Instance.DefaultAudioVarietyValue == Configure.DefaultAudioVariety.Favor)
                {
                    Configure.Instance.DefaultAudioVarietyValue = Configure.DefaultAudioVariety.UI;
                }
            }
        }
    }
}