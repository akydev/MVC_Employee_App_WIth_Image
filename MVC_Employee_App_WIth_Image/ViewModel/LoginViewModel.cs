using System.ComponentModel.DataAnnotations;

namespace MVC_Employee_App_WIth_Image.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
