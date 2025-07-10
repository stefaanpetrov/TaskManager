using Microsoft.EntityFrameworkCore;
using TaskManager.Data;

var builder = WebApplication.CreateBuilder(args);

// Конфигурација на SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Додавање тест податоци
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Tasks.Any())
    {
        context.Tasks.Add(new TaskManager.Models.TaskItem { Title = "Test Task 1", IsDone = false });
        context.Tasks.Add(new TaskManager.Models.TaskItem { Title = "Test Task 2", IsDone = true });
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 👉 Ова е default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tasks}/{action=Index}/{id?}");

// ✅ Додади редирекција за root "/"
app.MapGet("/", context =>
{
    context.Response.Redirect("/Tasks");
    return Task.CompletedTask;
});

app.Run();

