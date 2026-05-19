# Postman Manual do OmniMarket

Arquivos gerados nesta pasta:

- `postman/OmniMarket.manual-marketplace.postman_collection.json`
- `postman/OmniMarket.manual-marketplace.local.postman_environment.json`
- `postman/generate-manual-marketplace-postman.mjs`

## Objetivo

Esta pasta segue o modelo manual de teste no Postman, mas atualizada para a API atual do projeto.

Aqui voce preenche manualmente os dados de:

- usuario
- login
- endereco
- telefone
- loja
- produto
- entrega
- fluxo comercial

## Pastas da collection

- `01 - Cadastro`
- `02 - Login e Perfis`
- `03 - Enderecos e Telefones`
- `04 - Loja`
- `05 - Produto`
- `06 - Entregas da Loja`
- `07 - Comercial Comprador`
- `08 - Admin Operacional`
- `09 - Pos-venda Comprador`

## Como usar

1. Suba a API local em `http://localhost:5033`
2. Importe a collection `OmniMarket.manual-marketplace.postman_collection.json`
3. Importe o environment `OmniMarket.manual-marketplace.local.postman_environment.json`
4. Selecione o environment `OmniMarket Manual Marketplace Local`
5. Ajuste as variaveis do environment conforme o usuario que voce quer testar

Fluxo basico de vendedor:

- `Registrar Usuario Completo Manual`
- `Login Manual`
- `Obter Perfil Atual`
- `Salvar Sessao Atual Como Vendedor`
- requests das pastas `Loja`, `Produto` e `Entregas da Loja`

Fluxo basico de comprador:

- altere as variaveis de cadastro para um segundo usuario
- `Registrar Usuario Completo Manual`
- `Login Manual`
- `Obter Perfil Atual`
- `Salvar Sessao Atual Como Comprador`

Fluxo comercial completo:

- `Login Rapido Comprador`
- `Limpar Carrinho Atual`
- `Listar Entregas Publicas da Loja do Vendedor`
- `Adicionar Produto ao Carrinho`
- `Gerar Pedido do Carrinho`
- `Iniciar Pagamento`
- `Confirmar Pagamento Fake`
- `Login Rapido Admin`
- `Marcar Pedido Atual Como Enviado`
- `Login Rapido Comprador`
- `Baixar Recibo do Pedido Atual`
- `Confirmar Entrega do Pedido Atual`
- `Criar Avaliacao do Produto Atual`

## O que a collection salva automaticamente

Depois das requests principais, a collection tenta preencher:

- `loginEmail`
- `loginPassword`
- `tokenAtual`
- `usuarioAtualId`
- `usuarioAtualEmail`
- `usuarioAtualRole`
- `enderecoAtualId`
- `enderecoAtualCep`
- `enderecoAtualCidade`
- `enderecoAtualUf`
- `telefoneAtualId`
- `lojaAtualId`
- `lojaAtualSlug`
- `produtoAtualId`
- `produtoMidiaId`
- `entregaOpcaoAtualId`
- `pedidoAtualId`
- `planoPagamentoAtualId`
- `avaliacaoAtualId`

Tambem salva referencias especificas de:

- `vendedor...`
- `comprador...`
- `admin...`

Isso permite alternar entre as sessoes com `Login Rapido Vendedor`, `Login Rapido Comprador` e `Login Rapido Admin`.

## Observacoes importantes

- O endpoint `POST /api/usuario/registrar` exige telefone e endereco no mesmo payload.
- A criacao de loja agora reaproveita por padrao o endereco e o telefone do usuario atual.
- O documento fiscal da loja pode ser `CPF` ou `CNPJ`, sempre validado localmente e salvo apenas com numeros.
- O sistema nao permite comprar o proprio produto, entao o fluxo comercial precisa de um vendedor e um comprador diferentes.
- O fluxo operacional atual usa o endpoint admin `PUT /api/admin/pedidos/{pedidoId}/enviar` para avancar o pedido para envio.
- O endpoint `GET /api/pedidos/{pedidoId}/recibo` retorna um PDF apenas para pedidos pagos, enviados ou entregues.
- O endpoint `GET /api/lojas/minha/pedidos/{pedidoId}/recibo` permite ao vendedor baixar o recibo apenas dos pedidos que contem itens da propria loja.
- Os recibos usam os dados congelados no momento da compra para evitar que mudancas futuras em produto ou loja alterem o comprovante.
- Para testar o fluxo admin, preencha `adminEmail` e `adminPassword` no environment com um usuario admin valido.

## Regenerar os arquivos

```powershell
node .\postman\generate-manual-marketplace-postman.mjs
```
