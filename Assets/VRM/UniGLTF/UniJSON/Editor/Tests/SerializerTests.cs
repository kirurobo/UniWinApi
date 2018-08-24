using NUnit.Framework;


namespace UniJSON
{
    public class SerializerTests
    {

        [Test]
        public void SerializerTestsSimplePasses()
        {
            // Use the Assert class to test conditions.

            var s = JsonSerializer.Create();

            // number
            Assert.AreEqual("0", s.Serialize(0));
        }

    }
}
