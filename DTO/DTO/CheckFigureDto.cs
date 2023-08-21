using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.DTO
{
    public class CheckFigureDto
    {
        public string Name { get; set; }
        public string Centroid { get; set; }

        public CheckFigureDto(string name, string centroid)
        {
            Name = name;
            Centroid = centroid;
        }
    }
}
