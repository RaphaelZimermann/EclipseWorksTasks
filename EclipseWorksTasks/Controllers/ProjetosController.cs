using EclipseWorksTasks.Classes;
using EclipseWorksTasks.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EclipseWorksTasks.Controllers;

public class ProjetosController : Controller
{
    private readonly AppDbContext context;

    public ProjetosController(AppDbContext context)
    {
        this.context = context;
    }
    
    [HttpGet("{usuarioId}/projetos/listar")]
    public async Task<ResultadoLista<Projeto>> Listar(int usuarioId)
    {
        try
        {
            var projetos = (await context.ObterProjetos())
                .Where(x => x.usuarioId == usuarioId)
                .OrderBy(x => x.nome)
                .ToArray();
            
            return new ResultadoLista<Projeto>
            {
                result = projetos
            };
        }
        catch (Exception e)
        {
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoLista<Projeto>(e));
        }
    }
    
    [HttpPost("{usuarioId}/projetos/criar")]
    public async Task<ResultadoUnico<Projeto>> Criar(int usuarioId, [FromBody] Projeto projeto)
    {
        try
        {
            await context.Database.BeginTransactionAsync();

            if (projeto.usuarioId != usuarioId)
            {
                throw new Exception(Constantes.Erro_UsuarioProjetoDifere);
            }
            
            projeto.Validar();

            projeto = await context.CriarProjeto(projeto);
            
            await context.Database.CommitTransactionAsync();
            
            return new ResultadoUnico<Projeto>
            {
                result = projeto
            };
        }
        catch (Exception e)
        {
            await context.Database.RollbackTransactionAsync();
            
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoUnico<Projeto>(e));
        }
    }
    
    [HttpDelete("{usuarioId}/projetos/excluir/{projetoId}")]
    public async Task<ResultadoBase> Excluir(int usuarioId, int projetoId)
    {
        try
        {
            var projeto = (await context.ObterProjetos())
                .Where(x => x.id == projetoId)
                .FirstOrDefault();

            if (projeto == null)
            {
                throw new Exception(string.Format(Constantes.Erro_ProjetoInexistente, projetoId));
            }
            
            if (projeto.usuarioId != usuarioId)
            {
                throw new Exception(Constantes.Erro_UsuarioProjetoDifere);
            }

            var qtTarefas = (await context.ObterTarefas())
                .Where(x => x.projetoId == projetoId && x.status == Tarefa.STATUS_PENDENTE)
                .Count();

            if (qtTarefas > 0)
            {
                var s = qtTarefas > 1 ? "s" : "";

                throw new Exception(
                    $"O projeto ainda possui {qtTarefas} tarefa{s} pendente{s}. Conclua ou remova as tarefas antes de excluir o projeto");
            }
                
            
            await context.ExcluirProjeto(projeto);
 
            return new ResultadoBase();
            
        }
        catch (Exception e)
        {
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoBase(e));
        }
    }
    
    [HttpPost("{userId}/projetos/atualizar")]
    public async Task<ResultadoUnico<Projeto>> Atualizar(int userId, [FromBody] Projeto projeto)
    {
        try
        {
            await context.Database.BeginTransactionAsync();
            
            projeto.Validar();
            
            if ((await context.ObterProjetos()).Any(x => x.id == projeto.id))
            {
                if (projeto.usuarioId != userId)
                {
                    throw new Exception(Constantes.Erro_UsuarioProjetoDifere);
                }
                
                projeto = await context.AtualizarProjeto(projeto);
                
                await context.Database.CommitTransactionAsync();
                
                return new ResultadoUnico<Projeto>
                {
                    result = projeto
                };
            }
            else
            {
                throw new Exception(string.Format(Constantes.Erro_ProjetoInexistente, projeto.id));
            }
            
        }
        catch (Exception e)
        {
            await context.Database.RollbackTransactionAsync();
            
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoUnico<Projeto>(e));
        }
    }
    
    /*
     para caso seja necessário...
     
     [HttpGet("{usuarioId}/projetos/obter/{projetoId}")]
    public async Task<ResultadoUnico<Projeto>> Obter(int usuarioId, int projetoId, [FromQuery] bool listarTarefas)
    {
        try
        {
            var projeto = (await context.GetProjects())
                .Where(x => x.id == projetoId)
                .FirstOrDefault();

            if (projeto == null)
            {
                throw new Exception(string.Format(Constants.Error_ProjectNotFound, projetoId));
            }

            var usuario = (await context.ObterUsuarios())
                .Where(x => x.id == usuarioId)
                .FirstOrDefault();

            if (usuario.IsGerente || usuarioId == projeto.usuarioId)
            {
                if (listarTarefas)
                {
                    
                }
            }
            else
            {
                throw new Exception("Você não possui permissão para acessar o projeto.");
            }
            
            return new ResultadoUnico<Projeto>
            {
                result = projeto
            };
        }
        catch (Exception e)
        {
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoUnico<Projeto>(e));
        }
    }
    */
   

    
}