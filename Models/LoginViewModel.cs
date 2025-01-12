using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vänligen fyll i ett användarnamn.")]
        [StringLength(200)]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Vänligen fyll i ett lösenord.")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
