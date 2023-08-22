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
        public string LatLng { get; set; }

        public CheckFigureDto(string name, string latLng)
        {
            Name = name;
            LatLng = latLng;
        }
    }
}
