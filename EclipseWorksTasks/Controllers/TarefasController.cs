using EclipseWorksTasks.Classes;
using EclipseWorksTasks.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EclipseWorksTasks.Controllers;


public class TarefasController : Controller
{
    private readonly AppDbContext context;

    public TarefasController(AppDbContext context)
    {
        this.context = context;
    }
    
    [HttpGet("{usuarioId}/tarefas/{projetoId}/listar")]
    public async Task<ResultadoLista<Tarefa>> Listar(int usuarioId, int projetoId)
    {
        try
        {
            var usuario = (await context.ObterUsuarios())
                .Where(x => x.id == usuarioId)
                .FirstOrDefault();

            if (usuario == null)
            {
                throw new Exception(string.Format(Constantes.Erro_UsuarioInexistente, usuarioId));
            }

            var infoProjeto = (await context.ObterProjetos())
                .Where(x => x.id == projetoId)
                .Select(x => new
                {
                    x.usuarioId
                })
                .FirstOrDefault();
            
            if (infoProjeto == null)
            {
                throw new Exception(string.Format(Constantes.Erro_ProjetoInexistente, projetoId));
            }

            if (infoProjeto.usuarioId != usuarioId)
            {
                if (!usuario.IsGerente)
                {
                    throw new Exception("Você não tem permissão para acessar as tarefas do projeto");
                }
            }
            
            var tarefas = (await context.ObterTarefas())
                .Where(x => x.projetoId == projetoId)
                .OrderBy(x => x.vencimento)
                .ToArray();
            
            return new ResultadoLista<Tarefa>
            {
                result = tarefas
            };
        }
        catch (Exception e)
        {
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoLista<Tarefa>(e));
        }
    }


    protected async Task VerificaTarefa(int usuarioId, int projetoId, Tarefa tarefa, bool criando)
    {
        tarefa.Validar();

        tarefa.titulo = (tarefa.titulo ?? "").Trim();
        tarefa.descricao = (tarefa.descricao ?? "").Trim();

        if (tarefa.status == Tarefa.STATUS_CONCLUIDA)
        {
            if (!tarefa.dataConclusao.HasValue)
            {
                tarefa.dataConclusao = DateOnly.FromDateTime(DateTime.Now);
            }
        }
        else
        {
            tarefa.dataConclusao = null;
        }
        
        var qrTarefas = (await context.ObterTarefas());

        var projeto = (await context.ObterProjetos())
            .Where(x => x.id == projetoId)
            .Select(x => new
            {
                x.usuarioId,
                QtdeTarefas = qrTarefas.Count(t => t.projetoId == x.id)

            }).FirstOrDefault();

        if (projeto == null)
        {
            throw new Exception(string.Format(Constantes.Erro_ProjetoInexistente, projetoId));
        }

        if (tarefa.projetoId != projetoId)
        {
            throw new Exception("Projeto informado na tarefa não corresponde com o projeto informado");
        }

        var usuario = (await context.ObterUsuarios())
            .Where(x => x.id == usuarioId)
            .FirstOrDefault();

        if (usuarioId != projeto.usuarioId && !usuario.IsGerente)
        {
            throw new Exception(
                $"Você não tem permissão para {(criando ? "criar" : "alterar")} tarefas neste projeto");
        }

        if (criando)
        {
            if (projeto.QtdeTarefas >= 20)
            {
                throw new Exception("Quantidade máxima de tarefas por projeto foi excedida");
            }
        }
        else
        {
            var oldTarefa = (await context.ObterTarefas())
                .Where(x => x.id == tarefa.id)
                .Select(x => new
                {
                    x.prioridade,
                    x.usuarioCriouId,
                    x.projetoId,
                    x.titulo,
                    x.descricao,
                    x.vencimento,
                    x.status

                }).FirstOrDefault();

            if (tarefa.prioridade != oldTarefa.prioridade)
            {
                throw new Exception("Prioridade da tarefa não pode ser alterada");
            }

            if (tarefa.usuarioCriouId != oldTarefa.usuarioCriouId)
            {
                throw new Exception("Usuário que criou a tarefa não pode ser alterado");
            }

            if (tarefa.projetoId != oldTarefa.projetoId)
            {
                throw new Exception("Projeto da tarefa não pode ser alterado");
            }

            async Task verificaHistorico(string legenda, object vlA, object vlB)
            {
                if (!vlA.Equals(vlB))
                {
                    await context.CriarHistoricoTarefa(new HistoricoTarefa()
                    {
                        tipo = HistoricoTarefa.TIPO_ALTERACAO,
                        dataHora = DateTime.Now,
                        tarefaId = tarefa.id,
                        usuarioId = usuarioId,
                        conteudo = $"'{legenda}' alterado de '{vlA}' para '{vlB}'"
                    });
                }
            }

            await verificaHistorico("Título", oldTarefa.titulo, tarefa.titulo);
            await verificaHistorico("Descrição", oldTarefa.descricao, tarefa.descricao);
            await verificaHistorico("Vencimento", oldTarefa.vencimento.ToString("dd/MM/yyyy"),
                tarefa.vencimento.ToString("dd/MM/yyyy"));
            await verificaHistorico("Status", oldTarefa.status, tarefa.status);
        }


    }


    [HttpPost("{usuarioId}/tarefas/{projetoId}/criar")]
    public async Task<ResultadoUnico<Tarefa>> Criar(int usuarioId, int projetoId, [FromBody] Tarefa tarefa)
    {
        try
        {
            await context.Database.BeginTransactionAsync();

            if (projetoId != tarefa.projetoId)
            {
                throw new Exception("ID de projeto da tarefa não corresponde com o informado");
            }

            await VerificaTarefa(usuarioId, projetoId, tarefa, true);
            
            tarefa = await context.CriarTarefa(tarefa);
            
            await context.Database.CommitTransactionAsync();
            
            return new ResultadoUnico<Tarefa>
            {
                result = tarefa
            };
        }
        catch (Exception e)
        {
            await context.Database.RollbackTransactionAsync();
            
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoUnico<Tarefa>(e));
        }
    }
    
    
    [HttpPost("{usuarioId}/tarefa/{tarefaId}/atualizar")]
    public async Task<ResultadoUnico<Tarefa>> Atualizar(int usuarioId, int tarefaId, [FromBody] Tarefa tarefa)
    {
        try
        {
            await context.Database.BeginTransactionAsync();

            if (tarefaId != tarefa.id)
            {
                throw new Exception("Identificadores da tarefa não correspondem");
            }
            
            await VerificaTarefa(usuarioId, tarefa.projetoId, tarefa, false);
            
            tarefa = await context.AtualizarTarefa(tarefa);
            
            await context.Database.CommitTransactionAsync();
            
            return new ResultadoUnico<Tarefa>
            {
                result = tarefa
            };
        }
        catch (Exception e)
        {
            await context.Database.RollbackTransactionAsync();
            
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoUnico<Tarefa>(e));
        }
    }
    
    [HttpDelete("{usuarioId}/tarefa/{tarefaId}/excluir")]
    public async Task<ResultadoBase> Excluir(int usuarioId, int tarefaId)
    {
        try
        {
            var tarefa = (await context.ObterTarefas())
                .Where(x => x.id == tarefaId)
                .FirstOrDefault();

            if (tarefa == null)
            {
                throw new Exception(string.Format(Constantes.Erro_TarefaInexistente, tarefaId));
            }
            
            var projeto = (await context.ObterProjetos())
                .Where(x => x.id == tarefa.projetoId)
                .Select(x => new
                {
                    x.usuarioId
                }).FirstOrDefault();
            
            var usuario = (await context.ObterUsuarios())
                .Where(x => x.id == usuarioId)
                .FirstOrDefault();

            if (usuarioId != projeto.usuarioId && !usuario.IsGerente)
            {
                throw new Exception($"Você não tem permissão para excluir tarefas neste projeto");
            }
            
            await context.ExcluirTarefa(tarefa);
 
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
    
    public class ComentarTarefaParam
    {
        public string comentario { get; set; }
    }
    
    [HttpPost("{usuarioId}/tarefa/{tarefaId}/comentar")]
    public async Task<ResultadoBase> Comentar(int usuarioId, int tarefaId, [FromBody] ComentarTarefaParam comentario)
    {
        try
        {
            await context.Database.BeginTransactionAsync();

            var com = (comentario?.comentario ?? "").Trim();
            
            if (string.IsNullOrEmpty(com))
            {
                throw new Exception("Comentário inválido");
            }
            
            var tarefa = (await context.ObterTarefas())
                .Where(x => x.id == tarefaId)
                .FirstOrDefault();

            if (tarefa == null)
            {
                throw new Exception(string.Format(Constantes.Erro_TarefaInexistente, tarefaId));
            }

            await context.CriarHistoricoTarefa(new HistoricoTarefa()
            {
                tipo = HistoricoTarefa.TIPO_COMENTARIO,
                dataHora = DateTime.Now,
                usuarioId = usuarioId,
                tarefaId = tarefaId,
                conteudo = com
                
            });
            
            await context.Database.CommitTransactionAsync();
            
            return new ResultadoBase
            {
                
            };
        }
        catch (Exception e)
        {
            await context.Database.RollbackTransactionAsync();
            
            // Prefiro trabalhar com resultados totalmente 
            // controlados. Caso algo saia errado, o programador
            // poderá facilmente fazer um tratamento baseado
            // no padrão do retorno: 'ok' e 'message' 
            return (new ResultadoBase(e));
        }
    }
    
    [HttpGet("{usuarioId}/tarefa/{tarefaId}/historico")]
    public async Task<ResultadoLista<HistoricoTarefa>> Historico(int usuarioId, int tarefaId)
    {
        try
        {
            var qrProjetos = (await context.ObterProjetos());

            var tarefa = (from t in (await context.ObterTarefas())
                join p in qrProjetos on t.projetoId equals p.id
                where t.id == tarefaId
                select new
                {
                    p.usuarioId
                    
                }).FirstOrDefault();

            if (tarefa == null)
            {
                throw new Exception(string.Format(Constantes.Erro_TarefaInexistente, tarefaId));
            }

            if (tarefa.usuarioId != usuarioId)
            {
                var usuario = (await context.ObterUsuarios())
                    .Where(x => x.id == usuarioId)
                    .FirstOrDefault();

                if (!usuario.IsGerente)
                {
                    throw new Exception("Apenas gerentes possuem acesso a tarefas de outros usuários");
                }
            }

            var rst = (await context.ObterHistoricosTarefas())
                .Where(x => x.tarefaId == tarefaId)
                .OrderBy(x => x.dataHora)
                .ToArray();
            
            return new ResultadoLista<HistoricoTarefa>
            {
                result = rst
            };
        }
        catch (Exception e)
        {
            return (new ResultadoLista<HistoricoTarefa>(e));
        }
    }
}