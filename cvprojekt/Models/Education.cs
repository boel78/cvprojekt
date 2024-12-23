﻿using System;
using System.Collections.Generic;

namespace cvprojekt.Models;

public partial class Education
{
    public int Eid { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Cvid { get; set; }

    public virtual Cv Cv { get; set; } = null!;

    public virtual ICollection<Skill> Sids { get; set; } = new List<Skill>();
}
