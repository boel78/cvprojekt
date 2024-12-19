using System;
using System.Collections.Generic;

namespace cvprojekt.Models;

public partial class Cv
{
    public int Cvid { get; set; }

    public string Description { get; set; } = null!;

    public string? Owner { get; set; }

    public virtual CvView? CvView { get; set; }

    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    public virtual User? OwnerNavigation { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
