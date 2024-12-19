using System;
using System.Collections.Generic;

namespace cvprojekt.Models;

public partial class Cv
{
    public int Cvid { get; set; }

    public string Description { get; set; } = null!;

    public int Owner { get; set; }

    public virtual CvView? CvView { get; set; }

    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    public virtual User OwnerNavigation { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
