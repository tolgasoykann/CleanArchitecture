
TestHttpController:

 Kullanım Örnekleri (Postman ya da Browser):

POST /api/testhttp/set?key=MyKey&value=HelloWorld
→ Session’a veri yazar.

GET /api/testhttp/get?key=MyKey
→ Session’dan veri okur.

DELETE /api/testhttp/remove?key=MyKey
→ Belirli bir session verisini siler.

DELETE /api/testhttp/clear
→ Tüm session’ı temizler.

.NET’te session kullanmak için builder.Services.AddSession(); ve app.UseSession(); middleware’lerinin aktif olduğuna dikkat et.
------------------------------------------------------------
TestRedisController:

 // Redis bağlantısı
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(configuration);
});

// RedisSessionManager'ı kullanmak için:
builder.Services.AddScoped<ISessionManager, RedisSessionManager>();

// Alternatif olarak local session kullanmak istiyorsan şunu kullanırsın:
// builder.Services.AddHttpContextAccessor();
// builder.Services.AddSession();
// builder.Services.AddScoped<ISessionManager, HttpContextSessionManager>();
// Eğer local session kullanıyorsan aşağıyı ekle:
// app.UseSession();

4. appsettings.json için Redis Bağlantısı (Opsiyonel)
json
Kopyala
Düzenle
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}

HTTP Request

POST http://localhost:5000/api/testredis/set?key=myKey&value=myValue

GET http://localhost:5000/api/testredis/set?key=myKey&value=myValue

DELETE http://localhost:5000/api/testredis/remove?key=myKey

Swagger:

POST → /api/testredis/set
Query Params:
  key = myKey
  value = myValue

  GET → /api/testredis/get
Query Params:
  key = myKey

DELETE → /api/testredis/remove
Query Params:
  key = myKey

-------------------------------------------------------------

🔹 Kullanım Örnekleri
1. Config Değeri Okuma

GET http://localhost:5000/api/testconfiglog/config/MySetting
📌 MySetting, appsettings.json içindeki bir key olmalı.

2. Log Mesajı Yazma

INFO Log

POST http://localhost:5000/api/testconfiglog/log?message=Deneme log&level=info

WARN Log

POST http://localhost:5000/api/testconfiglog/log?message=Bu bir uyarıdır&level=warn

ERROR Log

POST http://localhost:5000/api/testconfiglog/log?message=Hata oluştu&level=error

🛠 Program.cs / Service Registration Kontrolü

Program.cs veya Startup.cs içindeki servis tanımlamaların eksiksiz olmalı:


builder.Services.AddConfigManager(); // IConfigManager + ConfigManager

 1. File Loglama İçin

builder.Services.AddLoggingManager("file");

 2. Console Loglama İçin

builder.Services.AddLoggingManager("console");

----------------------------------------
 1. appsettings.json ya da appsettings.Development.json Dosyasına Anahtar Ekle
Örnek:


{
  "MySettings": {
    "AppName": "MyTestApp",
    "Version": "1.0.0"
  }
}

 2. IConfigManager implementasyonunun bu ayarları okuyabildiğinden emin ol
Örneğin ConfigManager.Get("MySettings:AppName") gibi bir çağrı yapıldığında "MyTestApp" dönmeli.

 3. GET endpoint’ini test et
Test Endpoint:

GET http://localhost:{port}/api/testconfiglog/config/MySettings:AppName
Beklenen Cevap:

"MyTestApp"
Log dosyasında veya konsolda da şöyle bir çıktı olur:

2025-08-06 13:00:00 [INFO] Config key 'MySettings:AppName' fetched with value: MyTestApp

 4. Program.cs veya Startup.cs içinde Servisleri Doğru Kaydet
Program.cs:

builder.Services.AddConfigManager();        // senin extension methodun
builder.Services.AddLoggingManager("file"); // veya "console"

Ekstra: Hatalı Key Testi
Aşağıdaki URL’yi çağırarak olmayan bir config key’i test edebilirsin:

GET http://localhost:{port}/api/testconfiglog/config/UnknownKey

Beklenen cevap:

{
  "status": 404,
  "title": "No config value found for key: UnknownKey"
}

GET /api/testconfiglog/config/{key} → Config değerini test eder.

POST /api/testconfiglog/log?message=...&level=... → Log yazmayı test eder.

Her iki manager'ın doğru çalıştığını gözlemleyebilirsin.