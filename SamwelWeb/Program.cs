using Microsoft.EntityFrameworkCore;
using Samwel.DataAccess.Data;
using Samwel.DataAccess.Repository;
using Samwel.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Samwel.Utility;
using Stripe;
using Stripe.Identity;
using Samwel.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration
    .GetConnectionString("AppDbContextConnection") 
    ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");
// Add services to the container.
builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("Stripe"));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(op =>
{
    op.IdleTimeout = TimeSpan.FromSeconds(100);   
    op.Cookie.HttpOnly = true;  
    op.Cookie.IsEssential = true;   
}
);
builder.Services.AddAuthentication().AddFacebook(opt=>
{
    opt.AppId = "360767686413676";
    opt.AppSecret = "501ee33acb100ba9f1600ed06c2e2491";
});

builder.Services.AddAuthentication().AddMicrosoftAccount(opt =>
{
    opt.ClientId = "d76f2a50-047d-4dba-8734-21192f353d8f";
    opt.ClientSecret = "yKS8Q~1vXSAh2CCuTateucf_PPo2jeq1187GFcs~";
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();   
builder.Services.AddScoped<ICategoryRepository
    , CategoryRepository>();
builder.Services.AddScoped<IUnitOfWork
    , UnitOfWork>();
builder.Services.AddScoped<IEmailSender
    , EmailSender>();
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    var Constr = builder.Configuration["ConStr"];
    builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(Constr));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseSqlServer(connectionString));
}

//connectionString

builder.Services.AddScoped<IDbInitializer, DbInitializer>();    
builder.Services.AddIdentity<IdentityUser , IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.LoginPath = "/Identity/Account/Login";
    opt.LogoutPath = "/Identity/Account/Logout";
    opt.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
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
StripeConfiguration.ApiKey
    = builder.Configuration.GetSection("Stripe:Secretkey").Get<string>();
app.UseRouting();
app.UseAuthentication();    
app.UseAuthorization();
app.UseSession();
SeedDatabase(); 
app.MapRazorPages();    
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void SeedDatabase()
{
    using (var scope =app.Services.CreateScope() )
    {
       var db =  scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        db.Initialize(); 
    }
}