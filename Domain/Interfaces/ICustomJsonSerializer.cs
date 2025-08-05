
namespace Domain.Interfaces
{
    public interface ICustomJsonSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
}
