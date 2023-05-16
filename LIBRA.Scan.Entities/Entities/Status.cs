using System;
using System.Collections.Generic;

namespace LIBRA.Scan.Entities.Entities;

public partial class Status
{
    public long Id { get; set; }

    public string? StatusName { get; set; }

    public string? StatusDesc { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
