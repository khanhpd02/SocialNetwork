using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Post : IEntity
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public int LevelView { get; set; }

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<TagPost> TagPosts { get; set; } = new List<TagPost>();

    public virtual User User { get; set; }

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
