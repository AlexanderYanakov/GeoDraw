namespace GeoDraw.DTO
{
    public class FigureDto
    {
        public List<Coordinates> MarkerList { get; set; }
        public List<List<Coordinates>> LineList { get; set; }
        public List<List<Coordinates>> RectangleList { get; set; }
        public List<List<Coordinates>> PolygonList { get; set; }
    }

    public class Coordinates
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
    public enum FigureType
    {
        RECTANGLE,
        POLYGON
    }
}
