using System;
using System.Collections.Generic;

namespace Core_Service.Models;

public partial class ImportReceipt
{
    public long Id { get; set; }

    public long SupplierId { get; set; }

    public long CreatorUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();

    public virtual Supplier Supplier { get; set; } = null!;
}
