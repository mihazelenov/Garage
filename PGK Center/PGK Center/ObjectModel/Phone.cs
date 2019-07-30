namespace PGK_Center.ObjectModel
{
    public class Phone
    {
        public int Id { get; set; }

        public int GarageID { get; set; }

        public string Value { get; set; }

        public bool IsMobile { get; set; }

        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
