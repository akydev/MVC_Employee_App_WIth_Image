using Microsoft.AspNetCore.Identity;

namespace MVC_Employee_App_WIth_Image.Models
{
    public class ApplicationUser : IdentityUser
    {
       
            public string FullName { get; set; } = string.Empty;
        
    }
}
