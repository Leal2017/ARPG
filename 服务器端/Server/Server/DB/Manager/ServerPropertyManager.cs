using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DB.Manager {
    class ServerPropertyManager {
        //得到服务器的列表
        public List<ServerProperty> GetServerList()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var list = session.QueryOver<ServerProperty>();
                    transaction.Commit();
                    return (List<ServerProperty>) list.List<ServerProperty>();
                }
            }
        }
    }
}
