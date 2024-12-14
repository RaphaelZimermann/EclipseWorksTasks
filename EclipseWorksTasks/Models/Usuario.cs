using System.ComponentModel.DataAnnotations.Schema;
using EclipseWorksTasks.Classes;
using Microsoft.EntityFrameworkCore;
using EclipseWorksTasks.Models;

namespace EclipseWorksTasks.Models
{
    public class Usuario
    {
        public int id { get; set; }

        public string nome { get; set; }
        
        public string funcao { get; set; }

        [NotMapped]
        public bool IsGerente => funcao == "gerente";

        public void Validar()
        {
            if (!Utilidades.ValidarTexto(this.nome))
            {
                throw new Exception(Constantes.Erro_NomeInvalido);
            }

            if (!(funcao is "colaborador" or "gerente"))
            {
                throw new Exception("Função inválida. Funções válidas: 'colaborador' ou 'gerente'");
            }
        }
    }

    public partial class AppDbContext : DbContext
    {
        protected DbSet<Usuario> usuarios { get; set; }

        // Não foi solicitado, mas qualquer coisa está aqui,
        // só alterar para public
        protected async Task<Usuario> CriarUsuario(Usuario usuario)
        {
            usuario.id = 0;
            var r = (await usuarios.AddAsync(usuario)).Entity;
            await this.SaveChangesAsync();
            return r;
        }

        // Não foi solicitado, mas qualquer coisa está aqui,
        // só alterar para public
        protected async Task<Usuario> AtualizarUsuario(Usuario usuario)
        {
            // Feito de forma simples, mas em um projeto avançado,
            // seria interessante validar se o usuário foi alterado
            // por outro usuário e/ou se ele ainda existe na base
            Update(usuario);

            await this.SaveChangesAsync();

            return usuario;
        }

        // Não foi solicitado, mas qualquer coisa está aqui,
        // só alterar para public
        protected async Task ExcluirUsuario(Usuario usuario)
        {
            // Nota: Obviamente, há varias formas de executar a remoção,
            // por exemplo o código abaixo. Porém, pode não ser seguro
            // executar um delete dessa forma. Um erro de digitação
            // pode excluir o que não devia...
            // await Database.ExecuteSqlRawAsync($"delete from users where id = {user.id}"); -> apenas para conhecimento
            Remove(usuario);

            await this.SaveChangesAsync();
        }

        // Necessário para saber quais são os users
        public async Task<IQueryable<Usuario>> ObterUsuarios()
        {
            // Para otimizar o uso do Linq
            return usuarios.AsQueryable();
        }
    }
}