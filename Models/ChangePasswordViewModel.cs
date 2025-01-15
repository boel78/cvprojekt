using System.ComponentModel.DataAnnotations;

namespace Models;

public class ChangePasswordViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Nuvarande lösenord")]
    public string CurrentPassword { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Nytt lösenord")]
    public string NewPassword { get; set; } = "";

    [DataType(DataType.Password)]
    [Display(Name = "Bekräfta nytt lösenord")]
    [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
    public string ConfirmNewPassword { get; set; } = "";
}