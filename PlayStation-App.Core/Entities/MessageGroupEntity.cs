using System.Collections.Generic;

namespace PlayStation_App.Core.Entities
{
    public class MessageGroupEntity
    {
        public class Member
        {
            public string OnlineId { get; set; }
        }

        public class MessageGroupDetail
        {
            public int MessageGroupType { get; set; }
            public string MessageGroupName { get; set; }
            public int TotalMembers { get; set; }

            public List<Member> Members { get; set; }
        }

        public class LatestMessage
        {
            public int MessageUid { get; set; }
            public bool SeenFlag { get; set; }
            public int MessageKind { get; set; }
            public string SenderOnlineId { get; set; }
            public string ReceivedDate { get; set; }
            public string Body { get; set; }
        }

        public class MessageGroup
        {
            public string MessageGroupId { get; set; }
            public MessageGroupDetail MessageGroupDetail { get; set; }
            public int TotalUnseenMessages { get; set; }
            public int TotalMessages { get; set; }

            public LatestMessage LatestMessage { get; set; }
        }

        public List<MessageGroup> MessageGroups { get; set; }
        public int Start { get; set; }
        public int Size { get; set; }
        public int TotalResults { get; set; }
    }
}
