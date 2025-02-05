using StatesLanguage.Internal.Validation;
using StatesLanguage.ReferencePaths;
using Xunit;

namespace StatesLanguage.Tests
{
    public class ReferencePathTests
    {
        [Fact]
        public void TestDefaultReferencePath()
        {
            var path = ReferencePath.Parse("$");
            Assert.Equal("$", path.Path);
            Assert.Empty(path.Parts);
        }

        [Theory]
        [InlineData("$.test")]
        [InlineData("$['test']")]
        public void TestSimplePartPropertyReferencePath(string test)
        {
            var path = ReferencePath.Parse(test);
            Assert.Single(path.Parts);
            Assert.True(path.Parts[0] is FieldToken);
            Assert.Equal("test", (path.Parts[0] as FieldToken)?.Name);
        }

        [Fact]
        public void TestSimplePartArrayIndexReferencePath()
        {
            var path = ReferencePath.Parse("$[12]");
            Assert.Single(path.Parts);
            Assert.True(path.Parts[0] is ArrayIndexToken);
            Assert.Equal(12, (path.Parts[0] as ArrayIndexToken)?.Index);
        }

        [Fact]
        public void TestTwoPartArrayIndexReferencePath()
        {
            var path = ReferencePath.Parse("$[12][10]");
            Assert.Equal(2, path.Parts.Count);
            Assert.True(path.Parts[0] is ArrayIndexToken);
            Assert.Equal(12, (path.Parts[0] as ArrayIndexToken)?.Index);
            Assert.True(path.Parts[1] is ArrayIndexToken);
            Assert.Equal(10, (path.Parts[1] as ArrayIndexToken)?.Index);
        }


        [Theory]
        [InlineData("$.store.book")]
        [InlineData("$['store'].book")]
        [InlineData("$.store['book']")]
        [InlineData("$['store']['book']")]
        [InlineData("$.\\stor\\e.boo\\k")]
        public void TestTwoPartPropertyReferencePath(string test)
        {
            var path = ReferencePath.Parse(test);
            Assert.Equal(2, path.Parts.Count);
            Assert.True(path.Parts[0] is FieldToken);
            Assert.Equal("store", (path.Parts[0] as FieldToken)?.Name);
            Assert.True(path.Parts[1] is FieldToken);
            Assert.Equal("book", (path.Parts[1] as FieldToken)?.Name);
        }

        [Fact]
        public void TestMixedReferencePath()
        {
            var path = ReferencePath.Parse("$.ledgers[0][22][315].foo");
            Assert.Equal(5, path.Parts.Count);
            Assert.Equal("ledgers", (path.Parts[0] as FieldToken)?.Name);
            Assert.Equal(0, (path.Parts[1] as ArrayIndexToken)?.Index);
            Assert.Equal(22, (path.Parts[2] as ArrayIndexToken)?.Index);
            Assert.Equal(315, (path.Parts[3] as ArrayIndexToken)?.Index);
            Assert.Equal("foo", (path.Parts[4] as FieldToken)?.Name);
        }

        [Fact]
        public void TestUnicodeReferencePath()
        {
            var path = ReferencePath.Parse("$.&Ж中.\uD800\uDF46");
            Assert.Equal(2, path.Parts.Count);
            Assert.Equal("&Ж中", (path.Parts[0] as FieldToken)?.Name);
            Assert.Equal("\uD800\uDF46", (path.Parts[1] as FieldToken)?.Name);
        }

        [Theory]
        [InlineData("$.store\\.book")]
        [InlineData("$['store.book']")]
        public void TestEscapeSequenceSingleReferencePath(string test)
        {
            var path = ReferencePath.Parse(test);

            Assert.Single(path.Parts);
            Assert.Equal("store.book", (path.Parts[0] as FieldToken)?.Name);
        }


        [Theory]
        [InlineData("$.foo.\\.bar")]
        [InlineData("$['foo']['\\.bar']")]
        public void TestEscapeSequenceTwoReferencePath(string test)
        {
            var path = ReferencePath.Parse(test);
            Assert.Equal(2, path.Parts.Count);
            Assert.Equal("foo", (path.Parts[0] as FieldToken)?.Name);
            Assert.Equal(".bar", (path.Parts[1] as FieldToken)?.Name);
        }

        [Theory]
        [InlineData(@"$.foo\@bar.baz\[\[.\?pretty")]
        [InlineData(@"$['foo@bar']['baz[[']['?pretty']")]
        public void TestEscapeSequenceTwoReferencePathWeird(string test)
        {
            var path = ReferencePath.Parse(test);
            Assert.Equal(test, path.Path);
            Assert.Equal(3, path.Parts.Count);
            Assert.Equal("foo@bar", (path.Parts[0] as FieldToken)?.Name);
            Assert.Equal("baz[[", (path.Parts[1] as FieldToken)?.Name);
            Assert.Equal("?pretty", (path.Parts[2] as FieldToken)?.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData("$a")]
        [InlineData("$['dfdf")]
        [InlineData("$['dfdf'")]
        [InlineData("$.dfdf[")]
        [InlineData("$.eded.['dfdf']")]
        [InlineData("$.ede?")]
        [InlineData("$.*")]
        [InlineData("$..")]
        [InlineData("$.sdsd[0]..")]
        [InlineData("$[12aa]")]
        [InlineData("$[12,23]")]
        [InlineData("$[*]")]
        [InlineData("$[12:23]")]
        [InlineData("$['test','test2']")]
        [InlineData("$['test'wrong]")]
        public void InvalidReferencePath(string failed)
        {
            Assert.Throws<InvalidReferencePathException>(() => ReferencePath.Parse(failed));
        }

        [Theory]
        [InlineData("$.test")]
        [InlineData("$['test']")]
        public void TestSimplePropertyReferencePathTryParse(string test)
        {
            var check = ReferencePath.TryParse(test, out var path);
            Assert.Single(path.Parts);
            Assert.True(path.Parts[0] is FieldToken);
            Assert.Equal("test", (path.Parts[0] as FieldToken)?.Name);
        }

        [Fact]
        public void TestSimpleArrayIndexReferencePathTryParse()
        {
            var check = ReferencePath.TryParse("$[12]", out var path);
            Assert.True(check);
            Assert.True(path.Parts[0] is ArrayIndexToken);
            Assert.Equal(12, (path.Parts[0] as ArrayIndexToken)?.Index);
        }
    }
}