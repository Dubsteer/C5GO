using LogicLayer.FormModels;

namespace Unit_Tests
{
    [TestClass]
    public class TestMatchFormModel
    {
        [TestMethod]
        public void ConstructorInitializesProvidedValues()
        {
            var model = new MatchFormModel(42, "Final match", "Grand Final");

            Assert.AreEqual(42, model.Id);
            Assert.AreEqual("Final match", model.Description);
            Assert.AreEqual("Grand Final", model.Name);
        }
    }
}
