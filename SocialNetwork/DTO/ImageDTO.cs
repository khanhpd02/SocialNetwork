﻿namespace SocialNetwork.DTO;

public class ImageDTO
{
    public Guid? Id { get; set; }

    public string? Link { get; set; }

    public Guid? PostId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

}
