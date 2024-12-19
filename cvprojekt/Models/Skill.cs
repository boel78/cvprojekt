using System;
using System.Collections.Generic;

namespace cvprojekt.Models;

public partial class Skill
{
    public int Sid { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Education> Eids { get; set; } = new List<Education>();
}
