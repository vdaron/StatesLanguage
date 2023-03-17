using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class Base64DecodeIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.Base64Decode";

    [Theory]
    [InlineData("3", "{}", true)]
    [InlineData("'aGVsbG/wn5iA'", "{}", false, "'hello😀'")]
    public void TestBase64Decode(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}