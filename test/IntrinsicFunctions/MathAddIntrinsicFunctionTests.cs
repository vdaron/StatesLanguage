using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class MathAddIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.MathAdd";

    [Theory]
    [InlineData("3, '4'", "{}", true)]
    [InlineData("$.a, $.b", "{'a': 3, 'b': {}}", true)]
    [InlineData("3, 4", "{}", false, "7")]
    [InlineData("-1, 4", "{}", false, "3")]
    [InlineData("-1.1, 4", "{}", false, "2.9")]
    [InlineData("-1.1, 43.786", "{}", false, "42.686")]
    [InlineData("-1.1e-1, +0.43786E+2", "{}", false, "43.676")]
    public void TestMathAdd(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}