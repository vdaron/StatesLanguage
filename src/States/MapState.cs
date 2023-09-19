using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Internal;

namespace StatesLanguage.States
{
    public class MapState : RetryCatchState
    {
        public override StateType Type => StateType.Map;

        [JsonProperty(PropertyNames.MAX_CONCURENCY)]
        public int? MaxConcurrency { get; private set; }
        
        [JsonProperty(PropertyNames.MAX_CONCURENCY_PATH)]
        public string MaxConcurrencyPath { get; private set; }

        [JsonProperty(PropertyNames.ITEMS_PATH)]
        public string ItemsPath { get; private set; }
        
        [JsonProperty(PropertyNames.TOLERATED_FAILURE_PERCENTAGE)]
        public int? ToleratedFailurePercentage { get; private set; }
        
        [JsonProperty(PropertyNames.TOLERATED_FAILURE_COUNT)]
        public int? ToleratedFailureCount { get; private set; }
        
        [JsonProperty(PropertyNames.TOLERATED_FAILURE_PERCENTAGE_PATH)]
        public string ToleratedFailurePercentagePath { get; private set; }
        
        [JsonProperty(PropertyNames.TOLERATED_FAILURE_COUNT_PATH)]
        public string ToleratedFailureCountPath { get; private set; }

        [JsonProperty(PropertyNames.ITEM_SELECTOR)]
        public JObject ItemSelector { get; protected set; }
        
        [JsonProperty(PropertyNames.ITEM_READER)]
        public JObject ItemReader { get; protected set; }
        
        [JsonProperty(PropertyNames.RESULT_WRITER)]
        public JObject ResultWriter { get; protected set; }
        
        [JsonIgnore]
        [Obsolete("Parameter is now deprecated on Map, use ItemSelector instead")]
        public override JObject Parameters => ItemSelector;
        
        [JsonIgnore]
        [Obsolete("Replaced by ItemProcessor")]
        public SubStateMachine Iterator => ItemProcessor;
        
        [JsonIgnore]
        public SubStateMachine ItemProcessor { get; private set; }
        
        [JsonIgnore]
        public ItemBatcher ItemBatcher { get; private set; }

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
            private SubStateMachine.Builder _iterator;

            [JsonProperty(PropertyNames.ITEM_PROCESSOR)]
            private SubStateMachine.Builder _itemProcessor;
            
            [JsonProperty(PropertyNames.ITEM_BATCHER)]
            private ItemBatcher.Builder _itemBatcher;
            
            [JsonProperty(PropertyNames.ITEM_SELECTOR)]
            private JObject _itemSelector;
            
            [JsonProperty(PropertyNames.ITEM_READER)]
            private JObject _itemReader;
            
            [JsonProperty(PropertyNames.RESULT_WRITER)]
            private JObject _resultWriter;
            
            [JsonProperty(PropertyNames.MAX_CONCURENCY)]
            private int? _maxConcurrency;
            
            [JsonProperty(PropertyNames.MAX_CONCURENCY_PATH)]
            private string _maxConcurrencyPath;
            
            [JsonProperty(PropertyNames.TOLERATED_FAILURE_PERCENTAGE)]
            private int? _toleratedFailurePercentage;
            
            [JsonProperty(PropertyNames.TOLERATED_FAILURE_COUNT)]
            private int? _toleratedFailureCount;
            
            [JsonProperty(PropertyNames.TOLERATED_FAILURE_PERCENTAGE_PATH)]
            private string _toleratedFailurePercentagePath;
            
            [JsonProperty(PropertyNames.TOLERATED_FAILURE_COUNT_PATH)]
            private string _toleratedFailureCountPath;

            internal Builder()
            {
                _iterator = _itemProcessor = new SubStateMachine.Builder();
            }


            [Obsolete("Use ItemProcessor instead")]
            public Builder Iterator(SubStateMachine.Builder iteratorBuilder)
            {
                _itemProcessor = iteratorBuilder;
                return this;
            }
            
            public Builder ItemBatcher(ItemBatcher.Builder itemBatcherBuilder)
            {
                _itemBatcher = itemBatcherBuilder;
                return this;
            }

            /// <summary>
            ///     REQUIRED. Set the ItemProcessor for this MapState.
            /// </summary>
            /// <param name="itemProcessorBuilder">
            ///     Instance of <see cref="SubStateMachine" />. Note that the <see cref="SubStateMachine" /> object is
            ///     not built until the <see cref="MapState"/> is built so any modifications on the state model will be reflected in
            ///     this object.
            /// </param>
            /// <returns>This object for method chaining.</returns>
            public Builder ItemProcessor(SubStateMachine.Builder itemProcessorBuilder)
            {
                _itemProcessor = itemProcessorBuilder;
                return this;
            }

