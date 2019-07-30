using PGK_Center.DAL;
using PGK_Center.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PGK_Center.BLL
{
    public static class CommonData
    {
        public static readonly Regex DecimalRegex = new Regex(@"^[0-9]*(?:\.[0-9]*)?$");

        public static readonly Regex IntegerRegex = new Regex(@"d+");

        public static List<Tariff> Tariffs { get; } = DBManager.GetAllTariffs();

        public static List<ElectricityTariff> ElectricityTariffs { get; }
            = DBManager.GetAllElectricityTariffs();
    }
}
