using System;

namespace PGK_Center.ObjectModel
{
    public class Tariff : ICloneable
    {
        public int Id { get; set; }

        public int Year { get; set; } = DateTime.Now.Date.Year;

        public int Value { get; set; }

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
