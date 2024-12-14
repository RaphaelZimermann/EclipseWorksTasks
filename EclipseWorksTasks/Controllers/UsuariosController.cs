using EclipseWorksTasks.Classes;
using EclipseWorksTasks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EclipseWorksTasks.Controllers;


public class UsuariosController : Controller
{
    private readonly AppDbContext context;

    public UsuariosController(AppDbContext context)
    {
        this.context = context;
    }
    
    [HttpGet("usuarios/listar")]
    public async Task<ResultadoLista<Usuario>> Listar()
    {
        try
        {
            var usuarios = (await context.ObterUsuarios())
                .OrderBy(x => x.nome)
                .ToArray();
            
            return new ResultadoLista<Usuario>
            {
                result = usuarios
            };
        }
        catch (Exception e)
        {
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoLista<Usuario>(e));
        }
    }
    
    /*
     não necessário mas está aqui...
     
     [HttpGet("usuarios/obter/{id}")]
    public async Task<ResultadoUnico<Usuario>> Obter(int id)
    {
        try
        {
            var user = (await context.ObterUsuarios())
                .Where(x => x.id == id)
                .FirstOrDefault();

            if (user == null)
            {
                throw new Exception(string.Format(Constants.Error_UserNotFound, id));
            }
            
            return new ResultadoUnico<Usuario>
            {
                result = user
            };
        }
        catch (Exception e)
        {
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoUnico<Usuario>(e));
        }
    }*/
}