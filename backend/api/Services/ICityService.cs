using System;
using System.Collections.Generic;
using System.Linq;
using backend.Models;

namespace backend.Services
{
    public interface ICityService
    {
        City? GetCityFromZip(string zip);
        IEnumerable<string> GetZipsFromCity(string city);

        public static void ValidateZip(string zip)
        {
            if (!zip.All(char.IsDigit))
                throw new ArgumentException($"'{zip}' is not numeric", nameof(zip));
            if (zip.Length != 5)
                throw new ArgumentException($"'{zip}' with length '{zip.Length}') is invalid", nameof(zip));
        }
    }
}
