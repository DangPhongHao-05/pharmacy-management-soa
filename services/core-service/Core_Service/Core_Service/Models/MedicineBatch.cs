using System;
using System.Collections.Generic;

namespace Core_Service.Models;

public partial class MedicineBatch
{
    public long Id { get; set; }

    public long MedicineId { get; set; }

    public string BatchNumber { get; set; } = null!;

    public DateOnly ExpiryDate { get; set; }

    public int Quantity { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
