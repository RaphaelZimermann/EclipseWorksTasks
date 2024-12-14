using System.ComponentModel.DataAnnotations.Schema;
using EclipseWorksTasks.Classes;
using Microsoft.EntityFrameworkCore;
using EclipseWorksTasks.Models;

namespace EclipseWorksTasks.Models
{

    
    public class Tarefa
    {
        public const string PRIORIDADE_BAIXA = "baixa";
        public const string PRIORIDADE_MEDIA = "média";
        public const string PRIORIDADE_ALTA = "alta";
        
        public const string STATUS_PENDENTE = "pendente";
        public const string STATUS_ANDAMENTO = "andamento";
        public const string STATUS_CONCLUIDA = "concluída";
        
        
        public int id { get; set; }

        public int projetoId { get; set; }
        
        public int usuarioCriouId { get; set; }
        
        public string titulo { get; set; }
        
        public string prioridade { get; set; } = PRIORIDADE_MEDIA;
        
        public string descricao { get; set; }
        
        public DateOnly vencimento { get; set; }
        
        public string status { get; set; }

        public DateOnly? dataConclusao { get; set; }
        
        public void Validar()
        {
            titulo = (titulo ?? "").Trim();
            descricao = (descricao ?? "").Trim();
            
            if (projetoId == 0)
            {
                throw new Exception("Projeto inválido");
            }
            
            if (usuarioCriouId == 0)
            {
                throw new Exception("Usuário inválido");
            }
            
            if (!Utilidades.ValidarTexto(this.titulo))
            {
                throw new Exception("Título inválido");
            }
            
            if (!Utilidades.ValidarTexto(this.descricao))
            {
                throw new Exception("Descrição inválida");
            }
            
            if (vencimento < DateOnly.FromDateTime(DateTime.Now))
            {
                throw new Exception("Vencimento precisa ser a frente");
            }
            
            if (!(status is STATUS_PENDENTE or STATUS_ANDAMENTO or STATUS_CONCLUIDA))
            {
                throw new Exception("Status inválido");
            }
            
            if (!(prioridade is PRIORIDADE_BAIXA or PRIORIDADE_MEDIA or PRIORIDADE_ALTA))
            {
                throw new Exception("Prioridade inválida");
            }
        }
    }

    public partial class AppDbContext : DbContext
    {
        protected DbSet<Tarefa> tarefas { get; set; }

        public async Task<Tarefa> CriarTarefa(Tarefa tarefa)
        {
            tarefa.id = 0;
            
            var r = (await tarefas.AddAsync(tarefa)).Entity;
            await this.SaveChangesAsync();
            return r;
        }

        public async Task<Tarefa> AtualizarTarefa(Tarefa tarefa)
        {
            Update(tarefa);
            await this.SaveChangesAsync();
            return tarefa;
        }

        public async Task ExcluirTarefa(Tarefa tarefa)
        {
            Remove(tarefa);
            await this.SaveChangesAsync();
        }

        public async Task<IQueryable<Tarefa>> ObterTarefas()
        {
            // Para otimizar o uso do Linq
            return tarefas.AsQueryable();
        }
    }
}