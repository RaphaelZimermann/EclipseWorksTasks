namespace EclipseWorksTasks.Classes
{
    public static class Utilidades
    {
        // Se for necessário fazer tratamentos da origem da data,
        // pode ser alterado aqui
        public static DateOnly Hoje => DateOnly.FromDateTime(DateTime.Now);

        public static bool ValidarTexto(string name)
        {
            // Caso seja necessário validar tamanho mínimo,
            // máximo, entre outras coisas, ajustar aqui
            return !string.IsNullOrEmpty(name);
        }
    }
}