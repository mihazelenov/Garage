using PGK_Center.BLL;
using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text.RegularExpressions;

namespace PGK_Center.ObjectModel
{
    [Table]
    public class Garage : ICloneable
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int ID { get; set; }

        [Column]
        public string Number { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Comment { get; set; }

        [Column]
        public string Address { get; set; }

        [Column]
        public decimal Square { get; set; }

        [Column]
        public bool? CounterState { get; set; }

        [Column]
        public bool IsDeleted { get; set; }

        public GarageNumber GarageNumber => new GarageNumber(Number);

        public List<Phone> Phones { get; set; } = new List<Phone>();

        public List<Pay> Pays { get; set; } = new List<Pay>();

        public List<Phone> CellPhones => Phones.FindAll(a => a.IsMobile);
        public List<Phone> StaticPhones => Phones.FindAll(a => !a.IsMobile);

        public string CellPhone => CellPhones.FirstOrDefault()?.Value;
        public string StaticPhone => StaticPhones.FirstOrDefault()?.Value;

        public Garage WithPays(IEnumerable<Pay> pays)
        {
            Pays = pays.ToList();
            return this;
        }

        public Garage WithPhones(IEnumerable<Phone> phones)
        {
            Phones = phones.ToList();
            return this;
        }

        public decimal Total =>
            CommonData.Tariffs.Sum(a => a.Value * Square) - Pays.Sum(a => a.Value);

        public bool IsDebtor => Total > 0;

        public bool IsCounterSet
        {
            get => CounterState == true;
            set
            {
                CounterState = value;
            }
        }

        public bool IsCounterNotSet
        {
            get => CounterState == false;
            set
            {
                CounterState = !value;
            }
        }

        public string CounterStateToDisplay
        {
            get
            {
                switch (CounterState)
                {
                    case true:
                        return "Установлен";
                    case false:
                        return "Отказался";
                    default:
                        return null;
                }
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return Number;
        }
    }

    public class GarageNumber : IComparable<GarageNumber>
    {
        private static Regex _digitalRegex = new Regex(@"\d*");

        private protected string _value;
        private protected uint _number;

        public GarageNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                value = "0";

            _number = uint.TryParse(_digitalRegex.Match(value)?.Value, out uint number)
                ? number
                : 0;

            _value = value;
        }

        public int CompareTo(GarageNumber other)
        {
            if (_number == other._number)
                return string.Compare(_value, other._value);

            return _number.CompareTo(other._number);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}
