using StatesLanguage.Internal.Validation;
using StatesLanguage.IntrinsicFunctions;
using Xunit;

namespace StatesLanguage.Tests.IntrinsicFunctions
{
    public class IntrinsicFunctionTests
    {
        [Fact]
        public void OneParameterIntrinsicFunction()
        {
            var f = IntrinsicFunction.Parse("Test('hello')");
            Assert.Equal("Test", f.Name);
            Assert.Single(f.Parameters);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("hello", ((StringIntrinsicParam) f.Parameters[0]).Value);

            f = IntrinsicFunction.Parse("Test($.hello)");
            Assert.Equal("Test", f.Name);
            Assert.Single(f.Parameters);
            Assert.IsType<PathIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("$.hello", ((PathIntrinsicParam) f.Parameters[0]).Path);

            f = IntrinsicFunction.Parse("Test(33.45)");
            Assert.Equal("Test", f.Name);
            Assert.Single(f.Parameters);
            Assert.IsType<DecimalIntrinsicParam>(f.Parameters[0]);
            Assert.Equal(33.45m, ((DecimalIntrinsicParam) f.Parameters[0]).Number);

            f = IntrinsicFunction.Parse("Test(null)");
            Assert.Equal("Test", f.Name);
            Assert.Single(f.Parameters);
            Assert.IsType<NullIntrinsicParam>(f.Parameters[0]);
        }

        [Theory]
        [InlineData("Test(   $.test)")]
        [InlineData("Test($.test   )")]
        [InlineData("Test(    $.test   )")]
        public void IgnoreWhiteSpacesAroundUnquotePathParam(string test)
        {
            var f = IntrinsicFunction.Parse(test);
            Assert.Equal("Test", f.Name);
            Assert.Single(f.Parameters);
            Assert.IsType<PathIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("$.test", ((PathIntrinsicParam) f.Parameters[0]).Path);
        }

        [Fact]
        public void IgnoreWhiteSpacesAroundComplex()
        {
            var f = IntrinsicFunction.Parse("test(   1    ,  'a'  , a(  $p  , 'test'  )  )  ");
            Assert.Equal("test", f.Name);
            Assert.Equal(3, f.Parameters.Length);
            Assert.IsType<IntegerIntrinsicParam>(f.Parameters[0]);
            Assert.Equal(1, ((IntegerIntrinsicParam) f.Parameters[0]).Number);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[1]);
            Assert.Equal("a", ((StringIntrinsicParam) f.Parameters[1]).Value);
            Assert.IsType<IntrinsicFunction>(f.Parameters[2]);

            var f2 = (IntrinsicFunction) f.Parameters[2];
            Assert.Equal("a", f2.Name);
            Assert.Equal(2, f2.Parameters.Length);
            Assert.IsType<PathIntrinsicParam>(f2.Parameters[0]);
            Assert.Equal("$p", ((PathIntrinsicParam) f2.Parameters[0]).Path);
            Assert.IsType<StringIntrinsicParam>(f2.Parameters[1]);
            Assert.Equal("test", ((StringIntrinsicParam) f2.Parameters[1]).Value);
        }

        [Theory]
        [InlineData("Test(   'hello')")]
        [InlineData("Test('hello'   )")]
        [InlineData("Test(    'hello'   )")]
        public void IgnoreWhiteSpacesAroundQuoteStringParam(string test)
        {
            var f = IntrinsicFunction.Parse(test);
            Assert.Equal("Test", f.Name);
            Assert.Single(f.Parameters);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("hello", ((StringIntrinsicParam) f.Parameters[0]).Value);
        }

