using GeoDraw.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository;

namespace GeoDraw.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFigureRepository _figureRepository;

        public HomeController(IFigureRepository figureRepository)
        {
            _figureRepository = figureRepository;
        }
        // POST: HomeController/Create
        [HttpPost]
        public ActionResult CreateFigure([FromBody] object figureData)
        {
            try
            {
                // Обработайте figureData (например, сохраните его в базе данных)

                return Ok(new { message = "Figures were successfully created" });
            }
            catch
            {
                return BadRequest(new { message = "Failed to create figures" });
            }
        }
    }
}
