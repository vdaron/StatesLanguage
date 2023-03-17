using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class ArrayGetItemIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.ArrayGetItem";

    [Theory]
    [InlineData("42", "{}", true)]
    [InlineData("null, 0", "{}", true)]
    [InlineData("$.array, 2", "{'array': [1,2,3,4,5]}", false, "3")]
    [InlineData("$.array, 2", "{'array': [1,2,{'hi': 4},4,5]}", false, "{'hi': 4}")]
    [InlineData("$.array, 20", "{'array': [1,2,3,4,5]}", true)]
    [InlineData("$.array, -1", "{'array': [1,2,3,4,5]}", true)]
    [InlineData("$.array, 1.2", "{'array': [1,2,3,4,5]}", true)]
    public void TestArrayGetItem(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}