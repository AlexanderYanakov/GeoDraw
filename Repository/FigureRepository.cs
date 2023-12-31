﻿using GeoDraw.DTO;
using Npgsql;
using NetTopologySuite;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Geometries;
using Coordinates = GeoDraw.DTO.Coordinates;
using DTO.DTO;
using System.Text.Json;

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
                    command.Parameters.AddWithValue("lon", coordinates.Lng);
                    command.Parameters.AddWithValue("lat", coordinates.Lat);
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
                        wktLine += $"{coordinates.Lng} {coordinates.Lat},";
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
                        crdRectangle += $"{coordinates.Lng} {coordinates.Lat}, ";
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

    public async Task<string> CheckFigure(Coordinates coordinates)
    {

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {

            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand())
            {
                command.Connection = connection;

                string sql =
                    "SELECT 'rectangles' AS name, ST_AsText(geom) " +
                    "FROM rectangles " +
                    "WHERE ST_DWithin(geom::geography, ST_SetSRID(ST_MakePoint(@lng, @lat), 4326)::geography, 1) " +
                    "UNION ALL " +
                    "SELECT 'polygons' AS name, ST_AsText(geom) " +
                    "FROM polygons " +
                    "WHERE ST_DWithin(geom::geography, ST_SetSRID(ST_MakePoint(@lng, @lat), 4326)::geography, 1)" +
                    "UNION ALL " +
                    "SELECT 'markers' AS name, ST_AsText(geom) " +
                    "FROM markers " +
                    "WHERE ST_DWithin(geom::geography, ST_SetSRID(ST_MakePoint(@lng, @lat), 4326)::geography, 1)" +
                    "UNION ALL " +
                    "SELECT 'lines' AS name, ST_AsText(geom) " +
                    "FROM lines " +
                    "WHERE ST_DWithin(geom::geography, ST_SetSRID(ST_MakePoint(@lng, @lat), 4326)::geography, 1)";

                string replacedLng = sql.Replace("@lng", coordinates.Lng.ToString());
                string replacedLat = replacedLng.Replace("@lat", coordinates.Lat.ToString());
                command.CommandText = replacedLat;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    List<CheckFigureDto> listCheckFigure = new List<CheckFigureDto>();

                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string geom = reader.GetString(1);
                        var geometry = GeometryFromWKT(geom);

                        listCheckFigure.Add(new CheckFigureDto(name, geometry.ToString()));
                    }

                    if (listCheckFigure.Count == 0)
                    {
                        return "Object not found";
                    }

                    string responce = JsonSerializer.Serialize(listCheckFigure);
                    return responce;
                }
            }
        }
    }
    static Geometry GeometryFromWKT(string wkt)
    {
        var reader = new NetTopologySuite.IO.WKTReader();
        return reader.Read(wkt);
    }
}

