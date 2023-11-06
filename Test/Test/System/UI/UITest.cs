using Qwilight;
using Xunit;

namespace Test
{
    [Collection("Test")]
    public sealed class UITest
    {
        [Fact]
        public void LoadUI()
        {
            UI.Instance.LoadUIFiles();
            foreach (var value in UI.Instance.UIItems)
            {
                UI.Instance.LoadUI(null, value, false);
                Assert.Null(UI.Instance.FaultText);
            }
        }
    }
}
