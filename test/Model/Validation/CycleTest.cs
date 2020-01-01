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
using StatesLanguage.Model;
using StatesLanguage.Model.Internal.Validation;
using Xunit;

namespace StatesLanguage.Tests.Model.Validation
{
    public class CycleTest
    {
        private void AssertCycle(StateMachine.Builder stateMachineBuilder)
        {
            try
            {
                Validate(stateMachineBuilder);
            }
            catch (ArgumentException expected)
            {
                Assert.Contains("Cycle detected", expected.Message);
            }
        }

        private void AssertDoesNotHaveTerminalPath(StateMachine.Builder stateMachineBuilder)
        {
            try
            {
                Validate(stateMachineBuilder);
            }
            catch (ArgumentException expected)
            {
                Assert.Contains("No path to a terminal state exists in the state machine", expected.Message);
            }
        }

        private void AssertHasPathToTerminal(StateMachine.Builder stateMachineBuilder)
        {
            Validate(stateMachineBuilder);
        }

        private void AssertNoCycle(StateMachine.Builder stateMachineBuilder)
        {
            Validate(stateMachineBuilder);
        }

        private void Validate(StateMachine.Builder stateMachineBuilder)
        {
            new StateMachineValidator(stateMachineBuilder.Build()).Validate();
        }

