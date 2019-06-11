using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using bitstitches_api.Models;

namespace bitstitches_api.Services {
  public static class DMCFlossColorsService {
    static DMCFlossColorsService() {
      Dictionary<string, DMCFlossColor> ThisDictionary = new Dictionary<string, DMCFlossColor>();
      string dmcText = System.IO.File.ReadAllText("lib/constants/DMCFlossColors.json");
      DMCFlossColor[] DMCFlossColorsArray = JsonConvert.DeserializeObject<DMCFlossColor[]>(dmcText);

      foreach (DMCFlossColor DMCFlossColor in DMCFlossColorsArray)
      {
        ThisDictionary.Add(DMCFlossColor.ID, DMCFlossColor);
      }
      
      DMCFlossColors = ThisDictionary;
    }

    private static string ConvertColorToHexadecimal(Color color) {
      return $"{color.R.ToString("X2")}{color.G.ToString("X2")}{color.B.ToString("X2")}";
    }

    public static DMCFlossColor FindClosestDMCFlossColor(
      Color inColor,
      Dictionary<Color, DMCFlossColor> closestColorsCache,
      Dictionary<string, DMCFlossColor> selectedColors
    ) {
      Dictionary<string, DMCFlossColor> __selectedColors = selectedColors != null
        ? selectedColors
        : DMCFlossColors;      

      if (
        !closestColorsCache.ContainsKey(inColor)
          || !__selectedColors.ContainsKey(ConvertColorToHexadecimal(closestColorsCache[inColor].Color))
      ) {
        double? closestDistance = null;
        string closestIndex = null;

        foreach(KeyValuePair<string, DMCFlossColor> entry in __selectedColors) {
          Color entryColor = entry.Value.Color;

          double distance = Math.Sqrt(
            Math.Pow(inColor.R - entry.Value.Color.R, 2)
            + Math.Pow(inColor.G - entry.Value.Color.G, 2)
            + Math.Pow(inColor.B - entry.Value.Color.B, 2)
          );

          if (!closestDistance.HasValue || closestDistance > distance)
          {
            closestDistance = distance;
            closestIndex = entry.Key;
            if (distance == 0) break;
          }

        }

        closestColorsCache[inColor] = __selectedColors[closestIndex];
      }

      return closestColorsCache[inColor];
    }
    public static Dictionary<string, DMCFlossColor> DMCFlossColors { get; }
  }
}
