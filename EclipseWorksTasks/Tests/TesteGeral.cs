using System.Net.Http.Headers;
using EclipseWorksTasks.Classes;
using EclipseWorksTasks.Controllers;
using EclipseWorksTasks.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace EclipseWorksTasks.Tests
{

    [TestClass]
    public class TesteGeral
    {
        protected async Task<T> Comunicar<T>(string url, string evento, object dado = null)
        {
            var baseUrl = "https://localhost:7077";
            
            try
            {
                using (var http = new HttpClient())
                {
                    HttpResponseMessage ret = null;

                    if (evento == "get")
                    {
                        ret = await http.GetAsync($"{baseUrl}/{url}");
                    }
                    
                    if (evento == "post")
                    {
                        ret = await http.PostAsync($"{baseUrl}/{url}",
                            new StringContent(JsonConvert.SerializeObject(dado), MediaTypeHeaderValue.Parse("application/json")));
                    }
                    
                    if (evento == "del")
                    {
                        ret = await http.DeleteAsync($"{baseUrl}/{url}");
                    }
                    
                    var cont = await ret.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(cont);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Erro em '{url}': {(e.InnerException ?? e).Message}");
            }
            
        }
        
        [TestMethod]
        public async Task ExecutarTesteGeral()
        {
            var testeIdent = "";
            
            try
            {
                var baseUrl = "https://localhost:7077";
                int maximoTarefas = 20;
                var usuarioId = 1;

                Projeto projeto = null;
                
                // Verifica se projetos estão sendo criados normalmente
                {
                    testeIdent = "Criação de projetos";
                    
                    var ret = await Comunicar<ResultadoUnico<Projeto>>($"{usuarioId}/projetos/criar", "post", new Projeto()
                    {
                        nome = $"Projeto teste {DateTime.Now:dd/MM/yyyy HH:mm:ss}",
                        usuarioId = usuarioId
                    });

                    if (!ret.ok)
                    {
                        throw new Exception(ret.mensagem);
                    }
                    else
                    {
                        projeto = ret.result;
                    }
                }

                // Verifica se projetos estão sendo listados normalmente
                {
                    testeIdent = "Listagem de projetos";
                    
                    var ret = await Comunicar<ResultadoLista<Projeto>>($"{usuarioId}/projetos/listar", "get");

                    if (!ret.ok)
                    {
                        throw new Exception(ret.mensagem);
                    }
                    
                    if (!ret.result.Any(x => x.id == projeto.id))
                    {
                        throw new Exception("O projeto teste criado não foi listado");
                    }
                }

                // Verifica se projetos estão atualizados normalmente
                {
                    testeIdent = "Atualização de projetos";
                    projeto.nome += " (alterado)";
                    
                    var ret = await Comunicar<ResultadoUnico<Projeto>>($"{usuarioId}/projetos/atualizar", "post", projeto);

                    if (!ret.ok)
                    {
                        throw new Exception(ret.mensagem);
                    }
                    
                    var ret2 = await Comunicar<ResultadoLista<Projeto>>($"{usuarioId}/projetos/listar", "get");

                    if (!ret2.ok)
                    {
                        throw new Exception(ret.mensagem);
                    }

                    if ((ret2.result.FirstOrDefault(x => x.id == projeto.id)?.nome ?? "") != projeto.nome)
                    {
                        throw new Exception("O projeto teste criado não foi atualizado corretamente");   
                    }
                }
                
                // Tenta criar tarefas no projeto
                {
                    testeIdent = "Criação de tarefas";
                    
                    // Cria o máximo de tarefas, primeiro para testar se está criando;
                    // Segundo para ver se está criando apenas até o máximo
                    for (int i = 1; i <= maximoTarefas + 1; i++) // + de propósioto
                    {
                        var ret = await Comunicar<ResultadoUnico<Tarefa>>($"{usuarioId}/tarefas/{projeto.id}/criar", "post", new Tarefa()
                        {
                            projetoId = projeto.id,
                            titulo = $"Tarefa #{i}",
                            descricao = $"Descrição tarefa #{i}",
                            prioridade = Tarefa.PRIORIDADE_MEDIA,
                            status = Tarefa.STATUS_PENDENTE,
                            vencimento = DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
                            usuarioCriouId = usuarioId
                        });

                        if (!ret.ok)
                        {
                            if (i <= (maximoTarefas)) // até o máximo, não pode dar erro (ok)
                            {
                                throw new Exception($"Erro ao criar a tarefa #{i}: {ret.mensagem}");
                            }
                        }
                        else
                        {
                            if (i > maximoTarefas) 
                            {
                                // Acima do máximo, não podia criar
                                throw new Exception($"Erro ao criar a tarefa #{i} (tarefa além do máximo, não deveria criar): {ret.mensagem}");
                            }
                            
                            // Testa a alteração válida, para a metade
                            if (i <= (maximoTarefas / 2))
                            {
                                ret.result.status = "andamento";

                                ret = await Comunicar<ResultadoUnico<Tarefa>>(
                                    $"{usuarioId}/tarefa/{ret.result.id}/atualizar", "post", ret.result);

                                if (!ret.ok)
                                {
                                    throw new Exception($"Erro ao alterar a tarefa #{i}: {ret.mensagem}"); 
                                }
                                else
                                {
                                    // Testa o comentário
                                    var ret2 = await Comunicar<ResultadoBase>(
                                        $"{usuarioId}/tarefa/{ret.result.id}/comentar", "post", 
                                        new TarefasController.ComentarTarefaParam()
                                        {
                                            comentario = $"Comentário na tarefa {i}"
                                        });

                                    if (!ret2.ok)
                                    {
                                        throw new Exception($"Erro ao comentar na tarefa #{i}: {ret2.mensagem}"); 
                                    }
                                }
                            }
                            else
                            {
                                // Testa alteração inválida, para o resto
                                
                                ret.result.prioridade = "alta"; // não pode deixar alterar a prioridade

                                ret = await Comunicar<ResultadoUnico<Tarefa>>(
                                    $"{usuarioId}/tarefa/{ret.result.id}/atualizar", "post", ret.result);

                                if (ret.ok)
                                {
                                    throw new Exception($"Erro ao alterar a tarefa #{i}: A prioridade alterou, mas não podia ter deixado"); 
                                }
                            }

                        }
                    }
                    
                }
                
                // Tenta excluir o projeto
                {
                    testeIdent = "Exclusão de projetos";
                    
                    var ret = await Comunicar<ResultadoBase>($"{usuarioId}/projetos/excluir/{projeto.id}", "del");
                    
                    // Como tem tarefas pendentes, não pode deixar excluir
                    if (ret.ok)
                    {
                        // erro, não deveria ter excluído
                        throw new Exception("O projeto teste foi excluído, o que não deveria ter ocorrido, pois há tarefas pendentes");   
                    }
                    else
                    {
                        // Tenta concluir as tarefas
                        var ret2 = await Comunicar<ResultadoLista<Tarefa>>($"{usuarioId}/tarefas/{projeto.id}/listar", "get");

                        if (ret2.ok)
                        {
                            foreach (var tarefa in ret2.result)
                            {
                                tarefa.status = Tarefa.STATUS_CONCLUIDA;
                                
                                var ret3 = await Comunicar<ResultadoUnico<Tarefa>>(
                                    $"{usuarioId}/tarefa/{tarefa.id}/atualizar", "post", tarefa);

                                if (!ret3.ok)
                                {
                                    throw new Exception($"Erro ao alterar a tarefa para concluída: {ret3.mensagem}"); 
                                }
                            }
                        }
                        else
                        {
                            throw new Exception($"Erro ao obter as tarefas do projeto teste: {ret2.mensagem}");
                        }
                    }
                    
                    // Agora, não dando erro, tenta de novo excluir o projeto
                    ret = await Comunicar<ResultadoBase>($"{usuarioId}/projetos/excluir/{projeto.id}", "del");
                    
                    if (!ret.ok)
                    {
                        throw new Exception($"Erro ao excluir o projeto de teste: {ret.mensagem}");   
                    }
                }
                
            }
            catch (Exception e)
            {
                Assert.Fail($"{testeIdent} -> {(e.InnerException ?? e).Message}");
            }
            
        }
    }

}