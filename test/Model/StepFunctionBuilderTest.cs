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
using System.IO;
using System.Linq;
using StatesLanguage.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Model.States;
using test.Model;
using Xunit;

namespace StatesLanguage.Tests.Model
{
    public class StepFunctionBuilderTest
    {
        private static void Equal(JObject expected, JObject value)
        {
            Assert.True(JToken.DeepEquals(expected, value));
        }

        private void AssertStateMachine(StateMachine stateMachine, String resourcePath)
        {
            JObject expected = LoadExpected(resourcePath);

            Equal(expected,stateMachine.ToJObject());
            Equal(expected, roundTripStateMachine(stateMachine).ToJObject());
        }

        private StateMachine roundTripStateMachine(StateMachine stateMachine)
        {
            return StateMachine.FromJObject(StateMachine.FromJson(stateMachine.ToJson()).Build().ToJObject()).Build();
        }


        private JObject LoadExpected(string resourcePath)
        {
            using (Stream stream = GetType().Assembly
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
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .TimeoutSeconds(30)
                .Comment("My Simple State Machine")
                .State("InitialState", StepFunctionBuilder.SucceedState()
                    .Comment("Initial State")
                    .InputPath("$.input")
                    .OutputPath("$.output"))
                .Build();

            string value = stateMachine.States["InitialState"].ToJson();

            State s = State.FromJson(value);
            Assert.True(s.Type == StateType.Succeed);
        }

        [Fact]
        public void SingleSucceedState()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .TimeoutSeconds(30)
                .Comment("My Simple State Machine")
                .State("InitialState", StepFunctionBuilder.SucceedState()
                    .Comment("Initial State")
                    .InputPath("$.input")
                    .OutputPath("$.output"))
                .Build();
            AssertStateMachine(stateMachine, "SingleSucceedState.json");
        }

