using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Tag
{
    public Guid Id { get; set; }

    public string? Content { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<TagPost> TagPosts { get; set; } = new List<TagPost>();
}
