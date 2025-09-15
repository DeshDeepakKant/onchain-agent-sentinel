using api.Interfaces;
using api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient<IHackerNewsClient, HackerNewsClient>(client =>
{
    client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
})
.AddStandardResilienceHandler();

builder.Services.AddScoped<IStoriesService, StoriesService>();

const string CorsPolicy = "FrontendOnly";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins(
            "http://localhost:4200",
            "https://hacker-news-assessment.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new { ok = true, ts = DateTimeOffset.UtcNow }));

app.Run();
