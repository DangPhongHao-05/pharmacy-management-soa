using System;
using System.Collections.Generic;

namespace Core_Service.Models;

public partial class Medicine
{
    public long Id { get; set; }

    public long CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string BaseUnit { get; set; } = null!;

    public long SalePrice { get; set; }

    public string Status { get; set; } = null!;

    public virtual MedicineCategory Category { get; set; } = null!;

    public virtual ICollection<ImportDetail> ImportDetails { get; set; } = new List<ImportDetail>();

    public virtual ICollection<MedicineBatch> MedicineBatches { get; set; } = new List<MedicineBatch>();

    public virtual ICollection<UnitConversion> UnitConversions { get; set; } = new List<UnitConversion>();
}
