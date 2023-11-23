﻿namespace SocialNetwork.DTO
{
    public partial class NotifyDTO
    {
        public Guid Id { get; set; }

        public Guid UserTo { get; set; }

        public Guid? UserNotify { get; set; }

        public string Content { get; set; }

        public DateTime? CreateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool IsDeleted { get; set; }

        public Guid? NotifyType { get; set; }
    }
}
