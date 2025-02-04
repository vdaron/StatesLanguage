using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests
{
    public class FailPathTests
    {
        private readonly IInputOutputProcessor _inputOutputProcessor = new InputOutputProcessor(new IntrinsicFunctionRegistry());

        [Fact]
        public void FailPathCanBeJsonPath()
        {
            var failInput = _inputOutputProcessor.GetFailPathValue(
                JToken.Parse(@"{""fakepath"": ""error1""}"),
                "$.fakepath",
                null,
                null);

            Assert.Equal("error1", failInput.Value<string>());
        }

        [Fact]
        public void FailPathCanBeIntrinsicFunction()
        {
            var failInput = _inputOutputProcessor.GetFailPathValue(
                JToken.Parse(@"{""Cause"": ""ABCDF 01"",""Error"": ""GFHTD 23""}"),
                "States.Format('This is a custom error message for {}, caused by {}.', $.Error, $.Cause)",
                null,
                null);

            Assert.Equal("This is a custom error message for GFHTD 23, caused by ABCDF 01.", failInput.Value<string>());
        }

        [Fact]
        public void FailPathCannotReturnSomethingElseThanString()
        {
            Assert.Throws<PathMatchFailureException>(() =>
                _inputOutputProcessor.GetFailPathValue("{}", "$.notfound", null, new JObject()));
            Assert.Throws<PathMatchFailureException>(() =>
                _inputOutputProcessor.GetFailPathValue(@"{""value"":123}", "$.value", null, null));
            Assert.Throws<PathMatchFailureException>(() =>
                _inputOutputProcessor.GetFailPathValue(@"{""value"":[1,2,3]}", "$.value", null, null));
            Assert.Throws<PathMatchFailureException>(() =>
                _inputOutputProcessor.GetFailPathValue(@"{""value"":{}}", "$.value", null, null));
            Assert.Throws<PathMatchFailureException>(() =>
                _inputOutputProcessor.GetFailPathValue(@"{}", "States.MathAdd(1,2)", null, null));
        }
    }
}