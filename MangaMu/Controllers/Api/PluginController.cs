using MangaMu.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MangaMu.Controllers.Api
{
    public class PluginController : BaseApiController
    {
        private readonly PluginRepository _repo;

        public PluginController(PluginRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetPlugins()
        {
            return Ok(_repo.GetPlugins());
        }

        [HttpPost]
        public IActionResult UpdateDatabase(string id)
        {
            _repo.UpdateDatabase(id);
            return Ok();
        }
    }
}
