using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EditUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande lösenord")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nytt lösenord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta nytt lösenord")]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte.")]
        public string ConfirmNewPassword { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ]*$")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(256)]
        public string Email { get; set; }

        public bool IsActive { get; set; }

        public bool IsPrivate {  get; set; }


    }
}
