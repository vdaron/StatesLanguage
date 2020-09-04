using Newtonsoft.Json.Linq;
using StatesLanguage.Internal.Validation;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions
{
    public class JsonToStringIntrinsicFunctionTests
    {
        private readonly IntrinsicFunctionRegistry _registry = new IntrinsicFunctionRegistry();

        [Fact]
        public void TestJsonPathArray()
        {
            var f = IntrinsicFunction.Parse("States.JsonToString($.a)");
            var res = _registry.CallFunction(f, JObject.Parse("{'a':[1,2,3,4]}"), new JObject());
            Assert.Equal("[1,2,3,4]", res);
        }

        [Fact]
        public void TestJsonPathObject()
        {
            var f = IntrinsicFunction.Parse("States.JsonToString($.a)");
            var res = _registry.CallFunction(f, JObject.Parse("{'a':{'b':1,'c':'test'}}"), new JObject());
            Assert.Equal("{\"b\":1,\"c\":\"test\"}", res.Value<string>());
        }

        [Theory]
        [InlineData("States.JsonToString()", "'a'")]
        [InlineData("States.JsonToString(33)", "'a'")]
        [InlineData("States.JsonToString(null)", "'a'")]
        public void TestJsonToStringError(string function, string input)
        {
            var f = IntrinsicFunction.Parse(function);
            Assert.Throws<InvalidIntrinsicFunctionException>(() =>
                _registry.CallFunction(f, JToken.Parse(input), new JObject()));
        }
    }
}