using Dapper.FluentMap;
using ExpensesApp;
using ExpensesApp.Models.Mapper;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddTransient(x => 
    new MySqlConnection(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// Map MySQL columns to C# objects
FluentMapper.Initialize(m => {
    m.AddMap(new PayeeMap());
    m.AddMap(new VendorMap());
    m.AddMap(new DynamicExpenseMap());
    m.AddMap(new StaticExpenseMapper());
    m.AddMap(new ExpensesMap());
});

app.ConfigureApi();

app.Run();
