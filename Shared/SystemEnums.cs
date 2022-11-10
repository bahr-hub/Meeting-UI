using System.ComponentModel;
using static Shared.SystemEnums;

namespace Shared
{
    public static class SystemEnums
    {
        public enum AuthenticateMode
        {
            Database = 0,
            ActiveDirctory,
            Both
        }

        public enum ResponseMessage
        {
            [Description("meeting.started")]
            MeetingStarted = 1,
            [Description("meeting.ended")]
            MeetingEnded,
            [Description("meeting.joined")]
            MeetingJoined,
            [Description("meeting.cancelled")]
            MeetingCancelled,
            [Description("meeting.paused")]
            MeetingPaused,
            [Description("meeting.postponed")]
            MeetingPostponed,
            [Description("meeting.accepted")]
            MeetingAccepted,
            [Description("meeting.created")]
            MeetingCreated,
            [Description("task.created")]
            TaskCreated,
            [Description("task.closed")]
            TaskClosed,
            [Description("topic.created")]
            TopicCreated,
            [Description("topic.closed")]
            TopicClosed,
            [Description("participant.added")]
            ParticipantAdded,
            [Description("participant.removed")]
            ParticipantRemoved,
            [Description("meeting.deleted")]
            MeetingDeleted,
            [Description("meeting.updated")]
            MeetingUpdated,
            [Description("meeting.decline")]
            MeetingDecline,
            [Description("proposal.decline")]
            ProposalDecline,
        }
        public enum ErrorMsg
        {
            [Description("ErrorMsg.not_found")]
            NotFound = 1,
            [Description("ErrorMsg.not_unique")]
            NotUnique = 2,
            [Description("ErrorMsg.server_error")]
            ServerError = 3,
            [Description("ErrorMsg.unauthorized")]
            UnAuthorized = 4,
            [Description("ErrorMsg.meeting_already_started")]
            MeetingAlreadyStarted = 5,
            [Description("ErrorMsg.meeting_already_ended")]
            MeetingAlreadyEnded = 6,
            [Description("ErrorMsg.meeting_not_started_yet")]
            MeetingNotStartedYet = 7,
            [Description("ErrorMsg.meeting_not_valid")]
            NotValid = 8,
            [Description("ErrorMsg.meeting_in_past")]
            MeetingInPast = 9,
            [Description("ErrorMsg.topicsDuration_greaterThan_meetingDuration")]
            TopicsDurationGreaterThanMeetingDuration = 10,
            [Description("ErrorMsg.TaskAssignee_NotMeetingParticipant")]
            TaskAssigneeNotMeetingParticipant = 11,
            [Description("ErrorMsg.TaskDueDateInPast")]
            TaskDueDateInPast = 12,
            [Description("ErrorMsg.TopicPresenterIsNotMeetingParticipant")]
            PresenterNotParticipant = 13,
            [Description("ErrorMsg.MeetingEndEarlierThanMeetingStart")]
            MeetingEndEarlierThanMeetingStart = 14,
            [Description("ErrorMsg.MeetingMustHaveParticipants")]
            MeetingMustHaveParticipants = 15,
            [Description("ErrorMsg.MeetingMustHaveTopics")]
            MeetingMustHaveTopics = 16,
            [Description("ErrorMsg.VacationEndEarlierThanOrEqualVacationStart")]
            VacationEndEarlierThanVacationStart = 17,
            [Description("ErrorMsg.VacationStartInPast")]
            VacationStartInPast = 18,
            [Description("ErrorMsg.MeetingNotRunning")]
            MeetingNotRunning = 19,
            [Description("ErrorMsg.Vacation_Intersection_With_PreviousVacation")]
            VacationIntersection = 20,
            [Description("ErrorMsg.Participants_NotUnique")]
            DuplicateParticipants = 21,
            [Description("ErrorMsg.Topics_NotUnique")]
            DuplicateTopics = 22,
            [Description("ErrorMsg.CantStart_OutOfMeetingInterval")]
            CantStart_OutOfMeetingInterval = 23,
            [Description("ErrorMsg.meeting_not_found")]
            meetingNotFound = 24,
            [Description("ErrorMsg.participant_not_found")]
            ParticipantNotFound = 25,
            [Description("ErrorMsg.Password_is_required")]
            PasswordIsRequired = 26,
            [Description("ErrorMsg.Name_is_required")]
            NameIsRequired = 27,
            [Description("ErrorMsg.Only_Creator_CanStart")]
            OnlyCreatorCanStart = 28,
            [Description("ErrorMsg.NotMeetingCreator")]
            NotMeetingCreator = 29,
            [Description("ErrorMsg.task_notTaskAssignee")]
            NotTaskAssignee = 30,
            [Description("ErrorMsg.CannotDeleteUser")]
            CannotDeleteUser = 31,
            [Description("ErrorMsg.tagNameTooLong")]
            tagNameTooLong = 32,
            [Description("ErrorMsg.dateInThePast")]
            dateInThePast = 33,
            [Description("ErrorMsg.projectNameTooLong")]
            projectNameTooLong = 34,
            [Description("ErrorMsg.lastTopic")]
            lastTopic = 35,
            [Description("ErrorMsg.duplicatedName")]
            duplicatedName = 36,
            [Description("ErrorMsg.duplicatedEmail")]
            duplicatedEmail = 37,
            [Description("ErrorMsg.duplicatedMobile")]
            duplicatedMobile = 38,
            [Description("ErrorMsg.InvalidDate")]
            InvalidDate = 39,
            [Description("ErrorMsg.LiecenseExpired")]
            LiecenseExpired = 40
        }
        public enum MeetingStatus
        {
            New = 0,
            Started = 1,
            Ended = 2,
            Delayed = 3,
            Cancelled = 4,
            FollowUp = 5

        }
        public enum WeekDays
        {
            Sat = 1,
            Sun,
            Mon,
            Tue,
            Wed,
            Thu,
            Fri
        }
        public enum TaskStatus
        {
            pending = 0,
            completed = 1
        }
        public enum Privileges
        {
            view = 1,
            create = 2,
            edit = 3,
            details = 4,
            delete = 5,
            delay = 6,
            Activiation = 7
        }
        public enum Modules
        {
            task = 1,
            meeting = 2,
            user = 3,
            location = 4,
            project = 5,
            tag = 6,
            role = 7,
            systemconfiguration = 8,
            vacation = 9
        }
    }
    public static class MyEnumExtensions
    {
        public static string ToDescriptionString(this ErrorMsg val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
        public static string ToDescriptionString(this ResponseMessage val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
