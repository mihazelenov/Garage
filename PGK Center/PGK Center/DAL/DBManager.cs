using PGK_Center.BLL;
using PGK_Center.ObjectModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PGK_Center.DAL
{
    public static class DBManager
    {
        public static Garage[] GetAllGarages()
        {
            using (var dc = new DatabaseContext())
            {
                var garages = dc.Garages
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted)
                    .ToArray();

                var withPays = garages
                    .GroupJoin(dc.Pays
                        .AsNoTracking()
                        .Where(a => !a.IsDeleted),
                               g => g.Id,
                               p => p.GarageID,
                               (g, p) => g.WithPays(p));

                return withPays
                    .GroupJoin(dc.Phones
                        .AsNoTracking()
                        .Where(a => !a.IsDeleted)
                        .OrderBy(a => a.Id),
                               g => g.Id,
                               p => p.GarageID,
                               (g, p) => g.WithPhones(p))
                    .ToArray();
            }
        }

        public static void SaveGarage(Garage garage)
        {
            if (garage == null)
                return;

            using (var dc = new DatabaseContext())
            {
                for (int n = 0; n < garage.Pays.Count; n++)
                    dc.Entry(garage.Pays[n]).State = EntityState.Unchanged;
                for (int n = 0; n < garage.Phones.Count; n++)
                    dc.Entry(garage.Phones[n]).State = EntityState.Unchanged;

                var oldGarage = dc.Garages.FirstOrDefault(a => a.Id == garage.Id);

                if (oldGarage == null)
                    dc.Garages.Add(garage);
                else
                    garage.CopyTo(oldGarage);

                dc.SaveChanges();
            }
        }

        public static void SaveGaragePhones(int garageID, IEnumerable<Phone> phones)
        {
            using (var dc = new DatabaseContext())
            {
                foreach (var phone in dc.Phones.Where(a => a.GarageID == garageID))
                    phone.IsDeleted = true;

                dc.Phones.AddRange(phones);
                dc.SaveChanges();
            }
        }

        public static void SaveGaragePays(int garageID, IEnumerable<Pay> pays)
        {
            using (var dc = new DatabaseContext())
            {
                foreach (var pay in dc.Pays.Where(a => a.GarageID == garageID))
                    pay.IsDeleted = true;

                dc.Pays.AddRange(pays);
                dc.SaveChanges();
            }
        }

        public static void DeleteGarage(int id)
        {
            using (var dc = new DatabaseContext())
            {
                var oldGarage = dc.Garages.FirstOrDefault(a => a.Id == id);
                if (oldGarage != null)
                    oldGarage.IsDeleted = true;

                dc.SaveChanges();
            }
        }

        public static void DeleteTariff(int id)
        {
            using (var dc = new DatabaseContext())
            {
                var oldTariff = dc.Tariffs.FirstOrDefault(a => a.Id == id);
                if (oldTariff != null)
                    oldTariff.IsDeleted = true;

                dc.SaveChanges();
            }
        }

        public static void DeleteElectricityTariff(int id)
        {
            using (var dc = new DatabaseContext())
            {
                var oldTariff = dc.ElectricityTariffs.FirstOrDefault(a => a.Id == id);
                if (oldTariff != null)
                    oldTariff.IsDeleted = true;

                dc.SaveChanges();
            }
        }

        public static bool IsSameGarageExists(Garage garage)
        {
            using (var dc = new DatabaseContext())
                return dc.Garages
                    .AsNoTracking()
                    .Any(a => a.Id != garage.Id && a.Number == garage.Number);
        }

        public static List<Tariff> GetAllTariffs()
        {
            using (var dc = new DatabaseContext())
                return dc.Tariffs
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted)
                    .ToList();
        }

        public static List<ElectricityTariff> GetAllElectricityTariffs()
        {
            using (var dc = new DatabaseContext())
                return dc.ElectricityTariffs
                    .AsNoTracking()
                    .Where(a => !a.IsDeleted)
                    .ToList();
        }

        public static void SaveTariff(Tariff tariff)
        {
            if (tariff == null)
                return;

            using (var dc = new DatabaseContext())
            {
                var oldTariff = dc.Tariffs.FirstOrDefault(a => a.Id == tariff.Id);
                if (oldTariff == null)
                    dc.Tariffs.Add(tariff);
                else
                    tariff.CopyTo(oldTariff);

                dc.SaveChanges();
            }
        }

        public static void SaveElectricityTariff(ElectricityTariff tariff)
        {
            if (tariff == null)
                return;

            using (var dc = new DatabaseContext())
            {
                var oldTariff = dc.ElectricityTariffs
                    .FirstOrDefault(a => a.Id == tariff.Id);
                if (oldTariff == null)
                    dc.ElectricityTariffs.Add(tariff);
                else
                    tariff.CopyTo(oldTariff);

                dc.SaveChanges();
            }
        }
    }
}
