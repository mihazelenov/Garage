using System;
using System.Data.Linq.Mapping;

namespace PGK_Center.ObjectModel
{
    [Table]
    public class Tariff : ICloneable
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int ID { get; set; }

        [Column]
        public int Year { get; set; } = DateTime.Now.Date.Year;

        [Column]
        public int Value { get; set; }

        [Column]
        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            return Year.ToString();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
