﻿using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Infor : IEntity
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string FullName { get; set; }

    public string WorkPlace { get; set; }

    public bool? Gender { get; set; }

    public string PhoneNumber { get; set; }

    public string Image { get; set; }

    public string Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User User { get; set; }
}
