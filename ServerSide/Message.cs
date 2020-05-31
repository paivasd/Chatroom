using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    class Message
    {
        /// <summary>
        /// Default class constructor
        /// </summary>
        public Message()
        {

        }

        /// <summary>
        /// Class constructor with specific parameters.
        /// </summary>
        /// <param name="messageBody">The chat message itself.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="chatGuid">The Unique Identifier of the chatroom.</param>
        /// <param name="userGuid">The Unique Identifier of the user that owns the message.</param>
        public Message(string messageBody, Type messageType, Guid chatGuid, Guid userGuid)
        {
            this.MessageBody = messageBody;
            this.MessageType = messageType;
            this.chatGuid = chatGuid;
            this.userGuid = userGuid;
        }

        /// <summary>
        /// Class properties
        /// </summary>
        public string MessageBody { get; set; }
        public Type MessageType { get; set; }
        public Guid chatGuid { get; set; }
        public Guid userGuid { get; set; }
        public string UserName { get; set; }

        /// <summary>
        /// Enumerator to identify the type of message.
        /// </summary>
        public enum Type
        {
            Text,
            Room,
            Server,
            Guid,
            Join,
            Register,
            SuccessfulLogin
        }

    }
}
