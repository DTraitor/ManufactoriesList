namespace Interaction;

public class Enterprise
{
    public string Name { get; set; }
    public byte Class { get; set; }
    public string Address { get; set; }
    public string Speciality { get; set; }
    public OwnershipType Ownership { get; set; }
    public List<string> PhoneNumbers { get; set; }
    public List<string> Services { get; set; }
    public Dictionary<DayOfWeek, Tuple<TimeOnly, TimeOnly>> WorkingHours { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is Enterprise enterprise &&
                Name == enterprise.Name &&
                Class == enterprise.Class &&
                Address == enterprise.Address &&
                Speciality == enterprise.Speciality &&
                Ownership == enterprise.Ownership &&
                PhoneNumbers.SequenceEqual(enterprise.PhoneNumbers) &&
                Services.SequenceEqual(enterprise.Services) &&
                WorkingHours.SequenceEqual(enterprise.WorkingHours);
    }

    public enum OwnershipType
    {
        Private,
        Public,
        State,
    }

    public enum DayOfWeek
    {
        Monday = 0,
        Tuesday = 1,
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5,
        Sunday = 6
    }

    public class Filter
    {
        public string? Name { get; set; }
        public byte? Class { get; set; }
        public string? Address { get; set; }
        public string? Speciality { get; set; }
        public OwnershipType? Ownership { get; set; }
        public List<string> PhoneNumbers { get; set; } = new();
        public List<string> Services { get; set; } = new();
        public Dictionary<DayOfWeek, Tuple<TimeOnly, TimeOnly>> WorkingHours { get; set; } = new();
    }
}