        [Theory]
        [InlineData("Test(   'hello','world')")]
        [InlineData("Test('hello'   ,'world'   )")]
        [InlineData("Test('hello'   ,    'world'   )")]
        public void IgnoreWhiteSpacesAroundTwoQuoteStringParam(string test)
        {
            var f = IntrinsicFunction.Parse(test);
            Assert.Equal("Test", f.Name);
            Assert.Equal(2, f.Parameters.Length);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("hello", ((StringIntrinsicParam) f.Parameters[0]).Value);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[1]);
            Assert.Equal("world", ((StringIntrinsicParam) f.Parameters[1]).Value);
        }

        [Fact]
        public void OneParameterIntrinsicFunctionWithDotAndUnder()
        {
            var f = IntrinsicFunction.Parse("Tes.t_('hello')");
            Assert.Equal("Tes.t_", f.Name);
            Assert.Single(f.Parameters);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("hello", ((StringIntrinsicParam) f.Parameters[0]).Value);
        }

        [Fact]
        public void TwoParametersIntrinsicFunction()
        {
            var f = IntrinsicFunction.Parse("Test('hello',33)");
            Assert.Equal("Test", f.Name);
            Assert.Equal(2, f.Parameters.Length);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("hello", ((StringIntrinsicParam) f.Parameters[0]).Value);
            Assert.IsType<IntegerIntrinsicParam>(f.Parameters[1]);
            Assert.Equal(33, ((IntegerIntrinsicParam) f.Parameters[1]).Number);
        }

        [Fact]
        public void ThreeParametersIntrinsicFunction()
        {
            var f = IntrinsicFunction.Parse("Test('hello',33,null)");
            Assert.Equal("Test", f.Name);
            Assert.Equal(3, f.Parameters.Length);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("hello", ((StringIntrinsicParam) f.Parameters[0]).Value);
            Assert.IsType<IntegerIntrinsicParam>(f.Parameters[1]);
            Assert.Equal(33, ((IntegerIntrinsicParam) f.Parameters[1]).Number);
            Assert.IsType<NullIntrinsicParam>(f.Parameters[2]);
        }

        [Fact]
        public void FourParametersIntrinsicFunction()
        {
            var f = IntrinsicFunction.Parse("Test('hello',33,null,$.test)");
            Assert.Equal("Test", f.Name);
            Assert.Equal(4, f.Parameters.Length);
            Assert.IsType<StringIntrinsicParam>(f.Parameters[0]);
            Assert.Equal("hello", ((StringIntrinsicParam) f.Parameters[0]).Value);
            Assert.IsType<IntegerIntrinsicParam>(f.Parameters[1]);
            Assert.Equal(33, ((IntegerIntrinsicParam) f.Parameters[1]).Number);
            Assert.IsType<NullIntrinsicParam>(f.Parameters[2]);
            Assert.IsType<PathIntrinsicParam>(f.Parameters[3]);
            Assert.Equal("$.test", ((PathIntrinsicParam) f.Parameters[3]).Path);
        }

        [Fact]
        public void SubIntrinsicFunctiontest()
        {
            var f = IntrinsicFunction.Parse("Test(Test($.test))");
            Assert.Equal("Test", f.Name);
            Assert.Single(f.Parameters);
            Assert.IsType<IntrinsicFunction>(f.Parameters[0]);
            Assert.Equal("Test", ((IntrinsicFunction) f.Parameters[0]).Name);
            Assert.IsType<PathIntrinsicParam>(((IntrinsicFunction) f.Parameters[0]).Parameters[0]);
            Assert.Equal("$.test", ((PathIntrinsicParam) ((IntrinsicFunction) f.Parameters[0]).Parameters[0]).Path);
        }

        [Theory]
        [InlineData("S(")]
        [InlineData("S(test,)")]
        [InlineData("S(test,")]
        [InlineData("S('test',")]
        [InlineData("S(,rest)")]
        [InlineData("$ss")]
        [InlineData("ss")]
        [InlineData("ss)")]
        [InlineData("s('\\')")]
        [InlineData("s(hello)")]
        [InlineData("s('hello\\')")]
        [InlineData("s('hello\\")]
        public void IntrinsicFunctionParserError(string functionDefinition)
        {
            Assert.Throws<InvalidIntrinsicFunctionException>(() => IntrinsicFunction.Parse(functionDefinition));
        }
    }
}