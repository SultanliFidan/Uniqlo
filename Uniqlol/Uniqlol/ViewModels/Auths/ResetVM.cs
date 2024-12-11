using System.ComponentModel.DataAnnotations;

namespace Uniqlol.ViewModels.Auths
{
    public class ResetVM
    {
        [Required, MaxLength(128), EmailAddress]
        public string Email { get; set; }
    }
}
