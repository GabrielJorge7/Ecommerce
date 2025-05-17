using Ecommerce.Objects.Enums;

namespace Ecommerce.Objects.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public double Subtotal { get; set; }
        public double ValorFrete { get; set; }
        public StatusPedido EstadoAtual { get; set; }
        public TipoFrete TipoFrete { get; set; }

        public Pedido() { }

        public Pedido(int id, double subtotal, double valorFrete, TipoFrete tipoFrete)
        {
            Id = id;
            Subtotal = subtotal;
            ValorFrete = valorFrete;
            EstadoAtual = StatusPedido.AGUARDANDO_PAGAMENTO;
            TipoFrete = tipoFrete;
        }
    }
}
