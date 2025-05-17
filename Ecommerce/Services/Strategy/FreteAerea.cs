namespace Ecommerce.Services.Strategy
{
    public class FreteAereo : IFrete
    {
        public double CalcularFrete(double valorPedido)
        {
            return valorPedido * 0.1;
        }
    }
}