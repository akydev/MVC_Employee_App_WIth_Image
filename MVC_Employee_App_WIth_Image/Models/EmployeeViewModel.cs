using System.ComponentModel.DataAnnotations;

namespace MVC_Employee_App_WIth_Image.Models
{
    public class EmployeeViewModel
    {
        public int EmpId { get; set; }

        [Required]
        public string First_Name { get; set; }

        [Required]
        public string Last_Name { get; set; }

        public IFormFile ProfileImage { get; set; }
        public string Profile_Pic { get; set; }

        public IFormFile AddressProofFile { get; set; }
        public string Address_Proof { get; set; }

        [Required]
        public string Home_Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email_Address { get; set; }
    }
}
