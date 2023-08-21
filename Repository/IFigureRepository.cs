using GeoDraw.DTO;

namespace Repository
{
    public interface IFigureRepository
    {
        Task CreateMarker(List<Coordinates> markerList);
        Task CreateLine(List<List<Coordinates>> lineList);
        Task CreatePolygon(List<List<Coordinates>> RectangleList, FigureType type);
        Task<string> CheckFigure(Coordinates coordinates);
    }
}
