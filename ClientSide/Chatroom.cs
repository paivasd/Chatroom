using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
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

        public Type type { get; set; }

        public List<Guid> usersGuid = new List<Guid>();

        public enum Type
        {
            Text,
            Room
        }

    }
}
