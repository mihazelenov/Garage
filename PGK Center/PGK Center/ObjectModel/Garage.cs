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

        public decimal Total
        {
            get
            {
                var totalOnCurrentYear =
                    _sumOnQuarter * DateTime.Now.GetCurrentQuarter();

                return _sumOnPreviousYears + totalOnCurrentYear - _paysSum;
            }
        }

        public decimal TotalOnCurrentQuarter
        {
            get
            {
                var total = Math.Min(_sumOnQuarter, Total);
                return total < 0
                    ? 0
                    : total;
            }
        }

        public decimal TotalOnCurrentYear
        {
            get
            {
                var total = Math.Min(
                    _sumOnQuarter * DateTime.Now.GetCurrentQuarter(), Total);
                return total < 0
                    ? 0
                    : total;
            }
        }

        public decimal TotalOnPreviousYears
        {
            get
            {
                var total = _sumOnPreviousYears - _paysSum;
                return total < 0
                    ? 0
                    : total;
            }
        }

        private decimal _paysSum => Pays.Sum(a => a.Value);

        private decimal _sumOnPreviousYears => CommonData.Tariffs
            .Where(a => a.Year < DateTime.Now.Year)
            .Sum(a => a.Value) * Square;

        private decimal _sumOnQuarter => (CommonData.Tariffs
            .Find(a => a.Year == DateTime.Now.Year)
            ?.Value ?? 0) * Square / 4;

        public bool IsDebtor => Total > _sumOnQuarter;

        public bool IsQuarterDebtor => Total > 0 && Total <= _sumOnQuarter;

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
