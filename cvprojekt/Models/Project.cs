﻿using System;
using System.Collections.Generic;

namespace cvprojekt.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();
    //Kopplingen till users sambandet

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
