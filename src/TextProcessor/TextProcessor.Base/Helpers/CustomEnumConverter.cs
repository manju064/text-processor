using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using TextProcessor.Base.Extensions;

namespace TextProcessor.Base.Helpers
{
    public class CustomEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerCamelCase());
        }
    }
}
