namespace Solis.Gossip.Model.Messages
{
    public class GossipDataMessage : BaseMessage
    {
        public string PeerId
        {
            get;
            set;
        }
        
        public string Key
        {
            get;
            set;
        }

        public object Payload
        {
            get;
            set;
        }

        public long Timestamp
        {
            get;
            set;
        }


        public long ExpireAt
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"GossipDataMessage [nodeId={PeerId}, key={Key}, payload={Payload}, timestamp={Timestamp}, expireAt={ExpireAt}]";
        }        
    }
}