namespace Models
{
    public class IndexViewModel
    {
        public IQueryable<Cv> cvs {  get; set; }
        public IQueryable<Project> projects { get; set; }
        
        public bool isActive { get; set; }
    }
}
