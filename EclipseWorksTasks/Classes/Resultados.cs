namespace EclipseWorksTasks.Classes
{
    // para melhor padronização de respostas. Prefiro trabalhar com padrões herdados,
    // facilitando futuros factories e deixado o código mais limpo ;)

    public class ResultadoBase
    {
        public bool ok { get; set; }
        public string mensagem { get; set; }
        
        public ResultadoBase()
        {
            ok = true;
            mensagem = "";
        }

        public ResultadoBase(Exception ex)
        {
            ok = false;
            mensagem = (ex.InnerException ?? ex).Message;
        }
    }
    
    public class ResultadoLista<T> : ResultadoBase
    {
        public IEnumerable<T> result { get; set; }

        public ResultadoLista() : base()
        {
            
        }

        public ResultadoLista(Exception ex) : base(ex)
        {
            
        }
    }
    
    public class ResultadoUnico<T> : ResultadoBase
    {
        public T result { get; set; }

        public ResultadoUnico() : base()
        {
            
        }

        public ResultadoUnico(Exception ex) : base(ex)
        {
            
        }
    }

}