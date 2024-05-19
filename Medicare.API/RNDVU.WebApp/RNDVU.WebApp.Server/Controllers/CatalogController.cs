using System.Security.Claims;
using Medicare.WebApp.Server.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicare.WebApp.Server.Controllers
{

    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class CatalogController : ControllerBase
    {
        private MedicareContext _context;

        public CatalogController(MedicareContext mediator)
        {
            _context = mediator;
        }

        public async Task<IActionResult> GetInfo()
        {
            var specializations = _context.Specializations.ToList();

            return Ok(new
            {
                specializations 
            });
        }

    }
}
