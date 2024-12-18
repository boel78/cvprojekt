using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace cvprojekt.Models;

public partial class CvView
{
    public int Cvid { get; set; }

    public int ViewCount { get; set; }

    [ForeignKey(nameof(CvView))]
    public virtual Cv Cv { get; set; }
}
