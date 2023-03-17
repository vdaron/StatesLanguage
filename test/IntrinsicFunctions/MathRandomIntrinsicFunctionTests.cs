using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StatesLanguage.Internal.Validation;
using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class MathRandomIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.MathRandom";

    [Theory]
    [InlineData("3, '4'", "{}", true)]
    [InlineData("3, 4", "{}", false)]
    [InlineData("3, 4, 0", "{}", false)]
    public void TestMathRandomBasic(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);

    [Theory]
    [InlineData(1, 10)]
    [InlineData(1, 10, 1000)]
    public void TestMathRandom(int left, int right, int? seed = null)
    {
        var paramList = new List<int> {left, right};
        if (seed.HasValue)
        {
            paramList.Add(seed.Value);
        }

        var f = IntrinsicFunction.Parse($"{FUNCTION_NAME}({string.Join(",", paramList)})");
        var res = _registry.CallFunction(f, new JObject(), new JObject());

        Assert.Equal(JTokenType.Integer, res.Type);

        var resInt = res.Value<int>();

        Assert.True(resInt >= left);
        Assert.True(resInt <= right);

        if (seed.HasValue)
        {
            var res2 = _registry.CallFunction(f, new JObject(), new JObject());
            var res2Int = res2.Value<int>();
            Assert.Equal(resInt, res2Int);
        }
    }
}