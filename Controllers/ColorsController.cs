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
      string dmcText = System.IO.File.ReadAllText("lib/constants/DMCFlossColors.json");
      return JsonConvert.SerializeObject(DMCFlossColorsService.DMCFlossColors);
    }
  }
}
