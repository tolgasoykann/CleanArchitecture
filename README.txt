
-SESSION MANAGEMENT ICIN PROGRAM.CS ���NE BU KODLAR EKLENMELI

builder.Services.AddHttpContextAccessor(); // Gerekli
builder.Services.AddDistributedMemoryCache(); // Session i�in in-memory cache
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".MyApp.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<ISessionManager, SessionManager>();

-SESSION MANAGEMENT ICIN PROGRAM.CS ���NE BU KODLAR EKLENMELI

app.UseSession(); // Middleware olarak ekle
