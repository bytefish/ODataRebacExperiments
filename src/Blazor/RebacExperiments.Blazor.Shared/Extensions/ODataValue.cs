using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RebacExperiments.Blazor.Shared.Extensions
{
    public class ODataValue
    {
        [JsonPropertyName("typeAnnotation")]
        public required ODataTypeAnnotation TypeAnnotation;


    }
}
