using DataAccess;

namespace Interaction;

public class InteractionHolder : IDisposable
{
    public InteractionHolder(string fileName)
    {
        databaseAccess = new(fileName);
    }

    public InteractionHolder(FileStream stream)
    {
        databaseAccess = new(stream);
    }

    public void Dispose()
    {
        databaseAccess.Dispose();
    }

    public Enterprise CreateEnterprise(string name, byte enterpriseClass, string address, string speciality, Enterprise.OwnershipType ownership, List<string> phoneNumbers, List<string> services, Dictionary<Enterprise.DayOfWeek, Tuple<TimeOnly, TimeOnly>> workingHours)
    {
        Enterprise enterprise = new Enterprise()
        {
            Name = name,
            Class = enterpriseClass,
            Address = address,
            Speciality = speciality,
            Ownership = ownership,
            PhoneNumbers = phoneNumbers,
            Services = services,
            WorkingHours = workingHours
        };
        databaseAccess.Enterprises.Add(enterprise);
        databaseAccess.Save();
        return enterprise;
    }

    public void DeleteEnterprise(Enterprise enterprise)
    {
        databaseAccess.Enterprises.Remove(enterprise);
        databaseAccess.Save();
    }

    public void ChangeName(Enterprise enterprise, string newName)
    {
        enterprise.Name = newName;
        databaseAccess.Save();
    }

    public void ChangeClass(Enterprise enterprise, byte newClass)
    {
        enterprise.Class = newClass;
        databaseAccess.Save();
    }

    public void ChangeAddress(Enterprise enterprise, string newAddress)
    {
        enterprise.Address = newAddress;
        databaseAccess.Save();
    }

    public void ChangeSpeciality(Enterprise enterprise, string newSpeciality)
    {
        enterprise.Speciality = newSpeciality;
        databaseAccess.Save();
    }

    public void ChangeOwnership(Enterprise enterprise, Enterprise.OwnershipType newOwnership)
    {
        enterprise.Ownership = newOwnership;
        databaseAccess.Save();
    }

    public void ChangePhoneNumbers(Enterprise enterprise, int index, string? newValue)
    {
        if (newValue == null)
            enterprise.PhoneNumbers.RemoveAt(index);
        else if (index == -1)
            enterprise.PhoneNumbers.Add(newValue);
        else
            enterprise.PhoneNumbers[index] = newValue;
        databaseAccess.Save();
    }

    public void ChangeServices(Enterprise enterprise, int index, string? newValue)
    {
        if (newValue == null)
            enterprise.Services.RemoveAt(index);
        else if (index == -1)
            enterprise.Services.Add(newValue);
        else
            enterprise.Services[index] = newValue;
        databaseAccess.Save();
    }

    public void ChangeWorkingHours(Enterprise enterprise, Enterprise.DayOfWeek day, TimeOnly start, TimeOnly end)
    {
        enterprise.WorkingHours[day] = new Tuple<TimeOnly, TimeOnly>(start, end);
        databaseAccess.Save();
    }

    public void RemoveWorkingHours(Enterprise enterprise, Enterprise.DayOfWeek day)
    {
        enterprise.WorkingHours.Remove(day);
        databaseAccess.Save();
    }

    public List<Enterprise> FindByFilter(Enterprise.Filter filter)
    {
        List<Enterprise> result = new();
        foreach (Enterprise enterprise in databaseAccess.Enterprises)
        {
            if (filter.Name != null && !enterprise.Name.Contains(filter.Name))
                continue;
            if (filter.Class != null && enterprise.Class != filter.Class)
                continue;
            if (filter.Address != null && !enterprise.Address.Contains(filter.Address))
                continue;
            if (filter.Speciality != null && !enterprise.Speciality.Contains(filter.Speciality))
                continue;
            if (filter.Ownership != null && enterprise.Ownership != filter.Ownership)
                continue;
            if (filter.PhoneNumbers.Count != 0 && !filter.PhoneNumbers.All(enterprise.PhoneNumbers.Contains))
                continue;
            if (filter.Services.Count != 0 && !filter.Services.All(enterprise.Services.Contains))
                continue;
            if (filter.WorkingHours.Count != 0)
            {
                bool match = true;
                foreach (KeyValuePair<Enterprise.DayOfWeek, Tuple<TimeOnly, TimeOnly>> pair in filter.WorkingHours)
                {
                    if (!enterprise.WorkingHours.ContainsKey(pair.Key))
                    {
                        match = false;
                        break;
                    }
                    if (enterprise.WorkingHours[pair.Key].Item1 > pair.Value.Item1 || enterprise.WorkingHours[pair.Key].Item2 < pair.Value.Item2)
                    {
                        match = false;
                        break;
                    }
                }
                if (!match)
                    continue;

            }
            result.Add(enterprise);
        }
        return result;
    }

    private readonly ReadWriter<Enterprise> databaseAccess;
}