        [Fact]
        public void ChoiceStateWithClosedCycle_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                                                   AssertCycle(StepFunctionBuilder.StateMachine()
                                                                                  .StartAt("Initial")
                                                                                  .State("Initial", StepFunctionBuilder.PassState()
                                                                                                                       .Transition(StepFunctionBuilder.Next("Choice")))
                                                                                  .State("Choice", StepFunctionBuilder.ChoiceState()
                                                                                                                      .DefaultStateName("Terminal")
                                                                                                                      .Choice(StepFunctionBuilder.Choice()
                                                                                                                                                 .Transition(StepFunctionBuilder.Next("Terminal"))
                                                                                                                                                 .Condition(StepFunctionBuilder.Eq("$.foo", "bar")))
                                                                                                                      .Choice(StepFunctionBuilder.Choice()
                                                                                                                                                 .Transition(StepFunctionBuilder.Next("NonTerminal"))
                                                                                                                                                 .Condition(StepFunctionBuilder.Eq("$.foo", "bar"))))
                                                                                  .State("Terminal", StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.End()))
                                                                                  .State("NonTerminal",
                                                                                         StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.Next("Cyclic")))
                                                                                  .State("Cyclic",
                                                                                         StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.Next("NonTerminal")))));
        }

        [Fact]
        public void ChoiceStateWithOnlyCycles_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                                                   AssertDoesNotHaveTerminalPath(StepFunctionBuilder.StateMachine()
                                                                                                    .StartAt("Initial")
                                                                                                    .State("Initial", StepFunctionBuilder.PassState()
                                                                                                                                         .Transition(StepFunctionBuilder.Next("Choice")))
                                                                                                    .State("Choice", StepFunctionBuilder.ChoiceState()
                                                                                                                                        .DefaultStateName("Default")
                                                                                                                                        .Choice(StepFunctionBuilder.Choice()
                                                                                                                                                                   .Transition(StepFunctionBuilder
                                                                                                                                                                                   .Next("Initial"))
                                                                                                                                                                   .Condition(StepFunctionBuilder
                                                                                                                                                                                  .Eq("$.foo",
                                                                                                                                                                                      "bar")))
                                                                                                                                        .Choice(StepFunctionBuilder.Choice()
                                                                                                                                                                   .Transition(StepFunctionBuilder
                                                                                                                                                                                   .Next("Default"))
                                                                                                                                                                   .Condition(StepFunctionBuilder
                                                                                                                                                                                  .Eq("$.foo",
                                                                                                                                                                                      "bar"))))
                                                                                                    .State("Default",
                                                                                                           StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.Next("Choice")))));
        }

        /**
         * While the nested ChoiceTwo state only has cycles, it has a cycle out of the choice state to
         * a state that contains a path to a terminal. The validator doesn't validate that the path out actually
         * has a path to the terminal so there are some invalid state machines that will pass validation.
         */
        [Fact]
        public void ChoiceStateWithPathOut_IsValid()
        {
            AssertNoCycle(
                          StepFunctionBuilder.StateMachine()
                                             .StartAt("Initial")
                                             .State("Initial", StepFunctionBuilder.PassState()
                                                                                  .Transition(StepFunctionBuilder.Next("ChoiceOne")))
                                             .State("ChoiceOne", StepFunctionBuilder.ChoiceState()
                                                                                    .DefaultStateName("DefaultOne")
                                                                                    .Choice(StepFunctionBuilder.Choice()
                                                                                                               .Transition(StepFunctionBuilder.Next("ChoiceTwo"))
                                                                                                               .Condition(StepFunctionBuilder.Eq("$.foo", "bar"))))
                                             .State("DefaultOne", StepFunctionBuilder.SucceedState())
                                             .State("ChoiceTwo", StepFunctionBuilder.ChoiceState()
                                                                                    .DefaultStateName("DefaultTwo")
                                                                                    .Choice(StepFunctionBuilder.Choice()
                                                                                                               .Transition(StepFunctionBuilder.Next("ChoiceOne"))
                                                                                                               .Condition(StepFunctionBuilder.Eq("$.foo", "bar"))))
                                             .State("DefaultTwo",
                                                    StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.Next("ChoiceTwo"))));
        }

        [Fact]
        public void ChoiceStateWithPathToTerminal_IsValid()
        {
            AssertHasPathToTerminal(StepFunctionBuilder.StateMachine()
                                                       .StartAt("Initial")
                                                       .State("Initial", StepFunctionBuilder.PassState()
                                                                                            .Transition(StepFunctionBuilder.Next("Choice")))
                                                       .State("Choice", StepFunctionBuilder.ChoiceState()
                                                                                           .DefaultStateName("Default")
                                                                                           .Choice(StepFunctionBuilder.Choice()
                                                                                                                      .Transition(StepFunctionBuilder.Next("Initial"))
                                                                                                                      .Condition(StepFunctionBuilder.Eq("$.foo", "bar")))
                                                                                           .Choice(StepFunctionBuilder.Choice()
                                                                                                                      .Transition(StepFunctionBuilder.Next("Default"))
                                                                                                                      .Condition(StepFunctionBuilder.Eq("$.foo", "bar"))))
                                                       .State("Default", StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.End())));
        }

        [Fact]
        public void ParallelState_BranchContainsChoiceStateWithClosedCycle_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                                                   AssertCycle(
                                                               StepFunctionBuilder.StateMachine()
                                                                                  .StartAt("Initial")
                                                                                  .State("Initial", StepFunctionBuilder.PassState()
                                                                                                                       .Transition(StepFunctionBuilder.Next("Choice")))
                                                                                  .State("Choice", StepFunctionBuilder.ChoiceState()
                                                                                                                      .DefaultStateName("Terminal")
                                                                                                                      .Choice(StepFunctionBuilder.Choice()
                                                                                                                                                 .Transition(StepFunctionBuilder.Next("Terminal"))
                                                                                                                                                 .Condition(StepFunctionBuilder.Eq("$.foo", "bar")))
                                                                                                                      .Choice(StepFunctionBuilder.Choice()
                                                                                                                                                 .Transition(StepFunctionBuilder.Next("NonTerminal"))
                                                                                                                                                 .Condition(StepFunctionBuilder.Eq("$.foo", "bar"))))
                                                                                  .State("Terminal", StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.End()))
                                                                                  .State("NonTerminal",
                                                                                         StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.Next("Cyclic")))
                                                                                  .State("Cyclic",
                                                                                         StepFunctionBuilder.PassState().Transition(StepFunctionBuilder.Next("NonTerminal")))));
        }

        [Fact]
        public void ParallelState_ChoiceStateWithTerminalPath_IsValid()
        {
            AssertHasPathToTerminal(
                                    StepFunctionBuilder.StateMachine()
                                                       .StartAt("Parallel")
                                                       .State("Parallel", StepFunctionBuilder.ParallelState()
                                                                                             .Transition(StepFunctionBuilder.End())
                                                                                             .Branch(StepFunctionBuilder.SubStateMachine()
                                                                                                                        .StartAt("Initial")
                                                                                                                        .State("Initial", StepFunctionBuilder.PassState()
                                                                                                                                                             .Transition(StepFunctionBuilder
                                                                                                                                                                             .Next("Choice")))
                                                                                                                        .State("Choice", StepFunctionBuilder.ChoiceState()
                                                                                                                                                            .DefaultStateName("Default")
                                                                                                                                                            .Choice(StepFunctionBuilder.Choice()
                                                                                                                                                                                       .Transition(StepFunctionBuilder
                                                                                                                                                                                                       .Next("Initial"))
                                                                                                                                                                                       .Condition(StepFunctionBuilder
                                                                                                                                                                                                      .Eq("$.foo",
                                                                                                                                                                                                          "bar")))
                                                                                                                                                            .Choice(StepFunctionBuilder.Choice()
                                                                                                                                                                                       .Transition(StepFunctionBuilder
                                                                                                                                                                                                       .Next("Default"))
                                                                                                                                                                                       .Condition(StepFunctionBuilder
                                                                                                                                                                                                      .Eq("$.foo",
                                                                                                                                                                                                          "bar"))))
                                                                                                                        .State("Default",
                                                                                                                               StepFunctionBuilder
                                                                                                                                   .PassState().Transition(StepFunctionBuilder.End())))));
        }

        [Fact]
        public void ParallelState_NoCycles()
        {
            AssertNoCycle(StepFunctionBuilder.StateMachine()
                                             .StartAt("Initial")
                                             .State("Initial", StepFunctionBuilder.ParallelState()
                                                                                  .Branch(StepFunctionBuilder.SubStateMachine()
                                                                                                             .StartAt("BranchOneStart")
                                                                                                             .State("BranchOneStart", StepFunctionBuilder.SucceedState()))
                                                                                  .Branch(StepFunctionBuilder.SubStateMachine()
                                                                                                             .StartAt("BranchTwoStart")
                                                                                                             .State("BranchTwoStart", StepFunctionBuilder.PassState()
                                                                                                                                                         .Transition(StepFunctionBuilder
                                                                                                                                                                         .Next("NextState")))
                                                                                                             .State("NextState", StepFunctionBuilder.SucceedState()))
                                                                                  .Transition(StepFunctionBuilder.End())));
        }

        [Fact]
        public void ParallelState_WithChoiceThatHasNoTerminalPath_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                                                   AssertDoesNotHaveTerminalPath(
                                                                                 StepFunctionBuilder.StateMachine()
                                                                                                    .StartAt("Parallel")
                                                                                                    .State("Parallel", StepFunctionBuilder.ParallelState()
                                                                                                                                          .Transition(StepFunctionBuilder.End())
                                                                                                                                          .Branch(StepFunctionBuilder.SubStateMachine()
                                                                                                                                                                     .StartAt("Initial")
                                                                                                                                                                     .State("Initial",
                                                                                                                                                                            StepFunctionBuilder
                                                                                                                                                                                .PassState()
                                                                                                                                                                                .Transition(StepFunctionBuilder
                                                                                                                                                                                                .Next("Choice")))
                                                                                                                                                                     .State("Choice",
                                                                                                                                                                            StepFunctionBuilder
                                                                                                                                                                                .ChoiceState()
                                                                                                                                                                                .DefaultStateName("Default")
                                                                                                                                                                                .Choice(StepFunctionBuilder
                                                                                                                                                                                        .Choice()
                                                                                                                                                                                        .Transition(StepFunctionBuilder
                                                                                                                                                                                                        .Next("Initial"))
                                                                                                                                                                                        .Condition(StepFunctionBuilder
                                                                                                                                                                                                       .Eq("$.foo",
                                                                                                                                                                                                           "bar")))
                                                                                                                                                                                .Choice(StepFunctionBuilder
                                                                                                                                                                                        .Choice()
                                                                                                                                                                                        .Transition(StepFunctionBuilder
                                                                                                                                                                                                        .Next("Default"))
                                                                                                                                                                                        .Condition(StepFunctionBuilder
                                                                                                                                                                                                       .Eq("$.foo",
                                                                                                                                                                                                           "bar"))))
                                                                                                                                                                     .State("Default",
                                                                                                                                                                            StepFunctionBuilder
                                                                                                                                                                                .PassState()
                                                                                                                                                                                .Transition(StepFunctionBuilder
                                                                                                                                                                                                .Next("Choice")))))));
        }

        [Fact]
        public void ParallelState_WithCycles_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                                                   AssertCycle(StepFunctionBuilder.StateMachine()
                                                                                  .StartAt("Parallel")
                                                                                  .State("Parallel", StepFunctionBuilder.ParallelState()
                                                                                                                        .Branch(StepFunctionBuilder.SubStateMachine()
                                                                                                                                                   .StartAt("BranchOneInitial")
                                                                                                                                                   .State("BranchOneInitial", StepFunctionBuilder
                                                                                                                                                                              .PassState()
                                                                                                                                                                              .Transition(StepFunctionBuilder
                                                                                                                                                                                              .Next("CyclicState")))
                                                                                                                                                   .State("CyclicState", StepFunctionBuilder
                                                                                                                                                                         .PassState()
                                                                                                                                                                         .Transition(StepFunctionBuilder
                                                                                                                                                                                         .Next("BranchOneInitial"))))
                                                                                                                        .Transition(StepFunctionBuilder.End()))));
        }

        [Fact]
        public void SimpleStateMachine_WithCycle_IsNotValid()
        {
            Assert.Throws<ValidationException>(() =>
                                                   AssertCycle(StepFunctionBuilder.StateMachine()
                                                                                  .StartAt("Initial")
                                                                                  .State("Initial", StepFunctionBuilder.PassState()
                                                                                                                       .Transition(StepFunctionBuilder.Next("Next")))
                                                                                  .State("Next", StepFunctionBuilder.PassState()
                                                                                                                    .Transition(StepFunctionBuilder.Next("Initial")))));
        }

        [Fact]
        public void SingleTerminalState_HasNoCycle_IsValid()
        {
            AssertNoCycle(StepFunctionBuilder.StateMachine()
                                             .StartAt("Initial")
                                             .State("Initial", StepFunctionBuilder.SucceedState()));
        }
    }
}