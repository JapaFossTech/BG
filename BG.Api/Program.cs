using BG.Api.gRPC;
using BG.Repo.EF;
using BG.Repo.EF.Core.GraphQL;
using BG.Serv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PrjBase.SecurityBase;
using RestBase.Swagger;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Swashbuckle.AspNetCore.Annotations;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .ClearProviders()
    .AddSimpleConsole(
    //options =>
    //{
    //    options.SingleLine = true;
    //    options.TimestampFormat = "HH:mm:ss";
    //    options.UseUtcTimestamp = true;
    //}
    )
    //.AddJsonConsole( // Exercise 7.5.1
    //    options => {
    //        options.TimestampFormat = "HH:mm"; // Excercise 7.5.2
    //        options.UseUtcTimestamp = true; // Excercise 7.5.2
    //    }
    //)
    .AddDebug()
    //.AddApplicationInsights(telemetry => telemetry
    //    .ConnectionString = builder
    //        .Configuration["Azure:ApplicationInsights:ConnectionString"], //secrets.json
    //    loggerOptions => { })
    ;

//* Use Serilog

builder.Host.UseSerilog(
    (hostBuilderContext, logConfig) =>
    {
        logConfig.ReadFrom.Configuration(hostBuilderContext.Configuration);
        logConfig.Enrich.WithMachineName();
        logConfig.Enrich.WithThreadId();
        logConfig.Enrich.WithThreadName(); // Excercise 7.5.4
        logConfig.WriteTo.File(
                "Logs/log.txt",
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] " +
                            "[{MachineName} #{ThreadId} "
                            + "{ThreadName}"                         // Excercise 7.5.4
                            + "] {Message:lj}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day
            );

        logConfig.WriteTo.File(
            "Logs/errors.txt", // Excercise 7.5.5
            outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] " +
                    "[{MachineName} #{ThreadId} {ThreadName}] " +
                    "{Message:lj}{NewLine}{Exception}",
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
            rollingInterval: RollingInterval.Day);

        logConfig.WriteTo.MSSqlServer(
            connectionString:
                hostBuilderContext.Configuration.GetConnectionString("BGConnString"),
            sinkOptions: new MSSqlServerSinkOptions
            {
                TableName = "LogEvents",
                AutoCreateSqlTable = true
            },
            columnOptions: new ColumnOptions()
            {
                AdditionalColumns = new SqlColumn[]
                {
                    new SqlColumn()
                    {
                        ColumnName = "SourceContext",
                        PropertyName = "SourceContext",
                        DataType = System.Data.SqlDbType.NVarChar
                    }
                }
            }
            );
    }
    , writeToProviders: true
    );

// Add services to the container.

builder.Services.AddCors(setupAction: options =>
{
    options.AddDefaultPolicy(configurePolicy: policyBuilder =>
    {
        string origins = builder.Configuration["AllowedOrigins"] ?? "*";
        policyBuilder.WithOrigins(origins);
        //policyBuilder.WithOrigins("https://localhost:5000");
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyHeader();
    });
    options.AddPolicy(name: "AnyOrigin", configurePolicy: policyBuilder => {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
    });
});

//builder.Services.AddControllers();
//builder.Services.AddControllers()
//    .AddJsonOptions(jsonOptions =>
//        jsonOptions.JsonSerializerOptions.ReferenceHandler =
//            ReferenceHandler.IgnoreCycles
//            //ReferenceHandler.Preserve
//        )
//    ;

builder.Services.AddControllers(options =>
    {
        options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
            (x) => $"The value '{x}' is invalid.");
        options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
            (x) => $"The field {x} must be a number.");
        options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
            (x, y) => $"The value '{x}' is not valid for {y}.");
        options.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(
            () => $"A value is required.");

        options.CacheProfiles.Add("NoCache", new CacheProfile() { NoStore = true });
        options.CacheProfiles.Add("Any-60", new CacheProfile()
        {
            Location = ResponseCacheLocation.Any,
            Duration = 60
        }
        );
    })
    .AddJsonOptions(jsonOptions =>
        jsonOptions.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles
        //ReferenceHandler.Preserve
        )
