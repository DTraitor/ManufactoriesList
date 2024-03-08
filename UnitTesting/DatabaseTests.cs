namespace UnitTesting;

public class DatabaseTests
{
    private const string TestFileName = "DatabaseTest.json";

    [TearDown]
    public void CleanUp()
    {
        File.Delete(TestFileName);
    }

    [Test]
    public void ReadWriteTest()
    {
        FileStream stream = new FileStream(TestFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        Enterprise enterprise = new Enterprise()
        {
            Name = "Тестове підприємство",
            Class = 1,
            Address = "Київ, вул. Тестова, 1",
            Speciality = "Металургія",
            Ownership = Enterprise.OwnershipType.Private,
            PhoneNumbers = new (){"+380000000000", "+380000000001"},
            Services = new (){"Тестовий сервіс 1", "Тестовий сервіс 2"},
            WorkingHours = new Dictionary<Enterprise.DayOfWeek, Tuple<TimeOnly, TimeOnly>>
            {
                [Enterprise.DayOfWeek.Monday] = new Tuple<TimeOnly, TimeOnly>(new TimeOnly(8, 0), new TimeOnly(17, 0))
            },
        };

        stream.SetLength(0);

        ReadWriter<Enterprise> database = new (stream);

        Assert.That(database.Enterprises, Is.EqualTo(new List<Enterprise>()));

        database.Enterprises.Add(enterprise);
        database.Dispose();
        stream.Close();

        database = new ReadWriter<Enterprise>(TestFileName);
        Assert.That(database.Enterprises, Is.EqualTo(new List<Enterprise>{enterprise}));
        database.Dispose();
    }
}