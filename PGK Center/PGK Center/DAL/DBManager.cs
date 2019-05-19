using PGK_Center.BLL;
using PGK_Center.ObjectModel;
using PGK_Center.Properties;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace PGK_Center.DAL
{
    public static class DBManager
    {
        private static readonly string _connectionString = Settings.Default.ConnectionString;

        public static Garage[] GetAllGarages()
        {
            using (var dc = new DataContext(_connectionString))
            {
                var garages = dc.GetTable<Garage>()
                    .Where(a => !a.IsDeleted)
                    .ToArray();

                var withPays = garages
                    .GroupJoin(dc.GetTable<Pay>().Where(a => !a.IsDeleted),
                               g => g.ID,
                               p => p.GarageID,
                               (g, p) => g.WithPays(p));

                return withPays
                    .GroupJoin(dc.GetTable<Phone>()
                                 .Where(a => !a.IsDeleted)
                                 .OrderBy(a => a.ID),
                               g => g.ID,
                               p => p.GarageID,
                               (g, p) => g.WithPhones(p))
                    .ToArray();
            }
        }

        public static void SaveGarage(Garage garage)
        {
            if (garage == null)
                return;

            using (var dc = new DataContext(_connectionString))
            {
                var table = dc.GetTable<Garage>();

                var oldGarage = table.FirstOrDefault(a => a.ID == garage.ID);
                if (oldGarage == null)
                    table.InsertOnSubmit(garage);
                else
                    garage.CopyTo(oldGarage);

                dc.SubmitChanges();
            }
        }

        public static void SaveGaragePhones(int garageID, IEnumerable<Phone> phones)
        {
            using (var dc = new DataContext(_connectionString))
            {
                var table = dc.GetTable<Phone>();

                foreach (var phone in table.Where(a => a.GarageID == garageID))
                    phone.IsDeleted = true;

                table.InsertAllOnSubmit(phones);
                dc.SubmitChanges();
            }
        }

        public static void SaveGaragePays(int garageID, IEnumerable<Pay> pays)
        {
            using (var dc = new DataContext(_connectionString))
            {
                var table = dc.GetTable<Pay>();

                foreach (var pay in table.Where(a => a.GarageID == garageID))
                    pay.IsDeleted = true;

                table.InsertAllOnSubmit(pays);
                dc.SubmitChanges();
            }
        }

        public static void DeleteGarage(int id)
        {
            using (var dc = new DataContext(_connectionString))
            {
                var table = dc.GetTable<Garage>();

                var oldGarage = table.FirstOrDefault(a => a.ID == id);
                if (oldGarage != null)
                    oldGarage.IsDeleted = true;

                dc.SubmitChanges();
            }
        }

        public static void DeleteTariff(int id)
        {
            using (var dc = new DataContext(_connectionString))
            {
                var table = dc.GetTable<Tariff>();

                var oldTariff = table.FirstOrDefault(a => a.ID == id);
                if (oldTariff != null)
                    oldTariff.IsDeleted = true;

                dc.SubmitChanges();
            }
        }

        public static bool IsSameGarageExists(Garage garage)
        {
            using (var dc = new DataContext(_connectionString))
                return dc.GetTable<Garage>()
                    .Any(a => a.ID != garage.ID && a.Number == garage.Number);
        }

        public static List<Tariff> GetAllTariffs()
        {
            using (var dc = new DataContext(_connectionString))
                return dc.GetTable<Tariff>()
                    .Where(a => !a.IsDeleted)
                    .ToList();
        }

        public static void SaveTariff(Tariff tariff)
        {
            if (tariff == null)
                return;

            using (var dc = new DataContext(_connectionString))
            {
                var table = dc.GetTable<Tariff>();

                var oldTariff = table.FirstOrDefault(a => a.ID == tariff.ID);
                if (oldTariff == null)
                    table.InsertOnSubmit(tariff);
                else
                    tariff.CopyTo(oldTariff);

                dc.SubmitChanges();
            }
        }
    }
}
