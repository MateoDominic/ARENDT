using Fleck;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.AutoMapper;
using WebApi.Models;
using WebApi.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
  x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
option.SwaggerDoc("v1", new OpenApiInfo { Title = "PRA Web API", Version = "v1" });

option.AddSecurityDefinition("Bearer",
    new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter valid JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });
});
var secureKey = builder.Configuration["JWT:SecureKey"];
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        var Key = Encoding.UTF8.GetBytes(secureKey);
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Key)
        };
    });

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IDbService, DbServices>();
builder.Services.AddScoped<IConnectionHandler, ConnectionHandler>();


builder.Services.AddDbContext<PraContext>(options => {
    options.UseSqlServer("server=.;Database=PRA;User=sa;Password=neznam;TrustServerCertificate=True;MultipleActiveResultSets=true");
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);

app.MapControllers();

var serviceScope = app.Services.CreateScope();
var services = serviceScope.ServiceProvider;
IConnectionHandler connectionHandler = services.GetRequiredService<IConnectionHandler>();

var websocketServer = new WebSocketServer("ws://0.0.0.0:8181");
websocketServer.Start(connection =>
{
    connection.OnOpen = () =>
    {
        Console.WriteLine("OnOpen");
        Console.WriteLine(connection.ConnectionInfo.Id);
        connection.ConnectionInfo.Headers.TryGetValue("SessionCode", out string sessionCode);
        Console.WriteLine(sessionCode);
        connectionHandler.HandleConnection(connection);
    };
    connection.OnClose = () =>
      connectionHandler.HandleClose(connection);
    connection.OnMessage = message =>
    {
        connectionHandler.HandleMessage(connection, message);
    };
});

app.Run();
