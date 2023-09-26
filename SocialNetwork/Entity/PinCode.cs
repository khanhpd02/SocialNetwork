using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class PinCode
{
    public Guid Id { get; set; }

    public string? Email { get; set; }

    public string? Pin { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ExpiredTime { get; set; }

    public bool IsDeleted { get; set; }
}
