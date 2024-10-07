using CTLLunch.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCTLLunch
{
    [TestClass]
    public class UnitTest1
    {
        private IReserve Reserve;
        public UnitTest1(IReserve _Reserve)
        {
             Reserve = _Reserve;
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
