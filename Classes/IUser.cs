using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCGClassLib
{
    interface IUser
    {
        public string Username { get; }
        public string Password { get; }
        public Guid guid { get; }
        public string Token { get; }
    }
}
