using Microsoft.AspNetCore.Mvc;

namespace MangaMu.Controllers.Api
{
    [ApiController, Route("Api/[controller]/[action]/{id?}")]
    public class BaseApiController : ControllerBase
    {
    }
}
