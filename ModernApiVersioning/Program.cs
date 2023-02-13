using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(2,1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    //options.ApiVersionReader = new QueryStringApiVersionReader();
    //options.ApiVersionReader = new MediaTypeApiVersionReader("version");
    options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
    //options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1,0))
    .HasApiVersion(new ApiVersion(2,0))
    .HasApiVersion(new ApiVersion(2,1))
    .HasDeprecatedApiVersion(new ApiVersion(1,0))
    .ReportApiVersions()
    .Build();

app.MapGet("hello", (HttpContext context) => 
{
    var apiVersion = context.GetRequestedApiVersion();
    return $"Hello World! v{apiVersion}";
})
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(1,0);

app.MapGet("hello", () => "Hello World! v2.0")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(2, 0);

app.MapGet("hello", () => "Hello World! v2.1")
    .WithApiVersionSet(versionSet)
    .MapToApiVersion(2, 1);

app.Run();