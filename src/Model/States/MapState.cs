using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    public class MapState: TransitionState
    {
        public override StateType Type => StateType.Map;
        
        [JsonProperty(PropertyNames.COMMENT)]
        public string Comment { get; private set; }
        
        [JsonProperty(PropertyNames.INPUT_PATH)]
        public string InputPath { get; private set; }

        [JsonProperty(PropertyNames.RESULT_PATH)]
        public string ResultPath { get; private set; }

        [JsonProperty(PropertyNames.OUTPUT_PATH)]
        public string OutputPath { get; private set; }

        [JsonProperty(PropertyNames.PARAMETERS)]
        public JToken Parameters { get; private set; }
        
        [JsonProperty(PropertyNames.MAX_CONCURENCY)]
        public int? MaxConcurrency { get; private set; }
        
        [JsonProperty(PropertyNames.ITEMS_PATH)]
        public string ItemsPath { get; private set; }
        
        [JsonIgnore]
        public SubStateMachine Iterator { get; private set; }
        
        [JsonProperty(PropertyNames.RETRY)]
        public List<Retrier> Retriers { get; private set; }

        [JsonProperty(PropertyNames.CATCH)]
        public List<Catcher> Catchers { get; private set; }
        
        public override bool IsTerminalState => Transition.IsTerminal;
        
        /**
         * @return Builder instance to construct a {@link MapState}.
         */
        internal static Builder GetBuilder()
        {
            return new Builder();
        }
        
        public override T Accept<T>(StateVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public sealed class Builder : TransitionStateBuilder<MapState, Builder>
        {
            [JsonProperty(PropertyNames.ITERATOR)] 
            private SubStateMachine.Builder _iterator = new SubStateMachine.Builder();
            
            [JsonProperty(PropertyNames.CATCH)]
            private List<Catcher.Builder> _catchers = new List<Catcher.Builder>();
            
            [JsonProperty(PropertyNames.COMMENT)] private string _comment;

            [JsonProperty(PropertyNames.INPUT_PATH)]
            private string _inputPath;

            [JsonProperty(PropertyNames.OUTPUT_PATH)]
            private string _outputPath;

            [JsonProperty(PropertyNames.RESULT_PATH)]
            private string _resultPath;

            [JsonProperty(PropertyNames.PARAMETERS)]
            private JToken _parameters;

            [JsonProperty(PropertyNames.MAX_CONCURENCY)]
            private int? _maxConcurrency;

            [JsonProperty(PropertyNames.ITEMS_PATH)]
            private string _itemsPath;

            [JsonProperty(PropertyNames.RETRY)] 
            private List<Retrier.Builder> _retriers = new List<Retrier.Builder>();

            private ITransitionBuilder<ITransition> _transition = NullTransitionBuilder<ITransition>.Instance;
            
            internal Builder()
            {
            }
            
            
            /**
             * OPTIONAL. Human readable description for the state.
             *
             * @param comment New comment.
             * @return This object for method chaining.
             */
            public Builder Comment(string comment)
            {
                _comment = comment;
                return this;
            }

            /**
             * REQUIRED. Set the iterator for thi MapState.
             *
             * @param iteratorBuilder Instance of {@link Branch.Builder}. Note that the {@link
             *                      Branch} object is not built until the {@link ParallelState} is built so any modifications on the
             *                      state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Iterator(SubStateMachine.Builder iteratorBuilder)
            {
                _iterator = iteratorBuilder;
                return this;
            }

            /**
             * OPTIONAL. The value of “InputPath” MUST be a Path, which is applied to a State’s raw input to select some or all of
             * it;
             * that selection is used by the state. If not provided then the whole output from the previous state is used as input to
             * this state.
             *
             * @param inputPath New path value.
             * @return This object for method chaining.
             */
            public Builder InputPath(string inputPath)
            {
                _inputPath = inputPath;
                return this;
            }

            /**
             * OPTIONAL. The value of “ResultPath” MUST be a Reference Path, which specifies the combination with or replacement of
             * the state’s result with its raw input. If not provided then the output completely replaces the input.
             *
             * @param resultPath New path value.
             * @return This object for method chaining.
             */
            public Builder ResultPath(string resultPath)
            {
                _resultPath = resultPath;
                return this;
            }

            /**
             * OPTIONAL. The value of “OutputPath” MUST be a path, which is applied to the state’s output after the application of
             * ResultPath, leading in the generation of the raw input for the next state. If not provided then the whole output is
             * used.
             *
             * @param outputPath New path value.
             * @return This object for method chaining.
             */
            public Builder OutputPath(string outputPath)
            {
                _outputPath = outputPath;
                return this;
            }
            
            /// <summary>
            /// Reference path identifying where in the effective input the array field is found.
            /// </summary>
            /// <param name="itemPath"></param>
            /// <returns></returns>
            public Builder ItemPath(string itemPath)
            {
                _itemsPath = itemPath;
                return this;
            }
            
            /// <summary>
            /// Provides an upper bound on how many invocations of the Iterator may run in parallel.
            /// </summary>
            /// <param name="maxConcurrency"></param>
            /// <returns></returns>
            public Builder MaxConcurrency(int maxConcurrency)
            {
                _maxConcurrency = maxConcurrency;
                return this;
            }

            public Builder Parameters(JToken parameters)
            {
                _parameters = parameters;
                return this;
            }

            /**
             * REQUIRED. Sets the transition that will occur when all branches in this parallel
             * state have executed successfully.
             *
             * @param transition New transition.
             * @return This object for method chaining.
             */
            public override Builder Transition<U>(ITransitionBuilder<U> transition)
            {
                _transition = (ITransitionBuilder<ITransition>) transition;
                return this;
            }

            /**
             * OPTIONAL. Adds the {@link Retrier}s to this states retries. If a single branch fails then the entire parallel state is
             * considered failed and eligible for retry.
             *
             * @param retrierBuilders Instances of {@link Retrier.Builder}. Note that the {@link
             *                        Retrier} object is not built until the {@link ParallelState} is built so any modifications on
             *                        the state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Retriers(params Retrier.Builder[] retrierBuilders)
            {
                _retriers.AddRange(retrierBuilders);
                return this;
            }

            /**
             * OPTIONAL. Adds the {@link Retrier} to this states retries. If a single branch fails then the entire parallel state is
             * considered failed and eligible for retry.
             *
             * @param retrierBuilder Instance of {@link Retrier.Builder}. Note that the {@link
             *                       Retrier} object is not built until the {@link ParallelState} is built so any modifications on
             *                       the
             *                       state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Retrier(Retrier.Builder retrierBuilder)
            {
                _retriers.Add(retrierBuilder);
                return this;
            }

            /**
             * OPTIONAL. Adds the {@link Catcher}s to this states catchers.  If a single branch fails then the entire parallel state
             * is considered failed and eligible to be caught.
             *
             * @param catcherBuilders Instances of {@link Catcher.Builder}. Note that the {@link
             *                        Catcher} object is not built until the {@link ParallelState} is built so any modifications on
             *                        the state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Catchers(params Catcher.Builder[] catcherBuilders)
            {
                _catchers.AddRange(catcherBuilders);
                return this;
            }

            /**
             * OPTIONAL. Adds the {@link Catcher} to this states catchers.  If a single branch fails then the entire parallel state
             * is
             * considered failed and eligible to be caught.
             *
             * @param catcherBuilder Instance of {@link Catcher.Builder}. Note that the {@link
             *                       Catcher} object is not built until the {@link ParallelState} is built so any modifications on
             *                       the
             *                       state model will be reflected in this object.
             * @return This object for method chaining.
             */
            public Builder Catcher(Catcher.Builder catcherBuilder)
            {
                _catchers.Add(catcherBuilder);
                return this;
            }

            /**
             * @return An immutable {@link ParallelState} object.
             */
            public override MapState Build()
            {
                return new MapState()
                       {
                           Comment = _comment,
                           Iterator = _iterator.Build(),
                           ItemsPath = _itemsPath,
                           MaxConcurrency = _maxConcurrency,
                           InputPath = _inputPath,
                           ResultPath = _resultPath,
                           OutputPath = _outputPath,
                           Parameters = _parameters,
                           Transition = _transition.Build(),
                           Retriers = BuildableUtils.Build(_retriers),
                           Catchers = BuildableUtils.Build(_catchers)
                       };
            }
        }
    }
}