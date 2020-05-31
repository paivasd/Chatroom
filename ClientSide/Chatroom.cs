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
        /// <summary>
        /// Class default constructor
        /// </summary>
        public Chatroom()
        {

        }

        /// <summary>
        /// Class constructor with specific parameters.
        /// </summary>
        /// <param name="chatName">Chatroom name.</param>
        public Chatroom(string chatName)
        {
            this.ChatName = chatName;
            this.Identifier = Guid.NewGuid();
        }

        /// <summary>
        /// Class properties
        /// </summary>
        public string ChatName { get; set; }
        public Guid Identifier { get; set; }
        public List<Guid> usersGuid = new List<Guid>();

        /// <summary>
        /// Each Chatroom object has a Dictionary of KeyValuePair Guid,User to store users "connected" to the chatroom.
        /// </summary>
        public Dictionary<Guid, User> usersDictionary = new Dictionary<Guid, User>();

    }
}
