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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using bitstitches_api.Models;
using bitstitches_api.Services;

namespace bitstitches_api.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class PatternController : ControllerBase {
    private static int PixelSize = 5;

    private static Image ConvertBase64StringToImage(string base64String) {
      byte[] imageBytes = Convert.FromBase64String(base64String);

      using (MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length)) {
        ms.Write(imageBytes, 0, imageBytes.Length);
        return Image.FromStream(ms);
      }
    }

    private static string ConvertImageToBase64String(Image image) {
      using (MemoryStream ms = new MemoryStream()) {
        image.Save(ms, image.RawFormat);
        return Convert.ToBase64String(ms.ToArray());
      }
    }

    private static void DrawPixel(Color imagePixelColor, Bitmap convertedBitmap, int x, int y) {
      for (int i = 0; i < PixelSize; i++) {
        for (int j = 0; j < PixelSize; j++) {
          Color convertedPixelColor = i == PixelSize - 1 || j == PixelSize - 1
            ? Color.FromArgb(255, 63, 63, 63)
            : imagePixelColor;

          convertedBitmap.SetPixel(
            (x * PixelSize) + i,
            (y * PixelSize) + j,
            convertedPixelColor
          );
        }
      }
    }

    private static Image DrawConvertedImagePixels(
      Image image,
      Dictionary<string, DMCFlossColor> selectedColors,
      Dictionary<Color, DMCFlossColor> closestColorsCache
    ) {
      Bitmap imageBitmap = new Bitmap(image);
      Bitmap convertedBitmap = new Bitmap(image.Width * PixelSize, image.Height * PixelSize);

      for (int i = 0; i < image.Width; i++) {
        for (int j = 0; j < image.Height; j++) {
          Color pixelColor = DMCFlossColorsService
            .FindClosestDMCFlossColor(
              imageBitmap.GetPixel(i, j),
              closestColorsCache,
              selectedColors
            ).Color;

          DrawPixel(pixelColor, convertedBitmap, i, j);
        }
      }

      return convertedBitmap;
    }

    private static Dictionary<string, DMCFlossColor> GetSelectedColors(JToken requestSelectedColors) {
      Dictionary<string, DMCFlossColor> selectedColors = new Dictionary<string, DMCFlossColor>();
      string[] selectedColorsIDs = requestSelectedColors.ToObject<string[]>();

      foreach(string id in selectedColorsIDs) {
        selectedColors.Add(id, DMCFlossColorsService.DMCFlossColors[id]);
      }

      return selectedColors;
    }

    private static string ConvertColorToHexadecimal(Color color) {
      return $"{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
    }

    private static Dictionary<string, DMCFlossColor> GetTopColors(
      Image image,
      ushort colorsCount,
      Dictionary<Color, DMCFlossColor> closestColorsCache,
      Dictionary<string, DMCFlossColor> selectedColors
    ) {
      Dictionary <string, int> colorsCounts = new Dictionary<string, int>();
      Bitmap imageBitmap = new Bitmap(image);

      for (int i = 0; i < image.Width; i++) {
        for (int j = 0; j < image.Height; j++) {
          Color color = DMCFlossColorsService
            .FindClosestDMCFlossColor(
              imageBitmap.GetPixel(i, j),
              closestColorsCache,
              selectedColors
            ).Color;

          string colorString = ConvertColorToHexadecimal(color);

          if (colorsCounts.ContainsKey(colorString)) {
            colorsCounts[colorString] = colorsCounts[colorString] + 1;
          } else {
            colorsCounts.Add(colorString, 1);
          }
        }
      }

      return (from entry in colorsCounts orderby entry.Value descending select entry.Key)
        .Take(colorsCount)
        .ToDictionary(
          key => key,
          key => DMCFlossColorsService.DMCFlossColors
            .First(entry => ConvertColorToHexadecimal(entry.Value.Color) == key)
            .Value
        );
    }

    [HttpPost]
    public async Task<ActionResult<string>> GeneratePattern() {
      JObject requestJson = (JObject) JsonConvert.DeserializeObject(
        await new StreamReader(Request.Body).ReadToEndAsync()
      );

      Image image = ConvertBase64StringToImage(
        requestJson["imageString"]
          .ToString().Replace("data:image/png;base64,", "")
      );
      ushort colorCount = ushort.Parse(requestJson["colorCount"].ToString());
      Dictionary<string, DMCFlossColor> selectedColors = GetSelectedColors(requestJson["selectedColors"]);

      Dictionary<Color, DMCFlossColor> closestColorsCache = new Dictionary<Color, DMCFlossColor>();
      Dictionary<string, DMCFlossColor> topColors = GetTopColors(image, colorCount, closestColorsCache, selectedColors);
      Image convertedImage = DrawConvertedImagePixels(image, topColors, closestColorsCache);
      
      return JsonConvert.SerializeObject(new
        {
          image = ConvertImageToBase64String(convertedImage),
          usedColors = topColors.Values.ToArray()
        }
      );
    }
  }
}
