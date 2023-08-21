using DTO.DTO;
using GeoDraw.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Npgsql;
using Repository;

namespace GeoDraw.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFigureRepository FigureRepository;

        public HomeController(IFigureRepository figureRepository)
        {
            FigureRepository = figureRepository;
        }


        [HttpPost]
        public async Task<ActionResult> CreateFigure([FromBody] FigureDto figureData)
        {
            try
            {
                // checkfigureData
                if (figureData.MarkerList.Count() != 0)
                {
                    await FigureRepository.CreateMarker(figureData.MarkerList);
                    Dislocation(figureData.MarkerList);
                    await FigureRepository.CreateMarker(figureData.MarkerList);
                }

                if (figureData.LineList.Count() != 0)
                {
                    await FigureRepository.CreateLine(figureData.LineList);
                    figureData.LineList.ForEach(x => Dislocation(x));
                    await FigureRepository.CreateLine(figureData.LineList);
                }

                if (figureData.RectangleList.Count() != 0)
                {
                    await FigureRepository.CreatePolygon(figureData.RectangleList, FigureType.RECTANGLE);
                    figureData.RectangleList.ForEach(x => Dislocation(x));
                    await FigureRepository.CreatePolygon(figureData.RectangleList, FigureType.RECTANGLE);
                }

                if (figureData.PolygonList.Count() != 0)
                {
                    await FigureRepository.CreatePolygon(figureData.PolygonList, FigureType.POLYGON);
                    figureData.PolygonList.ForEach(x => Dislocation(x));
                    await FigureRepository.CreatePolygon(figureData.PolygonList, FigureType.POLYGON);
                }

                return Ok(new { message = "Figures were successfully created" });
            }
            catch
            {
                return BadRequest(new { message = "Failed to create figures" });
            }
        }

        // setDislocationToCoordinates
        private void Dislocation(List<Coordinates> coordinates)
        {
            coordinates.ForEach(x =>
            {
                x.lng += 1;
                x.lat += 1;
            });
        }

        [HttpGet]
        public async Task<ActionResult> Check()
        {
            try
            {
                string latLngModelJson = Request.Headers["LatLng-Model"];
                Coordinates latLngModel = JsonConvert.DeserializeObject<Coordinates>(latLngModelJson);

                string responce = await FigureRepository.CheckFigure(latLngModel);

                return Ok(responce);
            }
            catch
            {
                return BadRequest(new { message = "Failed to check figure" });
            }
        }
    }
}
