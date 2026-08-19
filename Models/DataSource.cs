using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class DataSource
{
    public int DataSourceId { get; set; }

    public string Ad { get; set; } = null!;

    public string Tip { get; set; } = null!;

    public int GuvenilirlikPuani { get; set; }

    public string HukukiDurum { get; set; } = null!;

    public DateTime? SonIceAktarim { get; set; }

    public virtual ICollection<SourceRecord> SourceRecords { get; set; } = new List<SourceRecord>();
}
