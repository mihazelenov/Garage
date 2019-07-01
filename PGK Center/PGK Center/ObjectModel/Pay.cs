using System;
using System.Data.Linq.Mapping;

namespace PGK_Center.ObjectModel
{
    [Table]
    public class Pay
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int ID { get; set; }

        [Column]
        public int GarageID { get; set; }

        [Column]
        public DateTime Date { get; set; } = DateTime.Now;

        [Column]
        public decimal Value { get; set; }

        [Column]
        public bool IsByAgreement { get; set; }

        [Column]
        public bool IsDeleted { get; set; }
    }
}
