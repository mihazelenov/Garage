using PGK_Center.ObjectModel;
using System.Data.Entity;

namespace PGK_Center.DAL
{
    class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DatabaseConnection")
        { }

        public DbSet<Garage> Garages { get; set; }

        public DbSet<Pay> Pays { get; set; }

        public DbSet<Phone> Phones { get; set; }

        public DbSet<Tariff> Tariffs { get; set; }

        public DbSet<ElectricityTariff> ElectricityTariffs { get; set; }
    }
}
