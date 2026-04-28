using System.ComponentModel.DataAnnotations;

namespace MVC_Employee_App_WIth_Image.Models
{
    public class Employee
    {
        [Key]
        public int EmpId { get; set; }

        [Required]
        public string First_Name { get; set; }

        [Required]
        public string Last_Name { get; set; }

        public string Profile_Pic { get; set; }

        public string Address_Proof { get; set; }

        public EmployeeDetails EmployeeDetails { get; set; }
    }
}
