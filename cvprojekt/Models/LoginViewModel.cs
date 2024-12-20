using System.ComponentModel.DataAnnotations;


namespace cvprojekt.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vänligen skriv ett användarnamn.")]
        [StringLength(200)]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Vänligen skriv ett lösenord.")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
