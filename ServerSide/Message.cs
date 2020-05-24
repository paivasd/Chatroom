using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    class Message
    {
        public string MessageBody { get; set; }
        public Type MessageType {get; set;} 

        public enum Type
        {
            Text,
            Room,
            Server
        }


    }
}
