using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class StringSplitIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.StringSplit";

    [Theory]
    [InlineData("3", "{}", true)]
    [InlineData("'1,2,3,,4', ''", "{}", true)]
    [InlineData("'1,2,3,,4', ','", "{}", false, "['1', '2', '3', '', '4']")]
    [InlineData("'1,2,3,,4', ',,'", "{}", false, "['1,2,3', '4']")]
    public void TestStringSplit(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}