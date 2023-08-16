using Domain.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public interface IFegureRepository
    {
        void CreateMarker(long lat, long lng);
    }
}
