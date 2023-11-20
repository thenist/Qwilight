using YamlDotNet.RepresentationModel;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static string GetText(YamlNode yamlNode, string target, string defaultValue = null)
        {
            if ((yamlNode as YamlMappingNode)?.Children?.TryGetValue(new YamlScalarNode(target), out var value) == true)
            {
                var text = value.ToString().Trim();
                return string.IsNullOrEmpty(text) ? defaultValue : text;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
