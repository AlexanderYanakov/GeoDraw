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
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
    public enum FigureType
    {
        RECTANGLE,
        POLYGON
    }
}
