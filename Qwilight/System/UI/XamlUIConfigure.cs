namespace Qwilight
{
    public sealed class XamlUIConfigure
    {
        public int Position { get; set; }

        public string[] Configures { get; set; }

        public string ConfigureComment { get; set; }

        public string UIConfigure
        {
            get => Configure.Instance.UIConfigureValue.UIConfiguresV2[Position];

            set => Configure.Instance.UIConfigureValue.UIConfiguresV2[Position] = value;
        }
    }
}