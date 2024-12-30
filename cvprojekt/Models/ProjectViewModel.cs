namespace cvprojekt.Models;

public class ProjectViewModel
{
    public IQueryable<Project> Projects { get; set; }
    public User User { get; set; }
}