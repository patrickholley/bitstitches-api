using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using bitstitches_api.Models;

namespace bitstitches_api.Services
{
  public static class DMCFlossColorsService
  {
    static DMCFlossColorsService()
    {
      Dictionary<string, DMCFlossColor> ThisDictionary = new Dictionary<string, DMCFlossColor>();
      string dmcText = System.IO.File.ReadAllText("lib/constants/DMCFlossColors.json");
      DMCFlossColor[] DMCFlossColorsArray = JsonConvert.DeserializeObject<DMCFlossColor[]>(dmcText);
      Console.WriteLine(DMCFlossColorsArray);

      foreach (DMCFlossColor DMCFlossColor in DMCFlossColorsArray)
      {
        ThisDictionary.Add(DMCFlossColor.ID, DMCFlossColor);
      }
      
      DMCFlossColors = ThisDictionary;
    }

    public static Dictionary<string, DMCFlossColor> DMCFlossColors { get; }
  }
}