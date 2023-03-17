using System.Linq;
using Newtonsoft.Json.Linq;
using StatesLanguage.Internal.Validation;
using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class ArrayUniqueIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.ArrayUnique";

    [Theory]
    [InlineData("$.array", "{array: [7, -13, -5]}", false, "[7, -13, -5]")]
    [InlineData("$.array", "{array: [1, 2, 2, 2, 7, -13, -5]}", false, "[7, -13, -5, 1, 2]")]
    [InlineData("$.array", "{array: [{'1': 2}, {'1': 'hello'}, {'1': 'hello'}]}", false, "[{'1': 2}, {'1': 'hello'}]")]
    public void TestArrayUnique(string parameterString, string inputStr, bool mustThrow, string expected = null)
    {
        var f = IntrinsicFunction.Parse($"{FUNCTION_NAME}({parameterString})");
        var input = string.IsNullOrWhiteSpace(inputStr) ? new JObject() : JToken.Parse(inputStr);

        if (mustThrow)
        {
            Assert.Throws<InvalidIntrinsicFunctionException>(
                () => _registry.CallFunction(f, input, new JObject()));
        }
        else
        {
            var res = _registry.CallFunction(f, input, new JObject());
            if (expected != null)
            {
                var expectedToken = JToken.Parse(expected) as JArray;
                Assert.IsType<JArray>(res);
                var resToken = res as JArray;

                Assert.NotNull(resToken);
                Assert.NotNull(expectedToken);

                Assert.Equal(
                    expectedToken.Select(x => x.ToString()).OrderBy(x => x),
                    resToken.Select(x => x.ToString()).OrderBy(x => x));
            }
        }
    }
}