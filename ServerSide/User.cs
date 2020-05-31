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
        /// <summary>
        /// Default constructor.
        /// </summary>
        public User()
        {

        }

        /// <summary>
        /// Properties
        /// </summary>
        public string Username { get; set; }

        public TcpClient UserTcp { get; set; }

        public string Password { get; set; }

        public Guid GlobalIdentifier { get; set; }

        public Guid CurrentChat { get; set; }

        public Guid ChatRoomIdentifier { get; set; }

        public Type UserType { get; set; }

        public Course CourseType { get; set; }

        public bool Registered { get; set; }


        public enum Type
        {
            Student,
            Teacher
        }
        public enum Course
        {
            LESI,
            EDJD,
            EEC,

        }
    }
}
