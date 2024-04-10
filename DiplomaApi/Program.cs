using DiplomaApi.DataRepository.GenericRepository;
using DiplomaApi;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddResponseCaching();

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<ApplicationDbContext>(
        dbContextOptions => dbContextOptions.UseNpgsql(builder.Configuration["ConnectionStrings:DbConnection"]));

builder.Services.AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));

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

app.MapControllers();

app.Run();

