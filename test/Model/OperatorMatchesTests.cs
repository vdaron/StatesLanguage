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
                                                                  .Condition(StepFunctionBuilder.Eq("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Eq("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Eq("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Eq("$.varbool", true)))
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
                                                                  .Condition(StepFunctionBuilder.Eq(null,"value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Eq(null, 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Eq(null, new DateTime(2018, 10, 22, 22, 33, 11))))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Eq(null, true)))
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
        public void TestGtOperator()
        {
            var c = StepFunctionBuilder.ChoiceState()
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Gt("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Gt("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Gt("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
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
                                                                  .Condition(StepFunctionBuilder.Gte("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Gte("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Gte("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
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
                                                                  .Condition(StepFunctionBuilder.Lt("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Lt("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Lt("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
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
                                                                  .Condition(StepFunctionBuilder.Lte("$.varstr", "value")))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Lte("$.varint", 33)))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Lte("$.vardate", new DateTime(2018, 10, 22, 22, 33, 11))))
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
                                                                  .Condition(StepFunctionBuilder.Not(StepFunctionBuilder.Eq("$.varstr", "value"))))
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
                                                                  .Condition(StepFunctionBuilder.And(StepFunctionBuilder.Eq("$.varstr", "value"),
                                                                                                     StepFunctionBuilder.Eq("$.varbool", true)))
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
                                                                  .Condition(StepFunctionBuilder.Or(StepFunctionBuilder.Eq("$.varstr", "value"),
                                                                                                     StepFunctionBuilder.Eq("$.varbool", false)))
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
                                                                  .Condition(StepFunctionBuilder.Or(StepFunctionBuilder.Eq("$.varint", 33),
                                                                                                    StepFunctionBuilder.Gt("$.varint", 33),
                                                                                                    StepFunctionBuilder.Gte("$.varint", 33),
                                                                                                    StepFunctionBuilder.Lt("$.varint", 33),
                                                                                                    StepFunctionBuilder.Lte("$.varint", 33))))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Or(StepFunctionBuilder.Eq("$.vardate", dt),
                                                                                                    StepFunctionBuilder.Gt("$.vardate", dt),
                                                                                                    StepFunctionBuilder.Gte("$.vardate", dt),
                                                                                                    StepFunctionBuilder.Lt("$.vardate", dt),
                                                                                                    StepFunctionBuilder.Lte("$.vardate", dt))))
                                       .Choice(StepFunctionBuilder.Choice()
                                                                  .Transition(StepFunctionBuilder.Next("NextState"))
                                                                  .Condition(StepFunctionBuilder.Eq("$.varbool", true)))
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
