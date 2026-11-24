using System;
using System.Collections.Generic;

namespace Core_Service.Models;

public partial class StockMovement
{
    public long Id { get; set; }

    public long BatchId { get; set; }

    public string MovementType { get; set; } = null!;

    public int QuantityChange { get; set; }

    public long? ReferenceId { get; set; }

    public long CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual MedicineBatch Batch { get; set; } = null!;
}
