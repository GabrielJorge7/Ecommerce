using Ecommerce.Data.Interfaces;
using Ecommerce.Objects.DTOs;
using Ecommerce.Objects.Enums;
using Ecommerce.Objects.Models;
using Ecommerce.Services.Interfaces;
using Ecommerce.Services.State;
using Ecommerce.Services.Strategy;

public class PedidoService : Pedido, IPedidoService
{
    private readonly IPedidoRepository _repository;

    public PedidoService(IPedidoRepository pedidoRepository)
    {
        _repository = pedidoRepository;
    }

    public async Task<IEnumerable<PedidoDTO>> ListarTodos()
    {
        var pedidos = await _repository.Get();
        List<PedidoDTO> pedidosDTO = [];

        foreach (var pedido in pedidos)
        {
            pedidosDTO.Add(ConverterParaDTO(pedido));
        }

        return pedidosDTO;
    }

    public async Task<PedidoDTO> ObterPorId(int id)
    {
        var pedido = await _repository.GetById(id);

        if (pedido is null)
        {
            return null;
        }

        return ConverterParaDTO(pedido);
    }

    public async Task<PedidoDTO> GerarPedido(PedidoDTO pedidoDTO)
    {
        var pedido = ConverterParaModel(pedidoDTO);
        IFrete frete = CriarFretePorTipo(pedido.TipoFrete);

        pedido.EstadoAtual = StatusPedido.AGUARDANDO_PAGAMENTO;
        pedido.ValorFrete = frete.CalcularFrete(pedido.Subtotal);

        await _repository.Add(pedido);
        return ConverterParaDTO(pedido);
    }

    public async Task<PedidoDTO> Atualizar(PedidoDTO pedidoDTO, int id)
    {
        var existingPedido = await _repository.GetById(id);

        if (existingPedido is null)
        {
            throw new KeyNotFoundException($"Pedido com id {id} não encontrado.");
        }

        if (existingPedido.EstadoAtual == StatusPedido.AGUARDANDO_PAGAMENTO)
        {
            // Evita que o campo de estado seja alterado diretamente pelo usuário
            pedidoDTO.StatusPedido = (int)existingPedido.EstadoAtual;

            // Recalcula o valor do frete
            IFrete frete = CriarFretePorTipo((TipoFrete)pedidoDTO.TipoFrete);
            pedidoDTO.ValorFrete = frete.CalcularFrete(pedidoDTO.Subtotal);
        }
        else
        {
            throw new Exception("Não é permitido atualizar o pedido, após sua confirmação/cancelamento.");
        }

        var pedido = ConverterParaModel(pedidoDTO);
        await _repository.Update(pedido);

        return pedidoDTO;
    }

    public async Task<PedidoDTO> SucessoAoPagar(PedidoDTO pedidoDTO)
    {
        // Converter estado do DTO para uma classe State
        IPedidoState state = ObterEstadoClasse(ConverterParaModel(pedidoDTO).EstadoAtual);

        // Fazer a transição de estado
        IPedidoState novoEstado = state.SucessoAoPagar();

        // Converter o novo estado para o estado da DTO
        pedidoDTO.StatusPedido = (int)ObterEstadoEnum(novoEstado);

        // Atualizar no banco
        await _repository.Update(ConverterParaModel(pedidoDTO));

        return pedidoDTO;
    }

    public async Task<PedidoDTO> DespacharPedido(PedidoDTO pedidoDTO)
    {
        IPedidoState state = ObterEstadoClasse(ConverterParaModel(pedidoDTO).EstadoAtual);
        IPedidoState novoEstado = state.DespacharPedido();
        pedidoDTO.StatusPedido = (int)ObterEstadoEnum(novoEstado);

        await _repository.Update(ConverterParaModel(pedidoDTO));

        return pedidoDTO;
    }

    public async Task<PedidoDTO> CancelarPedido(PedidoDTO pedidoDTO)
    {
        IPedidoState state = ObterEstadoClasse(ConverterParaModel(pedidoDTO).EstadoAtual);
        IPedidoState novoEstado = state.CancelarPedido();
        pedidoDTO.StatusPedido = (int)ObterEstadoEnum(novoEstado);

        await _repository.Update(ConverterParaModel(pedidoDTO));

        return pedidoDTO;
    }

    #region Métodos de Conversão
    private IPedidoState ObterEstadoClasse(StatusPedido estadoPedido)
    {
        return estadoPedido switch
        {
            StatusPedido.AGUARDANDO_PAGAMENTO => new AguardandoPagamentoState(),
            StatusPedido.PAGO => new PagoState(),
            StatusPedido.ENVIADO => new EnviadoState(),
            StatusPedido.CANCELADO => new CanceladoState(),
            _ => throw new ArgumentException("Estado inválido"),
        };
    }

    private StatusPedido ObterEstadoEnum(IPedidoState state)
    {
        return state switch
        {
            AguardandoPagamentoState _ => StatusPedido.AGUARDANDO_PAGAMENTO,
            PagoState _ => StatusPedido.PAGO,
            EnviadoState _ => StatusPedido.ENVIADO,
            CanceladoState _ => StatusPedido.CANCELADO,
            _ => throw new ArgumentException("Estado inválido"),
        };
    }

    private IFrete CriarFretePorTipo(TipoFrete tipoFrete)
    {
        return tipoFrete switch
        {
            TipoFrete.AEREO => new FreteAereo(),
            TipoFrete.TERRESTRE => new FreteTerrestre(),
            _ => throw new ArgumentException("Tipo de frete inválido"),
        };
    }

    private PedidoDTO ConverterParaDTO(Pedido pedido)
    {
        PedidoDTO pedidoDTO = new()
        {
            Id = pedido.Id,
            Subtotal = pedido.Subtotal,
            ValorFrete = pedido.ValorFrete,
            StatusPedido = (int)pedido.EstadoAtual,
            TipoFrete = (int)pedido.TipoFrete,
        };

        return pedidoDTO;
    }

    private Pedido ConverterParaModel(PedidoDTO pedidoDTO)
    {
        Pedido pedido = new()
        {
            Id = pedidoDTO.Id,
            Subtotal = pedidoDTO.Subtotal,
            ValorFrete = pedidoDTO.ValorFrete,
            EstadoAtual = (StatusPedido)pedidoDTO.StatusPedido,
            TipoFrete = (TipoFrete)pedidoDTO.TipoFrete,
        };

        return pedido;
    }
    #endregion
}