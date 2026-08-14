using LogicLayer.Enums;
using LogicLayer.FormModels;
using LogicLayer.Managers;
using LogicLayer.Models;
using Unit_Tests.MockRepos;

namespace Unit_Tests;

[TestClass]
public class TestCommunityManager
{
    private MockCommunityRepo communityRepo = null!;
    private MockNotificationRepo notificationRepo = null!;
    private CommunityManager manager = null!;

    [TestInitialize]
    public void Setup()
    {
        var users = new List<User>
        {
            CreateUser(1, "author"),
            CreateUser(2, "member"),
            CreateUser(3, "moderator")
        };
        var userRepo = new MockUserRepo(users);
        var roleRepo = new MockRoleRepo();
        roleRepo.Seed(1, PlatformRole.Member);
        roleRepo.Seed(2, PlatformRole.Member);
        roleRepo.Seed(3, PlatformRole.Member, PlatformRole.Moderator);

        communityRepo = new MockCommunityRepo();
        notificationRepo = new MockNotificationRepo();
        manager = new CommunityManager(
            communityRepo,
            userRepo,
            notificationRepo,
            new RoleManager(roleRepo, userRepo));
    }

    [TestMethod]
    public void CreateDiscussionAcceptsTextOnly()
    {
        var id = manager.CreateDiscussion(1, CreateDiscussionForm(), null);

        Assert.AreEqual(1, id);
        Assert.AreEqual("Useful discussion", communityRepo.Discussions[0].Title);
    }

    [TestMethod]
    public void CreateDiscussionAcceptsYouTubeUrlAndStoresVideoId()
    {
        var form = CreateDiscussionForm();
        form.Content = null;
        form.YouTubeUrl = "https://youtu.be/dQw4w9WgXcQ";

        manager.CreateDiscussion(1, form, null);

        Assert.AreEqual("dQw4w9WgXcQ", communityRepo.Discussions[0].YouTubeVideoId);
    }

    [TestMethod]
    public void CreateDiscussionRejectsEmptyContentAndMedia()
    {
        var form = CreateDiscussionForm();
        form.Content = " ";

        Assert.ThrowsExactly<ArgumentException>(() =>
            manager.CreateDiscussion(1, form, null));
    }

    [TestMethod]
    public void RepeatingSameVoteRemovesVote()
    {
        var discussionId = manager.CreateDiscussion(1, CreateDiscussionForm(), null);

        Assert.AreEqual(1, manager.SetDiscussionVote(discussionId, 2, 1));
        Assert.AreEqual(0, manager.SetDiscussionVote(discussionId, 2, 1));
    }

    [TestMethod]
    public void ReplyNotifiesParentCommentAuthor()
    {
        var discussionId = manager.CreateDiscussion(1, CreateDiscussionForm(), null);
        var parentId = manager.CreateComment(2, new DiscussionCommentFormModel
        {
            DiscussionId = discussionId,
            Content = "First comment"
        });
        notificationRepo.Notifications.Clear();

        manager.CreateComment(1, new DiscussionCommentFormModel
        {
            DiscussionId = discussionId,
            ParentCommentId = parentId,
            Content = "Thanks for the response"
        });

        Assert.AreEqual(1, notificationRepo.Notifications.Count);
        Assert.AreEqual(2, notificationRepo.Notifications[0].UserId);
    }

    [TestMethod]
    public void ReplyCannotCreateAThirdNestingLevel()
    {
        var discussionId = manager.CreateDiscussion(1, CreateDiscussionForm(), null);
        var parentId = manager.CreateComment(2, new DiscussionCommentFormModel
        {
            DiscussionId = discussionId,
            Content = "First comment"
        });
        var replyId = manager.CreateComment(1, new DiscussionCommentFormModel
        {
            DiscussionId = discussionId,
            ParentCommentId = parentId,
            Content = "First reply"
        });

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            manager.CreateComment(2, new DiscussionCommentFormModel
            {
                DiscussionId = discussionId,
                ParentCommentId = replyId,
                Content = "Nested reply"
            }));
    }

    [TestMethod]
    public void UserCannotReportOwnDiscussion()
    {
        var discussionId = manager.CreateDiscussion(1, CreateDiscussionForm(), null);

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            manager.CreateReport(1, new ContentReportFormModel
            {
                DiscussionId = discussionId,
                Reason = "Spam"
            }));
    }

    [TestMethod]
    public void ModeratorCanLockDiscussion()
    {
        var discussionId = manager.CreateDiscussion(1, CreateDiscussionForm(), null);

        Assert.IsTrue(manager.ModerateDiscussion(
            discussionId,
            3,
            ModerationActionType.LockDiscussion,
            "Reviewing the thread"));
        Assert.IsTrue(communityRepo.Discussions[0].IsLocked);
    }

    private static DiscussionFormModel CreateDiscussionForm()
    {
        return new DiscussionFormModel
        {
            CategoryId = 1,
            Title = "Useful discussion",
            Content = "Some helpful context"
        };
    }

    private static User CreateUser(int id, string username)
    {
        return new User(id, "Test", "User", 22, username, $"{username}@test.local", "hash", false);
    }
}
