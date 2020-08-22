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
            
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new { varstr = "val\\*ue" })));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new { varstr = "val\\*DFDFue" })));
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
