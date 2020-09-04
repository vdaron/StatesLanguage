using System.Linq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions
{
    public class ArrayIntrinsicFunctionTests
    {
        private readonly IntrinsicFunctionRegistry _registry = new IntrinsicFunctionRegistry();

        [Theory]
        [InlineData("States.Array()", 0)]
        [InlineData("States.Array(null)", 1)]
        [InlineData("States.Array(null,'test')", 2)]
        [InlineData("States.Array(null,'test', $.path, States.Array())", 4)]
        public void TestArrayParameterCount(string definition, int count)
        {
            var f = IntrinsicFunction.Parse(definition);
            var r = _registry.CallFunction(f, "'a'", new JObject());
            Assert.True(r is JArray);
            Assert.Equal(count, r.Count());
        }

        [Fact]
        public void TestObjectArrayItem()
        {
            var f = IntrinsicFunction.Parse("States.Array($.a)");
            var r = _registry.CallFunction(f, JObject.Parse("{'a':{'b':33}}"), new JObject());
            Assert.True(r is JArray);
            Assert.Equal(JToken.Parse("{'b':33}"), r.First);
        }
    }
}