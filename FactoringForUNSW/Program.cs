using Microsoft.Azure.Cosmos.Fluent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton((s) => {
    string endpoint = builder.Configuration["CosmoDB:URI"];
    if (string.IsNullOrEmpty(endpoint))
    {
        throw new ArgumentNullException("Please specify a valid endpoint in the appSettings.json file or your Azure Functions Settings.");
    }

    string authKey = builder.Configuration["CosmoDB:PrimaryKey"];
    if (string.IsNullOrEmpty(authKey) || string.Equals(authKey, "Super secret key"))
    {
        throw new ArgumentException("Please specify a valid AuthorizationKey in the appSettings.json file or your Azure Functions Settings.");
    }

    CosmosClientBuilder configurationBuilder = new CosmosClientBuilder(endpoint, authKey);
    return configurationBuilder
            .Build();
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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
