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
using System.Collections.Generic;
using StatesLanguage.Model.Conditions;
using StatesLanguage.Model.States;

namespace StatesLanguage.Model.Internal.Validation
{
    public class StateMachineValidator
    {
        private readonly ProblemReporter _problemReporter = new ProblemReporter();
        private readonly StateMachine _stateMachine;

        public StateMachineValidator(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public StateMachine Validate()
        {
            var context = ValidationContext.GetBuilder()
                                           .ProblemReporter(_problemReporter)
                                           .ParentContext(null)
                                           .Identifier("Root")
                                           .Location(Location.StateMachine)
                                           .Build();
            context.AssertStringNotEmpty(_stateMachine.StartAt, PropertyNames.START_AT);
            context.AssertIsPositiveIfPresent(_stateMachine.TimeoutSeconds, PropertyNames.TIMEOUT_SECONDS);
            context.AssertNotEmpty(_stateMachine.States, PropertyNames.STATES);

            ValidateStates(context, _stateMachine.States);

            if (!string.IsNullOrEmpty(_stateMachine.StartAt))
            {
                if (!_stateMachine.States.ContainsKey(_stateMachine.StartAt))
                {
                    _problemReporter.Report(new Problem(context, $"{PropertyNames.START_AT} state does not exist."));
                }
            }

            // If basic validation failed then the graph may not be in a good state to be able to Validate
            if (!_problemReporter.HasProblems())
            {
                new GraphValidator(context, _stateMachine).Validate();
            }

            if (_problemReporter.HasProblems())
            {
                throw _problemReporter.GetException();
            }

            return _stateMachine;
        }

        private void ValidateStates(ValidationContext parentContext, Dictionary<string, State> states)
        {
            foreach (var entry in states)
            {
                parentContext.AssertStringNotEmpty(entry.Key, "State Name");
                entry.Value.Accept(new StateValidationVisitor(states, parentContext.State(entry.Key)));
            }
        }

        /// <summary>
        ///     Validates the DFS does not contain unrecoverable cycles (i.e. cycles with no branching logic) or does not contain a
        ///     path to a terminal state.
        /// </summary>
        private sealed class GraphValidator
        {
            private readonly ValidationContext _currentContext;
            private readonly string _initialState;
            private readonly IDictionary<string, State> _parentVisited;
            private readonly IDictionary<string, State> _states;
            private readonly Dictionary<string, State> _visited = new Dictionary<string, State>();

            internal GraphValidator(ValidationContext context, StateMachine stateMachine) : this(context,
                                                                                                 new Dictionary<string, State>(),
                                                                                                 stateMachine.StartAt,
                                                                                                 stateMachine.States)
            {
            }

            private GraphValidator(ValidationContext context,
                                   IDictionary<string, State> parentVisited,
                                   string initialState,
                                   IDictionary<string, State> states)
            {
                _currentContext = context;
                _parentVisited = parentVisited;
                _initialState = initialState;
                _states = states;
            }

            public bool Validate()
            {
                var pathToTerminal = Visit(_initialState);
                if (_parentVisited.Count == 0 && !pathToTerminal)
                {
                    _currentContext.ProblemReporter.Report(new Problem(_currentContext, "No path to a terminal state exists."));
                }

                return pathToTerminal;
            }

            private bool Visit(string stateName)
            {
                var stateContext = _currentContext.State(stateName);
                var state = _states[stateName];
                if (!_parentVisited.ContainsKey(stateName) && _visited.ContainsKey(stateName))
                {
                    return false;
                }

                if (_parentVisited.ContainsKey(stateName))
                {
                    return false;
                }

                _visited.Add(stateName, state);
                if (state is ParallelState parallelState)
                {
                    ValidateParallelState(stateContext, parallelState);
                }

                if (state.IsTerminalState)
                {
                    return true;
                }

                if (state is TransitionState transitionState)
                {
                    var transition = transitionState.Transition;
                    return Visit(((NextStateTransition) transition).NextStateName);
                }

                if (state is ChoiceState choiceState)
                {
                    return ValidateChoiceState(stateContext, choiceState);
                }

                throw new ValidationException("Unexpected state type: " + state.GetType().Name);
            }

            private void ValidateParallelState(ValidationContext stateContext, ParallelState state)
            {
                var index = 0;
                foreach (var branch in state.Branches)
                {
                    new GraphValidator(stateContext.Branch(index),
                                       new Dictionary<string, State>(),
                                       branch.StartAt,
                                       branch.States).Validate();
                    index++;
                }
            }

            private bool ValidateChoiceState(ValidationContext stateContext, ChoiceState choiceState)
            {
                var merged = MergeParentVisited();
                var hasPathToTerminal = false;
                if(!string.IsNullOrEmpty(choiceState.DefaultStateName))
                {
                    hasPathToTerminal = new GraphValidator(stateContext, merged, choiceState.DefaultStateName, _states).Validate();
                }
                var index = 0;
                foreach (var choice in choiceState.Choices)
                {
                    var nextStateName = ((NextStateTransition) choice.Transition).NextStateName;
                    // It's important hasPathToTerminal is last in the OR so it doesn't short circuit the choice validation
                    hasPathToTerminal = new GraphValidator(stateContext.Choice(index), merged, nextStateName, _states)
                                            .Validate() ||
                                        hasPathToTerminal;
                    index++;
                }

                return hasPathToTerminal;
            }

            private Dictionary<string, State> MergeParentVisited()
            {
                var merged = new Dictionary<string, State>(_parentVisited.Count + _visited.Count);
                foreach (var p in _parentVisited)
                {
                    merged.Add(p.Key, p.Value);
                }

                foreach (var p in _visited)
                {
                    merged.Add(p.Key, p.Value);
                }

                return merged;
            }
        }

        /**
         * Validates all the supported states and their nested properties.
         */
        internal class StateValidationVisitor : StateVisitor<int>
        {
            private readonly ValidationContext _currentContext;

            private readonly ProblemReporter _problemReporter;
            private readonly Dictionary<string, State> _states;

            internal StateValidationVisitor(Dictionary<string, State> states, ValidationContext context)
            {
                _states = states;
                _currentContext = context;
                _problemReporter = context.ProblemReporter;
            }

            public override int Visit(ChoiceState choiceState)
            {
                _currentContext.AssertIsValidInputPath(choiceState.InputPath);
                _currentContext.AssertIsValidOutputPath(choiceState.OutputPath);
                if (choiceState.DefaultStateName != null)
                {
                    _currentContext.AssertStringNotEmpty(choiceState.DefaultStateName, PropertyNames.DEFAULT_STATE);
                    AssertContainsState(choiceState.DefaultStateName);
                }

                _currentContext.AssertNotEmpty(choiceState.Choices, PropertyNames.CHOICES);
                var index = 0;
                foreach (var choice in choiceState.Choices)
                {
                    var choiceContext = _currentContext.Choice(index);
                    ValidateTransition(choiceContext, choice.Transition);
                    ValidateCondition(choiceContext, choice.Condition);
                    index++;
                }

                return 0;
            }

            public override int Visit(FailState failState)
            {
                _currentContext.AssertStringNotEmpty(failState.Cause, PropertyNames.CAUSE);
                return 0;
            }

            public override int Visit(ParallelState parallelState)
            {
                _currentContext.AssertIsValidInputPath(parallelState.InputPath);
                _currentContext.AssertIsValidOutputPath(parallelState.OutputPath);
                _currentContext.AssertIsValidResultPath(parallelState.ResultPath);
                ValidateTransition(parallelState.Transition);
                ValidateRetriers(parallelState.Retriers);
                ValidateCatchers(parallelState.Catchers);
                ValidateBranches(parallelState);
                return 0;
            }
            
            public override int Visit(MapState mapState)
            {
                _currentContext.AssertIsValidInputPath(mapState.InputPath);
                _currentContext.AssertIsValidOutputPath(mapState.OutputPath);
                _currentContext.AssertIsValidResultPath(mapState.ResultPath);
                _currentContext.AssertIsValidItemPath(mapState.ItemsPath);
                _currentContext.AssertIsNotNegativeIfPresent(mapState.MaxConcurrency,PropertyNames.MAX_CONCURENCY);
                ValidateTransition(mapState.Transition);
                ValidateRetriers(mapState.Retriers);
                ValidateCatchers(mapState.Catchers);
                ValidateIterator(mapState);
                return 0;
            }

            public override int Visit(PassState passState)
            {
                _currentContext.AssertIsValidInputPath(passState.InputPath);
                _currentContext.AssertIsValidOutputPath(passState.OutputPath);
                _currentContext.AssertIsValidResultPath(passState.ResultPath);
                ValidateTransition(passState.Transition);
                return 0;
            }

            public override int Visit(SucceedState succeedState)
            {
                _currentContext.AssertIsValidInputPath(succeedState.InputPath);
                _currentContext.AssertIsValidOutputPath(succeedState.OutputPath);
                return 0;
            }

            public override int Visit(TaskState taskState)
            {
                _currentContext.AssertIsValidInputPath(taskState.InputPath);
                _currentContext.AssertIsValidOutputPath(taskState.OutputPath);
                _currentContext.AssertIsValidResultPath(taskState.ResultPath);
                _currentContext.AssertIsPositiveIfPresent(taskState.TimeoutSeconds, PropertyNames.TIMEOUT_SECONDS);
                _currentContext.AssertIsPositiveIfPresent(taskState.HeartbeatSeconds, PropertyNames.HEARTBEAT_SECONDS);
                if (taskState.HeartbeatSeconds != null)
                {
                    if (taskState.HeartbeatSeconds >= taskState.TimeoutSeconds)
                    {
                        _problemReporter.Report(new Problem(_currentContext, $"{PropertyNames.HEARTBEAT_SECONDS} must be smaller than {PropertyNames.TIMEOUT_SECONDS}"));
                    }
                }

                _currentContext.AssertStringNotEmpty(taskState.Resource, PropertyNames.RESOURCE);
                ValidateRetriers(taskState.Retriers);
                ValidateCatchers(taskState.Catchers);
                ValidateTransition(taskState.Transition);
                return 0;
            }

            public override int Visit(WaitState waitState)
            {
                _currentContext.AssertIsValidInputPath(waitState.InputPath);
                _currentContext.AssertIsValidOutputPath(waitState.OutputPath);
                ValidateTransition(waitState.Transition);
                ValidateWaitFor(waitState.WaitFor);
                return 0;
            }

            private void ValidateCondition(ValidationContext context, ICondition condition)
            {
                context.AssertNotNull(condition, "Condition");
                if (condition is BinaryCondition<string> strCondition)
                {
                    ValidateBinaryCondition(context, strCondition);
                }
                else if (condition is BinaryCondition binCondition)
                {
                    ValidateBinaryCondition(context, binCondition);
                }
                else if (condition is INaryCondition iNaryCondition)
                {
                    ValidateNAryCondition(context, iNaryCondition);
                }
                else if (condition is NotCondition notCondition)
                {
                    ValidateCondition(context, notCondition.Condition);
                }
                else if (condition != null)
                {
                    throw new ValidationException("Unsupported condition type: " + condition.GetType());
                }
            }

            private void ValidateStates(ValidationContext parentContext, Dictionary<string, State> states)
            {
                foreach (var entry in states)
                {
                    parentContext.AssertStringNotEmpty(entry.Key, "State Name");
                    entry.Value.Accept(new StateValidationVisitor(states, parentContext.State(entry.Key)));
                }
            }

            private void ValidateNAryCondition(ValidationContext context, INaryCondition condition)
            {
                context.AssertNotEmpty(condition.Conditions, "Conditions");
                foreach (var nestedCondition in condition.Conditions)
                {
                    ValidateCondition(context, nestedCondition);
                }
            }

            private void ValidateBinaryCondition(ValidationContext context, BinaryCondition condition)
            {
                context.AssertStringNotEmpty(condition.Variable, PropertyNames.VARIABLE);
                context.AssertIsValidJsonPath(condition.Variable, PropertyNames.VARIABLE);
            }

            private void ValidateBinaryCondition<T>(ValidationContext context, BinaryCondition<T> condition) where T : IComparable<T>
            {
                context.AssertStringNotEmpty(condition.Variable, PropertyNames.VARIABLE);
                context.AssertIsValidJsonPath(condition.Variable, PropertyNames.VARIABLE);
                context.AssertNotNull(condition.ExpectedValue, "ExpectedValue");
            }

            private void ValidateIterator(MapState mapState)
            {
                _currentContext.AssertNotNull(mapState.Iterator, PropertyNames.ITERATOR);
                var iteratorContext = _currentContext.Iterator();
                ValidateStates(iteratorContext, mapState.Iterator.States);
                
                _currentContext.AssertStringNotEmpty(mapState.Iterator.StartAt, PropertyNames.START_AT);
                if (!mapState.Iterator.States.ContainsKey(mapState.Iterator.StartAt))
                {
                    _problemReporter.Report(new Problem(iteratorContext, $"{PropertyNames.START_AT} references a non existent state."));
                }
            }
            private void ValidateBranches(ParallelState parallelState)
            {
                _currentContext.AssertNotEmpty(parallelState.Branches, PropertyNames.BRANCHES);
                var index = 0;
                foreach (var branch in parallelState.Branches)
                {
                    var branchContext = _currentContext.Branch(index);

                    ValidateStates(branchContext, branch.States);
                    _currentContext.AssertStringNotEmpty(branch.StartAt, PropertyNames.START_AT);
                    if (!branch.States.ContainsKey(branch.StartAt))
                    {
                        _problemReporter.Report(new Problem(branchContext, $"{PropertyNames.START_AT} references a non existent state."));
                    }

                    index++;
                }
            }

            private void ValidateRetriers(IEnumerable<Retrier> retriers)
            {
                var hasRetryAll = false;
                var index = 0;
                foreach (var retrier in retriers)
                {
                    var retrierContext = _currentContext.Retrier(index);
                    if (hasRetryAll)
                    {
                        _problemReporter.Report(
                                                new Problem(retrierContext,
                                                            $"When {ErrorCodes.ALL} is used in must be in the last Retrier"));
                    }

                    // MaxAttempts may be zero
                    retrierContext.AssertIsNotNegativeIfPresent(retrier.MaxAttempts, PropertyNames.MAX_ATTEMPTS);
                    retrierContext.AssertIsPositiveIfPresent(retrier.IntervalSeconds, PropertyNames.INTERVAL_SECONDS);
                    if (retrier.BackoffRate < 1.0)
                    {
                        _problemReporter.Report(new Problem(retrierContext,
                                                            $"{PropertyNames.BACKOFF_RATE} must be greater than or equal to 1.0"));
                    }

                    hasRetryAll = ValidateErrorEquals(retrierContext, retrier.ErrorEquals);
                    index++;
                }
            }

            private void ValidateCatchers(IEnumerable<Catcher> catchers)
            {
                var hasCatchAll = false;
                var index = 0;
                foreach (var catcher in catchers)
                {
                    var catcherContext = _currentContext.Catcher(index);
                    catcherContext.AssertIsValidResultPath(catcher.ResultPath);
                    if (hasCatchAll)
                    {
                        _problemReporter.Report(
                                                new Problem(catcherContext,
                                                            $"When {ErrorCodes.ALL} is used in must be in the last Catcher"));
                    }

                    ValidateTransition(catcherContext, catcher.Transition);
                    hasCatchAll = ValidateErrorEquals(catcherContext, catcher.ErrorEquals);
                    index++;
                }
            }

            private bool ValidateErrorEquals(ValidationContext currentContext, ICollection<string> errorEquals)
            {
                currentContext.AssertNotEmpty(errorEquals, PropertyNames.ERROR_EQUALS);
                if (errorEquals.Contains(ErrorCodes.ALL))
                {
                    if (errorEquals.Count != 1)
                    {
                        _problemReporter.Report(new Problem(currentContext,
                                                            $"When {ErrorCodes.ALL} is used in {PropertyNames.ERROR_EQUALS}, it must be the only error code in the array"));
                    }

                    return true;
                }

                return false;
            }

            private void ValidateWaitFor(IWaitFor waitFor)
            {
                _currentContext.AssertNotNull(waitFor, "WaitFor");
                if (waitFor is WaitForSeconds waitForSeconds)
                {
                    _currentContext.AssertIsPositiveIfPresent(waitForSeconds.Seconds,
                                                              PropertyNames.SECONDS);
                }
                else if (waitFor is WaitForSecondsPath waitForSecondsPath)
                {
                    AssertWaitForPath(waitForSecondsPath.SecondsPath, PropertyNames.SECONDS_PATH);
                }
                else if (waitFor is WaitForTimestamp waitForTimestamp)
                {
                    _currentContext.AssertNotNull(waitForTimestamp.Timestamp, PropertyNames.TIMESTAMP);
                }
                else if (waitFor is WaitForTimestampPath waitForTimestampPath)
                {
                    AssertWaitForPath(waitForTimestampPath.TimestampPath, PropertyNames.TIMESTAMP_PATH);
                }
                else if (waitFor != null)
                {
                    throw new Exception("Unsupported WaitFor strategy: " + waitFor.GetType());
                }
            }

            /**
             * TimestampPath and SecondsPath must have a valid reference path.
             */
            private void AssertWaitForPath(string pathValue, string propertyName)
            {
                _currentContext.AssertNotNull(pathValue, propertyName);
                _currentContext.AssertIsValidReferencePath(pathValue, propertyName);
            }

            private void ValidateTransition(ITransition transition)
            {
                ValidateTransition(_currentContext, transition);
            }

            private void ValidateTransition(ValidationContext context, ITransition transition)
            {
                context.AssertNotNull(transition, "Transition");
                if (transition is NextStateTransition nextStateTransition)
                {
                    var nextStateName = nextStateTransition.NextStateName;
                    context.AssertNotNull(nextStateName, PropertyNames.NEXT);
                    AssertContainsState(context, nextStateName);
                }
            }

            private void AssertContainsState(string nextStateName)
            {
                AssertContainsState(_currentContext, nextStateName);
            }

            private void AssertContainsState(ValidationContext context, string nextStateName)
            {
                if (!_states.ContainsKey(nextStateName))
                {
                    _problemReporter.Report(new Problem(context,
                                                        $"{nextStateName} is not a valid state"));
                }
            }
        }
    }
}