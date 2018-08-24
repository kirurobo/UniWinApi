using NUnit.Framework;
using System.IO;
using System.Text;
using UnityEngine;

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

        [Test]
        public void PathGithub()
        {
            var path = Path.GetFullPath(Application.dataPath + "/../tmp.json");
            if(File.Exists(path))
            {
                var json = File.ReadAllText(path, Encoding.UTF8);
                var result = JsonParser.Parse(json);
            }
        }
    }
}
