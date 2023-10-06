using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Post : IEntity
{
    public Guid Id { get; set; }

    public string? Content { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<TagPost> TagPosts { get; set; } = new List<TagPost>();

    public virtual ICollection<Video> Videos { get; set; } = new List<Video>();
}
