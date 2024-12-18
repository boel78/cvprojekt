using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace cvprojekt.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CreatedBy { get; set; }

    [ForeignKey(nameof(CreatedBy))]
    public virtual User User { get; set; }

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
