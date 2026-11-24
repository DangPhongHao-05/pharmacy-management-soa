using System;
using System.Collections.Generic;

namespace Core_Service.Models;

public partial class UnitConversion
{
    public long Id { get; set; }

    public long MedicineId { get; set; }

    public string UnitName { get; set; } = null!;

    public int Factor { get; set; }

    public virtual Medicine Medicine { get; set; } = null!;
}
