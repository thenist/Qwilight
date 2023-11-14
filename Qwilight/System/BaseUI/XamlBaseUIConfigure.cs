namespace Qwilight
{
    public sealed class XamlBaseUIConfigure
    {
        public int Position { get; set; }

        public string[] Configures { get; set; }

        public string ConfigureComment { get; set; }

        public string UIConfigure
        {
            get => Configure.Instance.BaseUIConfigureValue.UIConfigures[Position];

            set => Configure.Instance.BaseUIConfigureValue.UIConfigures[Position] = value;
        }
    }
}