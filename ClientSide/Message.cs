using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
{
    class Message
    {
        public string MessageBody { get; set; }

        public Type type { get; set; }

        public enum Type
        {
            Text,
            Room
        }
    }
}
