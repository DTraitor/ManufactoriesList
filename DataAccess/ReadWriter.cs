using System.Runtime.Serialization;
using System.Text.Json;

namespace DataAccess;

public class ReadWriter<T> : IDisposable {
    public ReadWriter(string file)
    {
        stream = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        Read();
    }

    public ReadWriter(FileStream stream)
    {
        streamOwned = false;
        this.stream = stream;
        Read();
    }

    public void Dispose()
    {
        Save();
        if (streamOwned)
            stream.Dispose();
    }

    private void Read()
    {
        stream.Position = 0;
        using StreamReader reader = new StreamReader(stream, leaveOpen: true);
        try
        {
            Enterprises = JsonSerializer.Deserialize<List<T>>(reader.ReadToEnd()) ?? new List<T>();
        }
        catch (JsonException e)
        {
            Enterprises = new List<T>();
        }
    }

    public void Save()
    {
        stream.SetLength(0);
        using StreamWriter writer = new StreamWriter(stream, leaveOpen: true);
        writer.Write(JsonSerializer.Serialize(Enterprises));
    }

    private bool streamOwned = true;
    public List<T> Enterprises { get; private set; }
    private readonly FileStream stream;
}