﻿namespace SocialNetwork.DTO;

public class ReportDTO
{
    public Guid? Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? PostId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

}
