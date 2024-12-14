using EclipseWorksTasks.Classes;
using EclipseWorksTasks.Models;
using Microsoft.AspNetCore.Mvc;

namespace EclipseWorksTasks.Controllers
{
    public class RelatorioDesempenho 
    {
        public int usuarioId { get; set; }
        
        public string usuario { get; set; }
        
        public decimal mediaConcluaoPeriodo { get; set; }
    }
    
    public class RelatorioConclusaoDetalhada
    {
        public int tarefaId { get; set; }
        public int usuarioId { get; set; }
        
        public string usuario { get; set; }
        public string projeto { get; set; }
        public string tarefa { get; set; }
        
        public DateOnly? data { get; set; }
    }
    
   
    
    public class RelatoriosController : Controller
    {
        private readonly AppDbContext context;

        public RelatoriosController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{usuarioId}/relatorios/desempenho")]
        public async Task<ResultadoLista<RelatorioDesempenho>> Desempenho(int usuarioId)
        {
            try
            {
                var usuario = (await context.ObterUsuarios())
                    .Where(x => x.id == usuarioId)
                    .FirstOrDefault();

                if (usuario.funcao != Usuario.FUNCAO_GERENTE)
                {
                    throw new Exception("Apenas gerentes possuem acesso ao relatório");
                }

                var dias = 30m;
                var inicio = Utilidades.Hoje.AddDays(-(int)dias);
                var qrUsuarios = (await context.ObterUsuarios());
                var qrTarefas = (await context.ObterTarefas());
                var qrProjetos = (await context.ObterProjetos());

                var qr = from t in qrTarefas
                    join p in qrProjetos on t.projetoId equals p.id
                    where t.status == Tarefa.STATUS_CONCLUIDA &&
                          t.dataConclusao >= inicio
                    select new
                    {
                        p.usuarioId
                    };
                        
                

                var rel = qrUsuarios.Select(u => new RelatorioDesempenho
                {
                    usuarioId = u.id,
                    usuario = u.nome,

                    mediaConcluaoPeriodo = qr.Count(q => q.usuarioId == u.id) / dias

                }).OrderBy(x => x.usuario).ToArray();
                
                

                return new ResultadoLista<RelatorioDesempenho>
                {
                    result = rel
                };
            }
            catch (Exception e)
            {
                return (new ResultadoLista<RelatorioDesempenho>(e));
            }
        }
        
        [HttpGet("{usuarioId}/relatorios/conclusao-detalhada")]
        public async Task<ResultadoLista<RelatorioConclusaoDetalhada>> ConclusaoDetalhada(int usuarioId)
        {
            try
            {
                var usuario = (await context.ObterUsuarios())
                    .Where(x => x.id == usuarioId)
                    .FirstOrDefault();

                if (usuario.funcao != Usuario.FUNCAO_GERENTE)
                {
                    throw new Exception("Apenas gerentes possuem acesso ao relatório");
                }
                
                var inicio = Utilidades.Hoje.AddDays(-30);
                
                var qrTarefas = (await context.ObterTarefas());
                var qrProjetos = (await context.ObterProjetos());
                var qrUsuarios = (await context.ObterUsuarios());

                var rel = (from t in qrTarefas
                    join p in qrProjetos on t.projetoId equals p.id
                    join u in qrUsuarios on p.usuarioId equals u.id
                    where
                        t.status == Tarefa.STATUS_CONCLUIDA &&
                        t.dataConclusao >= inicio
                    select new RelatorioConclusaoDetalhada
                    {
                        tarefaId = t.id,
                        usuarioId = u.id,
                        usuario = u.nome,
                        projeto = p.nome,
                        tarefa = t.titulo,
                        data = t.dataConclusao

                    })
                    .OrderBy(x => x.usuario)
                    .ThenBy(x => x.data)
                    .ToArray();
                
                return new ResultadoLista<RelatorioConclusaoDetalhada>
                {
                    result = rel
                };
            }
            catch (Exception e)
            {
                return (new ResultadoLista<RelatorioConclusaoDetalhada>(e));
            }
        }
    }

}