using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class User
{
    public int UserId { get; set; }

    public string AdSoyad { get; set; } = null!;

    public string Eposta { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool AktifMi { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
