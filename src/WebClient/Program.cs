using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = "Cookies";
	options.DefaultChallengeScheme = "oidc";
}).AddCookie("Cookies", options =>
{
		options.Cookie.Name = "demo";
		options.ExpireTimeSpan = TimeSpan.FromHours(8);
}).AddOpenIdConnect("oidc", options =>
	{
		options.Authority = "https://localhost:5001";
		options.ClientId = "web";
		options.ClientSecret = "secret";
		options.ResponseType = "code";
		options.Scope.Clear();
		options.Scope.Add("openid");
		options.Scope.Add("profile");
		options.Scope.Add("verification");
		options.GetClaimsFromUserInfoEndpoint = true;
        options.ClaimActions.MapJsonKey("email_verified", "email_verified");
        options.Scope.Add("api1");
        options.Scope.Add("offline_access");
        options.SaveTokens = true;
		options.Events = new OpenIdConnectEvents
        {
            OnRedirectToIdentityProvider = context =>
            {
                context.ProtocolMessage.SetParameter("acr_values", "mfa");
                return Task.FromResult(0);
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();
