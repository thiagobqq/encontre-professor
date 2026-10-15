using Backend.Repositories;
using Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IProfessorRepository>(_ =>
    new JsonProfessorRepository(Path.Combine(AppContext.BaseDirectory, "professores.json")));
builder.Services.AddSingleton<ValidadorBusca>();
builder.Services.AddSingleton<IBuscaProfessoresService, BuscaProfessoresService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
