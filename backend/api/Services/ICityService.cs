using System.Collections.Generic;
using backend.Models;

namespace backend.Services
{
    public interface ICityService
    {
        City? GetCityFromZip(string zip);
        IEnumerable<string> GetZipsFromCity(string city);
    }
}
