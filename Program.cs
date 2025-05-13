
using ProductApp_API;
using ProductApp_API.Authentications;
using ProductApp_API.Authorization;
using ProductApp_API.Data;
using ProductApp_API.Filters;
using ProductApp_API.MiddleWares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//builder.Configuration.AddJsonFile("config.json");
// Add services to the container.


builder.Services.AddLogging(cfg =>
{
    cfg.AddDebug();
});
builder.Services.Configure<AttachmentOptions>(builder.Configuration.GetSection("Attachments"));
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("AgeGreaterThan25", builder =>
    //{
    //    builder.RequireAssertion(context =>
    //    {
    //        var dob = DateTime.Parse(context.User.FindFirstValue("DateOfBirth"));
    //        return DateTime.Today.Year - dob.Year > 25;
    //    });
    //});
    //options.AddPolicy("EmployeeOnly", builder =>
    //{
    //    builder.RequireClaim("UserType", "Employee");
    //});

    options.AddPolicy("AgeGreaterThan25", builder =>
      builder.AddRequirements(new AgeGreaterThan25Requirement()));
});
builder.Services.AddSingleton<IAuthorizationHandler, AgeAuthorizationHandler>();


//var attachmentOptions=builder.Configuration.GetSection("Attachments").Get<AttachmentOptions>();
//builder.Services.AddSingleton(attachmentOptions);

//var attachmentOptions = new AttachmentOptions();
//builder.Configuration.GetSection("Attachments").Bind(attachmentOptions);
//builder.Services.AddSingleton(attachmentOptions);
builder.Services.AddControllers(Options => 
{
    Options.Filters.Add<LogActivityFilter>();
    Options.Filters.Add<PermissionBasedAuthorizationFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(cfg => cfg.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
        };
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseMiddleware<ProfilingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



//var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
//builder.Services.AddSingleton(jwtOptions);
//builder.Services.AddAuthentication()
//    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
//    {
//        options.SaveToken = true;
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = jwtOptions.Issuer,
//            ValidateAudience = true,
//            ValidAudience = jwtOptions.Audience,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
//        };
//    });
