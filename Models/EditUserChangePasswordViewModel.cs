namespace Models;

public class EditUserChangePasswordViewModel
{
    public EditUserViewModel editUserViewModel { get; set; } = new EditUserViewModel();
    public ChangePasswordViewModel changePasswordViewModel { get; set; } = new ChangePasswordViewModel();
}