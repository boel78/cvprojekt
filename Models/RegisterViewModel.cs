using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vänligen skriv ett användarnamn.")]
        [StringLength(200)]

        public string UserName { get; set; }

        [Required(ErrorMessage = "Vänligen skriv ett lösenord.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vänligen skriv ett lösenord.")]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta lösenordet")]
        public string ConfirmPassword { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "E-post är obligatoriskt.")]
        [EmailAddress(ErrorMessage = "Ange en giltig e-postadress.")]
        public string Email { get; set; }

    }
}
