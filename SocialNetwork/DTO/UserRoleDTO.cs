﻿namespace SocialNetwork.DTO;

public class UserRoleDTO
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? Id { get; set; }


}
