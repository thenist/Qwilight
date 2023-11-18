using Qwilight.Compute;
using Qwilight.Utilities;
using Xunit;

namespace Test.Utilities
{
    [Collection("Test")]
    public sealed class UtilityTest
    {
        [Fact]
        public void GetFavoriteItem()
        {
            Assert.Equal(9, Utility.GetFavoriteItem(new List<int>
            {
                1, 2, 2, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8, 9, 9, 9, 9, 9, 9, 9, 9, 9
            }));
        }

        [Fact]
        public void GetQuitStatusValue()
        {
            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(0.0, 0, 0.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.D, Utility.GetQuitStatusValue(0.0, 0, 1.0, 1));

            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(0.8, 0, 0.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.C, Utility.GetQuitStatusValue(0.8, 0, 1.0, 1));

            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(0.85, 0, 0.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.B, Utility.GetQuitStatusValue(0.85, 0, 1.0, 1));

            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(0.9, 0, 0.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.A, Utility.GetQuitStatusValue(0.9, 0, 1.0, 1));

            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(0.95, 0, 0.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.APlus, Utility.GetQuitStatusValue(0.95, 0, 1.0, 1));

            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(0.95, 900000, 0.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.S, Utility.GetQuitStatusValue(0.95, 900000, 1.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.APlus, Utility.GetQuitStatusValue(0.95, 900000, 1.0, 2));

            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(1.0, 900000, 0.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.SPlus, Utility.GetQuitStatusValue(1.0, 900000, 1.0, 1));
            Assert.Equal(DefaultCompute.QuitStatus.APlus, Utility.GetQuitStatusValue(1.0, 900000, 1.0, 2));

            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(0.95, 1800000, 0.0, 2));
            Assert.Equal(DefaultCompute.QuitStatus.S, Utility.GetQuitStatusValue(0.95, 1800000, 1.0, 2));

            Assert.Equal(DefaultCompute.QuitStatus.F, Utility.GetQuitStatusValue(1.0, 1800000, 0.0, 2));
            Assert.Equal(DefaultCompute.QuitStatus.SPlus, Utility.GetQuitStatusValue(1.0, 1800000, 1.0, 2));
        }

        [Fact]
        public void CompileSiteYells()
        {
            Assert.Equal("http://taehui.ddns.net", Utility.CompileSiteYells("http://taehui.ddns.net"));
            Assert.Equal("https://taehui.ddns.net", Utility.CompileSiteYells("https://taehui.ddns.net"));
            Assert.Equal("mailto://taehui@taehui.ddns.net", Utility.CompileSiteYells("mailto://taehui@taehui.ddns.net"));
            Assert.Equal(string.Empty, Utility.CompileSiteYells("taehui.ddns.net"));
        }

        [Fact]
        public void NewValue()
        {
            var valueMap = new Dictionary<int, List<int>>();

            Utility.NewValue(valueMap, 0, 0);
            Assert.Single(valueMap);
            Assert.Single(valueMap[0]);
            Utility.NewValue(valueMap, 0, 1);
            Assert.Single(valueMap);
            Assert.Equal(2, valueMap[0].Count);
            Utility.NewValue(valueMap, 0, 2);
            Assert.Single(valueMap);
            Assert.Equal(3, valueMap[0].Count);

            Utility.NewValue(valueMap, 1, 0);
            Assert.Equal(2, valueMap.Count);
            Assert.Single(valueMap[1]);
            Utility.NewValue(valueMap, 1, 1);
            Assert.Equal(2, valueMap.Count);
            Assert.Equal(2, valueMap[1].Count);
            Utility.NewValue(valueMap, 1, 2);
            Assert.Equal(2, valueMap.Count);
            Assert.Equal(3, valueMap[1].Count);

            Utility.NewValue(valueMap, 2, 0);
            Assert.Equal(3, valueMap.Count);
            Assert.Single(valueMap[2]);
            Utility.NewValue(valueMap, 2, 1);
            Assert.Equal(3, valueMap.Count);
            Assert.Equal(2, valueMap[2].Count);
            Utility.NewValue(valueMap, 2, 2);
            Assert.Equal(3, valueMap.Count);
            Assert.Equal(3, valueMap[2].Count);
        }
    }
}
