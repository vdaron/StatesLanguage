using Newtonsoft.Json;
using StatesLanguage.Internal;

namespace StatesLanguage.States
{
    public class MapState : RetryCatchState
    {
        public override StateType Type => StateType.Map;

        [JsonProperty(PropertyNames.MAX_CONCURENCY)]
        public int? MaxConcurrency { get; private set; }

        [JsonProperty(PropertyNames.ITEMS_PATH)]
        public string ItemsPath { get; private set; }
        
        [JsonProperty(PropertyNames.TOLERATED_FAILURE_PERCENTAGE)]
        public int? ToleratedFailurePercentage { get; private set; }
        
        [JsonProperty(PropertyNames.TOLERATED_FAILURE_COUNT)]
        public int? ToleratedFailureCount { get; private set; }

        [JsonIgnore]
        public SubStateMachine Iterator { get; private set; }

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

        public sealed class Builder : RetryCatchStateBuilder<MapState, Builder>
        {
            [JsonProperty(PropertyNames.ITEMS_PATH)]
            private string _itemsPath;

            [JsonProperty(PropertyNames.ITERATOR)]
            private SubStateMachine.Builder _iterator = new SubStateMachine.Builder();

            [JsonProperty(PropertyNames.MAX_CONCURENCY)]
            private int? _maxConcurrency;
            [JsonProperty(PropertyNames.TOLERATED_FAILURE_PERCENTAGE)]
            private int? _toleratedFailurePercentage;
            [JsonProperty(PropertyNames.TOLERATED_FAILURE_COUNT)]
            private int? _toleratedFailureCount;

            internal Builder()
            {
            }

            /// <summary>
            ///     REQUIRED. Set the iterator for this MapState.
            /// </summary>
            /// <param name="iteratorBuilder">
            ///     Instance of <see cref="SubStateMachine" />. Note that the <see cref="SubStateMachine" /> object is
            ///     not built until the {@link ParallelState} is built so any modifications on the state model will be reflected in
            ///     this object.
            /// </param>
            /// <returns>This object for method chaining.</returns>
            public Builder Iterator(SubStateMachine.Builder iteratorBuilder)
            {
                _iterator = iteratorBuilder;
                return this;
            }

            /// <summary>
            ///     Reference path identifying where in the effective input the array field is found.
            /// </summary>
            /// <param name="itemPath"></param>
            /// <returns></returns>
            public Builder ItemPath(string itemPath)
            {
                _itemsPath = itemPath;
                return this;
            }

            /// <summary>
            ///     Provides an upper bound on how many invocations of the Iterator may run in parallel.
            /// </summary>
            /// <param name="maxConcurrency"></param>
            /// <returns></returns>
            public Builder MaxConcurrency(int maxConcurrency)
            {
                _maxConcurrency = maxConcurrency;
                return this;
            }
            
            /// <summary>
            ///     Provides an upper bound on the percentage of items that may fail. 
            /// </summary>
            /// <param name="toleratedFailurePercentage"></param>
            /// <returns></returns>
            public Builder ToleratedFailurePercentage(int toleratedFailurePercentage)
            {
                _toleratedFailurePercentage = toleratedFailurePercentage;
                return this;
            }
            
            /// <summary>
            ///     Provides an upper bound on how many items may fail. 
            /// </summary>
            /// <param name="toleratedFailureCount"></param>
            /// <returns></returns>
            public Builder ToleratedFailureCount(int toleratedFailureCount)
            {
                _toleratedFailureCount = toleratedFailureCount;
                return this;
            }

            /**
             * @return An immutable {@link ParallelState} object.
             */
            public override MapState Build()
            {
                return new MapState
                {
                    Comment = _comment,
                    Iterator = _iterator.Build(),
                    ItemsPath = _itemsPath,
                    MaxConcurrency = _maxConcurrency,
                    ToleratedFailureCount = _toleratedFailureCount,
                    ToleratedFailurePercentage = _toleratedFailurePercentage,
                    InputPath = _inputPath,
                    ResultPath = _resultPath,
                    OutputPath = _outputPath,
                    Parameters = _parameters,
                    ResultSelector = _resultSelector,
                    Transition = _transition.Build(),
                    Retriers = BuildableUtils.Build(_retriers),
                    Catchers = BuildableUtils.Build(_catchers)
                };
            }
        }
    }
}