        [Fact]
        public void SingleTaskState()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.TaskState()
                    .Comment("Initial State")
                    .TimeoutSeconds(10)
                    .HeartbeatSeconds(1)
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Resource("resource-arn")
                    .InputPath("$.input")
                    .ResultPath("$.result")
                    .OutputPath("$.output")
                    .Parameters(JObject.FromObject(new {value = "param"}))
                    .ResultSelector(JObject.FromObject(new {value = "param"})))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleTaskState.json");
        }

        [Fact]
        public void TaskStateWithEnd()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.TaskState()
                    .Resource("resource-arn")
                    .Transition(StepFunctionBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "TaskStateWithEnd.json");
        }

        [Fact]
        public void TaskStateWithParametersAndEnd()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                                                           .StartAt("InitialState")
                                                           .State("InitialState", StepFunctionBuilder.TaskState()
                                                                                                     .Resource("resource-arn")
                                                                                                     .Parameters(JObject.FromObject(new {value = "param"}))
                                                                                                     .Transition(StepFunctionBuilder.End()))
                                                           .Build();

            AssertStateMachine(stateMachine, "TaskStateWithParametersAndEnd.json");
        }

        [Fact]
        public void SingleTaskStateWithRetries()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.TaskState()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Resource("resource-arn")
                    .Retriers(StepFunctionBuilder.Retrier()
                            .ErrorEquals("Foo", "Bar")
                            .IntervalSeconds(20)
                            .MaxAttempts(3)
                            .BackoffRate(2.0),
                        StepFunctionBuilder.Retrier()
                            .RetryOnAllErrors()
                            .IntervalSeconds(30)
                            .MaxAttempts(10)
                            .BackoffRate(2.0)))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleTaskStateWithRetries.json");
        }

        [Fact]
        public void SingleTaskStateWithCatchers()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.TaskState()
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Resource("resource-arn")
                    .Catchers(StepFunctionBuilder.Catcher()
                            .ErrorEquals("Foo", "Bar")
                            .Transition(StepFunctionBuilder.Next("RecoveryState"))
                            .ResultPath("$.result-path"),
                        StepFunctionBuilder.Catcher()
                            .CatchAll()
                            .Transition(StepFunctionBuilder.Next("OtherRecoveryState"))))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .State("RecoveryState", StepFunctionBuilder.SucceedState())
                .State("OtherRecoveryState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleTaskStateWithCatchers.json");
        }

        [Fact]
        public void SinglePassStateWithJsonResult()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.PassState()
                    .Comment("Pass through state")
                    .InputPath("$.input")
                    .OutputPath("$.output")
                    .ResultPath("$.result")
                    .Parameters(JObject.Parse("{'a':1}"))
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Result("{\"Foo\": \"Bar\"}"))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SinglePassStateWithJsonResult.json");
        }

        [Fact]
        public void SinglePassStateWithObjectResult()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.PassState()
                    .Transition(StepFunctionBuilder.End())
                    .Result(new SimplePojo("value")))
                .Build();

            AssertStateMachine(stateMachine, "SinglePassStateWithObjectResult.json");
        }

        [Fact]
        public void SingleWaitState_WaitForSeconds()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.WaitState()
                    .Comment("My wait state")
                    .InputPath("$.input")
                    .OutputPath("$.output")
                    .WaitFor(StepFunctionBuilder.Seconds(10))
                    .Transition(StepFunctionBuilder.Next("NextState")))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithSeconds.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilSecondsPath()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.WaitState()
                    .WaitFor(StepFunctionBuilder.SecondsPath("$.seconds"))
                    .Transition(StepFunctionBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithSecondsPath.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilTimestamp()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.WaitState()
                    .WaitFor(StepFunctionBuilder.Timestamp(DateTime.Parse("2016-03-14T01:59:00Z").ToUniversalTime()))
                    .Transition(StepFunctionBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithTimestamp.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilTimestampWithMillisecond()
        {
            var millis = DateTime.Parse("2016-03-14T01:59:00.123Z").ToUniversalTime();
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.WaitState()
                    .WaitFor(StepFunctionBuilder.Timestamp(millis))
                    .Transition(StepFunctionBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithTimestampWithMilliseconds.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilTimestampWithTimezone()
        {
            var epochMilli = DateTime.Parse("2016-03-14T01:59:00.123-08:00").ToUniversalTime();
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.WaitState()
                    .WaitFor(StepFunctionBuilder.Timestamp(epochMilli))
                    .Transition(StepFunctionBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithTimestampWithTimezone.json");
        }

        [Fact]
        public void SingleWaitState_WaitUntilTimestampWithPath()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.WaitState()
                    .WaitFor(StepFunctionBuilder.TimestampPath("$.timestamp"))
                    .Transition(StepFunctionBuilder.End()))
                .Build();

            AssertStateMachine(stateMachine, "SingleWaitStateWithTimestampWithPath.json");
        }

        [Fact]
        public void SingleFailState()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.FailState()
                    .Comment("My fail state")
                    .Cause("InternalError")
                    .Error("java.lang.Exception"))
                .Build();

            AssertStateMachine(stateMachine, "SingleFailState.json");
        }

        [Fact]
        public void SimpleChoiceState()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ChoiceState()
                    .Comment("My choice state")
                    .DefaultStateName("DefaultState")
                    .InputPath("$.input")
                    .OutputPath("$.output")
                    .Choice(StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("NextState"))
                        .Condition(StepFunctionBuilder.StringEquals("$.var", "value"))))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .State("DefaultState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleChoiceState.json");
        }

        [Fact]
        public void ChoiceStateWithMultipleChoices()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choices(
                        StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("NextState"))
                            .Condition(StepFunctionBuilder.StringEquals("$.var", "value")),
                        StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("OtherNextState"))
                            .Condition(StepFunctionBuilder.NumericGreaterThan("$.number", 10))))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .State("OtherNextState", StepFunctionBuilder.SucceedState())
                .State("DefaultState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithMultipleChoices.json");
        }

        [Fact]
        public void ChoiceStateWithAndCondition()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("NextState"))
                        .Condition(
                            StepFunctionBuilder.And(StepFunctionBuilder.StringEquals("$.var", "value"),
                                StepFunctionBuilder.NumericEquals("$.other-var", 10)
                            ))))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .State("DefaultState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithAndCondition.json");
        }

        [Fact]
        public void ChoiceStateWithOrCondition()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("NextState"))
                        .Condition(
                            StepFunctionBuilder.Or(StepFunctionBuilder.StringGreaterThan("$.var", "value"),
                                StepFunctionBuilder.NumericLessThanEquals("$.other-var", 10)
                            ))))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .State("DefaultState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithOrCondition.json");
        }

        [Fact]
        public void ChoiceStateWithoutDefaultState()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                                                           .StartAt("InitialState")
                                                           .State("InitialState", StepFunctionBuilder.ChoiceState()
                                                                                                     .Choice(StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("NextState"))
                                                                                                                                .Condition(
                                                                                                                                           StepFunctionBuilder.Or(StepFunctionBuilder.StringGreaterThan("$.var", "value"),
                                                                                                                                                                  StepFunctionBuilder.NumericLessThanEquals("$.other-var", 10)
                                                                                                                                                                 ))))
                                                           .State("NextState", StepFunctionBuilder.SucceedState())
                                                           .State("DefaultState", StepFunctionBuilder.SucceedState())
                                                           .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithoutDefault.json");
        }

        [Fact]
        public void ChoiceStateWithNotCondition()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("NextState"))
                        .Condition(StepFunctionBuilder.Not(StepFunctionBuilder.StringGreaterThanEquals("$.var", "value")))))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .State("DefaultState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithNotCondition.json");
        }

        [Fact]
        public void ChoiceStateWithComplexCondition()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("NextState"))
                        .Condition(StepFunctionBuilder.And(
                            StepFunctionBuilder.StringGreaterThanEquals("$.var", "value"),
                            StepFunctionBuilder.StringLessThanEquals("$.other-var", "foo"),
                            StepFunctionBuilder.Or(
                                StepFunctionBuilder.NumericLessThan("$.numeric", 9000.1),
                                StepFunctionBuilder.Not(StepFunctionBuilder.NumericGreaterThanEquals("$.numeric", 42))
                            )
                        ))))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .State("DefaultState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithComplexCondition.json");
        }

        [Fact]
        public void ChoiceStateWithAllPrimitiveConditions()
        {
            DateTime date = DateTime.Parse("2016-03-14T01:59:00.000Z").ToUniversalTime();
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ChoiceState()
                    .DefaultStateName("DefaultState")
                    .Choice(StepFunctionBuilder.Choice().Transition(StepFunctionBuilder.Next("NextState"))
                        .Condition(StepFunctionBuilder.And(
                            StepFunctionBuilder.StringEquals("$.string", "value"),
                            StepFunctionBuilder.StringGreaterThan("$.string", "value"),
                            StepFunctionBuilder.StringGreaterThanEquals("$.string", "value"),
                            StepFunctionBuilder.StringLessThan("$.string", "value"),
                            StepFunctionBuilder.StringLessThanEquals("$.string", "value"),
                            StepFunctionBuilder.NumericEquals("$.integral", 42),
                            StepFunctionBuilder.NumericGreaterThan("$.integral", 42),
                            StepFunctionBuilder.NumericGreaterThanEquals("$.integral", 42),
                            StepFunctionBuilder.NumericLessThan("$.integral", 42),
                            StepFunctionBuilder.NumericLessThanEquals("$.integral", 42),
                            StepFunctionBuilder.NumericEquals("$.double", 9000.1),
                            StepFunctionBuilder.NumericGreaterThan("$.double", 9000.1),
                            StepFunctionBuilder.NumericGreaterThanEquals("$.double", 9000.1),
                            StepFunctionBuilder.NumericLessThan("$.double", 9000.1),
                            StepFunctionBuilder.NumericLessThanEquals("$.double", 9000.1),
                            StepFunctionBuilder.TimestampEquals("$.timestamp", date),
                            StepFunctionBuilder.TimestampGreaterThan("$.timestamp", date),
                            StepFunctionBuilder.TimestampGreaterThanEquals("$.timestamp", date),
                            StepFunctionBuilder.TimestampLessThan("$.timestamp", date),
                            StepFunctionBuilder.TimestampLessThanEquals("$.timestamp", date),
                            StepFunctionBuilder.BooleanEquals("$.boolean", true),
                            StepFunctionBuilder.BooleanEquals("$.boolean", false)
                        ))))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .State("DefaultState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ChoiceStateWithAllPrimitiveCondition.json");
        }

        [Fact]
        public void SimpleParallelState()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ParallelState()
                    .Comment("My parallel state")
                    .InputPath("$.input")
                    .OutputPath("$.output")
                    .ResultPath("$.result")
                    .Parameters(JObject.FromObject(new {value = "param"}))
                    .ResultSelector(JObject.FromObject(new {value = "param"}))
                    .Transition(StepFunctionBuilder.Next("NextState"))
                    .Branches(
                        StepFunctionBuilder.SubStateMachine()
                            .Comment("Branch one")
                            .StartAt("BranchOneInitial")
                            .State("BranchOneInitial", StepFunctionBuilder.SucceedState()),
                        StepFunctionBuilder.SubStateMachine()
                            .Comment("Branch two")
                            .StartAt("BranchTwoInitial")
                            .State("BranchTwoInitial", StepFunctionBuilder.SucceedState())
                    ))
                .State("NextState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "SimpleParallelState.json");
        }

        [Fact]
        public void SimpleParallelStateWithTasks()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("para")
                .State("para", StepFunctionBuilder.ParallelState()
                    .Transition(StepFunctionBuilder.End())
                    .Branches(
                        StepFunctionBuilder.SubStateMachine()
                            .StartAt("t")
                            .State("t", StepFunctionBuilder.TaskState().Resource("t").Transition(StepFunctionBuilder.End())),
                        StepFunctionBuilder.SubStateMachine()
                            .StartAt("u")
                            .State("u", StepFunctionBuilder.TaskState().Resource("u").Transition(StepFunctionBuilder.End()))
                    ))
                .Build();

            AssertStateMachine(stateMachine, "SimpleParallelStateWithTasks.json");
        }

        [Fact]
        public void ParallelStateWithRetriers()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ParallelState()
                    .Transition(StepFunctionBuilder.End())
                    .Branches(
                        StepFunctionBuilder.SubStateMachine()
                            .Comment("Branch one")
                            .StartAt("BranchOneInitial")
                            .State("BranchOneInitial", StepFunctionBuilder.SucceedState()),
                        StepFunctionBuilder.SubStateMachine()
                            .Comment("Branch two")
                            .StartAt("BranchTwoInitial")
                            .State("BranchTwoInitial", StepFunctionBuilder.SucceedState())
                    )
                    .Retriers(StepFunctionBuilder.Retrier()
                            .ErrorEquals("Foo", "Bar")
                            .IntervalSeconds(10)
                            .BackoffRate(1.0)
                            .MaxAttempts(3),
                        StepFunctionBuilder.Retrier()
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
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("InitialState")
                .State("InitialState", StepFunctionBuilder.ParallelState()
                    .Transition(StepFunctionBuilder.End())
                    .Branches(
                        StepFunctionBuilder.SubStateMachine()
                            .Comment("Branch one")
                            .StartAt("BranchOneInitial")
                            .State("BranchOneInitial", StepFunctionBuilder.SucceedState()),
                        StepFunctionBuilder.SubStateMachine()
                            .Comment("Branch two")
                            .StartAt("BranchTwoInitial")
                            .State("BranchTwoInitial", StepFunctionBuilder.SucceedState())
                    )
                    .Catchers(StepFunctionBuilder.Catcher()
                            .ErrorEquals("Foo", "Bar")
                            .Transition(StepFunctionBuilder.Next("RecoveryState"))
                            .ResultPath("$.result"),
                        StepFunctionBuilder.Catcher()
                            .CatchAll()
                            .Transition(StepFunctionBuilder.Next("OtherRecoveryState"))
                            .ResultPath("$.result")
                    ))
                .State("RecoveryState", StepFunctionBuilder.SucceedState())
                .State("OtherRecoveryState", StepFunctionBuilder.SucceedState())
                .Build();

            AssertStateMachine(stateMachine, "ParallelStateWithCatchers.json");
        }

        [Fact]
        public void SimpleMapState()
        {
            StateMachine stateMachine = StepFunctionBuilder.StateMachine()
                .StartAt("Validate-All")
                .State("Validate-All", StepFunctionBuilder.MapState()
                    .InputPath("$.detail")
                    .ItemPath("$.shipped")
                    .ResultPath("$.detail.shipped")
                    .MaxConcurrency(0)
                    .Parameters(JObject.FromObject(new {value = "param"}))
                    .ResultSelector(JObject.FromObject(new {value = "param"}))
                    .Transition(StepFunctionBuilder.End())
                    .Iterator(StepFunctionBuilder.SubStateMachine()
                        .StartAt("Validate")
                        .State("Validate", StepFunctionBuilder.TaskState()
                            .Resource("arn:aws:lambda:us-east-1:123456789012:function:ship-val")
                            .Transition(StepFunctionBuilder.End()))))
                    .Build();

            AssertStateMachine(stateMachine, "SimpleMapState.json");
        }

        [Fact]
        public void StateMachineFromJson_MalformedJson_ThrowsException()
        {
            Assert.Throws<StatesLanguageException>(() => StateMachine.FromJson("{"));
        }
    }
}