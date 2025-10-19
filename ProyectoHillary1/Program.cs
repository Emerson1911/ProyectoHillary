using Microsoft.EntityFrameworkCore;
using ProyectoHillary1.Models.Dal;
using ProyectoHillary1.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProyectoHillaryContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("cnn")));
builder.Services.AddScoped<RolDal>();
builder.Services.AddScoped<EmpresaDal>();
builder.Services.AddScoped<UsuarioDal>();
builder.Services.AddScoped<EmailGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
