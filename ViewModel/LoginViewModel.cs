using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel;

public class LoginViewModel
{
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
}