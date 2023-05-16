using System;
using System.Collections.Generic;

namespace LIBRA.Scan.Entities.Entities;

public partial class Batch
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public DateTime CreatedDate { get; set; }

    public long StatusId { get; set; }

    public long JobId { get; set; }

    public bool? Deleted { get; set; }

    public virtual Job Job { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;
}
