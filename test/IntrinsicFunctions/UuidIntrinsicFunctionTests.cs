using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions;

public class UuidIntrinsicFunctionTests
{
    private readonly IntrinsicFunctionRegistry _registry = new();
    private const string FUNCTION_NAME = "States.UUID";

    [Theory]
    [InlineData("", "{}", false)]
    [InlineData("3", "{}", true)]
    public void TestUuidBasic(string parameterString, string inputStr, bool mustThrow, string expected = null) =>
        IntrinsicFunctionTests.GenericIntrinsicFunctionTest(
            _registry, FUNCTION_NAME, parameterString, inputStr, mustThrow, expected);

    [Fact]
    public void TestUuid()
    {
        var f = IntrinsicFunction.Parse($"{FUNCTION_NAME}()");

        var res = _registry.CallFunction(f, new JObject(), new JObject());
        Assert.Equal(JTokenType.String, res.Type);
        var resUuidString = res.Value<string>();

        var res2 = _registry.CallFunction(f, new JObject(), new JObject());
        Assert.Equal(JTokenType.String, res2.Type);
        var res2UuidString = res2.Value<string>();

        Assert.True(Guid.TryParse(resUuidString, out var uuid1));
        Assert.True(Guid.TryParse(res2UuidString, out var uuid2));
        Assert.NotEqual(uuid1, uuid2);
    }
}