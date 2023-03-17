using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class HashIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.Hash";

    [Theory]
    [InlineData("3", "{}", true)]
    [InlineData("'hello😀', 'SHA1'", "{}", true)] // wrong algo id
    [InlineData("'hello😀', 'MD5'", "{}", false, "'4beb420d66a45714b1bf0dece798fccc'")]
    [InlineData("'hello😀', 'SHA-1'", "{}", false, "'f49ad76256fa8f598c57f74c96b445add3a9de28'")]
    [InlineData("'hello😀', 'SHA-256'", "{}", false,
        "'43e085c2a106c941e8b30167304570382e9c168aee0d68eb8832b13baf3393a0'")]
    [InlineData("'hello😀', 'SHA-384'", "{}", false,
        "'bafae3c341153e8cd4b5cb5e78330b119b5d0a910f6c3ed70b337d5bc8f220889bbc9bdab6a20efc66dae6073c942c2e'")]
    [InlineData("'hello😀', 'SHA-512'", "{}", false,
        "'3ef321a33edd7c3af109bb255ad38dfbf6b33dd6ccb1a60f39a5afa20dcb89cc" +
        "a7bd2748d6e482d3657d5a5e83aea53625bc685a07496ca65b04c1838f7ae07c'")]
    public void TestHash(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);
}