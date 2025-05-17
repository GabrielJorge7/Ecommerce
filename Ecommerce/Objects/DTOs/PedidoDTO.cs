namespace Ecommerce.Objects.DTOs
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public double Subtotal { get; set; }
        public double ValorFrete { get; set; }
        public int StatusPedido { get; set; }
        public int TipoFrete { get; set; }
    }
}
