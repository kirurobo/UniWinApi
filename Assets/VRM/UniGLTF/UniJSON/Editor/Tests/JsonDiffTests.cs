using NUnit.Framework;
using System.Linq;


namespace UniJSON
{
    public class JsonDiffTests
    {
        [Test]
        public void PathTest()
        {
            var json=@"
{
    ""a"": [
        {
            ""aa"": 1
        }       
    ]
}
";
            var root = JsonParser.Parse(json);

            {
                var it = root.Traverse().GetEnumerator();
                it.MoveNext(); Assert.AreEqual("/", new JsonPointer(it.Current).ToString());
                it.MoveNext(); Assert.AreEqual("/a", new JsonPointer(it.Current).ToString());
                it.MoveNext(); Assert.AreEqual("/a/0", new JsonPointer(it.Current).ToString());
                it.MoveNext(); Assert.AreEqual("/a/0/aa", new JsonPointer(it.Current).ToString());
                Assert.False(it.MoveNext());
            }

            {
                var it = root.Traverse().GetEnumerator();
                root.SetValue("/a", "JsonPath");
                it.MoveNext(); Assert.AreEqual("/", new JsonPointer(it.Current).ToString());
                it.MoveNext(); Assert.AreEqual("/a", new JsonPointer(it.Current).ToString());
                Assert.False(it.MoveNext());
            }
        }

        [Test]
        public void DiffTest()
        {
            var a = @"{
""a"": 1
}";

            var b = @"{
}";

            var diff = JsonParser.Parse(a).Diff(JsonParser.Parse(b)).ToArray();
            Assert.AreEqual(1, diff.Length);
        }
    }
}
