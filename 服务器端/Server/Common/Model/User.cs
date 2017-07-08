using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon.Model {
    public class User {
        public virtual int ID { get; set; }
        public virtual  string Username { get; set; }
        public virtual  string Password { get; set; }
    }
}
