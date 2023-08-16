using Domain.Base;
using Domain;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Repository;

public class FegureRepository : IFegureRepository
{
    private string _connectionString = "Server=localhost;Port=5432;Database=FigureDb;UserId=postgres;Password=89187786606Alex";

    //public virtual IQueryable<T> GetAll()
    //{
    //    IList<T> metrics = new List<T>();
    //    var tableName = GetTableName();
    //    var sql = $"SELECT * FROM public.{tableName} ORDER BY id ASC";

    //    using (var connection = new NpgsqlConnection(_connectionString))
    //    {
    //        try
    //        {
    //            DbCommand command = (DbCommand)connection.CreateCommand();
    //            command.CommandText = sql;
    //            command.CommandType = CommandType.Text;

    //            connection.Open();

    //            DbDataReader reader = command.ExecuteReader();
    //            while (reader.Read())
    //            {
    //                metrics.Add(Map(reader)); // Maps data from the reader to an entity using the Map method.
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine("Exception.Message: {0}", ex.Message);
    //        }
    //    }

    //    return metrics.AsQueryable(); // Returns the list of entities as an IQueryable.
    //}

    public void CreateMarker(long lat, long lng)
    {
        var a = 4;
        throw new NotImplementedException();
    }
}

