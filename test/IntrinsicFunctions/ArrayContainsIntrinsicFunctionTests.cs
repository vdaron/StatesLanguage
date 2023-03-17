using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class ArrayContainsIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.ArrayContains";

    [Theory]
    [InlineData("42", "{}", true)]
    [InlineData("null, 0", "{}", false, "false")]
    [InlineData("$.array, 2", "{'array': [1,2,3,4,5]}", false, "true")]
    public void TestArrayContains(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}