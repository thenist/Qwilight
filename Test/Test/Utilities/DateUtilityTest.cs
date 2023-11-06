using Qwilight.Utilities;
using Xunit;

namespace Test.Utilities
{
    [Collection("Test")]
    public sealed class DateUtilityTest
    {
        [Fact]
        public void IsLowerDate()
        {
            Assert.False(Utility.IsLowerDate(new Version(1, 0, 0), 1, 0, 0));
            Assert.False(Utility.IsLowerDate(new Version(1, 0, 1), 1, 0, 0));
            Assert.False(Utility.IsLowerDate(new Version(1, 1, 0), 1, 0, 0));
            Assert.False(Utility.IsLowerDate(new Version(1, 1, 1), 1, 0, 0));
            Assert.False(Utility.IsLowerDate(new Version(2, 0, 0), 1, 0, 0));
            Assert.False(Utility.IsLowerDate(new Version(2, 0, 1), 1, 0, 0));
            Assert.False(Utility.IsLowerDate(new Version(2, 1, 0), 1, 0, 0));
            Assert.False(Utility.IsLowerDate(new Version(2, 1, 1), 1, 0, 0));

            Assert.True(Utility.IsLowerDate(new Version(1, 0, 0), 1, 0, 1));
            Assert.True(Utility.IsLowerDate(new Version(1, 0, 0), 1, 1, 0));
            Assert.True(Utility.IsLowerDate(new Version(1, 0, 0), 1, 1, 1));
            Assert.True(Utility.IsLowerDate(new Version(1, 0, 0), 2, 0, 0));
            Assert.True(Utility.IsLowerDate(new Version(1, 0, 0), 2, 0, 1));
            Assert.True(Utility.IsLowerDate(new Version(1, 0, 0), 2, 1, 0));
            Assert.True(Utility.IsLowerDate(new Version(1, 0, 0), 2, 1, 1));
        }
    }
}
