namespace Repository;

public class FigureRepository : IFigureRepository
{
    private string _connectionString = "Server=localhost;Port=5432;Database=FigureDb;UserId=postgres;Password=89187786606Alex";

    public void CreateMarker()
    {
        //INSERT INTO your_table_name(geom, name)
        //VALUES(
        //ST_SetSRID(ST_MakePoint(12.34, 56.78), 4326),
        //'Маркер A'
        //)
    }
    public void CreateLine()
    {
        //INSERT INTO your_table_name(geom, name)
        //VALUES(
        //ST_SetSRID(ST_MakeLine(
        //ST_MakePoint(12.34, 56.78),
        //ST_MakePoint(23.45, 67.89)
        //), 4326),
        //'Line A'
        //);
    }

    public void CreatePolygon()
    {
        //INSERT INTO your_table_name(geom, name)
        //VALUES(
        //ST_SetSRID(ST_GeomFromText('POLYGON((12.34 56.78, 23.45 56.78, 23.45 67.89, 12.34 67.89, 12.34 56.78))'), 4326),
        //'Полигон A'
        //);
    }

    public void CreateRectangle()
    {
        //INSERT INTO rectangles(geom, name)
        //VALUES(
        //ST_SetSRID(ST_MakePolygon(
        //ST_GeomFromText('LINESTRING(12.34 56.78, 23.45 56.78, 23.45 67.89, 12.34 67.89, 12.34 56.78)')
        //), 4326),
        //'Прямоугольник A'
        //);
    }
}

