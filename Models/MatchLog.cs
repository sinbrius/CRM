using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class MatchLog
{
    public int MatchLogId { get; set; }

    public int SourceRecordId { get; set; }

    public int? MatchedCompanyId { get; set; }

    public string MatchType { get; set; } = null!;

    public decimal? SimilarityScore { get; set; }

    public decimal AppliedThreshold { get; set; }

    public string Decision { get; set; } = null!;

    public string? ReviewedByUserId { get; set; }

    public bool? IsApproved { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Company? MatchedCompany { get; set; }

    public virtual SourceRecord SourceRecord { get; set; } = null!;
}
