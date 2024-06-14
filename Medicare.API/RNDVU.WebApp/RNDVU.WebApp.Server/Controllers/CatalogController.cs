using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Medicare.BLL.Interfaces;
using Medicare.BLL.DTO;

namespace Medicare.WebApp.Server.Controllers
{

    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogManager _catalogManager;

        public CatalogController(ICatalogManager catalogManager)
        {
            _catalogManager = catalogManager;
        }

        public IActionResult GetInfo()
        {
            var specializations = _catalogManager.GetSpecializations();

            var specializationsDto = new SpecializationsDTO
            {
                Specializations = specializations
            };

            return Ok(specializationsDto);
        }
    }
}
