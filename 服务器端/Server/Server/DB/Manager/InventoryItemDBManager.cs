using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DB.Manager {
    public class InventoryItemDBManager {
        public List<InventoryItemDB> GetInventoryItemDB( Role role )
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var res = session.QueryOver<InventoryItemDB>().Where(x => x.Role == role);
                    transaction.Commit();
                    return (List<InventoryItemDB>) res.List<InventoryItemDB>();
                }
            }
        }
        public void AddInventoryItemDB( InventoryItemDB itemDB ) {
            using (var session = NHibernateHelper.OpenSession())
            {
                using ( var transaction  = session.BeginTransaction())
                {
                    session.Save(itemDB);
                    transaction.Commit();
                }
            }

        }

        public void UpdateInventoryItemDB(InventoryItemDB itemDB)
        {
            using (var session = NHibernateHelper.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    session.Update(itemDB);
                    transaction.Commit();
                }
            }

        }

        public void UpdateInventoryItemDBList(List<InventoryItemDB> list)
        {
            using (var session = NHibernateHelper.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    foreach (var itemDB in list)
                    {
                        session.Update(itemDB);
                    }
                    transaction.Commit();
                }
            }
        }

        public void UpgradeEquip(InventoryItemDB itemDb4, Role role) {
            using (var session = NHibernateHelper.OpenSession()) {
                using (var transaction = session.BeginTransaction())
                {
                    session.Update(itemDb4);
                    session.Update(role);
                    transaction.Commit();
                }
            }
        }
    }

}
