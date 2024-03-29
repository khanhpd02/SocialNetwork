﻿namespace SocialNetwork.DTO;

public class FriendDTO
{
    public Guid? Id { get; set; }

    public Guid User1 { get; set; }

    public Guid User2 { get; set; }

    public int Level { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }
}
