Visão Geral do Sistema
Neste projeto de e-commerce, queremos orquestrar dois aspectos cruciais de cada compra:

Evolução do pedido — ele nasce, amadurece ou é encerrado seguindo regras rígidas (pago, enviado, cancelado, etc.).

Cobrança de frete — o custo de entrega varia conforme o modal (terra ou ar) e precisa continuar simples de expandir com novos modais.

Para atingir esses objetivos, recorremos a dois padrões clássicos de design orientado a objetos, cada um resolvendo um problema específico.

1. Mudança de status com State
Por que State?
Escrever ifs ou switch cases para trocar comportamento conforme o status logo vira um emaranhado difícil de manter. O padrão State troca essa selva de condicionais por um conjunto de objetos-estado especializados.

Como aplicamos:

Papel	Implementação
Interface PedidoState	Define as ações possíveis (ex.: pagar(), enviar(), cancelar()).
Estados concretos	AguardandoPagamento, Pago, Enviado, Cancelado – cada classe sabe exatamente o que pode ou não fazer e qual estado virá depois.
Contexto Pedido	Mantém uma referência ao estado atual e delega as chamadas: pedido.pagar() simplesmente invoca estadoAtual.pagar(this).

Benefícios alcançados

Legibilidade: os fluxos de transição estão “distribuídos” nos próprios estados, não escondidos em um monolito de if.

Segurança: tentar uma transição inválida dispara exceção na própria classe de estado — impossível esquecer uma regra.

Manutenção leve: adicionar um novo status exige só outra classe que implementa a interface.

2. Cálculo de frete com Strategy
Desafio: o preço do frete depende do meio de transporte e queremos plugar novos métodos sem alterar o código antigo.

Por que Strategy?
O padrão Strategy nos permite encapsular cada algoritmo de cálculo em um objeto intercambiável.

Estrutura adoptada:

Elemento	Descrição
Interface FreteStrategy	Método calcular(pedido) devolve o valor.
Estratégias concretas	FreteTerrestre, FreteAereo — cada uma implementa sua fórmula.
Contexto de cálculo	O Pedido mantém uma referência a FreteStrategy; ao chamar pedido.totalFrete(), a estratégia certa é executada.

Ganhos obtidos

Aberto para extensão, fechado para modificação (Princípio Open/Closed): adicionar FreteMaritimo não toca nas classes existentes.

Testabilidade: cada cálculo de frete é testado isoladamente.

Reutilização: a mesma estratégia pode ser usada em outros pontos da aplicação (ex.: carrinho, cotação).




Service:

![image](https://github.com/user-attachments/assets/eb754f42-3ab0-4f43-9114-102dfc0b5c3c)

Model:

![image](https://github.com/user-attachments/assets/ce4a7c10-3f33-4f65-9361-8193abe97731)


