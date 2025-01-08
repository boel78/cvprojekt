namespace Models;

public class ShowCvViewModel
{
    public User User { get; set; }
    public IQueryable<User> UsersMatch { get; set; }
    public List<Project> Projects { get; set; }
    public int ViewCount { get; set; }
    public bool IsWriter { get; set; }
    public bool isLoggedIn { get; set; } = false;
}