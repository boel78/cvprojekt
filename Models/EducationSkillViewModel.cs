using System.ComponentModel.DataAnnotations;

namespace Models;

public class EducationSkillViewModel
{
    public int Eid { get; set; }
    [Required(ErrorMessage = "Lägg till en titel.")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Beskrivningen får inte vara tom.")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Lägg till minst en kompetens.")]
    public string Skills { get; set; }
}

