using NUnit.Framework;


namespace UniJSON
{
    public class ParserTests
    {
        [Test]
        public void Tests()
        {
            {
                var result = JsonParser.Parse("1");
                Assert.AreEqual(1, result.GetInt32());
            }

            {
                var result = JsonParser.Parse("{ \"a\": { \"b\": 1 }}");
                Assert.True(result.ContainsKey("a"));
            }
        }
    }
}
