using System;
using System.Collections.Generic;

namespace LIBRA.Scan.Entities.Entities;

public partial class Job
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? Deleted { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();
}
