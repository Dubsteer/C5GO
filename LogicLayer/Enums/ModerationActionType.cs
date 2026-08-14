namespace LogicLayer.Enums;

public enum ModerationActionType : byte
{
    LockDiscussion = 1,
    UnlockDiscussion = 2,
    PinDiscussion = 3,
    UnpinDiscussion = 4,
    RemoveDiscussion = 5,
    RestoreDiscussion = 6,
    RemoveComment = 7,
    RestoreComment = 8,
    ResolveReport = 9,
    RejectReport = 10,
    WarnUser = 11
}
