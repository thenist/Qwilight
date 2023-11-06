using Microsoft.UI;
using Qwilight.Utilities;
using Xunit;

namespace Test.Utilities
{
    [Collection("Test")]
    public sealed class ColorUtilityTest
    {
        [Fact]
        public void ModifyColor()
        {
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.Black), Colors.Black);
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.Red), Colors.Red);
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.Orange), Colors.Orange);
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.Yellow), Colors.Yellow);
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.Green), Colors.Green);
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.Blue), Colors.Blue);
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.Indigo), Colors.Indigo);
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.Purple), Colors.Purple);
            Assert.Equal(Utility.ModifyColor(System.Windows.Media.Colors.White), Colors.White);

            Assert.Equal(Utility.ModifyColor(Colors.Black), System.Windows.Media.Colors.Black);
            Assert.Equal(Utility.ModifyColor(Colors.Red), System.Windows.Media.Colors.Red);
            Assert.Equal(Utility.ModifyColor(Colors.Orange), System.Windows.Media.Colors.Orange);
            Assert.Equal(Utility.ModifyColor(Colors.Yellow), System.Windows.Media.Colors.Yellow);
            Assert.Equal(Utility.ModifyColor(Colors.Green), System.Windows.Media.Colors.Green);
            Assert.Equal(Utility.ModifyColor(Colors.Blue), System.Windows.Media.Colors.Blue);
            Assert.Equal(Utility.ModifyColor(Colors.Indigo), System.Windows.Media.Colors.Indigo);
            Assert.Equal(Utility.ModifyColor(Colors.Purple), System.Windows.Media.Colors.Purple);
            Assert.Equal(Utility.ModifyColor(Colors.White), System.Windows.Media.Colors.White);
        }
    }
}