;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    string? xmlFilename =
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    string? xmlComments_FullPath = System.IO.Path
                                    .Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlComments_FullPath);

    options.EnableAnnotations();

    options.ParameterFilter<SortColumnParamFilter>();
    options.ParameterFilter<SortOrderParamFilter>();

    options.AddSecurityDefinition(
        "Bearer"
        , new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        }
    );

    //* removed: To show the padlock only on methods with the Authorize attribute (CH 11)
    //var securityRequirement = new OpenApiSecurityRequirement();
    //securityRequirement.Add(
    //    key: new OpenApiSecurityScheme
    //    {
    //        Reference = new OpenApiReference
    //        {
    //            Type = ReferenceType.SecurityScheme,
    //            Id = "Bearer"
    //        }
    //    }
    //    , value: Array.Empty<string>()
    //);

    //options.AddSecurityRequirement(securityRequirement);

    //* To show the authorized padlock in Swagger (CH 11)
    options.OperationFilter<AuthRequirementFilter>();

    options.DocumentFilter<CustomDocumentFilter>();
    options.RequestBodyFilter<PasswordRequestFilter>();
    options.SchemaFilter<CustomKeyValueFilter>();
});

builder.Services.AddBGServices_ForEF();

//builder.Services.Configure<ApiBehaviorOptions>(options =>
//                                options.SuppressModelStateInvalidFilter = true);

builder.Services.AddDbContext<BGDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BGConnString"))
    );

builder.Services.AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<QueryType>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

builder.Services.AddGrpc();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    //options.Password.RequireLowercase = true;
    //options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 7;
})
    .AddEntityFrameworkStores<BGDbContext>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
        options.DefaultChallengeScheme =
        options.DefaultForbidScheme =
        options.DefaultScheme =
        options.DefaultSignInScheme =
        options.DefaultSignOutScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
              System.Text.Encoding.UTF8.GetBytes(
                  builder.Configuration["JWT:SigningKey"] 
                    ?? "ChangeThisInAppSettings123!@#"
                  )
            )
        };
    }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ModeratorWithMobilePhone", policy =>
        policy
            .RequireClaim(ClaimTypes.Role, RoleName.MODERATOR)
            .RequireClaim(ClaimTypes.MobilePhone));

    options.AddPolicy("MinAge18", policy =>
        policy
            .RequireAssertion(ctx =>
                ctx.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth)
                && DateTime.ParseExact(
                    "yyyyMMdd",
                    ctx.User.Claims.First(c =>
                        c.Type == ClaimTypes.DateOfBirth).Value,
                    System.Globalization.CultureInfo.InvariantCulture)
                    >= DateTime.Now.AddYears(-18)));
});

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 32 * 1024 * 1024;
    options.SizeLimit = 50 * 1024 * 1024;
});

builder.Services.AddMemoryCache();

//* SQL Server Distributed Cache
// --------------------------------------
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration
                                .GetConnectionString("BGConnString");
    options.SchemaName = "dbo";
    options.TableName = "AppCache";
    //* To create the AppCache table in the database
    //dotnet tool install --global dotnet-sql-cache --version 8.0
    //dotnet sql-cache create "{connectionString}" dbo AppCache
    //dotnet sql-cache create "Server=localhost\SQLEXPRESS;Database=BG;Trusted_Connection=True;Connection Timeout=180;TrustServerCertificate=True;" dbo AppCache
});

//* Redis Distributed Cache
// --------------------------------------
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration["Redis:Configuration"];
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
    // HTTP Security Headers
    app.UseHsts();
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Frame-Options",
            "sameorigin");
        context.Response.Headers.Append("X-XSS-Protection",
            "1; mode=block");
        context.Response.Headers.Append("X-Content-Type-Options",
            "nosniff");
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; script-src 'self' 'nonce-23a98b38c'");
        context.Response.Headers.Append("Referrer-Policy",
            "strict-origin");
        await next();
    });
}

if (app.Configuration.GetValue<bool>("UseDeveloperExceptionPage"))
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/error");

    //app.UseExceptionHandler("/error");
    //app.UseExceptionHandler(appBuilder =>
    //{
    //    appBuilder.Run(async requestDelegate =>
    //    {
    //        IExceptionHandlerPathFeature? exceptionHandler = requestDelegate.Features
    //                            .Get<IExceptionHandlerPathFeature>();

    //        var details = new ProblemDetails();

    //        details.Detail = exceptionHandler?.Error.Message;
    //        details.Extensions["traceId"] = System.Diagnostics
    //                                        .Activity.Current?.Id 
    //                                        ?? requestDelegate.TraceIdentifier;
    //        details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
    //        details.Status = StatusCodes.Status500InternalServerError;

    //        await requestDelegate.Response.WriteAsync(
    //            System.Text.Json.JsonSerializer.Serialize(details));
    //    });
    //});
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();
//app.MapGraphQL("/graphql");

