using Microsoft.EntityFrameworkCore;
using RslDemo.Context;
using Softfluent.Asapp.Core.Data;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<RlsDemoContext>((provider, options) =>
{
	options.UseSqlServer(configuration.GetConnectionString(Environment.MachineName));
});

builder.Services.AddBaseRepository();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	using IServiceScope scope = app.Services.CreateScope();
	var context = scope.ServiceProvider.GetRequiredService<RlsDemoContext>();
	context.Database.EnsureDeleted();
	context.Database.EnsureCreated();
	app.UseSwagger();
	app.UseSwaggerUI(configure => configure.EnableTryItOutByDefault());
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
