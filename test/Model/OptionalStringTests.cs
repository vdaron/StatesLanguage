using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json;
using StatesLanguage.Model.Serialization;
using StatesLanguage.Model.States;
using Xunit;

namespace StatesLanguage.Tests.Model
{
    public class OptionalStringTests
    {
        class TestOptionalString
        {
            public OptionalString Test { get; set; }
        }

        [Fact]
        public void TestSerializingNotSet()
        {
            string json = Serialize(new TestOptionalString());
            Assert.Equal("{}",json);
        }
        [Fact]
        public void TestSerializingNull()
        {
            string json = Serialize(new TestOptionalString{Test = null});
            Assert.Equal("{\"Test\":null}",json);
        }
        [Fact]
        public void TestSerializingValue()
        {
            string json = Serialize(new TestOptionalString{Test = "test"});
            Assert.Equal("{\"Test\":\"test\"}",json);
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
            Assert.Null((string)s.Test);
        }
        [Fact]
        public void TestDeserializingValue()
        {
            var s = DeSerialize("{\"Test\":\"test\"}");
            Assert.True(s.Test.IsSet);
            Assert.True(s.Test.HasValue);
            Assert.Equal("test",s.Test.Value);
            Assert.Equal("test",s.Test);
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
    }
}