using GLMS2.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GLMS2.Services
{
    // Handles file validation and storage for contract agreements
    public class FileService : IFileService
    {
        // Checks whether the uploaded file is a valid PDF
        public bool IsPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName);

            return !string.IsNullOrWhiteSpace(extension) &&
                   extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        // Saves the PDF file to the server and returns its relative path
        public async Task<string> SavePdfAsync(IFormFile file, string rootPath)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }
            // Ensures only PDF files are uploaded
            if (!IsPdf(file))
            {
                throw new InvalidOperationException("Only PDF files are allowed.");
            }
            // Creates uploads folder if it does not exist
            var uploadsFolder = Path.Combine(rootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            // Generates unique filename to prevent duplicates
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);
            // Saves file to the server
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // Returns relative path used for downloading later
            return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
        }
    }
}