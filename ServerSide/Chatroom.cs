using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    class Chatroom
    {
        public Chatroom()
        {

        }

        public Chatroom(string chatName)
        {
            this.ChatName = chatName;
            this.Identifier = Guid.NewGuid();
            this.type = Type.Room;
        }
        public string ChatName { get; set; }
        public Guid Identifier { get; set; }

        public Dictionary<Guid, User> chatRoomUsers;

        public Type type { get; set; }

        public enum Type
        {
            Text,
            Room
        }

    }
}
