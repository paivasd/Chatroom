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
        public string Password { get; set;}
        public Guid GlobalIdentifier { get; set; }

        public Guid CurrentChat { get; set; }

        public bool Registered { get; set; }

        public override string ToString()
        {
            return Username;
        }
        public enum Type
        {
            Student,
            Teacher
        }
    }
}
