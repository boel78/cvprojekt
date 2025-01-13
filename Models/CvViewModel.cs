using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CvViewModel
    {
        public int Cvid { get; set; }
        [Required(ErrorMessage = "Beskrivningen får inte vara tom.")]
        public string Description { get; set; }
        public List<EducationSkillViewModel> Educations { get; set; } = new List<EducationSkillViewModel>();
    }
}