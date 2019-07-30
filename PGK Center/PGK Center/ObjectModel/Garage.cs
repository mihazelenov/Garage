using PGK_Center.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace PGK_Center.ObjectModel
{
    public class Garage : ICloneable
    {
        public int Id { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public string Address { get; set; }

        public decimal Square { get; set; }

        public bool? CounterState { get; set; }

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

        private decimal _sumOnYear => (CommonData.Tariffs
            .Find(a => a.Year == DateTime.Now.Year)
            ?.Value ?? 0) * Square;
        
        private decimal _sumOnQuarter => _sumOnYear / 4;

        public string ToPay =>
            $"{_sumOnYear}/{_sumOnQuarter * DateTime.Now.GetCurrentQuarter()}";

        private bool _isQuarterDebtor => Total > 0 && Total <= _sumOnQuarter;
        public bool IsCurrentYearDebtor => Total > _sumOnQuarter && Total <= TotalOnCurrentYear;
        public bool IsPreviousYearsDebtor => Total > TotalOnCurrentYear;

        public Brush Background
        {
            get
            {
                if (IsPreviousYearsDebtor || IsCurrentYearDebtor)
                    return Brushes.IndianRed;

                if (_isQuarterDebtor)
                    return Brushes.Yellow;

                return Brushes.Transparent;
            }
        }

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

        public bool IsCounterNoInfo => CounterState == null;        

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
