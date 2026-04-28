using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Employee_App_WIth_Image.Data;
using MVC_Employee_App_WIth_Image.Models;


namespace MVC_Employee_App_WIth_Image.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ===================== INDEX (JOIN) =====================
        public IActionResult Index()
        {
            var data = (from e in _context.Employees
                        join d in _context.EmployeeDetails
                        on e.EmpId equals d.EmpId
                        select new EmployeeViewModel
                        {
                            EmpId = e.EmpId,
                            First_Name = e.First_Name,
                            Last_Name = e.Last_Name,
                            Profile_Pic = e.Profile_Pic,
                            Address_Proof = e.Address_Proof,
                            Home_Address = d.Home_Address,
                            Email_Address = d.Email_Address
                        }).ToList();

            return View(data);
        }

        // ===================== CREATE (GET) =====================
        public IActionResult Create()
        {
            return View();
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        public IActionResult Create(EmployeeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                string img = UploadFile(vm.ProfileImage, "Uploads/Images");
                string doc = UploadFile(vm.AddressProofFile, "Uploads/Docs");

                var emp = new Employee
                {
                    First_Name = vm.First_Name,
                    Last_Name = vm.Last_Name,
                    Profile_Pic = img,
                    Address_Proof = doc
                };

                _context.Employees.Add(emp);
                _context.SaveChanges();

                var details = new EmployeeDetails
                {
                    EmpId = emp.EmpId,
                    Home_Address = vm.Home_Address,
                    Email_Address = vm.Email_Address
                };

                _context.EmployeeDetails.Add(details);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(vm);
        }

        // ===================== EDIT (GET) =====================
        public IActionResult Edit(int id)
        {
            var data = (from e in _context.Employees
                        join d in _context.EmployeeDetails
                        on e.EmpId equals d.EmpId
                        where e.EmpId == id
                        select new EmployeeViewModel
                        {
                            EmpId = e.EmpId,
                            First_Name = e.First_Name,
                            Last_Name = e.Last_Name,
                            Profile_Pic = e.Profile_Pic,
                            Address_Proof = e.Address_Proof,
                            Home_Address = d.Home_Address,
                            Email_Address = d.Email_Address
                        }).FirstOrDefault();

            if (data == null)
                return NotFound();

            return View(data);
        }

        // ===================== EDIT (POST) =====================
        [HttpPost]
        public IActionResult Edit(EmployeeViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var emp = _context.Employees.Find(vm.EmpId);
                var details = _context.EmployeeDetails.FirstOrDefault(x => x.EmpId == vm.EmpId);

                if (emp == null || details == null)
                    return NotFound();

                // IMAGE UPDATE
                if (vm.ProfileImage != null)
                {
                    DeleteFile("Uploads/Images", emp.Profile_Pic);
                    emp.Profile_Pic = UploadFile(vm.ProfileImage, "Uploads/Images");
                }

                // DOC UPDATE
                if (vm.AddressProofFile != null)
                {
                    DeleteFile("Uploads/Docs", emp.Address_Proof);
                    emp.Address_Proof = UploadFile(vm.AddressProofFile, "Uploads/Docs");
                }

                // UPDATE FIELDS
                emp.First_Name = vm.First_Name;
                emp.Last_Name = vm.Last_Name;

                details.Home_Address = vm.Home_Address;
                details.Email_Address = vm.Email_Address;

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(vm);
        }

        // ===================== DELETE =====================
        public IActionResult Delete(int id)
        {
            var emp = _context.Employees.Find(id);
            var details = _context.EmployeeDetails.FirstOrDefault(x => x.EmpId == id);

            if (emp == null)
                return NotFound();

            // DELETE FILES
            DeleteFile("Uploads/Images", emp.Profile_Pic);
            DeleteFile("Uploads/Docs", emp.Address_Proof);

            if (details != null)
                _context.EmployeeDetails.Remove(details);

            _context.Employees.Remove(emp);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ===================== FILE UPLOAD =====================
        private string UploadFile(IFormFile file, string folder)
        {
            if (file == null) return null;

            string uploadPath = Path.Combine(_env.WebRootPath, folder);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

        // ===================== FILE DELETE =====================
        private void DeleteFile(string folder, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            string path = Path.Combine(_env.WebRootPath, folder, fileName);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}