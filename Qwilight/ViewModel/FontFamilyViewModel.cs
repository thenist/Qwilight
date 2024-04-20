using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Qwilight.ViewModel
{
    public sealed class FontFamilyViewModel : BaseViewModel
    {
        readonly DispatcherTimer _wantHandler;
        readonly List<FontFamilyItem> _fontFamilyItems = new();
        FontFamilyItem? _fontFamilyItem;
        string _wantInput = string.Empty;

        public override double TargetLength => 0.2;

        public override double TargetHeight => 0.5;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public int FontPosition { get; set; }

        public ObservableCollection<FontFamilyItem> FontFamilyCollection { get; } = new();

        public FontFamilyItem? FontFamilyItem
        {
            get => _fontFamilyItem;

            set => SetProperty(ref _fontFamilyItem, value, nameof(FontFamilyItem));
        }

        public string WantInput
        {
            get => _wantInput;

            set => SetProperty(ref _wantInput, value, nameof(WantInput));
        }

        public void OnWant()
        {
            _wantHandler.Stop();
            _wantHandler.Start();
        }

        public void OnPointLower() => Close();

        public FontFamilyViewModel()
        {
            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                _fontFamilyItems.Add(new()
                {
                    FontFamilyValue = fontFamily
                });
            }
            _wantHandler = new(DispatcherPriority.Input, UIHandler.Instance.Handler)
            {
                Interval = TimeSpan.FromMilliseconds(QwilightComponent.StandardWaitMillis)
            };
            _wantHandler.Tick += (sender, e) =>
            {
                (sender as DispatcherTimer).Stop();
                SetFontFamilyCollection();
            };
        }

        public override void OnOpened()
        {
            base.OnOpened();
            SetFontFamilyCollection();
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            if (FontFamilyItem.HasValue)
            {
                Configure.Instance.FontFamilyValues[FontPosition] = FontFamilyItem.Value.FontFamilyValue;
                Configure.Instance.SetFontFamily();
            }
        }

        void SetFontFamilyCollection()
        {
            FontFamilyCollection.Clear();
            foreach (var fontFamilyItem in _fontFamilyItems.Where(fontFamily => fontFamily.FontFamilyNames.Any(fontFamilyName => fontFamilyName.ContainsCaselsss(WantInput))))
            {
                FontFamilyCollection.Add(fontFamilyItem);
            }
        }
    }
}