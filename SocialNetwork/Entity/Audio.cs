using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Audio: IEntity
{
    public Guid Id { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public string Link { get; set; }

    public string Name { get; set; }
}
