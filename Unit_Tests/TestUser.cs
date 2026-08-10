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
        public void TestAdminCanDeleteRegularUser()
        {
            var admin = CreateUser(1, "admin", "admin@test.local");
            admin.IsAdmin = true;
            userManager.CreateUser(admin);
            userManager.CreateUser(CreateUser(2, "member", "member@test.local"));

            userManager.DeleteUserAsAdmin(2, 1);

            Assert.IsNull(userManager.GetUserById(2));
        }

        [TestMethod]
        public void TestAdminCannotDeleteOwnAccount()
        {
            var admin = CreateUser(1, "admin", "admin@test.local");
            admin.IsAdmin = true;
            userManager.CreateUser(admin);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                userManager.DeleteUserAsAdmin(1, 1));
        }

        [TestMethod]
        public void TestAdminCannotDeleteAnotherAdmin()
        {
            var actingAdmin = CreateUser(1, "admin1", "admin1@test.local");
            actingAdmin.IsAdmin = true;
            userManager.CreateUser(actingAdmin);

            var admin = CreateUser(2, "admin2", "admin2@test.local");
            admin.IsAdmin = true;
            userManager.CreateUser(admin);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                userManager.DeleteUserAsAdmin(2, 1));
        }

        [TestMethod]
        public void TestRegularUserCannotDeleteAccountAsAdmin()
        {
            userManager.CreateUser(CreateUser(1, "member1", "member1@test.local"));
            userManager.CreateUser(CreateUser(2, "member2", "member2@test.local"));

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                userManager.DeleteUserAsAdmin(2, 1));
        }

        [TestMethod]
        public void TestPlayerProfileMustBeRemovedBeforeDeletingAccount()
        {
            var admin = CreateUser(1, "admin", "admin@test.local");
            admin.IsAdmin = true;
            userManager.CreateUser(admin);

            var player = CreateUser(2, "player", "player@test.local");
            player.SteamId = "steam123";
            userManager.CreateUser(player);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                userManager.DeleteUserAsAdmin(2, 1));
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
        public void TestGetUserByEmailIsCaseInsensitive()
        {
            userManager.CreateUser(CreateUser(1, "dubsteer", "Dubsteer@Test.Local"));

            var result = userManager.GetUserByEmail("  dubsteer@test.local  ");

            Assert.IsNotNull(result);
            Assert.AreEqual("dubsteer", result.Username);
        }

        [TestMethod]
        public void TestResetPasswordChangesLoginAndInvalidatesOldHash()
        {
            const string oldPassword = "old-password";
            const string newPassword = "new-password";
            var user = CreateUser(1, "dubsteer", "dubsteer@test.local", oldPassword);
            userManager.CreateUser(user);
            user.EmailConfirmed = true;
            var oldPasswordHash = user.Password;

            var changed = userManager.ResetPassword(
                user.Id!.Value,
                oldPasswordHash,
                newPassword);

            Assert.IsTrue(changed);
            Assert.IsNull(userManager.GetLoginUser(user.Username, oldPassword));
            Assert.IsNotNull(userManager.GetLoginUser(user.Username, newPassword));
            Assert.IsFalse(userManager.ResetPassword(
                user.Id.Value,
                oldPasswordHash,
                "another-password"));
        }

        [TestMethod]
        public void TestResetPasswordRejectsUnconfirmedUser()
        {
            var user = CreateUser(1, "dubsteer", "dubsteer@test.local");
            userManager.CreateUser(user);

            var changed = userManager.ResetPassword(
                user.Id!.Value,
                user.Password,
                "new-password");

            Assert.IsFalse(changed);
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

        [TestMethod]
        public void TestSearchUserByEmailOrName()
        {
            userManager.CreateUser(CreateUser(1, "member", "member@test.local"));

            Assert.AreEqual(1, userManager.SearchUser("member@test.local").Count);
            Assert.AreEqual(1, userManager.SearchUser("Test").Count);
        }

        private static User CreateUser(int id, string username, string email, string password = "password") =>
            new User(id, "Test", "User", 22, username, email, password, false);
    }
}
