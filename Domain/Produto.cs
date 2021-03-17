using System;

namespace Domain
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public Status Status { get; set; }
    }

    public enum Status
    {
        Pedente = 1,
        Cadastro = 2,
        Excluido = 3
    }
}
