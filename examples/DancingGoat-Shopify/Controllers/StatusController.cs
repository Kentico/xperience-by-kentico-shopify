using Microsoft.AspNetCore.Mvc;

namespace DancingGoat.Controllers
{
    /// <summary>
    /// This controller indicates wether the application is running or not.
    /// </summary>
    /// 
    [Route("[controller]")]
    public class StatusController : Controller
    {
        public IActionResult Index() => Ok();
    }
}
