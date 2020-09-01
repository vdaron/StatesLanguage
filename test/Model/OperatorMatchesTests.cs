using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using StatesLanguage.Model;
using StatesLanguage.Model.Conditions;
using Xunit;

namespace StatesLanguage.Tests.Model
{
    public class OperatorMatchesTests
    {
        [Fact]
        public void TestEqOperatorWithJObject()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringEquals("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericEquals("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampEquals("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.BooleanEquals("$.varbool", true)))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue" })));

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 33 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 34 })));

            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 11) })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 12) })));

            Assert.True(choices[3].Condition.Match(JObject.FromObject(new { varbool = true })));
            Assert.False(choices[3].Condition.Match(JObject.FromObject(new { varbool = false })));
            Assert.False(choices[3].Condition.Match(JToken.Parse("true")));
            Assert.False(choices[3].Condition.Match(JToken.Parse("false")));
        }

        [Fact]
        public void TestEqOperatorWithJTokens()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringEquals(null,"value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericEquals(null, 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampEquals(null, new DateTime(2018, 10, 22, 22, 33, 11))))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.BooleanEquals(null, true)))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(new JValue("value")));
            Assert.False(choices[0].Condition.Match(new JValue("not-value")));

            Assert.True(choices[1].Condition.Match(new JValue(33)));
            Assert.False(choices[1].Condition.Match(new JValue(34)));

            Assert.True(choices[2].Condition.Match(new JValue(new DateTime(2018, 10, 22, 22, 33, 11))));
            Assert.False(choices[2].Condition.Match(new JValue(new DateTime(2018, 10, 22, 22, 33, 12))));

            Assert.True(choices[3].Condition.Match(new JValue(true)));
            Assert.False(choices[3].Condition.Match(new JValue(false)));
            
        }
        
        [Fact]
        public void TestMatchOperatorWithJObject()
        {
            var c = StepFunctionBuilder.ChoiceState()
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.Match("$.varstr", "val*")))
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.Match("$.varstr", "val*ue")))
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.Match("$.varstr", "val\\*ue")))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "test" })));
            
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varstr = "value" })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varstr = "valDFDFDFue" })));
            
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { varstr = "val*ue" })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { varstr = "value" })));
        }
        
        [Fact]
        public void TestGtOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringGreaterThan("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericGreaterThan("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampGreaterThan("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "vvalue" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value" }))); //NotEqual

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 34 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 30 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 33 }))); //NotEqual

            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 20) })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 10) })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 11) })));//NotEqual
        }
        
        [Fact]
        public void TestGtPathOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringGreaterThanPath("$.varstr", "$.a")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericGreaterThanPath("$.varint", "$.a")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampGreaterThanPath("$.vardate", "$.a")))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "vvalue", a = "value" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue", a = "value" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value", a = "value" }))); //NotEqual

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 34, a = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 30, a = 33 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 33, a = 33 }))); //NotEqual

            var d = new DateTime(2018, 10, 22, 22, 33, 11);
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = d.AddDays(1), a = d })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = d.AddDays(-1), a = d })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = d, a = d })));//NotEqual
        }
        [Fact]
        public void TestGteOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringGreaterThanEquals("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericGreaterThanEquals("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampGreaterThanEquals("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "vvalue" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value" }))); //Equal

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 34 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 30 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 33 }))); //Equal

            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 20) })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 10) })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 11) })));//Equal
        }
        
        [Fact]
        public void TestGtePathOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringGreaterThanEqualsPath("$.varstr", "$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericGreaterThanEqualsPath("$.varint", "$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampGreaterThanEqualsPath("$.vardate", "$.b")))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "vvalue", b = "value" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue", b = "value" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value", b = "value" }))); //Equal

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 34, b = 33 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 30 , b = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 33 , b = 33}))); //Equal

            var d = new DateTime(2018, 10, 22, 22, 33, 20);

            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = d.AddHours(-1), b = d })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = d.AddHours(1), b = d })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = d, b = d  })));//Equal
        }
        [Fact]
        public void TestLtOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringLessThan("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericLessThan("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampLessThan("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "vvalue" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value" }))); //NotEqual

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 34 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 30 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 33 }))); //NotEqual

            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 20) })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 10) })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 11) })));//NotEqual
        }
        
        [Fact]
        public void TestLtPathOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringLessThanPath("$.varstr", "$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericLessThanPath("$.varint", "$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampLessThanPath("$.vardate", "$.b")))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "vvalue", b = "value" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue", b = "value" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value", b = "value" }))); //Equal

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 34, b = 33 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 30 , b = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 33 , b = 33}))); //Equal

            var d = new DateTime(2018, 10, 22, 22, 33, 20);

            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = d.AddHours(1), b = d })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = d.AddHours(-1), b = d })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = d, b = d  })));//Equal
        }

        [Fact]
        public void TestLteOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringLessThanEquals("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericLessThanEquals("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampLessThanEquals("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "vvalue" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value" }))); //Equal

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 34 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 30 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 33 }))); //Equal

            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 20) })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 10) })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = new DateTime(2018, 10, 22, 22, 33, 11) })));//Equal
        }
        
        [Fact]
        public void TestLtePathOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringLessThanEqualsPath("$.varstr", "$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericLessThanEqualsPath("$.varint", "$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampLessThanEqualsPath("$.vardate", "$.b")))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "vvalue", b = "value" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "notValue", b = "value" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value", b = "value" }))); //Equal

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varint = 34, b = 33 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 30 , b = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varint = 33 , b = 33}))); //Equal

            var d = new DateTime(2018, 10, 22, 22, 33, 20);

            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { vardate = d.AddHours(1), b = d })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = d.AddHours(-1), b = d })));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { vardate = d, b = d  })));//Equal
        }

        [Fact]
        public void TestEqualPathOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.StringEqualsPath("$.a","$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.NumericEqualsPath("$.a", "$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.TimestampEqualsPath("$.a", "$.b")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.BooleanEqualsPath("$.a", "$.b")))
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { a = "value", b = "value" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { a = "value", b = "not-value" })));

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { a = 33, b = 33 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { a = 33, b = 22 })));

            var d = new DateTime(2018, 10, 22, 22, 33, 11);
            
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new { a = d, b = d })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { a = d, b = DateTime.Now })));

            Assert.True(choices[3].Condition.Match(JObject.FromObject(new { a = true, b = true })));
            Assert.True(choices[3].Condition.Match(JObject.FromObject(new { a = false, b = false })));
            Assert.False(choices[3].Condition.Match(JObject.FromObject(new { a = false, b = true })));
        }

        [Fact]
        public void TestNotOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Not(StepFunctionBuilder.StringEquals("$.varstr", "value"))))
                                       .Build();

            var choices = c.Choices.ToArray();
            
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value" }))); //Equal
        }
        [Fact]
        public void TestAndOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.And(StepFunctionBuilder.StringEquals("$.varstr", "value"),
                                                                                                     StepFunctionBuilder.BooleanEquals("$.varbool", true)))
                                                                   )
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value", varbool = true })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "valuee", varbool = true })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "valuee", varbool = false })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "valuee", varbool = true })));
        }
        [Fact]
        public void TestOrOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Or(StepFunctionBuilder.StringEquals("$.varstr", "value"),
                                                                                                     StepFunctionBuilder.BooleanEquals("$.varbool", false)))
                                              )
                                       .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value", varbool = false })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "valuee", varbool = false })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { varstr = "value", varbool = true })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "valuee", varbool = true })));
        }

        [Fact]
        public void TestIsNull()
        {
            var c = StepFunctionBuilder.ChoiceState()
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsNull("$.isNull", true)))
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsNull("$.isNotNull", false))
                )
                .Build();
            
            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.Parse("{\"isNull\":null}")));
            Assert.False(choices[0].Condition.Match(JObject.Parse("{\"isNull\":33}")));
            
            Assert.True(choices[1].Condition.Match(JObject.Parse("{\"isNotNull\":22}")));
            Assert.False(choices[1].Condition.Match(JObject.Parse("{\"isNotNull\":null}")));
        }
        
        [Fact]
        public void TestIsPresent()
        {
            var c = StepFunctionBuilder.ChoiceState()
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsPresent("$.isPresent", true)))
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsPresent("$.isNotPresent", false))
                )
                .Build();
            
            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.Parse("{\"isPresent\":null}")));
            Assert.True(choices[0].Condition.Match(JObject.Parse("{\"isPresent\":33}")));
            Assert.False(choices[0].Condition.Match(JObject.Parse("{}")));

            Assert.True(choices[1].Condition.Match(JObject.Parse("{}")));
            Assert.False(choices[1].Condition.Match(JObject.Parse("{\"isNotPresent\":33}")));
        }
        
        [Fact]
        public void TestIsString()
        {
            var c = StepFunctionBuilder.ChoiceState()
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsString("$.isString", true)))
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsString("$.isString", false))
                )
                .Build();
            
            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { isString = "str" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { isString = DateTime.Now })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isString = 33 })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isString = 33.23 })));
            
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { isString = "str" })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { isString = DateTime.Now })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isString = 33 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isString = 33.23 })));
        }
        
        [Fact]
        public void TestIsNumeric()
        {
            var c = StepFunctionBuilder.ChoiceState()
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsNumeric("$.isNumeric", true)))
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsNumeric("$.isNumeric", false))
                )
                .Build();
            
            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isNumeric = "str" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isNumeric = DateTime.Now })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { isNumeric = 33 })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { isNumeric = 33.23 })));
            
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isNumeric = "str" })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isNumeric = DateTime.Now })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { isNumeric = 33 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { isNumeric = 33.23 })));
        }
        
        [Fact]
        public void TestIsTimestamp()
        {
            var c = StepFunctionBuilder.ChoiceState()
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsTimestamp("$.isTimestamp", true)))
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsTimestamp("$.isTimestamp", false))
                )
                .Build();
            
            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isTimestamp = "str" })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { isTimestamp = DateTime.Now })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isTimestamp = 33 })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isTimestamp = 33.23 })));
            
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isTimestamp = "str" })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { isTimestamp = DateTime.Now })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isTimestamp = 33 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isTimestamp = 33.23 })));
        }
        
        [Fact]
        public void TestIsBoolean()
        {
            var c = StepFunctionBuilder.ChoiceState()
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsBoolean("$.isBoolean", true)))
                .Choice(StepFunctionBuilder.Choice()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Condition(StepFunctionBuilder.IsBoolean("$.isBoolean", false))
                )
                .Build();
            
            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isBoolean = "str" })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isBoolean = DateTime.Now })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isBoolean = 33 })));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { isBoolean = 33.23 })));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new { isBoolean = true })));
            
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isBoolean = "str" })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isBoolean = DateTime.Now })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isBoolean = 33 })));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { isBoolean = 33.23 })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { isBoolean = true })));
        }

        [Fact]
        public void TestBadFormat()
        {
            var dt = new DateTime(2018, 10, 22, 22, 33, 11);
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Or(StepFunctionBuilder.NumericEquals("$.varint", 33),
                                                                                                    StepFunctionBuilder.NumericGreaterThan("$.varint", 33),
                                                                                                    StepFunctionBuilder.NumericGreaterThanEquals("$.varint", 33),
                                                                                                    StepFunctionBuilder.NumericLessThan("$.varint", 33),
                                                                                                    StepFunctionBuilder.NumericLessThanEquals("$.varint", 33))))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Or(StepFunctionBuilder.TimestampEquals("$.vardate", dt),
                                                                                                    StepFunctionBuilder.TimestampGreaterThan("$.vardate", dt),
                                                                                                    StepFunctionBuilder.TimestampGreaterThanEquals("$.vardate", dt),
                                                                                                    StepFunctionBuilder.TimestampLessThan("$.vardate", dt),
                                                                                                    StepFunctionBuilder.TimestampLessThanEquals("$.vardate", dt))))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.BooleanEquals("$.varbool", true)))
                                       .Build();

            var choices = c.Choices.ToArray();

            //Unknown prop
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { other = "value" })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { other = "value" })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { other = "value" })));

            //Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "1000" })));
            //string instead of correct type
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varint = "hello" })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { vardate = "hello" })));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new { varbool = "hello" })));

            //Invalid date
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varbool = "2016-14-14T01:59:00Z" })));
        }
    }
}
