using FastReport.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using PurchasingSystemApps.Areas.MasterData.Repositories;
using PurchasingSystemApps.Areas.Order.Repositories;
using PurchasingSystemApps.Areas.Transaction.Repositories;
using PurchasingSystemApps.Areas.Warehouse.Repositories;
using PurchasingSystemApps.Data;
using PurchasingSystemApps.Models;
using PurchasingSystemApps.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//Tambahan Baru
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
}).AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSession();

//Script Auto Show Login Account First Time
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

builder.Services.AddMvc(options =>
{
    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
}).AddXmlSerializerFormatters().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

AddScope();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaims>();

#region Areas Master Data
builder.Services.AddScoped<IUserActiveRepository, IUserActiveRepository>();
builder.Services.AddScoped<IPrincipalRepository, IPrincipalRepository>();
builder.Services.AddScoped<IDiscountRepository, IDiscountRepository>();
builder.Services.AddScoped<IMeasurementRepository, IMeasurementRepository>();
builder.Services.AddScoped<ICategoryRepository, ICategoryRepository>();
builder.Services.AddScoped<ITermOfPaymentRepository, ITermOfPaymentRepository>();
builder.Services.AddScoped<IBankRepository, IBankRepository>();
builder.Services.AddScoped<IProductRepository, IProductRepository>();
builder.Services.AddScoped<ILeadTimeRepository, ILeadTimeRepository>();
builder.Services.AddScoped<IInitialStockRepository, IInitialStockRepository>();
builder.Services.AddScoped<IWarehouseLocationRepository, IWarehouseLocationRepository>();
builder.Services.AddScoped<IUnitLocationRepository, IUnitLocationRepository>();
#endregion

#region Areas Order
builder.Services.AddScoped<IPurchaseRequestRepository, IPurchaseRequestRepository>();
builder.Services.AddScoped<IApprovalRepository, IApprovalRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, IPurchaseOrderRepository>();
builder.Services.AddScoped<IQtyDifferenceRequestRepository, IQtyDifferenceRequestRepository>();
#endregion

#region Areas Warehouse
builder.Services.AddScoped<IReceiveOrderRepository, IReceiveOrderRepository>();
builder.Services.AddScoped<IWarehouseRequestRepository, IWarehouseRequestRepository>();
builder.Services.AddScoped<IWarehouseTransferRepository, IWarehouseTransferRepository>();
builder.Services.AddScoped<IQtyDifferenceRepository, IQtyDifferenceRepository>();
#endregion

#region Areas Unit Request
builder.Services.AddScoped<IUnitRequestRepository, IUnitRequestRepository>();
builder.Services.AddScoped<IApprovalRequestRepository, IApprovalRequestRepository>();
#endregion

//Initialize Fast Report
FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Tambahan Baru
app.UseSession();
app.UseAuthentication();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.UseFastReport();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();

    endpoints.MapControllerRoute(
        name: "MyArea",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] {
        #region Area Master Data Menu Role Pengguna
        "Role", "IndexRole", "CreateRole", "DetailRole", "DeleteRole",
        #endregion
        
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }       
}

app.Run();

void AddScope()
{
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IAccountRepository, AccountRepository>();
}