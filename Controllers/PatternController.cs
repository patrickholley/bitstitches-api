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
        private static int pixelSize = 5;

        private static Image convertBase64StringToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms);
            }
        }

        private static string convertImageToBase64String(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static void drawPixel(Color imagePixelColor, Bitmap convertedBitmap, int x, int y)
        {
            for (int i = 0; i < pixelSize; i++) {
                for (int j = 0; j < pixelSize; j++) {
                    Color convertedPixelColor = i == pixelSize - 1 || j == pixelSize - 1
                        ? Color.FromArgb(255, 64, 64, 64)
                        : imagePixelColor;

                    convertedBitmap.SetPixel(
                        (x * pixelSize) + i,
                        (y * pixelSize) + j,
                        convertedPixelColor
                    );
                }
            }
        }

        private static Image iterateThroughImagePixels(Image image)
        {
            Bitmap imageBitmap = new Bitmap(image);
            Bitmap convertedBitmap = new Bitmap(image.Width * pixelSize, image.Height * pixelSize);

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    drawPixel(imageBitmap.GetPixel(i, j), convertedBitmap, i, j);
                }
            }

            return convertedBitmap;
        }

        [HttpPost]
        public async Task<ActionResult<string>> GeneratePattern()
        {
            StreamReader reader = new StreamReader(Request.Body);
            string requestString = await reader.ReadToEndAsync();
            string base64String = requestString.Replace("data:image/png;base64,", "");
            Image image = convertBase64StringToImage(base64String);
            Image convertedImage = iterateThroughImagePixels(image);
            return convertImageToBase64String(convertedImage);
        }
    }
}
