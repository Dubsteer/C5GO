using LogicLayer.Managers;
using Unit_Tests.MockRepos;

namespace Unit_Tests;

[TestClass]
public class TestNotificationManager
{
    [TestMethod]
    public void GetForUserCanReturnOnlyUnreadNotifications()
    {
        var repository = new MockNotificationRepo();
        repository.Create(1, "Old notification");
        repository.Create(1, "Unread notification");
        repository.Create(2, "Another user's notification");
        repository.Notifications[0].IsRead = true;
        var manager = new NotificationManager(repository);

        var notifications = manager.GetForUser(1, 10, unreadOnly: true);

        Assert.HasCount(1, notifications);
        Assert.AreEqual("Unread notification", notifications[0].Message);
    }

    [TestMethod]
    public void MarkAllAsReadAffectsOnlyCurrentUser()
    {
        var repository = new MockNotificationRepo();
        repository.Create(1, "First");
        repository.Create(1, "Second");
        repository.Create(2, "Other user");
        var manager = new NotificationManager(repository);

        var updated = manager.MarkAllAsRead(1);

        Assert.AreEqual(2, updated);
        Assert.AreEqual(0, manager.GetUnreadCount(1));
        Assert.AreEqual(1, manager.GetUnreadCount(2));
    }

    [TestMethod]
    public void InvalidIdentifiersDoNotReachRepository()
    {
        var manager = new NotificationManager(new MockNotificationRepo());

        Assert.IsEmpty(manager.GetForUser(0));
        Assert.IsNull(manager.MarkAsRead(0, 1));
        Assert.AreEqual(0, manager.MarkAllAsRead(0));
    }
}
