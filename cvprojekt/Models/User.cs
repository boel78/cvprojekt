﻿using System;
using System.Collections.Generic;

namespace cvprojekt.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsPrivate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? ProfilePicture { get; set; }

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectsNavigation { get; set; } = new List<Project>();
}
