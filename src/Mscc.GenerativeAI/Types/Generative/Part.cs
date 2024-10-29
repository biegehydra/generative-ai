﻿using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Mscc.GenerativeAI
{
    /// <summary>
    /// A datatype containing media that is part of a multi-part Content message.
    /// A part of a turn in a conversation with the model with a fixed MIME type. 
    /// It has one of the following mutually exclusive fields: 
    ///     1. text 
    ///     2. inline_data 
    ///     3. file_data 
    ///     4. functionResponse 
    ///     5. functionCall
    /// </summary>
    [DebuggerDisplay("{Text}")]
    public class Part
    {
        public Part() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public Part(string text)
        {
            Text = text;
        }

        public Part(FileData fileData) : this()
        {
            FileData = fileData;
        }

        /// <summary>
        /// A text part of a conversation with the model.
        /// </summary>
        public string Text
        {
            get { return TextData?.Text; }
            set { TextData = new TextData { Text = value }; }
        }
        /// <remarks/>
        [DebuggerHidden]
        internal TextData TextData { get; set; }
        /// <summary>
        /// Raw media bytes sent directly in the request. 
        /// </summary>
        // [JsonPropertyName("inline_data")]
        public InlineData InlineData { get; set; }
        /// <summary>
        /// URI based data.
        /// </summary>
        // [JsonPropertyName("file_data")]
        public FileData FileData { get; set; }
        /// <summary>
        /// The result output of a FunctionCall that contains a string representing the FunctionDeclaration.name and a structured JSON object containing any output from the function is used as context to the model.
        /// </summary>
        public FunctionResponse FunctionResponse { get; set; }
        /// <summary>
        /// A predicted FunctionCall returned from the model that contains a string representing the FunctionDeclaration.name with the arguments and their values.
        /// </summary>
        public FunctionCall FunctionCall { get; set; }
        /// <summary>
        /// Optional. For video input, the start and end offset of the video in Duration format.
        /// </summary>
        public VideoMetadata VideoMetadata { get; set; }

        public FileData FromUri(string uri, string mimetype)
        {
            return new FileData { FileUri = uri, MimeType = mimetype };
        }
    }
}