app.MapGrpcService<GrpcServices>();

//* Adds a default cache-control directive

app.Use((context, next) =>
{
    context.Response.GetTypedHeaders().CacheControl =
            new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
            {
                NoCache = true,
                NoStore = true
            };
    return next.Invoke();
});

// Minimal API
//app.MapGet(pattern: "/error"
//    , [EnableCors("AnyOrigin")][ResponseCache(NoStore = true)]
//    () => Results.Problem()
//);
app.MapGet("/error",
    [EnableCors("AnyOrigin")] [ResponseCache(NoStore = true)] 
    (HttpContext context) =>
    {
        IExceptionHandlerPathFeature? exceptionHandler = context.Features
                                .Get<IExceptionHandlerPathFeature>();

        var details = new ProblemDetails();

        details.Detail = exceptionHandler?.Error.Message;
        details.Extensions["traceId"] = System.Diagnostics
                                        .Activity.Current?.Id ?? context.TraceIdentifier;
        details.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
        details.Status = StatusCodes.Status500InternalServerError;

        //* Next line is causing duplicate error logging
        //app.Logger.LogError(
        //    50001,
        //    exceptionHandler?.Error,
        //    "An unhandled exception occurred: "
        //    + "{errorMessage}.", exceptionHandler?.Error.Message  // Exercise 7.5.3
        //    ); 

        return Results.Problem(details);
    });

//.RequireCors("AnyOrigin");
app.MapGet(pattern: "/error/test", 
    [EnableCors("AnyOrigin")] 
    () => { throw new Exception("test"); }
    );
//.RequireCors("AnyOrigin");

app.MapGet(pattern: "/cod/test",
    [EnableCors("AnyOrigin")]
    [ResponseCache(NoStore = true)] 
    () =>
    Results.Text("<script>" +
        "window.alert('Your client supports JavaScript!" +
        "\\r\\n\\r\\n" +
        $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
        "\\r\\n" +
        "Client time (UTC): ' + new Date().toISOString());" +
        "</script>" +
        "<noscript>Your client does not support JavaScript</noscript>",
        "text/html"));

app.MapGet("/cache/test/1",
    [EnableCors("AnyOrigin")]
    (HttpContext context) =>
    {
        context.Response.Headers["cache-control"] =
            "no-cache, no-store";
        return Results.Ok();
    });

app.MapGet("/cache/test/2",
    [EnableCors("AnyOrigin")]
    (HttpContext context) =>
    {
        return Results.Ok();
    });

//* Authorization Testing

app.MapGet("/auth/test/1",
    [Authorize]
    [EnableCors("AnyOrigin")]
    [SwaggerOperation(
        Tags = new string[] { "Auth" },
        //Tags = ["Auth"],
        Summary = "Auth test #1 (authenticated users).",
        Description = "Returns 200 - OK if called by " +
        "an authenticated user regardless of its role(s).")]
    [SwaggerResponse(StatusCodes.Status200OK, "Authorized")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Not authorized")]
    [ResponseCache(NoStore = true)]
    () =>
    {
        return Results.Ok("You are authorized!");
    });

app.MapGet("/auth/test/2",
    [Authorize(Roles = RoleName.MODERATOR)]
    [EnableCors("AnyOrigin")]
    [SwaggerOperation(
        Tags = new[] { "Auth" },
        Summary = "Auth test #2 (Moderator role).",
        Description = "Returns 200 - OK status code if called by " +
        "an authenticated user assigned to the Moderator role.")]
    [ResponseCache(NoStore = true)] () =>
    {
        return Results.Ok("You are authorized!");
    });

app.MapGet("/auth/test/3",
    [Authorize(Roles = RoleName.ADMIN)]
    [EnableCors("AnyOrigin")]
    [SwaggerOperation(
        Tags = new[] { "Auth" },
        Summary = "Auth test #3 (Administrator role).",
        Description = "Returns 200 - OK if called by " +
        "an authenticated user assigned to the Administrator role.")]
    [ResponseCache(NoStore = true)] () =>
    {
        return Results.Ok("You are authorized!");
    });

// Controllers
app.MapControllers().RequireCors("AnyOrigin");

app.Run();
