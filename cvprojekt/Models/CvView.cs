using System;
using System.Collections.Generic;

namespace cvprojekt.Models;

public partial class CvView
{
    public int Cvid { get; set; }

    public int ViewCount { get; set; }

    public virtual Cv Cv { get; set; } = null!;
}
