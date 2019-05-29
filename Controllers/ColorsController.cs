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
using bitstitches_api.Services;

namespace bitstitches_api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ColorsController : ControllerBase
  {
    [HttpGet]
    public ActionResult<object> Get()
    {
      /* Console.WriteLine(DMCFlossColorsService.DMCFlossColors.Count); */
      string dmcText = System.IO.File.ReadAllText("lib/constants/DMCFlossColors.json");
      Console.WriteLine(DMCFlossColorsService.DMCFlossColors["666"]);
      Console.WriteLine(DMCFlossColorsService.DMCFlossColors["666"].RGB);
      Console.WriteLine(DMCFlossColorsService.DMCFlossColors["666"].ID);
      Console.WriteLine(DMCFlossColorsService.DMCFlossColors["666"].Name);
      Console.ReadLine();
      return JsonConvert.DeserializeObject(DMCFlossColorsService.DMCFlossColors["666"].Name);
    }
  }
}
