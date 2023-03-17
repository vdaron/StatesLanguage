using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class JsonMergeIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.JsonMerge";

    [Theory]
    [InlineData("3, '4'", "{}", true)]
    [InlineData("$.a, $.b, false", "{'a': 3, 'b': {}}", true)]
    [InlineData("$.a, $.b, false", "{'a': {}, 'b': {}}", false, "{}")]
    [InlineData("$.a, $.b, true", "{'a': {}, 'b': {}}", false, "{}")]
    [InlineData("$.a, $.b, false",
        "{'a': {'hi': 1, 'hoi': {'a': 1}, 'hu': 'alpha'}, 'b': {'hi': 3, 'hoi': {'b': 2}, 'hey': true}}", false,
        "{'hi': 3, 'hoi': {'b': 2}, 'hu': 'alpha', 'hey': true}")]
    [InlineData("$.a, $.b, true",
        "{'a': {'hi': 1, 'hoi': {'a': 1}, 'hu': 'alpha'}, 'b': {'hi': 3, 'hoi': {'b': 2}, 'hey': true}}", false,
        "{'hi': 3, 'hoi': {'a': 1, 'b': 2}, 'hu': 'alpha', 'hey': true}")]
    public void TestJsonMerge(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}