            [Obsolete("Use ItemSelector instead on Map States")]
            public override Builder Parameters(JObject parameters)
            {
                _itemSelector = parameters;
                return this;
            }
            
            /// <summary>
            /// Overrides the values of the input array items before they're passed on to each Map state iteration.
            /// </summary>
            /// <param name="itemSelector"></param>
            /// <returns></returns>
            public Builder ItemSelector(JObject itemSelector)
            {
                _itemSelector = itemSelector;
                return this;
            }
            
            
            /// <summary>
            ///  JSON object, which specifies a dataset and its location.
            /// </summary>
            /// <param name="itemReader"></param>
            /// <returns></returns>
            public Builder ItemReader(JObject itemReader)
            {
                _itemReader = itemReader;
                return this;
            }
            
            /// <summary>
            /// JSON object that specifies the Amazon S3 location where Step Functions writes the results of the child workflow executions started by a Distributed Map state.
            /// </summary>
            /// <param name="resultWriter"></param>
            /// <returns></returns>
            public Builder ResultWriter(JObject resultWriter)
            {
                _resultWriter = resultWriter;
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
            ///     Json path to retrieve <see cref="MaxConcurrency"/> 
            /// </summary>
            /// <param name="maxConcurrencyPath"></param>
            /// <returns></returns>
            public Builder MaxConcurrencyPath(string maxConcurrencyPath)
            {
                _maxConcurrencyPath = maxConcurrencyPath;
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
            
            /// <summary>
            /// Json Path to retrieve the <see cref="ToleratedFailurePercentage"/>. 
            /// </summary>
            /// <param name="toleratedFailurePercentagePath"></param>
            /// <returns></returns>
            public Builder ToleratedFailurePercentagePath(string toleratedFailurePercentagePath)
            {
                _toleratedFailurePercentagePath = toleratedFailurePercentagePath;
                return this;
            }
            
            /// <summary>
            /// Json Path to retrieve the <see cref="ToleratedFailureCount"/>. 
            /// </summary>
            /// <param name="toleratedFailureCountPath"></param>
            /// <returns></returns>
            public Builder ToleratedFailureCountPath(string toleratedFailureCountPath)
            {
                _toleratedFailureCountPath = toleratedFailureCountPath;
                return this;
            }

            /**
             * @return An immutable {@link ParallelState} object.
             */
            public override MapState Build()
            {
                if (_maxConcurrency.HasValue && !string.IsNullOrWhiteSpace(_maxConcurrencyPath))
                    throw new StatesLanguageException("You cannot specify MaxConcurrency and MaxConcurrencyPath at the same time");
                
                if (_toleratedFailureCount.HasValue && !string.IsNullOrWhiteSpace(_toleratedFailureCountPath))
                    throw new StatesLanguageException("You cannot specify ToleratedFailureCount and ToleratedFailureCountPath at the same time");
                
                if (_toleratedFailurePercentage.HasValue && !string.IsNullOrWhiteSpace(_toleratedFailurePercentagePath))
                    throw new StatesLanguageException("You cannot specify ToleratedFailurePercentage and ToleratedFailurePercentagePath at the same time");

                if(_itemProcessor == null)
                    throw new StatesLanguageException("ItemProcessor is mandatory for MapStates");
                
                return new MapState
                {
                    Comment = _comment,
                    ItemProcessor = _itemProcessor.Build(),
                    ItemBatcher = _itemBatcher?.Build(),
                    ItemsPath = _itemsPath,
                    MaxConcurrency = _maxConcurrency,
                    MaxConcurrencyPath = _maxConcurrencyPath,
                    ToleratedFailureCount = _toleratedFailureCount,
                    ToleratedFailureCountPath = _toleratedFailureCountPath,
                    ToleratedFailurePercentage = _toleratedFailurePercentage,
                    ToleratedFailurePercentagePath = _toleratedFailurePercentagePath,
                    ItemReader = _itemReader,
                    ResultWriter = _resultWriter,
                    InputPath = _inputPath,
                    ResultPath = _resultPath,
                    OutputPath = _outputPath,
                    ItemSelector = _itemSelector ?? _parameters, // Take parameter if they where set and convert it into ItemSelector
                    ResultSelector = _resultSelector,
                    Transition = _transition.Build(),
                    Retriers = BuildableUtils.Build(_retriers),
                    Catchers = BuildableUtils.Build(_catchers)
                };
            }
        }
    }
}