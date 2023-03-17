using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class ArrayRangeIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.ArrayRange";

    [Theory]
    [InlineData("42", "{}", true)]
    [InlineData("42, 42", "{}", true)]
    [InlineData("-10, 1042, 1", "{}", true)]
    [InlineData("0, 1000000000, 1", "{}", true)]
    [InlineData("42, 42, 42", "{}", false, "[42]")]
    [InlineData("42, 41, 42", "{}", false, "[]")]
    [InlineData("41, 42, -42", "{}", false, "[]")]
    [InlineData("41, 48, 1", "{}", false, "[41, 42, 43, 44, 45, 46, 47, 48]")]
    [InlineData("41, 48, 2", "{}", false, "[41, 43, 45, 47]")]
    [InlineData("48, 41, -2", "{}", false, "[48, 46, 44, 42]")]
    [InlineData("7, -13, -5", "{}", false, "[7, 2, -3, -8, -13]")]
    public void TestArrayLength(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}