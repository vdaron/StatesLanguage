/*
 * Copyright 2010-2017 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * Copyright 2018- Vincent DARON All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.States;
using Xunit;

namespace StatesLanguage.Tests
{
    public class StepFunctionBuilderTest
    {
        private static void Equal(JObject expected, JObject value)
        {
            Assert.True(JToken.DeepEquals(expected, value));
        }

        private void AssertStateMachine(StateMachine stateMachine, string resourcePath)
        {
            var expected = LoadExpected(resourcePath);

            Equal(expected, stateMachine.ToJObject());
            Equal(expected, roundTripStateMachine(stateMachine).ToJObject());
        }

        private StateMachine roundTripStateMachine(StateMachine stateMachine)
        {
            return StateMachine.FromJObject(StateMachine.FromJson(stateMachine.ToJson()).Build().ToJObject()).Build();
        }


        private JObject LoadExpected(string resourcePath)
        {
            using (var stream = GetType().Assembly
                .GetManifestResourceStream("StatesLanguage.Tests.Resources.StateMachines." + resourcePath))
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                return JObject.Load(jsonReader);
            }
        }

        [Fact]
        public void CanSerializeStatesToJson()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .TimeoutSeconds(30)
                .Comment("My Simple State Machine")
                .State("InitialState", StateMachineBuilder.SucceedState()
                    .Comment("Initial State")
                    .InputPath("$.input")
                    .OutputPath("$.output"))
                .Build();

            var value = stateMachine.States["InitialState"].ToJson();

            var s = State.FromJson(value);
            Assert.True(s.Type == StateType.Succeed);
        }

        [Fact]
        public void SingleSucceedState()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .TimeoutSeconds(30)
                .Comment("My Simple State Machine")
                .State("InitialState", StateMachineBuilder.SucceedState()
                    .Comment("Initial State")
                    .InputPath("$.input")
                    .OutputPath("$.output"))
                .Build();
            AssertStateMachine(stateMachine, "SingleSucceedState.json");
        }

        [Fact]
        public void SingleTaskState()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.TaskState()
                    .Comment("Initial State")
                    .TimeoutSeconds(10)
                    .HeartbeatSeconds(1)
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Resource("resource-arn")
                    .InputPath("$.input")
                    .ResultPath("$.result")
                    .OutputPath("$.output")
                    .Parameters(JObject.FromObject(new {value = "param"}))
                    .ResultSelector(JObject.FromObject(new {value = "param"}))
                    .Credentials(JObject.Parse("{\"user\": \"vda\"}")))
                .State("NextState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleTaskState.json");
        }

        [Fact]
        public void TaskStateWithEnd()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.TaskState()
                    .Resource("resource-arn")
                    .Transition(StateMachineBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "TaskStateWithEnd.json");
        }

        [Fact]
        public void TaskStateWithParametersAndEnd()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.TaskState()
                    .Resource("resource-arn")
                    .Parameters(JObject.FromObject(new {value = "param"}))
                    .Transition(StateMachineBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "TaskStateWithParametersAndEnd.json");
        }

        [Fact]
        public void SingleTaskStateWithRetries()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.TaskState()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Resource("resource-arn")
                    .Retriers(StateMachineBuilder.Retrier()
                            .ErrorEquals("Foo", "Bar")
                            .IntervalSeconds(20)
                            .MaxAttempts(3)
                            .BackoffRate(2.0),
                        StateMachineBuilder.Retrier()
                            .RetryOnAllErrors()
                            .IntervalSeconds(30)
                            .MaxAttempts(10)
                            .BackoffRate(2.0)))
                .State("NextState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleTaskStateWithRetries.json");
        }

        [Fact]
        public void SingleTaskStateWithCatchers()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.TaskState()
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Resource("resource-arn")
                    .Catchers(StateMachineBuilder.Catcher()
                            .ErrorEquals("Foo", "Bar")
                            .Transition(StateMachineBuilder.Next("RecoveryState"))
                            .ResultPath("$.result-path"),
                        StateMachineBuilder.Catcher()
                            .CatchAll()
                            .Transition(StateMachineBuilder.Next("OtherRecoveryState"))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("RecoveryState", StateMachineBuilder.SucceedState())
                .State("OtherRecoveryState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleTaskStateWithCatchers.json");
        }

        [Fact]
        public void SinglePassStateWithJsonResult()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.PassState()
                    .Comment("Pass through state")
                    .InputPath("$.input")
                    .OutputPath("$.output")
                    .ResultPath("$.result")
                    .Parameters(JObject.Parse("{\"a\":1}"))
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Result("{\"Foo\": \"Bar\"}"))
                .State("NextState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SinglePassStateWithJsonResult.json");
        }

        [Fact]
        public void SinglePassStateWithObjectResult()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.PassState()
                    .Transition(StateMachineBuilder.End())
                    .Result(new SimplePojo("value")))
                .Build();

            AssertStateMachine(stateMachine, "SinglePassStateWithObjectResult.json");
        }

        [Fact]
        public void SingleWaitState_WaitForSeconds()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.WaitState()
                    .Comment("My wait state")
                    .InputPath("$.input")
                    .OutputPath("$.output")
                    .WaitFor(StateMachineBuilder.Seconds(10))
                    .Transition(StateMachineBuilder.Next("NextState")))
                .State("NextState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithSeconds.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilSecondsPath()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.WaitState()
                    .WaitFor(StateMachineBuilder.SecondsPath("$.seconds"))
                    .Transition(StateMachineBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithSecondsPath.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilTimestamp()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.WaitState()
                    .WaitFor(StateMachineBuilder.Timestamp(DateTime.Parse("2016-03-14T01:59:00Z",CultureInfo.InvariantCulture).ToUniversalTime()))
                    .Transition(StateMachineBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithTimestamp.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilTimestampWithMillisecond()
        {
            var millis = DateTime.Parse("2016-03-14T01:59:00.123Z",CultureInfo.InvariantCulture).ToUniversalTime();
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.WaitState()
                    .WaitFor(StateMachineBuilder.Timestamp(millis))
                    .Transition(StateMachineBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithTimestampWithMilliseconds.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilTimestampWithTimezone()
        {
            var epochMilli = DateTime.Parse("2016-03-14T01:59:00.123-08:00",CultureInfo.InvariantCulture).ToUniversalTime();
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.WaitState()
                    .WaitFor(StateMachineBuilder.Timestamp(epochMilli))
                    .Transition(StateMachineBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithTimestampWithTimezone.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilTimestampWithPath()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.WaitState()
                    .WaitFor(StateMachineBuilder.TimestampPath("$.timestamp"))
                    .Transition(StateMachineBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithTimestampWithPath.json");
        }

        [Fact]
        public void SingleFailState()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.FailState()
                    .Comment("My fail state")
                    .Cause("InternalError")
                    .Error("java.lang.Exception"))
                .Build();

            AssertStateMachine(stateMachine, "SingleFailState.json");
        }

        [Fact]
        public void SimpleChoiceState()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ChoiceState()
                    .Comment("My choice state")
                    .DefaultStateName("DefaultState")
                    .InputPath("$.input")
                    .OutputPath("$.output")
                    .Choice(StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("NextState"))
                        .Condition(StateMachineBuilder.StringEquals("$.var", "value"))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("DefaultState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleChoiceState.json");
        }

        [Fact]
        public void ChoiceStateWithMultipleChoices()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choices(
                        StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("NextState"))
                            .Condition(StateMachineBuilder.StringEquals("$.var", "value")),
                        StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("OtherNextState"))
                            .Condition(StateMachineBuilder.NumericGreaterThan("$.number", 10))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("OtherNextState", StateMachineBuilder.SucceedState())
                .State("DefaultState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithMultipleChoices.json");
        }

        [Fact]
        public void ChoiceStateWithAndCondition()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("NextState"))
                        .Condition(
                            StateMachineBuilder.And(StateMachineBuilder.StringEquals("$.var", "value"),
                                StateMachineBuilder.NumericEquals("$.other-var", 10)
                            ))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("DefaultState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithAndCondition.json");
        }

        [Fact]
        public void ChoiceStateWithOrCondition()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("NextState"))
                        .Condition(
                            StateMachineBuilder.Or(StateMachineBuilder.StringGreaterThan("$.var", "value"),
                                StateMachineBuilder.NumericLessThanEquals("$.other-var", 10)
                            ))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("DefaultState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithOrCondition.json");
        }

        [Fact]
        public void ChoiceStateWithoutDefaultState()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ChoiceState()
                    .Choice(StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("NextState"))
                        .Condition(
                            StateMachineBuilder.Or(StateMachineBuilder.StringGreaterThan("$.var", "value"),
                                StateMachineBuilder.NumericLessThanEquals("$.other-var", 10)
                            ))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("DefaultState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithoutDefault.json");
        }

        [Fact]
        public void ChoiceStateWithNotCondition()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("NextState"))
                        .Condition(StateMachineBuilder.Not(
                            StateMachineBuilder.StringGreaterThanEquals("$.var", "value")))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("DefaultState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithNotCondition.json");
        }

        [Fact]
        public void ChoiceStateWithComplexCondition()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("NextState"))
                        .Condition(StateMachineBuilder.And(
                            StateMachineBuilder.StringGreaterThanEquals("$.var", "value"),
                            StateMachineBuilder.StringLessThanEquals("$.other-var", "foo"),
                            StateMachineBuilder.Or(
                                StateMachineBuilder.NumericLessThan("$.numeric", 9000.1),
                                StateMachineBuilder.Not(StateMachineBuilder.NumericGreaterThanEquals("$.numeric", 42))
                            )
                        ))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("DefaultState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithComplexCondition.json");
        }

        [Fact]
        public void ChoiceStateWithAllPrimitiveConditions()
        {
            var date = DateTime.Parse("2016-03-14T01:59:00.000Z",CultureInfo.InvariantCulture).ToUniversalTime();
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StateMachineBuilder.Choice().Transition(StateMachineBuilder.Next("NextState"))
                        .Condition(StateMachineBuilder.And(
                            StateMachineBuilder.StringEquals("$.string", "value"),
                            StateMachineBuilder.StringGreaterThan("$.string", "value"),
                            StateMachineBuilder.StringGreaterThanEquals("$.string", "value"),
                            StateMachineBuilder.StringLessThan("$.string", "value"),
                            StateMachineBuilder.StringLessThanEquals("$.string", "value"),
                            StateMachineBuilder.NumericEquals("$.integral", 42),
                            StateMachineBuilder.NumericGreaterThan("$.integral", 42),
                            StateMachineBuilder.NumericGreaterThanEquals("$.integral", 42),
                            StateMachineBuilder.NumericLessThan("$.integral", 42),
                            StateMachineBuilder.NumericLessThanEquals("$.integral", 42),
                            StateMachineBuilder.NumericEquals("$.double", 9000.1),
                            StateMachineBuilder.NumericGreaterThan("$.double", 9000.1),
                            StateMachineBuilder.NumericGreaterThanEquals("$.double", 9000.1),
                            StateMachineBuilder.NumericLessThan("$.double", 9000.1),
                            StateMachineBuilder.NumericLessThanEquals("$.double", 9000.1),
                            StateMachineBuilder.TimestampEquals("$.timestamp", date),
                            StateMachineBuilder.TimestampGreaterThan("$.timestamp", date),
                            StateMachineBuilder.TimestampGreaterThanEquals("$.timestamp", date),
                            StateMachineBuilder.TimestampLessThan("$.timestamp", date),
                            StateMachineBuilder.TimestampLessThanEquals("$.timestamp", date),
                            StateMachineBuilder.BooleanEquals("$.boolean", true),
                            StateMachineBuilder.BooleanEquals("$.boolean", false),
                            StateMachineBuilder.IsPresent("$.present", false),
                            StateMachineBuilder.IsPresent("$.present", true),
                            StateMachineBuilder.IsBoolean("$.boolean", false),
                            StateMachineBuilder.IsBoolean("$.boolean", true),
                            StateMachineBuilder.IsNumeric("$.numeric", false),
                            StateMachineBuilder.IsNumeric("$.numeric", true),
                            StateMachineBuilder.IsString("$.string", false),
                            StateMachineBuilder.IsString("$.string", true),
                            StateMachineBuilder.IsNull("$.null", false),
                            StateMachineBuilder.IsNull("$.null", true),
                            StateMachineBuilder.IsTimestamp("$.timestamp", false),
                            StateMachineBuilder.IsTimestamp("$.timestamp", true)
                        ))))
                .State("NextState", StateMachineBuilder.SucceedState())
                .State("DefaultState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithAllPrimitiveCondition.json");
        }

        [Fact]
        public void SimpleParallelState()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ParallelState()
                    .Comment("My parallel state")
                    .InputPath("$.input")
                    .OutputPath("$.output")
                    .ResultPath("$.result")
                    .Parameters(JObject.FromObject(new {value = "param"}))
                    .ResultSelector(JObject.FromObject(new {value = "param"}))
                    .Transition(StateMachineBuilder.Next("NextState"))
                    .Branches(
                        StateMachineBuilder.SubStateMachine()
                            .Comment("Branch one")
                            .StartAt("BranchOneInitial")
                            .State("BranchOneInitial", StateMachineBuilder.SucceedState()),
                        StateMachineBuilder.SubStateMachine()
                            .Comment("Branch two")
                            .StartAt("BranchTwoInitial")
                            .State("BranchTwoInitial", StateMachineBuilder.SucceedState())
                    ))
                .State("NextState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleParallelState.json");
        }

        [Fact]
        public void SimpleParallelStateWithTasks()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("para")
                .State("para", StateMachineBuilder.ParallelState()
                    .Transition(StateMachineBuilder.End())
                    .Branches(
                        StateMachineBuilder.SubStateMachine()
                            .StartAt("t")
                            .State("t",
                                StateMachineBuilder.TaskState().Resource("t").Transition(StateMachineBuilder.End())),
                        StateMachineBuilder.SubStateMachine()
                            .StartAt("u")
                            .State("u",
                                StateMachineBuilder.TaskState().Resource("u").Transition(StateMachineBuilder.End()))
                    ))
                .Build();

            AssertStateMachine(stateMachine, "SimpleParallelStateWithTasks.json");
        }

        [Fact]
        public void ParallelStateWithRetriers()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ParallelState()
                    .Transition(StateMachineBuilder.End())
                    .Branches(
                        StateMachineBuilder.SubStateMachine()
                            .Comment("Branch one")
                            .StartAt("BranchOneInitial")
                            .State("BranchOneInitial", StateMachineBuilder.SucceedState()),
                        StateMachineBuilder.SubStateMachine()
                            .Comment("Branch two")
                            .StartAt("BranchTwoInitial")
                            .State("BranchTwoInitial", StateMachineBuilder.SucceedState())
                    )
                    .Retriers(StateMachineBuilder.Retrier()
                            .ErrorEquals("Foo", "Bar")
                            .IntervalSeconds(10)
                            .BackoffRate(1.0)
                            .MaxAttempts(3),
                        StateMachineBuilder.Retrier()
                            .RetryOnAllErrors()
                            .IntervalSeconds(10)
                            .BackoffRate(1.0)
                            .MaxAttempts(3)
                    ))
                .Build();

            AssertStateMachine(stateMachine, "ParallelStateWithRetriers.json");
        }

        [Fact]
        public void ParallelStateWithCatchers()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StateMachineBuilder.ParallelState()
                    .Transition(StateMachineBuilder.End())
                    .Branches(
                        StateMachineBuilder.SubStateMachine()
                            .Comment("Branch one")
                            .StartAt("BranchOneInitial")
                            .State("BranchOneInitial", StateMachineBuilder.SucceedState()),
                        StateMachineBuilder.SubStateMachine()
                            .Comment("Branch two")
                            .StartAt("BranchTwoInitial")
                            .State("BranchTwoInitial", StateMachineBuilder.SucceedState())
                    )
                    .Catchers(StateMachineBuilder.Catcher()
                            .ErrorEquals("Foo", "Bar")
                            .Transition(StateMachineBuilder.Next("RecoveryState"))
                            .ResultPath("$.result"),
                        StateMachineBuilder.Catcher()
                            .CatchAll()
                            .Transition(StateMachineBuilder.Next("OtherRecoveryState"))
                            .ResultPath("$.result")
                    ))
                .State("RecoveryState", StateMachineBuilder.SucceedState())
                .State("OtherRecoveryState", StateMachineBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ParallelStateWithCatchers.json");
        }

        [Fact]
        public void SimpleMapState()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("Validate-All")
                .State("Validate-All", StateMachineBuilder.MapState()
                    .InputPath("$.detail")
                    .ItemPath("$.shipped")
                    .ResultPath("$.detail.shipped")
                    .ItemReader(JObject.Parse("{\"Resource\": \"arn:aws:states:::s3:listObjectsV2\",\"Parameters\": {\"Bucket\": \"myBucket\",\"Prefix\": \"processData\"}}"))
                    .ResultWriter(JObject.Parse("{\"Resource\": \"arn:aws:states:::s3:putObject\",\"Parameters\": {\"Bucket\": \"myOutputBucket\",\"Prefix\": \"csvProcessJobs\"}}"))
                    .ItemBatcher(ItemBatcher.GetBuilder()
                                            .MaxItemsPerBatch(100)
                                            .MaxInputBytesPerBatch(250)
                                            .BatchInput(JObject.Parse("{\"factCheck\": \"December 2022\"}")))
                    .MaxConcurrency(0)
                    .ToleratedFailureCount(20)
                    .ToleratedFailurePercentage(5)
                    .ItemSelector(JObject.FromObject(new {value = "param"}))
                    .ResultSelector(JObject.FromObject(new {value = "param"}))
                    .Transition(StateMachineBuilder.End())
                    .ItemProcessor(StateMachineBuilder.SubStateMachine()
                        .StartAt("Validate")
                        .State("Validate", StateMachineBuilder.TaskState()
                            .Resource("arn:aws:lambda:us-east-1:123456789012:function:ship-val")
                            .Transition(StateMachineBuilder.End()))))
                .Build();

            AssertStateMachine(stateMachine, "SimpleMapState.json");
        }
        
        
        [Fact]
        public void SimpleMapStateDeprecated()
        {
            //Ensure We are converting deprecated values automatically
            var expected = LoadExpected("SimpleMapStateDeprecated.json");
            var stateMachine = StateMachine.FromJObject(expected).Build();

            AssertStateMachine(stateMachine, "SimpleMapState.json");
        }

        [Fact]
        public void StateMachineFromJson_MalformedJson_ThrowsException()
        {
            Assert.Throws<StatesLanguageException>(() => StateMachine.FromJson("{"));
        }

        [Fact]
        public void OutputPAthIsNull()
        {
            var stateMachine = StateMachineBuilder.StateMachine()
                .StartAt("helloPass")
                .State("helloPass", StateMachineBuilder.PassState()
                    .Result(JArray.Parse("[1,2,3]"))
                    .InputPath("$.a")
                    .OutputPath(null)
                    .Transition(StateMachineBuilder.End())).Build();
            
            AssertStateMachine(stateMachine, "PassStateNullOutputPath.json");
        }
    }
}