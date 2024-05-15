using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DiplomaApi.Services;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("http://192.168.31.107:6743");

builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy", builder =>
    {
        builder.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddResponseCaching();

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<ApplicationDbContext>(
        dbContextOptions => dbContextOptions.UseNpgsql(builder.Configuration["ConnectionStrings:DbConnection"]));

builder.Services.Configure<UserSettings>(
    builder.Configuration.GetSection("UserSettings"));

builder.Services.AddSingleton<IUserSettings>(provider =>
    provider.GetRequiredService<IOptions<UserSettings>>().Value);

builder.Services.Configure<SmtpSettings>(
    builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddSingleton<ISmtpSettings>(provider =>
    provider.GetRequiredService<IOptions<SmtpSettings>>().Value);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISmtpService, SmtpService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));

builder.Services.Configure<PrometheusSettings>(builder.Configuration.GetSection("ApiKeys"));
builder.Services.AddSingleton<IPrometheusSettings>(provider =>
    provider.GetRequiredService<IOptions<PrometheusSettings>>().Value);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["UserSettings:SecretKey"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PrometheusPolicy", policy =>
        policy.Requirements.Add(new PrometheusService()));
});

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseResponseCaching();

app.UseCors("Policy");

app.MapControllers();
app.MapMetrics().RequireAuthorization("PrometheusPolicy");

app.Run();

