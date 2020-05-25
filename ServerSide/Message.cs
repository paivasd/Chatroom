using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    class Message
    {
        public Message()
        {

        }

        public Message(string messageBody, Type messageType, Guid chatGuid, Guid userGuid)
        {
            this.MessageBody = messageBody;
            this.MessageType = messageType;
            this.chatGuid = chatGuid;
            this.userGuid = userGuid;
        }
        public string MessageBody { get; set; }
        public Type MessageType {get; set;}
        public Guid chatGuid { get; set; }
        public Guid userGuid { get; set; }
        public string UserName { get; set; }



        public enum Type
        {
            Text,
            Room,
            Server,
            Guid,
            Join
        }


    }
}
