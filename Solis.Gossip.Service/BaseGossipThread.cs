using Newtonsoft.Json;
using Solis.Gossip.Model.Messages;

namespace Solis.Gossip.Service
{
    public class BaseGossipThread
    {
        public static BaseMessage Deserialize(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            return JsonConvert.DeserializeObject<BaseMessage>(json, settings);
        }

        public static string Serialize(object message)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            return JsonConvert.SerializeObject(message, settings);
        }
    }
}
