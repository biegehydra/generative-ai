﻿using System.Collections.Generic;

namespace Mscc.GenerativeAI
{
    /// <summary>
    /// The Schema object allows the definition of input and output data types. These types can be objects, but also primitives and arrays. Represents a select subset of an OpenAPI 3.0 schema object.
    /// </summary>
    public class Schema
    {
        /// <summary>
        /// Required. Data type.
        /// </summary>
        public ParameterType? Type { get; set; }
        /// <summary>
        /// Optional. The format of the data.
        /// Supported formats:
        ///  for NUMBER type: float, double
        ///  for INTEGER type: int32, int64
        /// </summary>
        public string Format { get; set; } = "";
        /// <summary>
        /// Optional. A brief description of the parameter. This could contain examples of use. Parameter description may be formatted as Markdown.
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// Optional. Indicates if the value may be null.
        /// </summary>
        public bool? Nullable { get; set; }
        /// <summary>
        /// Optional. Schema of the elements of Type.ARRAY.
        /// </summary>
        public Schema? Items { get; set; }
        /// <summary>
        /// Optional. Possible values of the element of Type.STRING with enum format.
        /// For example we can define an Enum Direction as :
        /// {type:STRING, format:enum, enum:["EAST", NORTH", "SOUTH", "WEST"]}
        /// </summary>
        public List<string>? Enum { get; set; }
        /// <summary>
        /// Optional. Properties of Type.OBJECT.
        /// An object containing a list of "key": value pairs. Example: { "name": "wrench", "mass": "1.3kg", "count": "3" }.
        /// </summary>
        public dynamic? Properties { get; set; }
        /// <summary>
        /// Optional. Required properties of Type.OBJECT.
        /// </summary>
        public List<string>? Required { get; set; }
    }
}
