using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Employee_App_WIth_Image.Data;
using MVC_Employee_App_WIth_Image.Models;
using MVC_Employee_App_WIth_Image.ViewModel;

namespace MVC_Employee_App_WIth_Image.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ===================== INDEX =====================
        public async Task<IActionResult> Index()
        {
            var data = await (from e in _context.Employees
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
                              }).ToListAsync();

            return View(data);
        }

        // ===================== CREATE (GET) =====================
        public IActionResult Create()
        {
            return View();
        }

        // ===================== CREATE (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                string? img = await UploadFileAsync(vm.ProfileImage, "Uploads/Images");
                string? doc = await UploadFileAsync(vm.AddressProofFile, "Uploads/Docs");

                var emp = new Employee
                {
                    First_Name = vm.First_Name,
                    Last_Name = vm.Last_Name,
                    Profile_Pic = img,
                    Address_Proof = doc
                };

                await _context.Employees.AddAsync(emp);

                var details = new EmployeeDetails
                {
                    Employee = emp,
                    Home_Address = vm.Home_Address,
                    Email_Address = vm.Email_Address
                };

                await _context.EmployeeDetails.AddAsync(details);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Employee added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        // ===================== EDIT (GET) =====================
        public async Task<IActionResult> Edit(int id)
        {
            var data = await (from e in _context.Employees
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
                              }).FirstOrDefaultAsync();

            if (data == null)
                return NotFound();

            return View(data);
        }

        // ===================== EDIT (POST) =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var emp = await _context.Employees.FindAsync(vm.EmpId);
            var details = await _context.EmployeeDetails
                .FirstOrDefaultAsync(x => x.EmpId == vm.EmpId);

            if (emp == null || details == null)
                return NotFound();

            try
            {
                // IMAGE UPDATE
                if (vm.ProfileImage != null)
                {
                    DeleteFile("Uploads/Images", emp.Profile_Pic);
                    emp.Profile_Pic = await UploadFileAsync(vm.ProfileImage, "Uploads/Images");
                }

                // DOC UPDATE
                if (vm.AddressProofFile != null)
                {
                    DeleteFile("Uploads/Docs", emp.Address_Proof);
                    emp.Address_Proof = await UploadFileAsync(vm.AddressProofFile, "Uploads/Docs");
                }

                // UPDATE FIELDS
                emp.First_Name = vm.First_Name;
                emp.Last_Name = vm.Last_Name;

                details.Home_Address = vm.Home_Address;
                details.Email_Address = vm.Email_Address;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Employee updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(vm);
            }
        }

        // ===================== DELETE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var emp = await _context.Employees.FindAsync(id);

            if (emp == null)
                return NotFound();

            try
            {
                DeleteFile("Uploads/Images", emp.Profile_Pic);
                DeleteFile("Uploads/Docs", emp.Address_Proof);

                _context.Employees.Remove(emp);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Employee deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // ===================== FILE UPLOAD =====================
        private async Task<string?> UploadFileAsync(IFormFile? file, string folder)
        {
            if (file == null) return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
                throw new Exception("Invalid file type");

            if (file.Length > 5 * 1024 * 1024)
                throw new Exception("File size exceeds 5MB");

            string uploadPath = Path.Combine(_env.WebRootPath, folder);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string fileName = Guid.NewGuid() + ext;
            string filePath = Path.Combine(uploadPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        // ===================== FILE DELETE =====================
        private void DeleteFile(string folder, string? fileName)
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