using Microsoft.EntityFrameworkCore;
using EclipseWorksTasks.Models;

// Desafio: https://meteor-ocelot-f0d.notion.site/NET-C-5281edbec2e4480d98552e5ca0242c5b

{

    var builder = WebApplication.CreateBuilder(args);

    // Swagger para um bom entendimento da equipe
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Prefiro trabalhar com Controllers
    builder.Services.AddControllers();

    // Adicionar DbContext com MySQL
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            new MySqlServerVersion(new Version(8, 0, 33)) // Substitua pela versão do seu MySQL
        ));

    var app = builder.Build();

// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}