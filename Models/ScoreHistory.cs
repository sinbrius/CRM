using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class ScoreHistory
{
    public int ScoreHistoryId { get; set; }

    public int CompanyId { get; set; }

    public decimal SkorDegeri { get; set; }

    public string KriterVersiyonu { get; set; } = null!;

    public DateTime HesaplamaTarihi { get; set; }

    public string? Detay { get; set; }

    public virtual Company Company { get; set; } = null!;
}
