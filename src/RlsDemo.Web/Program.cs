using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RlsDemo.Web;
using RslDemo.Context;
using Softfluent.Asapp.Core.Data;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<RlsDemoContext>((provider, options) =>
{
	options.UseSqlServer(configuration.GetConnectionString(Environment.MachineName));
});
// Add Asapp repository
builder.Services.AddBaseRepository();

// Add Controllers and OpenApi
builder.Services.AddControllers()
	.AddJsonOptions(configure => configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "oauth2",
				Name = "Bearer",
				In = ParameterLocation.Header,
			},
			new List<string>()
		}
	});
});

// Add Authentification
var secret = Encoding.ASCII.GetBytes(configuration.GetValue<string>("Authentication:JwtToken:Secret", "TheB€stKept_secretIn-the*World")!);
builder.Services.AddAuthentication(configure =>
{
	configure.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	configure.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
	.AddJwtBearer(configure =>
	{
		configure.SaveToken = true;
		configure.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(secret),
			ValidateIssuer = false,
			ValidateAudience = false
		};
	});
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperConfiguration));

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

// Configure Authentification
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
