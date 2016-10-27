using Metric;
using Metric.Extensions;
using Xunit;

namespace UnitTest
{
    public class ExtensionsTest
    {
        [Fact]
        public void ExtensionsMultiply()
        {
            Assert.Equal(6.m(2), 2.m() * 3.m());
        }

        [Fact]
        public void ExtensionsAdd()
        {
            Assert.Equal(2.m(Prefix.k), 1000.m() + 1000.m());
        }
        [Fact]
        public void ExtensionsDerived()
        {
            Assert.Equal(2.m(Prefix.k) * 3.g() / 1.s(2), 6.N());
        }
    }
}