using System;
using System.IO;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace bitstitches_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatternController : ControllerBase
    {
        private Image convertBase64StringToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms);
            }
        }

        private string convertImageToBase64String(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> GeneratePattern()
        {
            StreamReader reader = new StreamReader(Request.Body);
            string requestString = await reader.ReadToEndAsync();
            string base64String = requestString.Replace("data:image/png;base64,", "");
            Console.WriteLine(base64String);
            Image image = convertBase64StringToImage(base64String);
            string result = convertImageToBase64String(image);

            return result;
        }
    }
}
