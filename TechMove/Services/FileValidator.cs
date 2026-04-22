﻿using Microsoft.AspNetCore.Http;

namespace TechMove.Services
{
    // Interface for file validation rules
    public interface IFileValidator
    {
        bool IsPdf(IFormFile? file);
    }

    // Implementation that strictly checks for PDF extensions
    public class FileValidator : IFileValidator
    {
        // Checks if the uploaded file is a valid PDF
        public bool IsPdf(IFormFile? file)
        {
            // If no file was uploaded, it's not a valid PDF
            if (file == null) return false;

            var fileName = file.FileName;
            if (string.IsNullOrWhiteSpace(fileName)) return false;

            // Only allow the .pdf extension
            var extension = Path.GetExtension(fileName).ToLower();
            return extension == ".pdf";
        }
    }
}
