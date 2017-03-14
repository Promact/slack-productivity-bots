using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Promact.Erp.DomainModel.ApplicationClass.SlackRequestAndResponse;
using System;

namespace Promact.Erp.DomainModel.JSonConverterUtil
{

    public class SlackChannelDetailsConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
                       

        /// <summary>
        /// Reads the JSON representation of the object. - JJ
        /// </summary>
        /// <param name="reader">The Newtonsoft.Json.JsonReader to read from</param>
        /// <param name="objectType">Type of the object</param>
        /// <param name="existingValue">The existing value of object being read</param>
        /// <param name="serializer">The calling serializer</param>
        /// <returns>The object value</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Object)
            {
                return token.ToObject<SlackChannelDetails>();
            }
            else
            {
                SlackChannelDetails result = new SlackChannelDetails();
                result.ChannelId = (string)token;
                return result;
            }
        }


        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

