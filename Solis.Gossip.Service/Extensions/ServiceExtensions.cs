using Newtonsoft.Json;
using Solis.Gossip.Model.Messages;
using System.Text;

namespace Solis.Gossip.Service
{
    internal static class ServiceExtensions
    {
        internal static BaseMessage Deserialize(this byte[] packet)
        {
            if(packet == null)
            {
                return null;
            }

            var json = Encoding.UTF8.GetString(packet, 0, packet.Length);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            return JsonConvert.DeserializeObject<BaseMessage>(json, settings);
        }

        internal static byte[] Serialize(this BaseMessage message)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            var json = JsonConvert.SerializeObject(message, settings);

            return Encoding.UTF8.GetBytes(json);
        }
    }
}
