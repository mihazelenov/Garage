using System;

namespace PGK_Center.ObjectModel
{
    public class Pay
    {
        public int Id { get; set; }

        public int GarageID { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public decimal Value { get; set; }

        public bool IsByAgreement { get; set; }

        public bool IsDeleted { get; set; }
    }
}
