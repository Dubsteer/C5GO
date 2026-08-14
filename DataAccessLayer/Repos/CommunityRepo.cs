using LogicLayer.Enums;
using LogicLayer.IRepos;
using LogicLayer.Models;
using LogicLayer.Models.Community;
using MySql.Data.MySqlClient;
using System.Data;

namespace DataLayer.Repos;

public class CommunityRepo : ICommunityRepo
{
    private readonly IConnection conn;

    public CommunityRepo(IConnection conn)
    {
        this.conn = conn;
    }

    public IReadOnlyList<CommunityCategory> GetCategories()
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            SELECT id, slug, name, description, display_order, is_active
            FROM community_category
            WHERE is_active = 1
            ORDER BY display_order, name
        ", conn.Connection);

        var categories = new List<CommunityCategory>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            categories.Add(MapCategory(reader));

        return categories;
    }

    public IReadOnlyDictionary<int, CommunityContributionStats> GetContributionStats()
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            SELECT
                u.id AS user_id,
                (SELECT COUNT(*)
                 FROM discussion d
                 WHERE d.author_id = u.id AND d.status_int = 0) AS discussion_count,
                (SELECT COUNT(*)
                 FROM discussion_comment dc
                 WHERE dc.author_id = u.id AND dc.status_int = 0) AS comment_count,
                COALESCE((SELECT SUM(v.vote_value)
                          FROM discussion d
                          INNER JOIN discussion_vote v ON v.discussion_id = d.id
                          WHERE d.author_id = u.id), 0)
                +
                COALESCE((SELECT SUM(v.vote_value)
                          FROM discussion_comment dc
                          INNER JOIN discussion_comment_vote v ON v.comment_id = dc.id
                          WHERE dc.author_id = u.id), 0) AS vote_score,
                (SELECT COUNT(*)
                 FROM content_report r
                 LEFT JOIN discussion rd ON rd.id = r.discussion_id
                 LEFT JOIN discussion_comment rc ON rc.id = r.comment_id
                 WHERE r.status_int = 0
                   AND COALESCE(rd.author_id, rc.author_id) = u.id) AS pending_report_count
            FROM `user` u
        ", conn.Connection);

        var stats = new Dictionary<int, CommunityContributionStats>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var userId = reader.GetInt32("user_id");
            stats[userId] = new CommunityContributionStats
            {
                UserId = userId,
                DiscussionCount = Convert.ToInt32(reader["discussion_count"]),
                CommentCount = Convert.ToInt32(reader["comment_count"]),
                VoteScore = Convert.ToInt32(reader["vote_score"]),
                PendingReportCount = Convert.ToInt32(reader["pending_report_count"])
            };
        }

        return stats;
    }

    public IReadOnlyList<Discussion> GetDiscussions(
        int? categoryId,
        CommunitySort sort,
        int page,
        int pageSize,
        int? viewerId)
    {
        EnsureConnection();
        var orderBy = sort switch
        {
            CommunitySort.Top => "score DESC, d.created_at DESC",
            CommunitySort.Active => "last_activity DESC",
            _ => "d.created_at DESC"
        };

        var categoryFilter = categoryId.HasValue
            ? "AND d.category_id = @CATEGORY_ID"
            : string.Empty;

        using var command = new MySqlCommand($@"
            SELECT
                d.id, d.author_id, d.category_id, d.title, d.content,
                d.image_path, d.youtube_video_id, d.is_spoiler, d.is_locked,
                d.is_pinned, d.status_int, d.created_at, d.updated_at,
                u.username AS author_username,
                c.slug AS category_slug, c.name AS category_name,
                c.description AS category_description,
                COALESCE((
                    SELECT SUM(v.vote_value)
                    FROM discussion_vote v
                    WHERE v.discussion_id = d.id
                ), 0) AS score,
                (
                    SELECT COUNT(*)
                    FROM discussion_comment dc
                    WHERE dc.discussion_id = d.id AND dc.status_int = 0
                ) AS comment_count,
                COALESCE((
                    SELECT v.vote_value
                    FROM discussion_vote v
                    WHERE v.discussion_id = d.id AND v.user_id = @VIEWER_ID
                ), 0) AS current_user_vote,
                GREATEST(
                    d.created_at,
                    COALESCE((
                        SELECT MAX(dc.created_at)
                        FROM discussion_comment dc
                        WHERE dc.discussion_id = d.id AND dc.status_int = 0
                    ), d.created_at)
                ) AS last_activity
            FROM discussion d
            INNER JOIN `user` u ON u.id = d.author_id
            INNER JOIN community_category c ON c.id = d.category_id
            WHERE d.status_int = 0 {categoryFilter}
            ORDER BY d.is_pinned DESC, {orderBy}
            LIMIT @PAGE_SIZE OFFSET @OFFSET
        ", conn.Connection);
        command.Parameters.AddWithValue("@VIEWER_ID", viewerId ?? 0);
        command.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
        command.Parameters.AddWithValue("@OFFSET", (page - 1) * pageSize);
        if (categoryId.HasValue)
            command.Parameters.AddWithValue("@CATEGORY_ID", categoryId.Value);

        var discussions = new List<Discussion>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            discussions.Add(MapDiscussion(reader));

        return discussions;
    }

    public int GetDiscussionCount(int? categoryId)
    {
        EnsureConnection();
        var categoryFilter = categoryId.HasValue
            ? "AND category_id = @CATEGORY_ID"
            : string.Empty;

        using var command = new MySqlCommand($@"
            SELECT COUNT(*)
            FROM discussion
            WHERE status_int = 0 {categoryFilter}
        ", conn.Connection);
        if (categoryId.HasValue)
            command.Parameters.AddWithValue("@CATEGORY_ID", categoryId.Value);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public Discussion? GetDiscussionById(int discussionId, int? viewerId)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            SELECT
                d.id, d.author_id, d.category_id, d.title, d.content,
                d.image_path, d.youtube_video_id, d.is_spoiler, d.is_locked,
                d.is_pinned, d.status_int, d.created_at, d.updated_at,
                u.username AS author_username,
                c.slug AS category_slug, c.name AS category_name,
                c.description AS category_description,
                COALESCE((
                    SELECT SUM(v.vote_value)
                    FROM discussion_vote v
                    WHERE v.discussion_id = d.id
                ), 0) AS score,
                (
                    SELECT COUNT(*)
                    FROM discussion_comment dc
                    WHERE dc.discussion_id = d.id AND dc.status_int = 0
                ) AS comment_count,
                COALESCE((
                    SELECT v.vote_value
                    FROM discussion_vote v
                    WHERE v.discussion_id = d.id AND v.user_id = @VIEWER_ID
                ), 0) AS current_user_vote
            FROM discussion d
            INNER JOIN `user` u ON u.id = d.author_id
            INNER JOIN community_category c ON c.id = d.category_id
            WHERE d.id = @DISCUSSION_ID
            LIMIT 1
        ", conn.Connection);
        command.Parameters.AddWithValue("@DISCUSSION_ID", discussionId);
        command.Parameters.AddWithValue("@VIEWER_ID", viewerId ?? 0);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapDiscussion(reader) : null;
    }

    public int CreateDiscussion(Discussion discussion)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            INSERT INTO discussion
                (author_id, category_id, title, content, image_path,
                 youtube_video_id, is_spoiler, is_locked, is_pinned,
                 status_int, created_at)
            VALUES
                (@AUTHOR_ID, @CATEGORY_ID, @TITLE, @CONTENT, @IMAGE_PATH,
                 @VIDEO_ID, @IS_SPOILER, 0, 0, @STATUS, @CREATED_AT)
        ", conn.Connection);
        AddDiscussionParameters(command, discussion);
        command.Parameters.AddWithValue("@CREATED_AT", discussion.CreatedAt);
        command.ExecuteNonQuery();
        return checked((int)command.LastInsertedId);
    }

    public bool UpdateDiscussion(Discussion discussion, int authorId)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            UPDATE discussion
            SET category_id = @CATEGORY_ID,
                title = @TITLE,
                content = @CONTENT,
                image_path = @IMAGE_PATH,
                youtube_video_id = @VIDEO_ID,
                is_spoiler = @IS_SPOILER,
                updated_at = @UPDATED_AT
            WHERE id = @DISCUSSION_ID
              AND author_id = @AUTHOR_ID
              AND status_int = 0
              AND is_locked = 0
        ", conn.Connection);
        AddDiscussionParameters(command, discussion);
        command.Parameters.AddWithValue("@DISCUSSION_ID", discussion.Id);
        command.Parameters.AddWithValue("@UPDATED_AT", discussion.UpdatedAt);
        return command.ExecuteNonQuery() == 1;
    }

    public bool RemoveOwnDiscussion(int discussionId, int authorId)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            UPDATE discussion
            SET status_int = 1,
                removed_at = UTC_TIMESTAMP(),
                removed_by = @AUTHOR_ID,
                removal_reason = 'Removed by author'
            WHERE id = @DISCUSSION_ID
              AND author_id = @AUTHOR_ID
              AND status_int = 0
        ", conn.Connection);
        command.Parameters.AddWithValue("@DISCUSSION_ID", discussionId);
        command.Parameters.AddWithValue("@AUTHOR_ID", authorId);
        return command.ExecuteNonQuery() == 1;
    }

    public int SetDiscussionVote(int discussionId, int userId, sbyte voteValue)
    {
        EnsureConnection();
        using var transaction = conn.Connection.BeginTransaction();
        SetVote(
            "discussion_vote",
            "discussion_id",
            discussionId,
            userId,
            voteValue,
            transaction);
        var score = GetVoteScore(
            "discussion_vote",
            "discussion_id",
            discussionId,
            transaction);
        transaction.Commit();
        return score;
    }

    public IReadOnlyList<DiscussionComment> GetComments(
        int discussionId,
        int? viewerId)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            SELECT
                dc.id, dc.discussion_id, dc.author_id, dc.parent_comment_id,
                dc.content, dc.status_int, dc.created_at, dc.updated_at,
                u.username AS author_username,
                COALESCE((
                    SELECT SUM(v.vote_value)
                    FROM discussion_comment_vote v
                    WHERE v.comment_id = dc.id
                ), 0) AS score,
                COALESCE((
                    SELECT v.vote_value
                    FROM discussion_comment_vote v
                    WHERE v.comment_id = dc.id AND v.user_id = @VIEWER_ID
                ), 0) AS current_user_vote
            FROM discussion_comment dc
            INNER JOIN `user` u ON u.id = dc.author_id
            WHERE dc.discussion_id = @DISCUSSION_ID
            ORDER BY COALESCE(dc.parent_comment_id, dc.id),
                     dc.parent_comment_id IS NOT NULL,
                     dc.created_at
        ", conn.Connection);
        command.Parameters.AddWithValue("@DISCUSSION_ID", discussionId);
        command.Parameters.AddWithValue("@VIEWER_ID", viewerId ?? 0);

        var comments = new List<DiscussionComment>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            comments.Add(MapComment(reader));

        return comments;
    }

    public DiscussionComment? GetCommentById(int commentId)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            SELECT
                dc.id, dc.discussion_id, dc.author_id, dc.parent_comment_id,
                dc.content, dc.status_int, dc.created_at, dc.updated_at,
                u.username AS author_username,
                COALESCE((
                    SELECT SUM(v.vote_value)
                    FROM discussion_comment_vote v
                    WHERE v.comment_id = dc.id
                ), 0) AS score,
                0 AS current_user_vote
            FROM discussion_comment dc
            INNER JOIN `user` u ON u.id = dc.author_id
            WHERE dc.id = @COMMENT_ID
            LIMIT 1
        ", conn.Connection);
        command.Parameters.AddWithValue("@COMMENT_ID", commentId);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapComment(reader) : null;
    }

    public int CreateComment(DiscussionComment comment)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            INSERT INTO discussion_comment
                (discussion_id, author_id, parent_comment_id, content,
                 status_int, created_at)
            VALUES
                (@DISCUSSION_ID, @AUTHOR_ID, @PARENT_ID, @CONTENT,
                 @STATUS, @CREATED_AT)
        ", conn.Connection);
        command.Parameters.AddWithValue("@DISCUSSION_ID", comment.DiscussionId);
        command.Parameters.AddWithValue("@AUTHOR_ID", comment.AuthorId);
        command.Parameters.AddWithValue(
            "@PARENT_ID",
            comment.ParentCommentId.HasValue
                ? comment.ParentCommentId.Value
                : DBNull.Value);
        command.Parameters.AddWithValue("@CONTENT", comment.Content);
        command.Parameters.AddWithValue("@STATUS", (byte)comment.Status);
        command.Parameters.AddWithValue("@CREATED_AT", comment.CreatedAt);
        command.ExecuteNonQuery();
        return checked((int)command.LastInsertedId);
    }

    public bool RemoveOwnComment(int commentId, int authorId)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            UPDATE discussion_comment
            SET status_int = 1,
                removed_at = UTC_TIMESTAMP(),
                removed_by = @AUTHOR_ID,
                removal_reason = 'Removed by author'
            WHERE id = @COMMENT_ID
              AND author_id = @AUTHOR_ID
              AND status_int = 0
        ", conn.Connection);
        command.Parameters.AddWithValue("@COMMENT_ID", commentId);
        command.Parameters.AddWithValue("@AUTHOR_ID", authorId);
        return command.ExecuteNonQuery() == 1;
    }

    public int SetCommentVote(int commentId, int userId, sbyte voteValue)
    {
        EnsureConnection();
        using var transaction = conn.Connection.BeginTransaction();
        SetVote(
            "discussion_comment_vote",
            "comment_id",
            commentId,
            userId,
            voteValue,
            transaction);
        var score = GetVoteScore(
            "discussion_comment_vote",
            "comment_id",
            commentId,
            transaction);
        transaction.Commit();
        return score;
    }

    public bool CreateReport(ContentReport report)
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            INSERT IGNORE INTO content_report
                (reporter_id, discussion_id, comment_id, reason, details,
                 status_int, created_at)
            VALUES
                (@REPORTER_ID, @DISCUSSION_ID, @COMMENT_ID, @REASON, @DETAILS,
                 @STATUS, @CREATED_AT)
        ", conn.Connection);
        command.Parameters.AddWithValue("@REPORTER_ID", report.ReporterId);
        command.Parameters.AddWithValue(
            "@DISCUSSION_ID",
            report.DiscussionId.HasValue ? report.DiscussionId.Value : DBNull.Value);
        command.Parameters.AddWithValue(
            "@COMMENT_ID",
            report.CommentId.HasValue ? report.CommentId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@REASON", report.Reason);
        command.Parameters.AddWithValue(
            "@DETAILS",
            report.Details == null ? DBNull.Value : report.Details);
        command.Parameters.AddWithValue("@STATUS", (byte)report.Status);
        command.Parameters.AddWithValue("@CREATED_AT", report.CreatedAt);
        return command.ExecuteNonQuery() == 1;
    }

    public IReadOnlyList<ContentReport> GetPendingReports()
    {
        EnsureConnection();
        using var command = new MySqlCommand(@"
            SELECT
                r.id, r.reporter_id,
                COALESCE(r.discussion_id, dc.discussion_id) AS discussion_id,
                r.comment_id,
                r.reason, r.details, r.status_int, r.created_at,
                r.reviewed_by, r.reviewed_at, r.resolution_note,
                reporter.username AS reporter_username,
                CASE
                    WHEN r.comment_id IS NOT NULL THEN dc.author_id
                    ELSE d.author_id
                END AS target_author_id,
                COALESCE(d.title, 'Comment') AS target_title,
                CASE
                    WHEN r.comment_id IS NOT NULL THEN LEFT(dc.content, 300)
                    ELSE COALESCE(LEFT(d.content, 300), '')
                END AS target_preview
            FROM content_report r
            INNER JOIN `user` reporter ON reporter.id = r.reporter_id
            LEFT JOIN discussion_comment dc ON dc.id = r.comment_id
            LEFT JOIN discussion d ON d.id = COALESCE(r.discussion_id, dc.discussion_id)
            WHERE r.status_int = 0
            ORDER BY r.created_at
        ", conn.Connection);

        var reports = new List<ContentReport>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            reports.Add(MapReport(reader));

        return reports;
    }

    public bool ReviewReport(
        long reportId,
        int reviewerId,
        ReportStatus status,
        string? resolutionNote)
    {
        EnsureConnection();
        using var transaction = conn.Connection.BeginTransaction();
        using var command = new MySqlCommand(@"
            UPDATE content_report
            SET status_int = @STATUS,
                reviewed_by = @REVIEWER_ID,
                reviewed_at = UTC_TIMESTAMP(),
                resolution_note = @NOTE
            WHERE id = @REPORT_ID AND status_int = 0
        ", conn.Connection, transaction);
        command.Parameters.AddWithValue("@STATUS", (byte)status);
        command.Parameters.AddWithValue("@REVIEWER_ID", reviewerId);
        command.Parameters.AddWithValue("@NOTE", resolutionNote ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@REPORT_ID", reportId);

        if (command.ExecuteNonQuery() != 1)
        {
            transaction.Commit();
            return false;
        }

        AddModerationAction(
            reviewerId,
            status == ReportStatus.Resolved
                ? ModerationActionType.ResolveReport
                : ModerationActionType.RejectReport,
            ModerationTargetType.Report,
            reportId,
            resolutionNote,
            transaction);
        transaction.Commit();
        return true;
    }

    public bool ApplyDiscussionModeration(
        int discussionId,
        int moderatorId,
        ModerationActionType action,
        string? reason)
    {
        EnsureConnection();
        var update = action switch
        {
            ModerationActionType.LockDiscussion => "is_locked = 1",
            ModerationActionType.UnlockDiscussion => "is_locked = 0",
            ModerationActionType.PinDiscussion => "is_pinned = 1",
            ModerationActionType.UnpinDiscussion => "is_pinned = 0",
            ModerationActionType.RemoveDiscussion =>
                "status_int = 1, removed_at = UTC_TIMESTAMP(), removed_by = @MODERATOR_ID, removal_reason = @REASON",
            ModerationActionType.RestoreDiscussion =>
                "status_int = 0, removed_at = NULL, removed_by = NULL, removal_reason = NULL",
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };

        return ApplyModerationUpdate(
            "discussion",
            discussionId,
            moderatorId,
            action,
            ModerationTargetType.Discussion,
            update,
            reason);
    }

    public bool ApplyCommentModeration(
        int commentId,
        int moderatorId,
        ModerationActionType action,
        string? reason)
    {
        EnsureConnection();
        var update = action switch
        {
            ModerationActionType.RemoveComment =>
                "status_int = 1, removed_at = UTC_TIMESTAMP(), removed_by = @MODERATOR_ID, removal_reason = @REASON",
            ModerationActionType.RestoreComment =>
                "status_int = 0, removed_at = NULL, removed_by = NULL, removal_reason = NULL",
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };

        return ApplyModerationUpdate(
            "discussion_comment",
            commentId,
            moderatorId,
            action,
            ModerationTargetType.Comment,
            update,
            reason);
    }

    private bool ApplyModerationUpdate(
        string tableName,
        int targetId,
        int moderatorId,
        ModerationActionType action,
        ModerationTargetType targetType,
        string update,
        string? reason)
    {
        using var transaction = conn.Connection.BeginTransaction();
        using var command = new MySqlCommand($@"
            UPDATE {tableName}
            SET {update}
            WHERE id = @TARGET_ID
        ", conn.Connection, transaction);
        command.Parameters.AddWithValue("@TARGET_ID", targetId);
        command.Parameters.AddWithValue("@MODERATOR_ID", moderatorId);
        command.Parameters.AddWithValue("@REASON", reason ?? (object)DBNull.Value);

        if (command.ExecuteNonQuery() != 1)
        {
            transaction.Commit();
            return false;
        }

        AddModerationAction(
            moderatorId,
            action,
            targetType,
            targetId,
            reason,
            transaction);
        transaction.Commit();
        return true;
    }

    private void AddModerationAction(
        int moderatorId,
        ModerationActionType action,
        ModerationTargetType targetType,
        long targetId,
        string? reason,
        MySqlTransaction transaction)
    {
        using var command = new MySqlCommand(@"
            INSERT INTO moderation_action
                (moderator_id, action_type, target_type, target_id, reason, created_at)
            VALUES
                (@MODERATOR_ID, @ACTION_TYPE, @TARGET_TYPE, @TARGET_ID, @REASON, UTC_TIMESTAMP())
        ", conn.Connection, transaction);
        command.Parameters.AddWithValue("@MODERATOR_ID", moderatorId);
        command.Parameters.AddWithValue("@ACTION_TYPE", (byte)action);
        command.Parameters.AddWithValue("@TARGET_TYPE", (byte)targetType);
        command.Parameters.AddWithValue("@TARGET_ID", targetId);
        command.Parameters.AddWithValue("@REASON", reason ?? (object)DBNull.Value);
        command.ExecuteNonQuery();
    }

    private void SetVote(
        string tableName,
        string targetColumn,
        int targetId,
        int userId,
        sbyte voteValue,
        MySqlTransaction transaction)
    {
        using var selectCommand = new MySqlCommand($@"
            SELECT vote_value
            FROM {tableName}
            WHERE {targetColumn} = @TARGET_ID AND user_id = @USER_ID
        ", conn.Connection, transaction);
        selectCommand.Parameters.AddWithValue("@TARGET_ID", targetId);
        selectCommand.Parameters.AddWithValue("@USER_ID", userId);
        var existingValue = selectCommand.ExecuteScalar();

        if (existingValue != null && Convert.ToSByte(existingValue) == voteValue)
        {
            using var deleteCommand = new MySqlCommand($@"
                DELETE FROM {tableName}
                WHERE {targetColumn} = @TARGET_ID AND user_id = @USER_ID
            ", conn.Connection, transaction);
            deleteCommand.Parameters.AddWithValue("@TARGET_ID", targetId);
            deleteCommand.Parameters.AddWithValue("@USER_ID", userId);
            deleteCommand.ExecuteNonQuery();
            return;
        }

        using var writeCommand = new MySqlCommand($@"
            INSERT INTO {tableName}
                ({targetColumn}, user_id, vote_value, created_at, updated_at)
            VALUES
                (@TARGET_ID, @USER_ID, @VOTE_VALUE, UTC_TIMESTAMP(), NULL)
            ON DUPLICATE KEY UPDATE
                vote_value = @VOTE_VALUE,
                updated_at = UTC_TIMESTAMP()
        ", conn.Connection, transaction);
        writeCommand.Parameters.AddWithValue("@TARGET_ID", targetId);
        writeCommand.Parameters.AddWithValue("@USER_ID", userId);
        writeCommand.Parameters.AddWithValue("@VOTE_VALUE", voteValue);
        writeCommand.ExecuteNonQuery();
    }

    private int GetVoteScore(
        string tableName,
        string targetColumn,
        int targetId,
        MySqlTransaction transaction)
    {
        using var command = new MySqlCommand($@"
            SELECT COALESCE(SUM(vote_value), 0)
            FROM {tableName}
            WHERE {targetColumn} = @TARGET_ID
        ", conn.Connection, transaction);
        command.Parameters.AddWithValue("@TARGET_ID", targetId);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private static void AddDiscussionParameters(
        MySqlCommand command,
        Discussion discussion)
    {
        command.Parameters.AddWithValue("@AUTHOR_ID", discussion.AuthorId);
        command.Parameters.AddWithValue("@CATEGORY_ID", discussion.CategoryId);
        command.Parameters.AddWithValue("@TITLE", discussion.Title);
        command.Parameters.AddWithValue(
            "@CONTENT",
            discussion.Content == null ? DBNull.Value : discussion.Content);
        command.Parameters.AddWithValue(
            "@IMAGE_PATH",
            discussion.ImagePath == null ? DBNull.Value : discussion.ImagePath);
        command.Parameters.AddWithValue(
            "@VIDEO_ID",
            discussion.YouTubeVideoId == null ? DBNull.Value : discussion.YouTubeVideoId);
        command.Parameters.AddWithValue("@IS_SPOILER", discussion.IsSpoiler);
        command.Parameters.AddWithValue("@STATUS", (byte)discussion.Status);
    }

    private static CommunityCategory MapCategory(MySqlDataReader reader)
    {
        return new CommunityCategory
        {
            Id = reader.GetInt32("id"),
            Slug = reader.GetString("slug"),
            Name = reader.GetString("name"),
            Description = reader.GetString("description"),
            DisplayOrder = reader.GetInt32("display_order"),
            IsActive = reader.GetBoolean("is_active")
        };
    }

    private static Discussion MapDiscussion(MySqlDataReader reader)
    {
        var authorId = reader.GetInt32("author_id");
        return new Discussion
        {
            Id = reader.GetInt32("id"),
            AuthorId = authorId,
            CategoryId = reader.GetInt32("category_id"),
            Title = reader.GetString("title"),
            Content = SafeString(reader, "content"),
            ImagePath = SafeString(reader, "image_path"),
            YouTubeVideoId = SafeString(reader, "youtube_video_id"),
            IsSpoiler = reader.GetBoolean("is_spoiler"),
            IsLocked = reader.GetBoolean("is_locked"),
            IsPinned = reader.GetBoolean("is_pinned"),
            Status = (CommunityContentStatus)reader.GetByte("status_int"),
            CreatedAt = reader.GetDateTime("created_at"),
            UpdatedAt = SafeDateTime(reader, "updated_at"),
            Score = Convert.ToInt32(reader["score"]),
            CommentCount = Convert.ToInt32(reader["comment_count"]),
            CurrentUserVote = Convert.ToSByte(reader["current_user_vote"]),
            Author = new User(authorId)
            {
                Username = reader.GetString("author_username")
            },
            Category = new CommunityCategory
            {
                Id = reader.GetInt32("category_id"),
                Slug = reader.GetString("category_slug"),
                Name = reader.GetString("category_name"),
                Description = reader.GetString("category_description"),
                IsActive = true
            }
        };
    }

    private static DiscussionComment MapComment(MySqlDataReader reader)
    {
        var authorId = reader.GetInt32("author_id");
        return new DiscussionComment
        {
            Id = reader.GetInt32("id"),
            DiscussionId = reader.GetInt32("discussion_id"),
            AuthorId = authorId,
            ParentCommentId = reader.IsDBNull("parent_comment_id")
                ? null
                : reader.GetInt32("parent_comment_id"),
            Content = reader.GetString("content"),
            Status = (CommunityContentStatus)reader.GetByte("status_int"),
            CreatedAt = reader.GetDateTime("created_at"),
            UpdatedAt = SafeDateTime(reader, "updated_at"),
            Score = Convert.ToInt32(reader["score"]),
            CurrentUserVote = Convert.ToSByte(reader["current_user_vote"]),
            Author = new User(authorId)
            {
                Username = reader.GetString("author_username")
            }
        };
    }

    private static ContentReport MapReport(MySqlDataReader reader)
    {
        return new ContentReport
        {
            Id = reader.GetInt64("id"),
            ReporterId = reader.GetInt32("reporter_id"),
            DiscussionId = reader.IsDBNull("discussion_id")
                ? null
                : reader.GetInt32("discussion_id"),
            CommentId = reader.IsDBNull("comment_id")
                ? null
                : reader.GetInt32("comment_id"),
            Reason = reader.GetString("reason"),
            Details = SafeString(reader, "details"),
            Status = (ReportStatus)reader.GetByte("status_int"),
            CreatedAt = reader.GetDateTime("created_at"),
            ReviewedBy = reader.IsDBNull("reviewed_by")
                ? null
                : reader.GetInt32("reviewed_by"),
            ReviewedAt = SafeDateTime(reader, "reviewed_at"),
            ResolutionNote = SafeString(reader, "resolution_note"),
            ReporterUsername = reader.GetString("reporter_username"),
            TargetAuthorId = reader.GetInt32("target_author_id"),
            TargetTitle = reader.GetString("target_title"),
            TargetPreview = reader.GetString("target_preview")
        };
    }

    private static string? SafeString(MySqlDataReader reader, string column)
    {
        return reader.IsDBNull(column) ? null : reader.GetString(column);
    }

    private static DateTime? SafeDateTime(MySqlDataReader reader, string column)
    {
        return reader.IsDBNull(column) ? null : reader.GetDateTime(column);
    }

    private void EnsureConnection()
    {
        if (conn.Connection.State != ConnectionState.Open)
            conn.Open();
    }
}
