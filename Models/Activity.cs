using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class Activity
{
    public int ActivityId { get; set; }

    public int CompanyId { get; set; }

    public int UserId { get; set; }

    public string Tip { get; set; } = null!;

    public string Icerik { get; set; } = null!;

    public DateTime Tarih { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
