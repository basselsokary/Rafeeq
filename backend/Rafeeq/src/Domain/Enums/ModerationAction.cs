namespace Domain.Enums;

public enum ModerationAction
{
    NoAction = 1,
    ContentRemoved = 2,
    UserWarned = 3,
    UserSuspended = 4,
    UserBanned = 5,
    ContentEdited = 6
}