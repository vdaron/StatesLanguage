using Newtonsoft.Json.Linq;
using StatesLanguage.Internal.Validation;
using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions
{
    public class StringToJsonIntrinsicFunctionTests
    {
        private readonly IntrinsicFunctionRegistry _registry = new IntrinsicFunctionRegistry();

        [Fact]
        public void TestRawJson()
        {
            var f = IntrinsicFunction.Parse("States.StringToJson('{\"number\": 20}')");
            var res = _registry.CallFunction(f, "'a'", new JObject());
            Assert.Equal(JObject.Parse("{'number': 20}"), res);
        }

        [Fact]
        public void TestJsonPath()
        {
            var f = IntrinsicFunction.Parse("States.StringToJson($.someString)");
            var res = _registry.CallFunction(f,
                JObject.Parse("{'someString': '{\\\"number\\\": 20}','zebra': 'stripe'}"), new JObject());
            Assert.Equal(JObject.Parse("{'number': 20}"), res);
        }

        [Fact]
        public void TestIntrinsicFunction()
        {
            var f = IntrinsicFunction.Parse("States.StringToJson(States.JsonToString($.a))");
            var res = _registry.CallFunction(f, JObject.Parse("{'a':[1,2,3,4]}"), new JObject());
            Assert.Equal(JArray.Parse("[1,2,3,4]"), res);
        }

        [Theory]
        [InlineData("States.StringToJson()", "'a'")]
        [InlineData("States.StringToJson(33)", "'a'")]
        [InlineData("States.StringToJson(null)", "'a'")]
        public void TestStringToJsonError(string function, string input)
        {
            var f = IntrinsicFunction.Parse(function);
            Assert.Throws<InvalidIntrinsicFunctionException>(() =>
                _registry.CallFunction(f, JToken.Parse(input), new JObject()));
        }
    }
}