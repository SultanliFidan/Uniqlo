using System.ComponentModel.DataAnnotations;

namespace Uniqlol.ViewModels.Auths
{
    public class NewPasswordVM
    {
        [Required]
        public string Token { get; set; }
        
        [Required, MaxLength(128), EmailAddress]
        public string Email { get; set; }
        [Required, MaxLength(32), DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required, MaxLength(32), DataType(DataType.Password),Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}
