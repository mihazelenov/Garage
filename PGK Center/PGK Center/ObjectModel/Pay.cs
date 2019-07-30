using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace PGK_Center.ObjectModel
{
    public abstract class Pay
    {
        public int Id { get; set; }

        public int GarageID { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public decimal Value { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public Visibility IsByAgreementVisibility { get; protected set; } = Visibility.Hidden;
    }
}
