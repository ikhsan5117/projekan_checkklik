using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AMRVI.Services;

namespace AMRVI.Controllers
{
    [Authorize]
    public class InputMenuController : Controller
    {
        private readonly PlantService _plantService;

        public InputMenuController(PlantService plantService)
        {
            _plantService = plantService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Input Menu";
            ViewBag.CurrentPlant = _plantService.GetPlantName();
            return View();
        }
    }
}
