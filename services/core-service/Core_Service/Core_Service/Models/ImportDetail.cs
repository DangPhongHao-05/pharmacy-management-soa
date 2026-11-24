using System;
using System.Collections.Generic;

namespace Core_Service.Models;

public partial class ImportDetail
{
    public long Id { get; set; }

    public long ImportId { get; set; }

    public long MedicineId { get; set; }

    public string BatchNumber { get; set; } = null!;

    public DateOnly ExpiryDate { get; set; }

    public int Quantity { get; set; }

    public decimal ImportPrice { get; set; }

    public virtual ImportReceipt Import { get; set; } = null!;

    public virtual Medicine Medicine { get; set; } = null!;
}
