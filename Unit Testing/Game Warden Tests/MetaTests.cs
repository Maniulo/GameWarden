using GameWarden;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTesting
{
    [TestClass]
    public class MetaTests
    {
        [TestMethod]
        public void ItemTest()
        {
            var i = new Meta();
            i["Key"] = "Value";
            i["Key2"] = "Value2";
            Assert.AreEqual("Value", i["Key"]);
            Assert.AreEqual("Value2", i["Key2"]);
        }
    }
}
