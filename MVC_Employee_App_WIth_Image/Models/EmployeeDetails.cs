using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_Employee_App_WIth_Image.Models
{
    public class EmployeeDetails
    {
        [Key]
        public int DetailsId { get; set; }

        [Required]
        public string Home_Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email_Address { get; set; }

        [ForeignKey("Employee")]
        public int EmpId { get; set; }

        public Employee Employee { get; set; }
    }
}
