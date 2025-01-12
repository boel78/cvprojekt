namespace Models
{
    public class CvViewModel
    {
        public int Cvid { get; set; }
        public string Description { get; set; }
        public List<EducationSkillViewModel> Educations { get; set; } = new List<EducationSkillViewModel>();
    }
}