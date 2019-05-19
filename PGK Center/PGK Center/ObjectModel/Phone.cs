using System.Data.Linq.Mapping;

namespace PGK_Center.ObjectModel
{
    [Table]
    public class Phone
    {
        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public int ID { get; set; }

        [Column]
        public int GarageID { get; set; }

        [Column]
        public string Value { get; set; }

        [Column]
        public bool IsMobile { get; set; }

        [Column]
        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
