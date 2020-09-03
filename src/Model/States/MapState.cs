using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    public class MapState: RetryCatchState
    {
        public override StateType Type => StateType.Map;
        
        [JsonProperty(PropertyNames.MAX_CONCURENCY)]
        public int? MaxConcurrency { get; private set; }
        
        [JsonProperty(PropertyNames.ITEMS_PATH)]
        public string ItemsPath { get; private set; }
        
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
            [JsonProperty(PropertyNames.ITERATOR)] 
            private SubStateMachine.Builder _iterator = new SubStateMachine.Builder();

            [JsonProperty(PropertyNames.MAX_CONCURENCY)]
            private int? _maxConcurrency;

            [JsonProperty(PropertyNames.ITEMS_PATH)]
            private string _itemsPath;

            internal Builder()
            {
            }
            
            /// <summary>
            /// REQUIRED. Set the iterator for this MapState.
            /// </summary>
            /// <param name="iteratorBuilder">Instance of <see cref="SubStateMachine"/>. Note that the <see cref="SubStateMachine"/> object is
            /// not built until the {@link ParallelState} is built so any modifications on the state model will be reflected in this object.
            /// </param>
            /// <returns>This object for method chaining.</returns>
            public Builder Iterator(SubStateMachine.Builder iteratorBuilder)
            {
                _iterator = iteratorBuilder;
                return this;
            }

            /// <summary>
            /// Reference path identifying where in the effective input the array field is found.
            /// </summary>
            /// <param name="itemPath"></param>
            /// <returns></returns>
            public Builder ItemPath(ReferencePath itemPath)
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
                           ResultSelector = _resultSelector,
                           Transition = _transition.Build(),
                           Retriers = BuildableUtils.Build(_retriers),
                           Catchers = BuildableUtils.Build(_catchers)
                       };
            }
        }
    }
}