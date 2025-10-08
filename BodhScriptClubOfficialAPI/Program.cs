using BodhScriptClubOfficialAPI.DbLayer;
using BodhScriptClubOfficialAPI.Repositories;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Register dependencies (DbLayer + Repo)
builder.Services.AddSingleton<DbLayer>();
builder.Services.AddScoped<Repo>();

var app = builder.Build();

// ✅ Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run(); // ✅ this is the last line — nothing below this
