using System.Reflection;
using System.Text;
using Domain.Interfaces;

namespace Infrastructure.Services.JsonSerializer
{
    public class CustomJsonSerializer : ICustomJsonSerializer
    {
        public string Serialize<T>(T obj)
        {
            if (obj == null) return "null";

            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");

            foreach (var prop in props)
            {
                // Indexer property'leri atla
                if (prop.GetIndexParameters().Length > 0)
                    continue;

                var name = prop.Name;

                object value;
                try
                {
                    value = prop.GetValue(obj);
                }
                catch
                {
                    continue; // Property okunamıyorsa atla
                }

                jsonBuilder.Append($"\"{name}\":");

                if (value == null)
                {
                    jsonBuilder.Append("null,");
                }
                else if (value is string)
                {
                    jsonBuilder.Append($"\"{value}\",");
                }
                else if (value is bool)
                {
                    jsonBuilder.Append($"{value.ToString().ToLower()},");
                }
                else if (value is DateTime dt)
                {
                    jsonBuilder.Append($"\"{dt:O}\","); // ISO 8601 formatı
                }
                else if (value.GetType().IsPrimitive || value is decimal)
                {
                    jsonBuilder.Append($"{value},");
                }
                else
                {
                    jsonBuilder.Append($"\"{value.ToString()}\",");
                }
            }

            if (jsonBuilder[^1] == ',')
                jsonBuilder.Length--; // Son virgülü sil

            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }

        public T Deserialize<T>(string json)
        {
            var targetType = typeof(T);

            // Primitive tipler için hızlı çıkış (bu kısım aynı kalabilir)
            if (targetType == typeof(string))
                return (T)(object)json.Trim('"');
            if (targetType.IsPrimitive || targetType.IsValueType && targetType != typeof(DateTime)) // DateTime'ı nesne olarak işleyeceğiz
            {
                try { return (T)Convert.ChangeType(json, targetType); }
                catch { return default(T); }
            }

            // === YENİ VE SAĞLAM PARSER BAŞLANGICI ===

            var keyValuePairs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var jsonSpan = json.AsSpan().Trim(); // Performans için AsSpan()

            if (!jsonSpan.StartsWith("{") || !jsonSpan.EndsWith("}"))
            {
                // Geçerli bir JSON nesnesi değilse boş nesne dön
                return Activator.CreateInstance<T>();
            }

            // Dıştaki { ve } karakterlerini at
            var innerJson = jsonSpan.Slice(1, jsonSpan.Length - 2);

            int position = 0;
            bool inString = false;
            bool isEscaped = false;
            var currentKey = new StringBuilder();
            var currentValue = new StringBuilder();
            bool isParsingKey = true;

            while (position < innerJson.Length)
            {
                char c = innerJson[position];

                if (isEscaped)
                {
                    // Eğer önceki karakter escape ise (örn: \"), bu karakteri olduğu gibi ekle
                    if (isParsingKey) currentKey.Append(c);
                    else currentValue.Append(c);
                    isEscaped = false;
                }
                else if (c == '\\')
                {
                    isEscaped = true;
                }
                else if (c == '"')
                {
                    // Tırnak işaretine girdik veya çıktık
                    inString = !inString;
                }
                else if (c == ':' && !inString)
                {
                    // Anahtar (key) bitti, değer (value) başlıyor
                    isParsingKey = false;
                }
                else if (c == ',' && !inString)
                {
                    // Key-value çifti bitti, sözlüğe ekle ve sıfırla
                    keyValuePairs.Add(
                        currentKey.ToString().Trim().Trim('"'),
                        currentValue.ToString().Trim()
                    );
                    currentKey.Clear();
                    currentValue.Clear();
                    isParsingKey = true;
                }
                else
                {
                    // Normal karakter, ilgili yere ekle
                    if (isParsingKey)
                    {
                        currentKey.Append(c);
                    }
                    else
                    {
                        currentValue.Append(c);
                    }
                }
                position++;
            }

            // Son kalan key-value çiftini de ekle (çünkü sonunda virgül yok)
            if (currentKey.Length > 0)
            {
                keyValuePairs.Add(
                    currentKey.ToString().Trim().Trim('"'),
                    currentValue.ToString().Trim()
                );
            }

            // === YENİ PARSER SONU ===

            // === Nesneyi doldurma kısmı (bu kısım aynı kalıyor) ===
            var obj = Activator.CreateInstance<T>();
            var props = targetType.GetProperties();

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
                        var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        object convertedValue;

                        // Değerin kendisi de bir string ise tırnaklarını temizle
                        if (valueStr.StartsWith("\"") && valueStr.EndsWith("\""))
                        {
                            valueStr = valueStr.Trim('"');
                        }

                        convertedValue = Convert.ChangeType(valueStr, propType);
                        prop.SetValue(obj, convertedValue);
                    }
                }
                catch
                {
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
