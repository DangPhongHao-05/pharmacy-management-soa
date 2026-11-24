using System;
using System.Collections.Generic;

namespace Core_Service.Models;

public partial class MedicineCategory
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
