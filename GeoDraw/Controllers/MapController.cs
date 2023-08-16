using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeoDraw.Controllers
{
    public class MapController : Controller
    {
        public IActionResult Index()
        {
            string geoserverPath = @"C:\Drive\C#Programm\geoserver-2.16.1\bin";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                WorkingDirectory = geoserverPath,
                Arguments = "/C startup.bat"
            };

            Process.Start(startInfo);

            return View();
        }
    }
}
