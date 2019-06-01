using System;
using System.Drawing;
using Newtonsoft.Json;

namespace bitstitches_api.Models
{
  public class DMCFlossColor
  {
    public DMCFlossColor(string ID, string Name, string RGB)
    {
      Console.WriteLine(ID);
      Console.WriteLine(Name);
      Console.WriteLine(RGB);
      this.ID = ID;
      this.Name = Name;
      this.RGB = ColorTranslator.FromHtml($"#{RGB}");
    }

    [JsonProperty("ID")]
    public string ID { get; }
    [JsonProperty("Name")]
    public string Name { get; }
    [JsonProperty("RGB")]
    public Color RGB { get; }
  }
}