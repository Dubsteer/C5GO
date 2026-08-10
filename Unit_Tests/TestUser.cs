using LogicLayer.Exceptions;
using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests
{
    [TestClass]
    public class TestUser
    {
        private static List<User> users = null!;
        private static UserManager userManager = null!;

        [ClassInitialize]
        public static void TestClassSetup(TestContext _)
        {
            users = new List<User>();
            userManager = new UserManager(new MockUserRepo(users));
        }

        [TestInitialize]
        public void Setup() => users.Clear();

        [TestMethod]
        public void TestCreateUser()
        {
            userManager.CreateUser(CreateUser(1, "dubsteer", "dubsteer@test.local"));

            Assert.AreEqual(1, users.Count);
            Assert.IsTrue(users[0].Password.StartsWith("$2"));
        }

        [TestMethod]
        public void TestCreateUserWithUsernameAlreadyInUse()
        {
            userManager.CreateUser(CreateUser(1, "dubsteer", "first@test.local"));

            Assert.ThrowsExactly<UsernameAlreadyInUseException>(() =>
                userManager.CreateUser(CreateUser(2, "dubsteer", "second@test.local")));
        }

        [TestMethod]
        public void TestUpdateUser()
        {
            var user = CreateUser(1, "dubsteer", "dubsteer@test.local");
            userManager.CreateUser(user);
            user.Firstname = "Updated";

            userManager.UpdateUser(user);

            Assert.AreEqual("Updated", users[0].Firstname);
        }

        [TestMethod]
        public void TestDeleteUser()
        {
            var user = CreateUser(1, "dubsteer", "dubsteer@test.local");
            userManager.CreateUser(user);

            userManager.DeleteUser(user);

            Assert.AreEqual(0, users.Count);
        }

        [TestMethod]
        public void TestGetAllUsers()
        {
            userManager.CreateUser(CreateUser(1, "dubsteer", "dubsteer@test.local"));

            Assert.AreEqual(1, userManager.GetAllUsers().Count);
        }

        [TestMethod]
        public void TestGetLoginUser()
        {
            const string password = "password123";
            var user = CreateUser(1, "dubsteer", "dubsteer@test.local", password);
            userManager.CreateUser(user);
            user.EmailConfirmed = true;

            var result = userManager.GetLoginUser(user.Username, password);

            Assert.AreSame(user, result);
        }

        [TestMethod]
        public void TestGetLoginUserWithWrongCredentials()
        {
            var user = CreateUser(1, "dubsteer", "dubsteer@test.local");
            userManager.CreateUser(user);
            user.EmailConfirmed = true;

            var result = userManager.GetLoginUser(user.Username, "wrong-password");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestSearchUser()
        {
            userManager.CreateUser(CreateUser(1, "dubsteer", "one@test.local"));
            userManager.CreateUser(CreateUser(2, "dubsteer1", "two@test.local"));
            userManager.CreateUser(CreateUser(3, "dubsteer2", "three@test.local"));

            var broadResults = userManager.SearchUser("dubsteer");
            var exactResults = userManager.SearchUser("dubsteer2");

            Assert.AreEqual(3, broadResults.Count);
            Assert.AreEqual(1, exactResults.Count);
            Assert.AreEqual("dubsteer2", exactResults[0].Username);
        }

        private static User CreateUser(int id, string username, string email, string password = "password") =>
            new User(id, "Test", "User", 22, username, email, password, false);
    }
}
