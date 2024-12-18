using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace cvprojekt.Models;

public partial class Cv
{
    public int Cvid { get; set; }

    public string Description { get; set; } = null!;

    public int Owner { get; set; }

    public virtual CvView? CvView { get; set; }

    [ForeignKey(nameof(Owner))]
    public virtual User User { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
