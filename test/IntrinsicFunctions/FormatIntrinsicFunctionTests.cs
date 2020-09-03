using Newtonsoft.Json.Linq;
using StatesLanguage.Model;
using StatesLanguage.Model.Internal.Validation;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions
{
    public class FormatIntrinsicFunctionTests
    {
        private readonly IntrinsicFunctionRegistry _registry = new IntrinsicFunctionRegistry();

        [Fact]
        public void TestWithoutArgument()
        {
            var f = IntrinsicFunction.Parse("States.Format('value')");

            var r = _registry.CallFunction(f, "test", new JObject());
            Assert.Equal("value", r);
        }

        [Fact(Skip = "Check if it is supported")]
        public void TestWithoutArgumentFromPath()
        {
            var f = IntrinsicFunction.Parse("States.Format($.p)");

            var r = _registry.CallFunction(f, JToken.Parse("{'p':'value'}"), new JObject());
            Assert.Equal("value", r.Value<string>());
        }

        [Fact]
        public void TestWithStringArgument()
        {
            var f = IntrinsicFunction.Parse("States.Format('hello {}','world')");

            var r = _registry.CallFunction(f, "test", new JObject());
            Assert.Equal("hello world", r.Value<string>());
        }

        [Fact]
        public void TestWithNumberArgument()
        {
            var f = IntrinsicFunction.Parse("States.Format('hello {}',12.34)");

            var r = _registry.CallFunction(f, "test", new JObject());
            Assert.Equal("hello 12.34", r.Value<string>());
        }

        [Fact]
        public void TestNullArgument()
        {
            var f = IntrinsicFunction.Parse("States.Format('hello {}', null)");

            var r = _registry.CallFunction(f, "test", new JObject());
            Assert.Equal("hello ", r.Value<string>());
        }

        [Fact]
        public void TestPathArgument()
        {
            var f = IntrinsicFunction.Parse("States.Format('hello {}', $.p)");

            var r = _registry.CallFunction(f, JToken.Parse("{'p':'world'}"), new JObject());
            Assert.Equal("hello world", r.Value<string>());
        }

        [Fact]
        public void TestFunctionArgument()
        {
            var f = IntrinsicFunction.Parse("States.Format('hello {}', States.Format('world'))");

            var r = _registry.CallFunction(f, "test", new JObject());
            Assert.Equal("hello world", r.Value<string>());
        }

        [Fact]
        public void TestEscapeCharsFunctionArgument()
        {
            var f = IntrinsicFunction.Parse("States.Format('hello \\\\{\\\\}')");

            var r = _registry.CallFunction(f, "test", new JObject());
            Assert.Equal("hello {}", r.Value<string>());
        }

        [Theory]
        [InlineData("States.Format('hello world')", "'a'", "{}")]
        [InlineData("States.Format('{} {}','hello','world')", "'a'", "{}")]
        [InlineData("States.Format('{}{}{}{}{}{}',null,'hello',null,' ','world',null)", "'a'", "{}")]
        [InlineData("States.Format('hello {}', States.Format('world'))", "'a'", "{}")]
        [InlineData("States.Format('{} {}', $.a, States.Format('world'))", "{'a':'hello'}", "{}")]
        [InlineData("States.Format('{} {}', $.a, States.Format('{}',$.b))", "{'a':'hello','b':'world'}", "{}")]
        [InlineData("States.Format('{} {}', $.a, $.b)", "{'a':'hello','b':'world'}", "{}")]
        [InlineData("States.Format('{} {}', $.a, $$.b)", "{'a':'hello'}", "{'b':'world'}")]
        [InlineData("States.Format('{} {}', $.a, States.Format('{}',$$.b))", "{'a':'hello'}", "{'b':'world'}")]
        public void HelloWorldTest(string function, string input, string context)
        {
            var f = IntrinsicFunction.Parse(function);

            var r = _registry.CallFunction(f, JToken.Parse(input), JObject.Parse(context));
            Assert.Equal("hello world", r.Value<string>());
        }

        [Theory]
        [InlineData("States.Format()", "'a'", "{}")]
        [InlineData("States.Format(33)", "'a'", "{}")]
        [InlineData("States.Format(null)", "'a'", "{}")]
        [InlineData("States.Format('{}')", "'a'", "{}")]
        [InlineData("States.Format('hello',33)", "'a'", "{}")]
        [InlineData("States.Format('hello {}',$.a)", "{'a':{'b':33}}", "{}")]
        [InlineData("States.Format('hello {}',$.a)", "{'a':[]}", "{}")]
        [InlineData("States.Format('hello {}',States.Array())", "'a'", "{}")]
        public void InvalidStatesFormat(string function, string input, string context)
        {
            var f = IntrinsicFunction.Parse(function);

            Assert.Throws<InvalidIntrinsicFunctionException>(() =>
                _registry.CallFunction(f, JToken.Parse(input), JObject.Parse(context)));
        }
    }
}