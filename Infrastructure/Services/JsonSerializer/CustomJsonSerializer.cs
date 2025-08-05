using System.Reflection;
using System.Text;
using Domain.Interfaces;

namespace Infrastructure.Services.JsonSerializer
{
    public class CustomJsonSerializer : ICustomJsonSerializer
    {
        public string Serialize<T>(T obj)
        {
            var type = typeof(T);
            var props = type.GetProperties();
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");

            foreach (var prop in props)
            {
                var name = prop.Name;
                var value = prop.GetValue(obj);

                jsonBuilder.Append($"\"{name}\":");

                if (value == null)
                {
                    jsonBuilder.Append("null,");
                }
                else
                {
                    // Eğer string ise çift tırnakla sar
                    if (prop.PropertyType == typeof(string))
                        jsonBuilder.Append($"\"{value}\",");
                    else
                        jsonBuilder.Append($"\"{value.ToString()}\",");
                }
            }

            if (props.Length > 0)
                jsonBuilder.Length--; // Son virgülü sil

            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        public T Deserialize<T>(string json)
        {
            var obj = Activator.CreateInstance<T>(); // new() kısıtı gerekmez
            var type = typeof(T);
            var props = type.GetProperties();

            json = json.Trim('{', '}');
            var keyValuePairs = json.Split(',')
                .Select(kv => kv.Split(':'))
                .Where(kv => kv.Length == 2)
                .ToDictionary(
                    kv => kv[0].Trim('"', ' '),
                    kv => kv[1].Trim('"', ' '));

            foreach (var prop in props)
            {
                if (!keyValuePairs.TryGetValue(prop.Name, out var valueStr))
                    continue;

                try
                {
                    if (valueStr == "null")
                    {
                        prop.SetValue(obj, null);
                    }
                    else
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        object convertedValue = Convert.ChangeType(valueStr, targetType);
                        prop.SetValue(obj, convertedValue);
                    }
                }
                catch
                {
                    // Opsiyonel: hatalı değerleri loglayabilir veya ignore edebilirsin
                    prop.SetValue(obj, GetDefault(prop.PropertyType));
                }
            }

            return obj;
        }

        private static object GetDefault(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
