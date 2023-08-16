using Domain.Base;
using Domain.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Figure : PersistentObject
    {

        public FigureType Type { get; set; }
    }
}
