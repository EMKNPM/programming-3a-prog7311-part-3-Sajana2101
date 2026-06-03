using Microsoft.AspNetCore.Http;

namespace GLMS2.Interfaces
{
    // Handles file upload logic for contract agreements
    // Ensures only PDF files are saved to the server
    public interface IFileService
    {
        // Saves uploaded PDF file and returns the file path
        Task<string> SavePdfAsync(IFormFile file, string rootPath);

        // Checks that the uploaded file is a PDF
        bool IsPdf(IFormFile file);
    }
}