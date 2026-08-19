using System;
using System.Collections.Generic;

namespace CrmDb.Models;

public partial class ContactPerson
{
    public int ContactId { get; set; }

    public int CompanyId { get; set; }

    public string? AdSoyad { get; set; }

    public string? Unvan { get; set; }

    public string? Telefon { get; set; }

    public string? Eposta { get; set; }

    public string VeriKaynagi { get; set; } = null!;

    public DateTime OlusturmaTarihi { get; set; }

    public virtual Company Company { get; set; } = null!;
}
