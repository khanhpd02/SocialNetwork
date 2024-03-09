﻿using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class District
{
    public string Code { get; set; }

    public string Name { get; set; }

    public string NameEn { get; set; }

    public string FullName { get; set; }

    public string FullNameEn { get; set; }

    public string CodeName { get; set; }

    public string ProvinceCode { get; set; }

    public int? AdministrativeUnitId { get; set; }

    public virtual AdministrativeUnit AdministrativeUnit { get; set; }

    public virtual Province ProvinceCodeNavigation { get; set; }

    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
