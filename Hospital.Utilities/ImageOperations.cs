using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Utilities
{
    public class ImageOperations
    {
        IWebHostEnvironment _env;

        public ImageOperations(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string ImageUpload(IFormFile file)
        {
            return SaveFile(file, "Images");
        }

        public string DiagnosticUpload(IFormFile file, string patientId)
        {
            // Secure storage path per patient for clinical documentation
            string subFolder = Path.Combine("Uploads", "Diagnostics", patientId);
            return SaveFile(file, subFolder);
        }

        private string SaveFile(IFormFile file, string subFolder)
        {
            string filename = "no-image.png";
            if (file != null)
            {
                string fileDirectory = Path.Combine(_env.WebRootPath, subFolder);
                if (!Directory.Exists(fileDirectory)) Directory.CreateDirectory(fileDirectory);
                filename = Guid.NewGuid() + "-" + file.FileName;
                string filepath = Path.Combine(fileDirectory, filename);
                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }
                // Return relative path for database storage
                return Path.Combine(subFolder, filename);
            }
            return filename;
        }
    }
}
