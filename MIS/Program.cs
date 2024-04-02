using AutoMapper;
using DeliveryBackend.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MIS.Data;
using MIS.Data.Models;
using MIS.Services;
using MIS.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContextForMIS>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ForMISConnection")));





builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISpeciality, SpecialityService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IInspectionServise, InspectionServise>();
builder.Services.AddScoped<IConsultationServise, ConsultationSevrice>();
builder.Services.AddScoped<IReportService, ReportSevrice>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = TokenConfigurations.Issuer,
            ValidateAudience = true,
            ValidAudience = TokenConfigurations.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = TokenConfigurations.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    });

// AutoMapping
var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddMvc();
var app = builder.Build();
/*string jsonFilePath = "D:\\Bol\\1.2.643.5.1.13.13.11.1005_2.24.json";

string jsonText = File.ReadAllText(jsonFilePath);

var jObject = JObject.Parse(jsonText);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContextForMIS>();

    try
    {
        // Создание базы данных (если она еще не создана)
        dbContext.Database.EnsureCreated();

        // Создание записей с новыми GUID'ами
        var idMap = new Dictionary<int, Guid>();
        foreach (var token in jObject)
        {
            var newId = Guid.NewGuid();
            var id = (int)token.Value["ID"];
            var idParent = (string)token.Value["ID_PARENT"];

            var record = new Record
            {
                ID = newId,
                ACTUAL = (int)token.Value["ACTUAL"],
                MKB_CODE = (string)token.Value["MKB_CODE"],
                MKB_NAME = (string)token.Value["MKB_NAME"],
                REC_CODE = (string)token.Value["REC_CODE"],
                ID_PARENT = idParent.Equals(id.ToString()) ? newId.ToString() : idParent,
                CreateTime = DateTime.UtcNow
            };

            idMap.Add(id, newId);
            dbContext.Records.Add(record);
        }

        dbContext.SaveChanges();

        // Обновление ID_PARENT на новые GUID'ы
        foreach (var token in jObject)
        {
            var idParent = (string)token.Value["ID_PARENT"];
            if (int.TryParse(idParent, out var parentId))
            {
                if (idMap.TryGetValue(parentId, out var newParentId))
                {
                    var record = dbContext.Records.FirstOrDefault(r => r.ID == idMap[(int)token.Value["ID"]]);
                    if (record != null)
                    {
                        record.ID_PARENT = newParentId.ToString();
                    }
                }
            }
        }

        dbContext.SaveChanges();

        Console.WriteLine("Данные успешно сохранены в базе данных PostgreSQL.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
        throw;
    }
}*/


/*using (var context = new MyContext(ApplicationDbContext))
{
    context.Database.Migrate();
}*/


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
        if (!dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.EnsureCreated(); 
            Console.WriteLine("Automatically applied migration.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occurred while migrating: {ex.Message}");
    }
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
