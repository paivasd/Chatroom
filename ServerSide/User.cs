using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    class User
    {
        public string Username { get; set; }
        public TcpClient UserTcp { get; set; }

        public Guid GlobalIdentifier { get; set; }

        public Chatroom CurrentChat { get; set; }

        public override string ToString()
        {
            return Username;
        }
    }
}
