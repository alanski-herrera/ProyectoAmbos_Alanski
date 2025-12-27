using Microsoft.AspNetCore.Mvc;

namespace ProyectoAmbos_Alanski.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
