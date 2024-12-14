using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EclipseWorksTasks.Models
{
    public class HistoricoTarefa
    {
        public const string TIPO_ALTERACAO = "alteração";
        public const string TIPO_COMENTARIO = "comentário";
        
        public int id { get; set; }

        public int tarefaId { get; set; }

        public int usuarioId { get; set; }

        public DateTime dataHora { get; set; }

        public string tipo { get; set; }

        public string conteudo { get; set; }
    }

    public partial class AppDbContext : DbContext
    {
        protected DbSet<HistoricoTarefa> historicosTarefas { get; set; }
        
        public async Task<HistoricoTarefa> CriarHistoricoTarefa(HistoricoTarefa historico)
        {
            historico.id = 0;
            var r = (await historicosTarefas.AddAsync(historico)).Entity;
            await this.SaveChangesAsync();
            return r;
        }
        
        public async Task<IQueryable<HistoricoTarefa>> ObterHistoricosTarefas()
        {
            // Para otimizar o uso do Linq
            return historicosTarefas.AsQueryable();
        }
    }
}