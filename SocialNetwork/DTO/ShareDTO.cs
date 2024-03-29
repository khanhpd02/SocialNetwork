﻿using SocialNetwork.Entity;

namespace SocialNetwork.DTO
{
    public class ShareDTO
    {
        public Guid Id { get; set; }

        public string Content { get; set; }

        public Guid? PostId { get; set; }

        public Guid? UserId { get; set; }

        public DateTime? CreateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool IsDeleted { get; set; }
        public string Link { get; set; }
        public int LevelView { get; set; }

    }
}
