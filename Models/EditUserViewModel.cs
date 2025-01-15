using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EditUserViewModel
    {

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
