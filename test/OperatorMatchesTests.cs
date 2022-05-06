using System;
using Newtonsoft.Json.Linq;
using Xunit;

namespace StatesLanguage.Tests
{
    public class OperatorMatchesTests
    {
        [Fact]
        public void TestEqOperatorWithJObject()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringEquals("$.varstr", "value")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericEquals("$.varint", 33)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampEquals("$.vardate",
                        new DateTime(2018, 10, 22, 22, 33, 11))))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.BooleanEquals("$.varbool", true)))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue"})));

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 34})));

            Assert.True(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 11)})));
            Assert.False(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 12)})));

            Assert.True(choices[3].Condition.Match(JObject.FromObject(new {varbool = true})));
            Assert.False(choices[3].Condition.Match(JObject.FromObject(new {varbool = false})));
            Assert.False(choices[3].Condition.Match(JToken.Parse("true")));
            Assert.False(choices[3].Condition.Match(JToken.Parse("false")));
        }

        [Fact]
        public void TestEqOperatorWithJTokens()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringEquals(null, "value")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericEquals(null, 33)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampEquals(null, new DateTime(2018, 10, 22, 22, 33, 11))))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.BooleanEquals(null, true)))
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
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.Match("$.varstr", "val*")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.Match("$.varstr", "val*ue")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.Match("$.varstr", "val\\*ue")))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "test"})));

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varstr = "value"})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varstr = "valDFDFDFue"})));

            Assert.True(choices[2].Condition.Match(JObject.FromObject(new {varstr = "val*ue"})));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {varstr = "value"})));
        }

        [Fact]
        public void TestGtOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringGreaterThan("$.varstr", "value")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericGreaterThan("$.varint", 33)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampGreaterThan("$.vardate",
                        new DateTime(2018, 10, 22, 22, 33, 11))))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "vvalue"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value"}))); //NotEqual

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 34})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 30})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 33}))); //NotEqual

            Assert.True(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 20)})));
            Assert.False(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 10)})));
            Assert.False(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 11)}))); //NotEqual
        }

        [Fact]
        public void TestGtPathOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringGreaterThanPath("$.varstr", "$.a")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericGreaterThanPath("$.varint", "$.a")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampGreaterThanPath("$.vardate", "$.a")))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "vvalue", a = "value"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue", a = "value"})));
            Assert.False(choices[0].Condition
                .Match(JObject.FromObject(new {varstr = "value", a = "value"}))); //NotEqual

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 34, a = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 30, a = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 33, a = 33}))); //NotEqual

            var d = new DateTime(2018, 10, 22, 22, 33, 11);
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new {vardate = d.AddDays(1), a = d})));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {vardate = d.AddDays(-1), a = d})));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {vardate = d, a = d}))); //NotEqual
        }

        [Fact]
        public void TestGteOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringGreaterThanEquals("$.varstr", "value")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericGreaterThanEquals("$.varint", 33)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampGreaterThanEquals("$.vardate",
                        new DateTime(2018, 10, 22, 22, 33, 11))))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "vvalue"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value"}))); //Equal

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 34})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 30})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 33}))); //Equal

            Assert.True(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 20)})));
            Assert.False(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 10)})));
            Assert.True(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 11)}))); //Equal
        }

        [Fact]
        public void TestGtePathOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringGreaterThanEqualsPath("$.varstr", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericGreaterThanEqualsPath("$.varint", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampGreaterThanEqualsPath("$.vardate", "$.b")))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "vvalue", b = "value"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue", b = "value"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value", b = "value"}))); //Equal

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 34, b = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 30, b = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 33, b = 33}))); //Equal

            var d = new DateTime(2018, 10, 22, 22, 33, 20);

            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {vardate = d.AddHours(-1), b = d})));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new {vardate = d.AddHours(1), b = d})));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new {vardate = d, b = d}))); //Equal
        }

        [Fact]
        public void TestLtOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringLessThan("$.varstr", "value")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericLessThan("$.varint", 33)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampLessThan("$.vardate",
                        new DateTime(2018, 10, 22, 22, 33, 11))))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "vvalue"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value"}))); //NotEqual

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 34})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 30})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 33}))); //NotEqual

            Assert.False(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 20)})));
            Assert.True(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 10)})));
            Assert.False(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 11)}))); //NotEqual
        }

        [Fact]
        public void TestLtPathOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringLessThanPath("$.varstr", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericLessThanPath("$.varint", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampLessThanPath("$.vardate", "$.b")))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "vvalue", b = "value"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue", b = "value"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value", b = "value"}))); //Equal

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 34, b = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 30, b = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 33, b = 33}))); //Equal

            var d = new DateTime(2018, 10, 22, 22, 33, 20);

            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {vardate = d.AddHours(1), b = d})));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new {vardate = d.AddHours(-1), b = d})));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {vardate = d, b = d}))); //Equal
        }

        [Fact]
        public void TestLteOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringLessThanEquals("$.varstr", "value")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericLessThanEquals("$.varint", 33)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampLessThanEquals("$.vardate",
                        new DateTime(2018, 10, 22, 22, 33, 11))))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "vvalue"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value"}))); //Equal

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 34})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 30})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 33}))); //Equal

            Assert.False(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 20)})));
            Assert.True(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 10)})));
            Assert.True(choices[2].Condition
                .Match(JObject.FromObject(new {vardate = new DateTime(2018, 10, 22, 22, 33, 11)}))); //Equal
        }

        [Fact]
        public void TestLtePathOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringLessThanEqualsPath("$.varstr", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericLessThanEqualsPath("$.varint", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampLessThanEqualsPath("$.vardate", "$.b")))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "vvalue", b = "value"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "notValue", b = "value"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value", b = "value"}))); //Equal

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varint = 34, b = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 30, b = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {varint = 33, b = 33}))); //Equal

            var d = new DateTime(2018, 10, 22, 22, 33, 20);

            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {vardate = d.AddHours(1), b = d})));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new {vardate = d.AddHours(-1), b = d})));
            Assert.True(choices[2].Condition.Match(JObject.FromObject(new {vardate = d, b = d}))); //Equal
        }

        [Fact]
        public void TestEqualPathOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.StringEqualsPath("$.a", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.NumericEqualsPath("$.a", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.TimestampEqualsPath("$.a", "$.b")))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.BooleanEqualsPath("$.a", "$.b")))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {a = "value", b = "value"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {a = "value", b = "not-value"})));

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {a = 33, b = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {a = 33, b = 22})));

            var d = new DateTime(2018, 10, 22, 22, 33, 11);

            Assert.True(choices[2].Condition.Match(JObject.FromObject(new {a = d, b = d})));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {a = d, b = DateTime.Now})));

            Assert.True(choices[3].Condition.Match(JObject.FromObject(new {a = true, b = true})));
            Assert.True(choices[3].Condition.Match(JObject.FromObject(new {a = false, b = false})));
            Assert.False(choices[3].Condition.Match(JObject.FromObject(new {a = false, b = true})));
        }

        [Fact]
        public void TestNotOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.Not(StateMachineBuilder.StringEquals("$.varstr", "value"))))
                .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value"}))); //Equal
        }

        [Fact]
        public void TestAndOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.And(StateMachineBuilder.StringEquals("$.varstr", "value"),
                        StateMachineBuilder.BooleanEquals("$.varbool", true)))
                )
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value", varbool = true})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "valuee", varbool = true})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "valuee", varbool = false})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "valuee", varbool = true})));
        }

        [Fact]
        public void TestOrOperator()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.Or(StateMachineBuilder.StringEquals("$.varstr", "value"),
                        StateMachineBuilder.BooleanEquals("$.varbool", false)))
                )
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value", varbool = false})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "valuee", varbool = false})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {varstr = "value", varbool = true})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varstr = "valuee", varbool = true})));
        }

        [Fact]
        public void TestIsNull()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsNull("$.isNull", true)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsNull("$.isNotNull", false))
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
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsPresent("$.isPresent", true)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsPresent("$.isNotPresent", false))
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
        public void TestIfEmptyArrayWithIsPresent()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsPresent("$.files[0]", true)))
                .Build();

            var js = c.ToJson();
            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.Parse("{\"files\":[1,2,3]}")));
            Assert.False(choices[0].Condition.Match(JObject.Parse("{\"files\":[]}")));
        }

        [Fact]
        public void TestIsString()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsString("$.isString", true)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsString("$.isString", false))
                )
                .Build();

            var choices = c.Choices.ToArray();

            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {isString = "str"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {isString = DateTime.Now})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isString = 33})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isString = 33.23})));

            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {isString = "str"})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {isString = DateTime.Now})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isString = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isString = 33.23})));
        }

        [Fact]
        public void TestIsNumeric()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsNumeric("$.isNumeric", true)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsNumeric("$.isNumeric", false))
                )
                .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isNumeric = "str"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isNumeric = DateTime.Now})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {isNumeric = 33})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {isNumeric = 33.23})));

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isNumeric = "str"})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isNumeric = DateTime.Now})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {isNumeric = 33})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {isNumeric = 33.23})));
        }

        [Fact]
        public void TestIsTimestamp()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsTimestamp("$.isTimestamp", true)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsTimestamp("$.isTimestamp", false))
                )
                .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isTimestamp = "str"})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {isTimestamp = DateTime.Now})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isTimestamp = 33})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isTimestamp = 33.23})));

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isTimestamp = "str"})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {isTimestamp = DateTime.Now})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isTimestamp = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isTimestamp = 33.23})));
        }

        [Fact]
        public void TestIsBoolean()
        {
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsBoolean("$.isBoolean", true)))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.IsBoolean("$.isBoolean", false))
                )
                .Build();

            var choices = c.Choices.ToArray();

            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isBoolean = "str"})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isBoolean = DateTime.Now})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isBoolean = 33})));
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {isBoolean = 33.23})));
            Assert.True(choices[0].Condition.Match(JObject.FromObject(new {isBoolean = true})));

            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isBoolean = "str"})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isBoolean = DateTime.Now})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isBoolean = 33})));
            Assert.True(choices[1].Condition.Match(JObject.FromObject(new {isBoolean = 33.23})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {isBoolean = true})));
        }

        [Fact]
        public void TestBadFormat()
        {
            var dt = new DateTime(2018, 10, 22, 22, 33, 11);
            var c = StateMachineBuilder.ChoiceState()
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.Or(StateMachineBuilder.NumericEquals("$.varint", 33),
                        StateMachineBuilder.NumericGreaterThan("$.varint", 33),
                        StateMachineBuilder.NumericGreaterThanEquals("$.varint", 33),
                        StateMachineBuilder.NumericLessThan("$.varint", 33),
                        StateMachineBuilder.NumericLessThanEquals("$.varint", 33))))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.Or(StateMachineBuilder.TimestampEquals("$.vardate", dt),
                        StateMachineBuilder.TimestampGreaterThan("$.vardate", dt),
                        StateMachineBuilder.TimestampGreaterThanEquals("$.vardate", dt),
                        StateMachineBuilder.TimestampLessThan("$.vardate", dt),
                        StateMachineBuilder.TimestampLessThanEquals("$.vardate", dt))))
                .Choice(StateMachineBuilder.Choice()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Condition(StateMachineBuilder.BooleanEquals("$.varbool", true)))
                .Build();

            var choices = c.Choices.ToArray();

            //Unknown prop
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {other = "value"})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {other = "value"})));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {other = "value"})));

            //Assert.False(choices[0].Condition.Match(JObject.FromObject(new { varstr = "1000" })));
            //string instead of correct type
            Assert.False(choices[0].Condition.Match(JObject.FromObject(new {varint = "hello"})));
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {vardate = "hello"})));
            Assert.False(choices[2].Condition.Match(JObject.FromObject(new {varbool = "hello"})));

            //Invalid date
            Assert.False(choices[1].Condition.Match(JObject.FromObject(new {varbool = "2016-14-14T01:59:00Z"})));
        }
    }
}