using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Internal;

namespace StatesLanguage.States;

public class ItemBatcher
{
    private ItemBatcher()
    {
    }

    [JsonProperty(PropertyNames.BATCH_INPUT)]
    public JObject BatchInput { get; private set; }

    [JsonProperty(PropertyNames.MAX_ITEMS_PER_BATCH)]
    public int? MaxItemsPerBatch { get; private set; }
    
    [JsonProperty(PropertyNames.MAX_INPUT_BYTES_PER_BATCH)]
    public int? MaxInputBytesPerBatch { get; private set; }
    
    [JsonProperty(PropertyNames.MAX_ITEMS_PER_BATCH_PATH)]
    public string MaxItemsPerBatchPath { get; private set; }

    [JsonProperty(PropertyNames.MAX_INPUT_BYTES_PER_BATCH_PATH)]
    public string MaxInputBytesPerBatchPath { get; private set; }

    /// <returns>Builder instance to construct a <see cref="ItemBatcher" />.</returns>
    public static Builder GetBuilder()
    {
        return new Builder();
    }

    public sealed class Builder : IBuildable<ItemBatcher>
    {
        [JsonProperty(PropertyNames.BATCH_INPUT)] 
        private JObject _batchInput;

        [JsonProperty(PropertyNames.MAX_ITEMS_PER_BATCH)]
        private int? _maxItemsPerBatch;

        [JsonProperty(PropertyNames.MAX_INPUT_BYTES_PER_BATCH)]
        private int? _maxInputBytesPerBatch;

        [JsonProperty(PropertyNames.MAX_ITEMS_PER_BATCH_PATH)]
        private string _maxItemsPerBatchPath;

        [JsonProperty(PropertyNames.MAX_INPUT_BYTES_PER_BATCH_PATH)]
        private string _maxInputBytesPerBatchPath;
        
        internal Builder()
        {
        }

        /// <summary>
        ///     An immutable <see cref="ItemBatcher" /> object.
        /// </summary>
        /// <returns></returns>
        public ItemBatcher Build()
        {
            if(!_maxItemsPerBatch.HasValue &&
               !_maxInputBytesPerBatch.HasValue && 
               string.IsNullOrWhiteSpace(_maxInputBytesPerBatchPath) && 
               string.IsNullOrWhiteSpace(_maxItemsPerBatchPath))
                throw new StatesLanguageException("You must specify at least one MaxItemsPerBatch, MaxInputBytesPerBatch, MaxItemsPerBatchPath or MaxInputBytesPerBatchPath");
            
            if (_maxItemsPerBatch.HasValue && !string.IsNullOrWhiteSpace(_maxItemsPerBatchPath))
                throw new StatesLanguageException("You cannot specify MaxItemsPerBatch and MaxItemsPerBatchPath at the same time");
            
            if (_maxInputBytesPerBatch.HasValue && !string.IsNullOrWhiteSpace(_maxInputBytesPerBatchPath))
                throw new StatesLanguageException("You cannot specify MaxInputBytesPerBatch and MaxInputBytesPerBatchPath at the same time");
            
            if(_maxItemsPerBatch is <= 0)
                throw new StatesLanguageException("MaxItemsPerBatch must be > 0");

            if(_maxInputBytesPerBatch is <= 0)
                throw new StatesLanguageException("MaxInputBytesPerBatch must be > 0");

            return new ItemBatcher
            {
                BatchInput = _batchInput,
                MaxItemsPerBatch = _maxItemsPerBatch,
                MaxItemsPerBatchPath = _maxItemsPerBatchPath,
                MaxInputBytesPerBatch = _maxInputBytesPerBatch,
                MaxInputBytesPerBatchPath = _maxInputBytesPerBatchPath
            };
        }

        /// <summary>
        /// Specifies the maximum number of items that each child workflow execution processes.
        /// The interpreter limits the number of items batched in the Items array to this value.
        /// If you specify both a batch number and size, the interpreter reduces the number of
        /// items in a batch to avoid exceeding the specified batch size limit. 
        /// </summary>
        /// <param name="maxItemsPerBatch"></param>
        /// <returns>This object for method chaining.</returns>
        public Builder MaxItemsPerBatch(int maxItemsPerBatch)
        {
            _maxItemsPerBatch = maxItemsPerBatch;
            return this;
        }
        
        /// <summary>
        /// Specifies the maximum size of a batch in bytes, up to 256 KBs. If you specify both a
        /// maximum batch number and size, Step Functions reduces the number of items
        /// in a batch to avoid exceeding the specified batch size limit.
        /// </summary>
        /// <param name="maxInputBytesPerBatch"></param>
        /// <returns>This object for method chaining.</returns>
        public Builder MaxInputBytesPerBatch(int maxInputBytesPerBatch)
        {
            _maxInputBytesPerBatch = maxInputBytesPerBatch;
            return this;
        }
        
        /// <summary>
        /// JSON path to retrieve the <see cref="MaxItemsPerBatch"/>.
        /// </summary>
        /// <param name="maxItemsPerBatchPath"></param>
        /// <returns>This object for method chaining.</returns>
        public Builder MaxItemsPerBatchPath(string maxItemsPerBatchPath)
        {
            _maxItemsPerBatchPath = maxItemsPerBatchPath;
            return this;
        }
        
        /// <summary>
        /// JSON path to retrieve the <see cref="MaxItemsPerBatchPath"/>.
        /// </summary>
        /// <param name="maxInputBytesPerBatchPath"></param>
        /// <returns>This object for method chaining.</returns>
        public Builder MaxInputBytesPerBatchPath(string maxInputBytesPerBatchPath)
        {
            _maxInputBytesPerBatchPath = maxInputBytesPerBatchPath;
            return this;
        }
        
        /// <summary>
        /// JSON input to include in each batch passed to each child workflow execution.
        /// Step Functions merges this input with the input for each individual child workflow executions.
        /// </summary>
        /// <param name="batchInput"></param>
        /// <returns>This object for method chaining.</returns>
        public Builder BatchInput(JObject batchInput)
        {
            _batchInput = batchInput;
            return this;
        }

    }
}