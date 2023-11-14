using Qwilight.Utilities;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class BinInput
    {
        [GeneratedRegex("[^-\\d\\.]")]
        private static partial Regex GetTextComputer();

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(BinInput), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) =>
        {
            var binInput = d as BinInput;
            binInput.Text = ((double)e.NewValue).ToString(binInput.Format);
        }));
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(nameof(Format), typeof(string), typeof(BinInput), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.None, (d, e) =>
        {
            var binInput = d as BinInput;
            binInput.Text = binInput.Value.ToString(e.NewValue as string);
        }));
        public static readonly DependencyProperty LowestProperty = DependencyProperty.Register(nameof(Lowest), typeof(double), typeof(BinInput), new FrameworkPropertyMetadata(double.MinValue));
        public static readonly DependencyProperty HighestProperty = DependencyProperty.Register(nameof(Highest), typeof(double), typeof(BinInput), new FrameworkPropertyMetadata(double.MaxValue));
        public static readonly RoutedEvent ValueModifiedEvent = EventManager.RegisterRoutedEvent(nameof(ValueModified), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(BinInput));

        public event RoutedPropertyChangedEventHandler<double> ValueModified
        {
            add => AddHandler(ValueModifiedEvent, value);

            remove => RemoveHandler(ValueModifiedEvent, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);

            set => SetValue(ValueProperty, value);
        }

        public string Format
        {
            get => GetValue(FormatProperty) as string;

            set => SetValue(FormatProperty, value);
        }

        public double Lowest
        {
            get => (double)GetValue(LowestProperty);

            set => SetValue(LowestProperty, value);
        }

        public double Highest
        {
            get => (double)GetValue(HighestProperty);

            set => SetValue(HighestProperty, value);
        }

        public BinInput()
        {
            InitializeComponent();
            LostFocus += (sender, e) => ModifyValue(0.0);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    ModifyValue(1.0);
                    e.Handled = true;
                    break;
                case Key.Down:
                    ModifyValue(-1.0);
                    e.Handled = true;
                    break;
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            var textPosition = CaretIndex;
            var lastValue = Value;
            var text = GetTextComputer().Replace(Text, string.Empty);
            Utility.ToFloat64(text, out var value);
            Value = Math.Clamp(value, Lowest, Highest);
            CaretIndex = textPosition;
            RaiseEvent(new RoutedPropertyChangedEventArgs<double>(lastValue, Value, ValueModifiedEvent));
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (IsFocused)
            {
                ModifyValue(Math.Sign(e.Delta));
                e.Handled = true;
            }
        }

        void ModifyValue(double delta) => Text = Math.Clamp((double.TryParse(GetTextComputer().Replace(Text, string.Empty), out var value) ? value : 0.0) + delta, Lowest, Highest).ToString(Format);

        void OnLoaded(object sender, RoutedEventArgs e) => Text = Value.ToString(Format);
    }
}
