using System;
using System.Collections.Generic;

namespace LIBRA.Scan.Entities.Dtos;

public partial class JobDto
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? Deleted { get; set; }
    public virtual ICollection<BatchDto>? Batches { get; set; } = new List<BatchDto>();

}
