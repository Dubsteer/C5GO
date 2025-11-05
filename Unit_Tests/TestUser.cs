using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicLayer.Exceptions;
using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestUser
    {
        private static List<User> users;
        private static MockUserRepo mockUserRepo;
        private static UserManager UserManager;

        [ClassInitialize]
        public static void TestClassSetuo(TestContext context)
        {
            users = new List<User>();
            mockUserRepo = new MockUserRepo(users);
            UserManager = new UserManager(mockUserRepo);
        }

        [TestInitialize]
        public void Setup()
        {
            users.Clear();
        }

        [TestMethod]
        public void TestCreateUser()
        {
            //arrange
            var user = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);

            //act
            UserManager.CreateUser(user);
            //assert
            Assert.AreEqual(users.Count, 1);

        }

        [TestMethod]
        [ExpectedException(typeof(UsernameAlreadyInUseException))]
        public void TestCreateUserWithUsernameAlreadyInUse()
        {

            //arrange
            var user = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
            UserManager.CreateUser(user);

            //act
            UserManager.CreateUser(user);

            //assert

        }

        [TestMethod]
        public void TestUpdateUser()
        {
            //arrange
            var user = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
            UserManager.CreateUser(user);

            //act
            UserManager.UpdateUser(user);

            //assert
            Assert.AreEqual(users.Count, 1);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            //arrange
            var user = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
            UserManager.CreateUser(user);

            //act
            UserManager.DeleteUser(user);

            //assert
            Assert.AreEqual(users.Count, 0);
        }

        [TestMethod]
        public void TestAllUsers()
        {
            //arrange
            var user = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
            UserManager.CreateUser(user);

            //act
            UserManager.GetAllUsers();

            //assert
            Assert.AreEqual(users.Count, 1);
        }

        [TestMethod]
        public void TestGetLoginUser()
        {
            var user = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
            UserManager.CreateUser(user);

            //act
            UserManager.GetLoginUser(user.Username, user.Password);

            //assert
            Assert.AreEqual(users.Count, 1);
        }

        [TestMethod]
        public void TestGetLoginUserWithWrongCredentials()
        {
            var user = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
            UserManager.CreateUser(user);

            //act
            UserManager.GetLoginUser("dsadas", "sdasd");

            //assert
            Assert.IsNotNull(users);


        }

        [TestMethod]
        public void TestSearchUser()
        {
            // Arrange
            var user1 = new User(1, "Vladimir", "Stijepovic", 22, "dubsteer", "dovla98765@gmail.com", "123", false);
            var user2 = new User(2, "Vladimir", "Stijepovic", 22, "dubsteer2", "dovla98765@gmail.com", "123", false);
            var user3 = new User(3, "Vladimir", "Stijepovic", 22, "dubsteer1", "dovla98765@gmail.com", "123", false);
            UserManager.CreateUser(user1);
            UserManager.CreateUser(user2);
            UserManager.CreateUser(user3);

            // Act
            var searchResults1 = UserManager.SearchUser("dubsteer");   // Search for users with "dubsteer" in username or email
            var searchResults2 = UserManager.SearchUser("dubsteer2");  // Search for users with "dubsteer2" in username or email

            // Assert
            Assert.AreEqual(3, searchResults1.Count);
            Assert.IsTrue(searchResults1.Any(u => u.Username == "dubsteer" || u.Gmail == "dubsteer"));
            Assert.IsTrue(searchResults1.Any(u => u.Username == "dubsteer1" || u.Gmail == "dubsteer1"));
            Assert.IsTrue(searchResults1.Any(u => u.Username == "dubsteer2" || u.Gmail == "dubsteer2"));
            Assert.AreEqual(1, searchResults2.Count);
            Assert.IsTrue(searchResults2.Any(u => u.Username == "dubsteer2" || u.Gmail == "dubsteer2"));
        }
    }
}
