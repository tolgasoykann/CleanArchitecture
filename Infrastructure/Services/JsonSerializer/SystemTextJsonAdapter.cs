// Infrastructure.Services.JsonSerializer
using Domain.Interfaces;
// System.Text.Json'ı buraya ekleyin
using System.Text.Json;

// Bu sınıf, sizin arayüzünüz ile gerçek kütüphane arasında bir köprüdür.
public class SystemTextJsonAdapter : ICustomJsonSerializer
{
    // İsterseniz options'ları dışarıdan alabilirsiniz.
    private readonly JsonSerializerOptions _options;

    public SystemTextJsonAdapter()
    {
        // Standart web ayarları ile uyumlu options'lar
        _options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    public string Serialize<T>(T obj)
    {
        // İşi gerçek, test edilmiş ve performanslı serializer'a devredin.
        return JsonSerializer.Serialize(obj, _options);
    }

    public T Deserialize<T>(string json)
    {
        // İşi gerçek, test edilmiş ve performanslı serializer'a devredin.
        return JsonSerializer.Deserialize<T>(json, _options);
    }
}