using System.ComponentModel.DataAnnotations;

namespace IT.WebHost.App.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email / Username")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}