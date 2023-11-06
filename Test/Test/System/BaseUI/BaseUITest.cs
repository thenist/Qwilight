using Qwilight;
using Xunit;

namespace Test
{
    [Collection("Test")]
    public sealed class BaseUITest
    {
        [Fact]
        public void LoadUI()
        {
            BaseUI.Instance.LoadUIFiles();
            foreach (var value in BaseUI.Instance.UIItems)
            {
                BaseUI.Instance.LoadUI(null, value, false);
                Assert.Null(BaseUI.Instance.FaultText);
            }
        }
    }
}
