using System.Text.Json;

namespace UnitTesting;

public class BusinessLogicTests
{
    private const string TestFileName = "BusinessLogicTest.json";

    [TearDown]
    public void CleanUp()
    {
        File.Delete(TestFileName);
    }

    [Test]
    public void InteractionTest()
    {
        FileStream stream = new FileStream(TestFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        stream.SetLength(0);
        InteractionHolder interaction = new InteractionHolder(stream);
        Enterprise.Filter filter = new Enterprise.Filter();
        Enterprise createdEvent = interaction.CreateEnterprise(
                    "Тестове підприємство",
                    1,
                    "Київ, вул. Тестова, 1",
                    "Металургія",
                    Enterprise.OwnershipType.Private,
                    new (){"+380000000000", "+380000000001"},
                    new (){"Тестовий сервіс 1", "Тестовий сервіс 2"},
                    new()
                    {
                        [Enterprise.DayOfWeek.Monday] = new Tuple<TimeOnly, TimeOnly>(new TimeOnly(8, 0), new TimeOnly(17, 0))
                    }
            );

        interaction.ChangeName(createdEvent, "TestName");
        interaction.ChangeClass(createdEvent, 3);
        interaction.ChangeAddress(createdEvent, "TestPlace");
        interaction.ChangeSpeciality(createdEvent, "Speciality");
        interaction.ChangeOwnership(createdEvent, Enterprise.OwnershipType.Public);
        interaction.ChangePhoneNumbers(createdEvent, 0, "+380000000002");
        interaction.ChangePhoneNumbers(createdEvent, -1, "+380000000003");
        interaction.ChangePhoneNumbers(createdEvent, 2, null);
        interaction.ChangeServices(createdEvent, 0, "TestService");
        interaction.ChangeServices(createdEvent, -1, "TestService2");
        interaction.ChangeServices(createdEvent, 2, null);
        interaction.ChangeWorkingHours(
            createdEvent,
            Enterprise.DayOfWeek.Tuesday,
            new TimeOnly(12, 0),
            new TimeOnly(18, 0)
            );

        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{createdEvent}));
        stream.Position = 0;
        using (StreamReader reader = new StreamReader(stream, leaveOpen: true))
        {
            Assert.That(reader.ReadToEnd(), Is.EqualTo(JsonSerializer.Serialize(new List<Enterprise>{createdEvent})));
        }

        Enterprise second = interaction.CreateEnterprise(
            "Second enterprise",
            2,
            "Kyiv, Test street, 1",
            "Test speciality",
            Enterprise.OwnershipType.State,
            new (){"+380000000004", "+380000000005"},
            new (){"Test service 1", "Test service 2"},
            new()
            {
                [Enterprise.DayOfWeek.Tuesday] = new Tuple<TimeOnly, TimeOnly>(new TimeOnly(10, 0), new TimeOnly(17, 0))
            }
            );

        filter.Name = "Second enterprise";
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        filter.Name = null;
        filter.Class = 2;
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        filter.Class = null;
        filter.Address = "Kyiv, Test street, 1";
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        filter.Address = null;
        filter.Speciality = "Test speciality";
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        filter.Speciality = null;
        filter.Ownership = Enterprise.OwnershipType.State;
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        filter.Ownership = null;
        filter.PhoneNumbers = new (){"+380000000004", "+380000000005"};
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        filter.PhoneNumbers.Clear();
        filter.Services = new (){"Test service 1"};
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        filter.Services.Clear();
        filter.WorkingHours = new()
        {
            [Enterprise.DayOfWeek.Tuesday] = new Tuple<TimeOnly, TimeOnly>(new TimeOnly(10, 0), new TimeOnly(12, 0))
        };
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        interaction.RemoveWorkingHours(second, Enterprise.DayOfWeek.Tuesday);
        filter.WorkingHours = new()
        {
            [Enterprise.DayOfWeek.Tuesday] = new Tuple<TimeOnly, TimeOnly>(new TimeOnly(10, 0), new TimeOnly(18, 0))
        };
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{}));
        filter.WorkingHours.Clear();

        interaction.DeleteEnterprise(createdEvent);
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));

        stream.Close();

        interaction = new InteractionHolder(TestFileName);
        Assert.That(interaction.FindByFilter(filter), Is.EqualTo(new List<Enterprise>{second}));
        interaction.Dispose();
    }
}