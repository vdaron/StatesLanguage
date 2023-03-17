using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class ArrayLengthIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.ArrayLength";

    [Theory]
    [InlineData("42", "{}", true)]
    [InlineData("null", "{}", true)]
    [InlineData("$.array", "{'array': [1,2,3,4,5]}", false, "5")]
    [InlineData("$.array", "{'array': [1,2,{'hi': 4},4]}", false, "4")]
    [InlineData("$.array", "{'array': []}", false, "0")]
    public void TestArrayLength(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}