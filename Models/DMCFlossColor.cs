using System;
using System.Drawing;
using Newtonsoft.Json;

namespace bitstitches_api.Models
{
  public class DMCFlossColor
  {
    public DMCFlossColor(string ID, string Name, string RGB)
    {
      this.ID = ID;
      this.Name = Name;
      this.Color = ColorTranslator.FromHtml($"#{RGB}");
    }

    [JsonProperty("ID")]
    public string ID { get; }
    [JsonProperty("Name")]
    public string Name { get; }
    [JsonProperty("RGB")]
    public Color Color { get; }
  }
}