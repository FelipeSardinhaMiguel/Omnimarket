# Roteiro de apresentacao - modulo financeiro (TCC)

## 1) Objetivo (fala inicial - 1 minuto)

Neste modulo financeiro eu modelei o marketplace panar com multiplos vendedores no mesmo pedido.
ra funcio
O foco foi separar com clareza:
- como o comprador paga;
- como cada vendedor recebe;
- como a plataforma calcula comissao;
- como ficam recibo e historico financeiro.

No MVP a integracao com gateway foi simulada, mas a modelagem ja esta pronta para gateway real com split.
O OmniMarket gera um comprovante de venda em PDF para auxiliar compradores e vendedores no acompanhamento das transacoes. O documento possui carater informativo e nao substitui nota fiscal.
Os recibos sao gerados sob demanda a partir dos dados congelados no pedido. Isso evita armazenamento desnecessario de arquivos e garante que o comprovante mantenha as informacoes da venda no momento da compra.

## 2) Decisao principal (falar o por que)

Modelo escolhido:
- comprador paga dentro da plataforma;
- plataforma envia para gateway com split;
- gateway distribui comissao da plataforma e liquido dos vendedores.

Por que esse modelo:
- melhor rastreabilidade;
- menos risco de erro manual;
- mais facil de auditar no banco;
- mais coerente com marketplace real.

## 3) Entidades principais para explicar em slide

- `Pedido`: compra geral do comprador.
- `Venda`: subpedido por vendedor (uma venda por vendedor).
- `PlanoPagamento`: plano total de pagamento do pedido.
- `ItemPlanoPagamento`: partes do pagamento (ex.: Pix + Cartao).
- `VendaPagamento`: impacto financeiro de cada venda dentro do plano.
- `RepasseVendedor`: controle do repasse do liquido para vendedor.
- `ReciboFinanceiro`: comprovante da operacao.
- `HistoricoFinanceiro`: trilha de eventos financeiros.
- `TermoPolitica` + `AceiteUsuarioTermo`: aceite eletronico do vendedor.

## 4) Regra de comissao (falar com exemplo)

Formula:

`Comissao = A + B * C`

Onde:
- `A = valor fixo` (R$ 1,00)
- `B = percentual` (8%)
- `C = valor bruto da venda`

Regras aplicadas:
- comissao arredondada em 2 casas;
- comissao nunca maior que valor da venda;
- liquido nunca negativo;
- parametros usados ficam salvos por venda.

Exemplo rapido:
- venda = R$ 100,00
- comissao = 1,00 + 8% * 100 = 9,00
- liquido vendedor = 91,00

## 5) Fluxo financeiro (16 passos resumidos)

1. comprador fecha carrinho;
2. cria `Pedido`;
3. separa por vendedor e cria `Venda`;
4. cria `PlanoPagamento`;
5. cria `ItemPlanoPagamento`;
6. calcula bruto por venda;
7. calcula comissao por venda;
8. calcula liquido por venda;
9. cria `VendaPagamento`;
10. monta split para gateway;
11. comprador paga;
12. gateway retorna status;
13. atualiza status financeiros;
14. gera `ReciboFinanceiro`;
15. grava `HistoricoFinanceiro`;
16. cria/atualiza `RepasseVendedor`.

## 6) Demo no Swagger (ordem sugerida)

### 6.1 Criar pedido (endpoint ja existente)

Use o fluxo normal de pedido da API.

### 6.2 Iniciar pagamento

`POST /api/financeiro/pagamentos/iniciar`

Exemplo body (Pix + Cartao):

```json
{
  "pedidoId": 1,
  "observacao": "Pagamento misto para demonstracao do TCC",
  "itensPagamento": [
    {
      "formaPagamentoId": 1,
      "condicaoPagamentoId": 1,
      "valor": 50.00,
      "quantidadeParcelas": 1,
      "observacao": "Parte no Pix"
    },
    {
      "formaPagamentoId": 4,
      "condicaoPagamentoId": 3,
      "valor": 150.00,
      "quantidadeParcelas": 3,
      "observacao": "Parte no cartao"
    }
  ]
}
```

### 6.3 Confirmar pagamento fake

`POST /api/financeiro/pagamentos/{planoPagamentoId}/confirmar-fake`

Mostre no retorno:
- status aprovado;
- transaction id fake;
- quantidade de vendas atualizadas;
- quantidade de repasses criados;
- quantidade de recibos gerados.

### 6.4 Consultar extrato do vendedor

`GET /api/financeiro/vendedor/extrato`

Mostra:
- bruto, comissao e liquido;
- status financeiro;
- status do repasse.

### 6.5 Consultar comissao da plataforma

`GET /api/financeiro/plataforma/comissoes`

Mostra:
- total bruto;
- total de comissao;
- total liquido dos vendedores.

## 7) Quatro cenarios para banca

1. 1 vendedor e 1 forma de pagamento.
2. 2 vendedores e 1 forma de pagamento.
3. 1 pedido com Pix + Cartao.
4. pagamento parcelado no cartao.

## 8) Seguranca e validacoes que eu implementei

- bloqueio de compra do proprio produto;
- validacao de soma dos itens de pagamento;
- comissao nunca negativa e nunca maior que valor da venda;
- liquido nunca negativo;
- vendedor ve apenas extrato dele;
- recibo so apos pagamento confirmado;
- logs de eventos em `HistoricoFinanceiro`;
- armazenamento de IDs do gateway (transaction/payout).

## 9) Encerramento (fala final - 30 segundos)

O modulo foi feito para ser simples de entender e apresentar:
- estrutura separada por responsabilidade;
- regras financeiras claras;
- pronto para evoluir de gateway fake para gateway real.

Como proximo passo, basta integrar webhook do gateway real e automatizar conciliacao.
