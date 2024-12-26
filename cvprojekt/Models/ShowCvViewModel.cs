namespace cvprojekt.Models;

public class ShowCvViewModel
{
    public User User { get; set; }
    public IQueryable<User> UsersMatch { get; set; }
    public int ViewCount { get; set; }
    public bool IsWriter { get; set; }
}