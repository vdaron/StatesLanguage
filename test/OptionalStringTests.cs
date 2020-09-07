using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using StatesLanguage.Serialization;
using StatesLanguage.States;
using Xunit;

namespace StatesLanguage.Tests
{
    public class OptionalStringTests
    {
        [Fact]
        public void TestSerializingNotSet()
        {
            var json = Serialize(new TestOptionalString());
            Assert.Equal("{}", json);
        }

        [Fact]
        public void TestSerializingNull()
        {
            var json = Serialize(new TestOptionalString {Test = null});
            Assert.Equal("{\"Test\":null}", json);
        }

        [Fact]
        public void TestSerializingValue()
        {
            var json = Serialize(new TestOptionalString {Test = "test"});
            Assert.Equal("{\"Test\":\"test\"}", json);
        }

        [Fact]
        public void TestDeSerializingNotSet()
        {
            var s = DeSerialize("{}");
            Assert.False(s.Test.IsSet);
        }

        [Fact]
        public void TestDeserializingNull()
        {
            var s = DeSerialize("{\"Test\":null}");
            Assert.True(s.Test.IsSet);
            Assert.False(s.Test.HasValue);
            Assert.Null(s.Test.Value);
            Assert.Null((string) s.Test);
        }

        [Fact]
        public void TestDeserializingValue()
        {
            var s = DeSerialize("{\"Test\":\"test\"}");
            Assert.True(s.Test.IsSet);
            Assert.True(s.Test.HasValue);
            Assert.Equal("test", s.Test.Value);
            Assert.Equal("test", s.Test);
        }

        [Fact]
        [SuppressMessage("ReSharper", "EqualExpressionComparison")]
        public void TestOptionalStringEquality()
        {
            Assert.True(new OptionalString() == new OptionalString());
            Assert.True(new OptionalString(null) == new OptionalString(null));
            Assert.False(new OptionalString() == new OptionalString("test"));
            Assert.False(new OptionalString() == new OptionalString(null));
            Assert.False(new OptionalString("test") == new OptionalString(null));
            
            Assert.False(new OptionalString() != new OptionalString());
            Assert.False(new OptionalString(null) != new OptionalString(null));
            Assert.True(new OptionalString() != new OptionalString("test"));
            Assert.True(new OptionalString() != new OptionalString(null));
            Assert.True(new OptionalString("test") != new OptionalString(null));
        }

        private string Serialize(TestOptionalString s)
        {
            return JsonConvert.SerializeObject(s, new JsonSerializerSettings
            {
                ContractResolver = new StatesContractResolver()
            });
        }

        private TestOptionalString DeSerialize(string json)
        {
            return JsonConvert.DeserializeObject<TestOptionalString>(json, new JsonSerializerSettings
            {
                ContractResolver = new StatesContractResolver()
            });
        }

        private class TestOptionalString
        {
            public OptionalString Test { get; set; }
        }
    }
}