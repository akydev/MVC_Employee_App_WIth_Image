# Employee Management System (ASP.NET MVC)

This is a simple Employee Management System built using ASP.NET Core MVC with Entity Framework Core.  
It allows users to manage employee records with image and document uploads.

---

## Features

- Add new employee
- Edit employee details
- Delete employee
- Upload profile picture
- Upload address proof document
- View employee list
- Search and filter employees
- Export employee list to PDF
- Authentication (Login/Register using ASP.NET Identity)

---

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- Bootstrap 5
- jQuery DataTables
- jsPDF (for PDF export)

---

## Project Structure

- Controllers - Application logic
- Models - Database entities
- ViewModels - UI data handling
- Views - UI pages (Razor views)
- Data - DbContext and database configuration
- wwwroot - Static files (images, uploads, css, js)

---

## File Uploads

- Profile Images are stored in: wwwroot/Uploads/Images
- Documents are stored in: wwwroot/Uploads/Docs

---

## How to Run the Project

1. Clone or download the project
2. Open in Visual Studio
3. Update connection string in appsettings.json
4. Run migrations if needed
5. Run the project using IIS Express or Kestrel

---

## Notes

- Only authenticated users can access employee management
- File size limit: 5 MB
- Allowed formats: JPG, PNG, PDF, DOC, DOCX

---

## Author

Developed for learning ASP.NET MVC and full-stack web development practice.
---

**Version**: 1.0  
**Last Updated**: 2026-05-05 
**Author**: Development Team
