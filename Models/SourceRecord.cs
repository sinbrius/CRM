using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class SourceRecord
{
    public int SourceRecordId { get; set; }

    public int DataSourceId { get; set; }

    public int? CompanyId { get; set; }

    public string HamVeriJson { get; set; } = null!;

    public string EslestirmeDurumu { get; set; } = null!;

    public DateTime IceAktarmaTarihi { get; set; }

    public virtual Company? Company { get; set; }

    public virtual DataSource DataSource { get; set; } = null!;

    public virtual ICollection<MatchLog> MatchLogs { get; set; } = new List<MatchLog>();
}
