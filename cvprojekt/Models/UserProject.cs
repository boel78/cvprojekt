namespace cvprojekt.Models;

public class UserProject
{
    public string UserID { get; set; }
    public int ProjectID { get; set; }
    public virtual User User { get; set; }
    public virtual Project Project { get; set; }
}