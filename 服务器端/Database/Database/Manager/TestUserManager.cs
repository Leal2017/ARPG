using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouDatabase.Model;

namespace TaidouDatabase.Manager {
    class TestUserManager {
        public IList<TestUser> GetAllUser() {
            using (var session = NHibernateHelper.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    var userList = session.QueryOver<TestUser>();
                    transaction.Commit();
                    return userList.List();
                }
            }
        }

        public IList<TestUser> GetUserByUsername(string username) {
            using (var session = NHibernateHelper.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    var userList = session.QueryOver<TestUser>().Where(user => user.Username == username);
                    transaction.Commit();
                    return userList.List();
                }
            }
        }

        public void SaveUser( TestUser user  ) {
            using (var session = NHibernateHelper.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    session.Save(user);
                    transaction.Commit();
                }
            }
        }

        public void DeleteById(int id) {
            using (var session = NHibernateHelper.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    TestUser tu = new TestUser();
                    tu.Id = id;
                    session.Delete(tu);
                    transaction.Commit();
                }
            }
        }

        public void UpdateUser(TestUser tu) {
            using (var session = NHibernateHelper.OpenSession()) {
                using (var transaction = session.BeginTransaction()) {
                    session.Update(tu);
                    transaction.Commit();
                }
            }
        }

        static void Main(string[] args) {
            TestUserManager testuserManager = new TestUserManager();
            IList<TestUser> testuserList = testuserManager.GetAllUser();
            foreach (TestUser tu in testuserList) {
                Console.WriteLine(tu.Username);
            }
            Console.WriteLine("---------------");
            IList<TestUser> testuserList2 = testuserManager.GetUserByUsername("siki");
            foreach (TestUser tu in testuserList2) {
                Console.WriteLine(tu.Username);
            }
            Console.WriteLine("--------------- save");
            TestUser tu2 = new TestUser();
            tu2.Username = "taikr"; tu2.Password = "泰课在线"; tu2.Age = 2;
            //testuserManager.SaveUser(tu2);
            Console.WriteLine("--------------- delete");
            //testuserManager.DeleteById(2);
            Console.WriteLine("--------------- update");
            TestUser tu3 = testuserList2[0];
            tu3.Password = "1234567890";
            testuserManager.UpdateUser(tu3);

            Console.ReadKey();
        }
    }
}
