using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using TaidouCommon.Model;

namespace TaidouServer.DB.Mapping {
    class UserMap :ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.ID).Column("id");
            Map(x => x.Username).Column("username");
            Map(x => x.Password).Column("password");
            Table("user");
        }
    }
}
