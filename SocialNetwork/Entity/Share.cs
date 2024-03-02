using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Share : IEntity
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

    public int? LevelView { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual User User { get; set; }
}
