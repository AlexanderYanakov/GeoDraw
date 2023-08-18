using DTO;
using GeoDraw.DTO;
using Npgsql;
using System.Data.SqlClient;

namespace Repository;

public class FigureRepository : IFigureRepository
{
    private string connectionString = "Server=localhost;Port=5432;Database=FigureDb;UserId=postgres;Password=285020";

    public async Task CreateMarker(List<Coordinates> markerList)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            foreach (Coordinates coordinates in markerList)
            {
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "INSERT INTO markers(geom, name) VALUES(ST_SetSRID(ST_MakePoint(@lon, @lat), 4326), @name)";
                    command.Parameters.AddWithValue("lon", coordinates.lng);
                    command.Parameters.AddWithValue("lat", coordinates.lat);
                    command.Parameters.AddWithValue("name", "Marker");

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
    public async Task CreateLine(List<List<Coordinates>> lineList)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            foreach (List<Coordinates> lineCoordinates in lineList)
            {
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    // Create the WKT (Well-Known Text) representation of the line
                    string wktLine = "LINESTRING(";
                    foreach (Coordinates coordinates in lineCoordinates)
                    {
                        wktLine += $"{coordinates.lng} {coordinates.lat},";
                    }
                    wktLine = wktLine.TrimEnd(',') + ")";

                    command.CommandText = "INSERT INTO lines(geom, name) VALUES(ST_SetSRID(ST_GeomFromText(@wktLine), 4326), @name)";
                    command.Parameters.AddWithValue("wktLine", wktLine);
                    command.Parameters.AddWithValue("name", "Line");

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
    public async Task CreatePolygon(List<List<Coordinates>> polygonList, FigureType type)
    {
        string keyWord = null;
        switch (type)
        {
            case FigureType.RECTANGLE:
                keyWord = "rectangles";
                break;

            case FigureType.POLYGON:
                keyWord = "polygons";
                break;
        }

        foreach (var list in polygonList)
        {
            list.Add(list.First());
        }
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            foreach (List<Coordinates> rectangleCoordinates in polygonList)
            {
                using (NpgsqlCommand command = new NpgsqlCommand())
                {
                    command.Connection = connection;

                    string crdRectangle = "'LINESTRING(";
                    foreach (Coordinates coordinates in rectangleCoordinates)
                    {
                        crdRectangle += $"{coordinates.lng} {coordinates.lat}, ";
                    }
                    int lastIndex = crdRectangle.LastIndexOf(",");
                    string coordinatesString = null;
                    if (lastIndex >= 0)
                    {
                        coordinatesString = crdRectangle.Substring(0, lastIndex) + crdRectangle.Substring(lastIndex + 1);
                    }
                    var resultString = "INSERT INTO @tableName(geom, name)VALUES(ST_SetSRID(ST_MakePolygon(ST_GeomFromText(@coordinatesString)')), 4326), 'Rectangle')";
                    var replacedString = resultString.Replace("@coordinatesString", coordinatesString);
                    var finalSQL = replacedString.Replace("@tableName", keyWord);
                    command.CommandText = finalSQL;

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}

