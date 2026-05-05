using System.ComponentModel.DataAnnotations;
namespace MVC_Employee_App_WIth_Image.ViewModel
{
    public class EmployeeViewModel
    {
        public int EmpId { get; set; }

        [Required]
        public string First_Name { get; set; }

        [Required]
        public string Last_Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email_Address { get; set; }

        public string? Home_Address { get; set; }

        // DB fields
        public string? Profile_Pic { get; set; }
        public string? Address_Proof { get; set; }

        // Upload fields
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? AddressProofFile { get; set; }
    }

}
