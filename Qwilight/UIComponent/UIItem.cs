using System.IO;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class UIItem : Model
    {
        public string UIEntry { get; init; }

        public string YamlName { get; init; }

        public string Title => YamlName[(YamlName.IndexOf('@') + 1)..];

        public bool IsConfigured => YamlName.StartsWith('@') ? Equals(Configure.Instance.BaseUIItemValue) : Equals(Configure.Instance.UIItemValue);

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string GetYamlFilePath() => Path.Combine(QwilightComponent.UIEntryPath, UIEntry, Path.ChangeExtension(YamlName, ".yaml"));

        public override string ToString() => YamlName[(YamlName.IndexOf('@') + 1)..];

        public void NotifyUI() => OnPropertyChanged(nameof(IsConfigured));

        public override bool Equals(object obj) => obj is UIItem valueUIItem && UIEntry == valueUIItem.UIEntry && YamlName == valueUIItem.YamlName;

        public override int GetHashCode() => HashCode.Combine(UIEntry, YamlName);

        public static bool operator ==(UIItem left, UIItem right) => left.Equals(right);

        public static bool operator !=(UIItem left, UIItem right) => !(left == right);
    }
}
