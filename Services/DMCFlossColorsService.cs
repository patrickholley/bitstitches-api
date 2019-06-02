using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using bitstitches_api.Models;

namespace bitstitches_api.Services
{
  public static class DMCFlossColorsService
  {
    static DMCFlossColorsService()
    {
      ClosestDMCFlossColorsCache = new Dictionary<Color, DMCFlossColor>();

      Dictionary<string, DMCFlossColor> ThisDictionary = new Dictionary<string, DMCFlossColor>();
      string dmcText = System.IO.File.ReadAllText("lib/constants/DMCFlossColors.json");
      DMCFlossColor[] DMCFlossColorsArray = JsonConvert.DeserializeObject<DMCFlossColor[]>(dmcText);

      foreach (DMCFlossColor DMCFlossColor in DMCFlossColorsArray)
      {
        ThisDictionary.Add(DMCFlossColor.ID, DMCFlossColor);
      }
      
      DMCFlossColors = ThisDictionary;
    }

    public static DMCFlossColor FindClosestDMCFlossColor(Color inColor)
    {
      if (!ClosestDMCFlossColorsCache.ContainsKey(inColor))
      {

        double? closestDistance = null;
        string closestIndex = null;

        foreach(KeyValuePair<string, DMCFlossColor> entry in DMCFlossColors)
        {
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

        ClosestDMCFlossColorsCache[inColor] = DMCFlossColors[closestIndex];
      }

      return ClosestDMCFlossColorsCache[inColor];
    }

    public static Dictionary<Color, DMCFlossColor> ClosestDMCFlossColorsCache { get; set; }
    public static Dictionary<string, DMCFlossColor> DMCFlossColors { get; }
  }
}