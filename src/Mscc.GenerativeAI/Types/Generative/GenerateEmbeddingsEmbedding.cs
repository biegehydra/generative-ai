#if NET472_OR_GREATER || NETSTANDARD2_0
using System.Collections.Generic;
#endif

namespace Mscc.GenerativeAI
{
    /// <summary>
    /// An embedding vector generated by the model.
    /// </summary>
    public class GenerateEmbeddingsEmbedding
    {
        /// <summary>
        /// Output only. The embedding vector generated for the input.
        /// Can be either a list of floats or a base64 string encoding the a list of floats with C-style layout (Numpy compatible).
        /// </summary>
        public List<float> Embedding { get; set; }
        /// <summary>
        /// Output only. Index of the embedding in the list of embeddings.
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Output only. Always \"embedding\", required by the SDK.
        /// </summary>
        public string Object { get; set; } = "embedding";
    }
}