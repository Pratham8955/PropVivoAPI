using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PropVivoAPI.Models;

namespace PropVivoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperiorController : ControllerBase
    {
        private readonly PropvivoContext _propvivoContext;
        public SuperiorController(PropvivoContext propvivoContext)
        {
            _propvivoContext = propvivoContext;
        }


    }
}
