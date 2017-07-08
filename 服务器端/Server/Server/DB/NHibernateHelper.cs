using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace TaidouServer.DB {
    public class NHibernateHelper {
        private static ISessionFactory _sessionFactory = null;//单例模式

        private static void InitializeSessionFactory() {
            _sessionFactory =
                Fluently.Configure().Database(MySQLConfiguration.Standard.ConnectionString(db => db.Server("localhost").Database("taidouserver").Username("root").Password("root")))
                    .Mappings(x => x.FluentMappings.AddFromAssemblyOf<NHibernateHelper>()).BuildSessionFactory();
        }

        private static ISessionFactory SessionFactory {
            get {
                if (_sessionFactory == null)
                    InitializeSessionFactory();
                return _sessionFactory;
            }
        }

        public static ISession OpenSession() {
            return SessionFactory.OpenSession();
        }
    }
}
