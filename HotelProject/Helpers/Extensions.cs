using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HotelProject.Helpers
{
    public static class Extensions
    {
        public static bool IsImage(this IFormFile file)
        {
            return file.ContentType.Contains("image/");
        }
        public static bool IsOlderTwoMb(this IFormFile file)
        {
            return file.Length > 2 * 1024 * 1024;
        }
        public static async Task<string> SaveFileAsync(this IFormFile file, string folder)//path-Sekilin harada yerlesdiyidir
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string fullPath = Path.Combine(folder, fileName);
            using(FileStream fileStream =new FileStream(fullPath, FileMode.Create))
            {
               await file.CopyToAsync(fileStream);
            }
            return fileName;
        }
    }
}
