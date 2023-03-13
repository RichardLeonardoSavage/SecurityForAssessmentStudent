using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecurityForAssessmentStudent.Data;

var CORSAllowSpecificOrigins = "CORSAllowed";
var builder = WebApplication.CreateBuilder(args);



//add CORS to project
builder.Services.AddCors(options =>
{
    options.AddPolicy(name :CORSAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "http://www.contoso.com");
        });
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();



builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.SignIn.RequireConfirmedEmail = false;
    
    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

//The AddPolicy method takes the name of the policy, and an AuthosizationPolicyBuilder which has a RequireRole methpd, enaling us to state which roles are required.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RoleAdminPolicy", policyBuilder => policyBuilder.RequireRole("Admin"));
    options.AddPolicy("ClaimAdminPolicy", policyBuilder => policyBuilder.RequireClaim("Admin"));

    //We use the RequireAssertion method, which takes an AuthorizationHandlerContext as a parameter providing access to the current user
    options.AddPolicy("ViewRolesPolicy", policyBuilder => policyBuilder.RequireAssertion(context =>
    {
        //We use the FindFirst method to access a claim and obrain its value(if there is one) and convert it to a DateTime
        var joiningDateCalim = context.User.FindFirst(c => c.Type == "Joining Date")?.Value;
        var joiningDate = Convert.ToDateTime(joiningDateCalim);

        //We use the HasClaim method to establish that a claim with a specified value exists
        //We compare the joining date value with DateTime.MinValue and the current date to ensure that the claim is not null, and that the date is earlier than six months ago
        return context.User.HasClaim("Permission", "View Roles") && joiningDate > DateTime.MinValue && joiningDate < DateTime.Now.AddMonths(-6);
    }));
});
//Having configured the policy named AdminPolicy, we can apply it to the AuthorizeFolder method to ensure that only members of the Admin role can access the content:
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/RolesManager", "ViewRolesPolicy");
    //options.Conventions.AuthorizeFolder("/ClaimsManager", "ViewClaimsPolicy");
    //options.Conventions.AuthorizeFolder("/RolesManager", "AdminPolicy");
});

builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors(CORSAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.UseRouting();

app.Run();
