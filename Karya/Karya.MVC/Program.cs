using Karya.MVC.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "cookie";
    options.DefaultChallengeScheme = "oidc";
}).AddCookie("cookie")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = configuration["InteractiveServiceSettings:AuthorityUrl"];
    options.ClientId = configuration["InteractiveServiceSettings:ClientId"];
    options.ClientSecret = configuration["InteractiveServiceSettings:ClientSecret"];
    options.Scope.Add(configuration["InteractiveServiceSettings:Scopes:0"]);
    options.ResponseType = "code";
    options.UsePkce = true;
    options.ResponseMode = "query";
    options.SaveTokens = true;
});

builder.Services.Configure<IdentityServerSettings>(builder.Configuration.GetSection("IdentityServerSettings"));
builder.Services.AddSingleton<ITokenService, TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
