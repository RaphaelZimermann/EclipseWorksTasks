using EclipseWorksTasks.Classes;
using Microsoft.EntityFrameworkCore;
using EclipseWorksTasks.Models;

namespace EclipseWorksTasks.Models
{
    public class Projeto
    {
        public int id { get; set; }

        public string nome { get; set; }

        public int usuarioId { get; set; }
        
        public void Validar()
        {
            if (!Utilidades.ValidarTexto(this.nome))
            {
                throw new Exception(Constantes.Erro_NomeInvalido);
            }
        }
    }

    public partial class AppDbContext : DbContext
    {
        protected DbSet<Projeto> projetos { get; set; }

        public async Task<Projeto> CriarProjeto(Projeto projeto)
        {
            projeto.id = 0;
            var r = (await projetos.AddAsync(projeto)).Entity;
            await this.SaveChangesAsync();
            return r;
        }

        public async Task<Projeto> AtualizarProjeto(Projeto projeto)
        {
            // Feito de forma simples, mas em um projeto avançado,
            // seria interessante validar se o projeto foi alterado
            // por outro usuário e/ou se ele ainda existe na base
            Update(projeto);

            await this.SaveChangesAsync();

            return projeto;
        }

        public async Task ExcluirProjeto(Projeto projeto)
        {
            // Nota: Obviamente, há varias formas de executar a remoção,
            // por exemplo o código abaixo. Porém, pode não ser seguro
            // executar um delete dessa forma. Um erro de digitação
            // pode excluir o que não devia...
            // await Database.ExecuteSqlRawAsync($"delete from projects where id = {project.id}"); -> apenas para conhecimento
            Remove(projeto);

            await this.SaveChangesAsync();
        }

        public async Task<IQueryable<Projeto>> ObterProjetos()
        {
            // Para otimizar o uso do Linq
            return projetos.AsQueryable();
        }
    }
}