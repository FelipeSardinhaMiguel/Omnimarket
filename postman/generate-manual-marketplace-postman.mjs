import { mkdir, writeFile } from "node:fs/promises";
import { randomUUID } from "node:crypto";

const outputDir = "postman";
const defaultBaseUrl = "http://localhost:5033";

const lines = (text) => text.trim().split("\n");

const buildEnvironment = (name, values) => ({
  id: randomUUID(),
  name,
  values: values.map(([key, value]) => ({
    key,
    value,
    type: "default",
    enabled: true
  })),
  _postman_variable_scope: "environment",
  _postman_exported_at: new Date().toISOString(),
  _postman_exported_using: "OpenAI Codex"
});

const collectionInfo = (name, description) => ({
  info: {
    _postman_id: randomUUID(),
    name,
    schema: "https://schema.getpostman.com/json/collection/v2.1.0/collection.json",
    description
  },
  variable: [],
  item: []
});

const url = (path, query = []) => {
  const rawBase = `{{baseUrl}}/${path.join("/")}`;
  const rawQuery = query.length
    ? `?${query.map(({ key, value }) => `${key}=${value}`).join("&")}`
    : "";

  return {
    raw: `${rawBase}${rawQuery}`,
    host: ["{{baseUrl}}"],
    path,
    ...(query.length ? { query } : {})
  };
};

const requestItem = ({
  name,
  method,
  path,
  query,
  authVar,
  body,
  description,
  headers = [],
  preRequest,
  tests,
  formData
}) => {
  const requestHeaders = [...headers];

  if (
    body &&
    !formData &&
    !requestHeaders.some((header) => header.key.toLowerCase() === "content-type")
  ) {
    requestHeaders.push({
      key: "Content-Type",
      value: "application/json"
    });
  }

  const request = {
    method,
    header: requestHeaders,
    url: url(path, query),
    description
  };

  if (formData) {
    request.body = {
      mode: "formdata",
      formdata: formData
    };
  } else if (body) {
    request.body = {
      mode: "raw",
      raw: body,
      options: {
        raw: {
          language: "json"
        }
      }
    };
  }

  if (authVar) {
    request.auth = {
      type: "bearer",
      bearer: [
        {
          key: "token",
          value: `{{${authVar}}}`,
          type: "string"
        }
      ]
    };
  }

  const event = [];

  if (preRequest?.length) {
    event.push({
      listen: "prerequest",
      script: {
        type: "text/javascript",
        exec: preRequest
      }
    });
  }

  if (tests?.length) {
    event.push({
      listen: "test",
      script: {
        type: "text/javascript",
        exec: tests
      }
    });
  }

  return {
    name,
    request,
    ...(event.length ? { event } : {}),
    response: []
  };
};

const folder = (name, items) => ({ name, item: items });

const runtimeScopeHelpers = lines(`
function getScopedVar(key, fallbackValue = "") {
  const value = pm.variables.get(key);
  if (value === undefined || value === null) {
    return fallbackValue;
  }
  const text = String(value).trim();
  if (text === "" || text.toLowerCase() === "null" || text.toLowerCase() === "undefined") {
    return fallbackValue;
  }
  return text;
}

function setScopedVar(key, value) {
  pm.collectionVariables.set(key, value);
  try {
    pm.environment.set(key, value);
  } catch (error) {
    // Sem environment ativo, mantemos a collection funcional.
  }
}

function unsetScopedVar(key) {
  pm.collectionVariables.unset(key);
  try {
    pm.environment.unset(key);
  } catch (error) {
    // Sem environment ativo, limpamos apenas as collection variables.
  }
}

function maybeSetScopedVar(key, value) {
  if (value === undefined || value === null) {
    return;
  }
  const text = String(value);
  if (text === "") {
    return;
  }
  setScopedVar(key, text);
}

function requireScopedVar(key, customMessage) {
  if (!getScopedVar(key)) {
    throw new Error(customMessage || "Defina a variavel " + key + " antes de executar esta request.");
  }
}

function readJsonResponse() {
  try {
    return pm.response.json();
  } catch (error) {
    throw new Error("A resposta nao retornou JSON valido.");
  }
}

function emailsMatch(left, right) {
  if (!left || !right) {
    return false;
  }
  return String(left).trim().toLowerCase() === String(right).trim().toLowerCase();
}

function getPageItems(payload) {
  if (Array.isArray(payload)) {
    return payload;
  }
  if (payload && Array.isArray(payload.items)) {
    return payload.items;
  }
  if (payload && Array.isArray(payload.Items)) {
    return payload.Items;
  }
  return [];
}

function syncCurrentAddressFromObject(address) {
  if (!address) {
    return;
  }
  maybeSetScopedVar("enderecoAtualId", address.id);
  maybeSetScopedVar("enderecoAtualCep", address.cep);
  maybeSetScopedVar("enderecoAtualCidade", address.cidade);
  maybeSetScopedVar("enderecoAtualUf", address.uf);
}

function syncCurrentPhoneFromObject(phone) {
  if (!phone) {
    return;
  }
  maybeSetScopedVar("telefoneAtualId", phone.id);
  maybeSetScopedVar("telefoneAtualNumero", phone.numeroE164 || phone.numero);
}

function syncCurrentStoreFromObject(store) {
  if (!store) {
    return;
  }
  maybeSetScopedVar("lojaAtualId", store.id);
  maybeSetScopedVar("lojaAtualSlug", store.slug);

  if (
    !getScopedVar("vendedorEmail") ||
    emailsMatch(getScopedVar("usuarioAtualEmail"), getScopedVar("vendedorEmail"))
  ) {
    maybeSetScopedVar("vendedorLojaId", store.id);
    maybeSetScopedVar("vendedorLojaSlug", store.slug);
  }
}

function syncCurrentProductFromObject(product) {
  if (!product) {
    return;
  }
  maybeSetScopedVar("produtoAtualId", product.id);
  maybeSetScopedVar("compraProdutoId", product.id);
  maybeSetScopedVar("lojaAtualId", product.lojaId);
  maybeSetScopedVar("lojaAtualSlug", product.slugLoja);

  if (
    !getScopedVar("vendedorEmail") ||
    emailsMatch(getScopedVar("usuarioAtualEmail"), getScopedVar("vendedorEmail"))
  ) {
    maybeSetScopedVar("vendedorProdutoId", product.id);
    maybeSetScopedVar("vendedorLojaId", product.lojaId);
    maybeSetScopedVar("vendedorLojaSlug", product.slugLoja);
  }
}

function syncCurrentDeliveryFromObject(option) {
  if (!option) {
    return;
  }
  maybeSetScopedVar("entregaOpcaoAtualId", option.id);
  maybeSetScopedVar("entregaAtualTipoId", option.tipoEntregaId);
  maybeSetScopedVar("entregaAtualNome", option.nome);
  maybeSetScopedVar("entregaAtualValorFrete", option.valorFrete);
  maybeSetScopedVar("entregaAtualPrazoEntregaDias", option.prazoEntregaDias);
  maybeSetScopedVar("pedidoTipoEntregaId", option.tipoEntregaId);

  if (
    !getScopedVar("vendedorEmail") ||
    emailsMatch(getScopedVar("usuarioAtualEmail"), getScopedVar("vendedorEmail"))
  ) {
    maybeSetScopedVar("vendedorEntregaOpcaoId", option.id);
  }
}

function syncCurrentOrderFromObject(order) {
  if (!order) {
    return;
  }
  maybeSetScopedVar("pedidoAtualId", order.id || order.pedidoId);
  maybeSetScopedVar("pedidoStatusAtual", order.status || order.statusPedidosId);
  maybeSetScopedVar("pedidoValorProdutos", order.valorTotalProdutos || order.valorProdutos);
  maybeSetScopedVar("pedidoValorFrete", order.valorFrete);
  maybeSetScopedVar("pedidoValorTotal", order.valorTotalPedido || order.valorTotal);
}

function saveCurrentSessionAs(prefix) {
  requireScopedVar(
    "usuarioAtualId",
    "Execute primeiro o login e a request de perfil antes de salvar a sessao."
  );
  requireScopedVar(
    "usuarioAtualEmail",
    "Execute primeiro o login e a request de perfil antes de salvar a sessao."
  );

  maybeSetScopedVar(prefix + "Email", getScopedVar("usuarioAtualEmail"));
  maybeSetScopedVar(prefix + "Password", getScopedVar("loginPassword"));
  maybeSetScopedVar(prefix + "UsuarioId", getScopedVar("usuarioAtualId"));
  maybeSetScopedVar(prefix + "EnderecoId", getScopedVar("enderecoAtualId"));
  maybeSetScopedVar(prefix + "EnderecoCep", getScopedVar("enderecoAtualCep"));
  maybeSetScopedVar(prefix + "EnderecoCidade", getScopedVar("enderecoAtualCidade"));
  maybeSetScopedVar(prefix + "EnderecoUf", getScopedVar("enderecoAtualUf"));
  maybeSetScopedVar(prefix + "TelefoneId", getScopedVar("telefoneAtualId"));

  if (prefix === "vendedor") {
    maybeSetScopedVar("vendedorLojaId", getScopedVar("lojaAtualId"));
    maybeSetScopedVar("vendedorLojaSlug", getScopedVar("lojaAtualSlug"));
    maybeSetScopedVar("vendedorProdutoId", getScopedVar("produtoAtualId"));
    maybeSetScopedVar("vendedorEntregaOpcaoId", getScopedVar("entregaOpcaoAtualId"));
  }
}

function syncRoleFromCurrentIfMatch(prefix) {
  if (!emailsMatch(getScopedVar("usuarioAtualEmail"), getScopedVar(prefix + "Email"))) {
    return;
  }
  saveCurrentSessionAs(prefix);
}

function prepareQuickLogin(prefix) {
  requireScopedVar(
    prefix + "Email",
    "Salve ou preencha primeiro as credenciais de " + prefix + " antes de usar o login rapido."
  );
  requireScopedVar(
    prefix + "Password",
    "A senha de " + prefix + " nao foi encontrada. Salve a sessao novamente."
  );
  setScopedVar("loginEmail", getScopedVar(prefix + "Email"));
  setScopedVar("loginPassword", getScopedVar(prefix + "Password"));
}
`);

const environmentDefaults = [
  ["baseUrl", defaultBaseUrl],
  ["cadastroCpf", "52998224725"],
  ["cadastroNome", "Maria"],
  ["cadastroSobrenome", "Silva"],
  ["cadastroEmail", "maria.silva.manual@omnimarket.test"],
  ["cadastroPassword", "Senha@123"],
  ["cadastroConfirmPassword", "Senha@123"],
  ["cadastroAceitouTermos", "true"],
  ["cadastroTelefoneDdd", "11"],
  ["cadastroTelefoneNumero", "999998888"],
  ["cadastroTelefonePrincipal", "true"],
  ["cadastroEnderecoCep", "01310930"],
  ["cadastroEnderecoTipoLogradouro", "Rua"],
  ["cadastroEnderecoNome", "Avenida Paulista"],
  ["cadastroEnderecoNumero", "1000"],
  ["cadastroEnderecoComplemento", "Apto 101"],
  ["cadastroEnderecoCidade", "Sao Paulo"],
  ["cadastroEnderecoUf", "SP"],
  ["cadastroEnderecoPrincipal", "true"],
  ["loginEmail", "maria.silva.manual@omnimarket.test"],
  ["loginPassword", "Senha@123"],
  ["enderecoCep", "22041001"],
  ["enderecoTipoLogradouro", "Avenida"],
  ["enderecoNome", "Atlantica"],
  ["enderecoNumero", "500"],
  ["enderecoComplemento", "Bloco B"],
  ["enderecoCidade", "Rio de Janeiro"],
  ["enderecoUf", "RJ"],
  ["enderecoPrincipal", "false"],
  ["telefoneDdd", "21"],
  ["telefoneNumero", "988887777"],
  ["telefonePrincipal", "false"],
  ["lojaNomeFantasia", "Loja Manual Exemplo"],
  ["lojaSlug", "loja-manual-exemplo"],
  ["lojaTipoDocumentoFiscal", "CPF"],
  ["lojaDocumentoFiscal", "52998224725"],
  ["lojaDescricao", "Loja criada manualmente pelo Postman."],
  ["lojaEmailContato", "contato.loja.manual@omnimarket.test"],
  ["lojaUsarEnderecoUsuario", "true"],
  ["lojaEnderecoUsuarioId", "null"],
  ["lojaUsarTelefoneUsuario", "true"],
  ["lojaTelefoneUsuarioId", "null"],
  ["lojaAtiva", "true"],
  ["produtoNome", "Produto Manual Exemplo"],
  ["produtoCategoria", "Doces"],
  ["produtoSku", "MANUAL-001"],
  ["produtoPreco", "29.90"],
  ["produtoDescricao", "Produto criado manualmente pelo Postman."],
  ["produtoEstoque", "10"],
  ["produtoEstoqueAtualizado", "15"],
  ["produtoStatusPublicacao", "Publicado"],
  ["produtoImagensJson", "[]"],
  ["entregaTipoId", "3"],
  ["entregaNome", "Entrega manual padrao"],
  ["entregaValorFrete", "12.50"],
  ["entregaPrazoEntregaDias", "3"],
  ["entregaObservacao", "Entrega configurada para testes manuais."],
  ["entregaAtiva", "true"],
  ["compraProdutoId", "null"],
  ["compraQuantidade", "1"],
  ["pedidoObservacao", "Pedido manual via Postman."],
  ["pedidoEnderecoId", "null"],
  ["pedidoTipoEntregaId", "null"],
  ["pagamentoFormaPagamentoId", "1"],
  ["tokenAtual", ""],
  ["usuarioAtualId", ""],
  ["usuarioAtualEmail", ""],
  ["usuarioAtualRole", ""],
  ["enderecoAtualId", ""],
  ["enderecoAtualCep", ""],
  ["enderecoAtualCidade", ""],
  ["enderecoAtualUf", ""],
  ["telefoneAtualId", ""],
  ["telefoneAtualNumero", ""],
  ["lojaAtualId", ""],
  ["lojaAtualSlug", ""],
  ["produtoAtualId", ""],
  ["produtoMidiaId", ""],
  ["entregaOpcaoAtualId", ""],
  ["entregaAtualTipoId", ""],
  ["entregaAtualNome", ""],
  ["entregaAtualValorFrete", ""],
  ["entregaAtualPrazoEntregaDias", ""],
  ["pedidoAtualId", ""],
  ["pedidoValorProdutos", ""],
  ["pedidoValorFrete", ""],
  ["pedidoValorTotal", ""],
  ["pedidoStatusAtual", ""],
  ["planoPagamentoAtualId", ""],
  ["avaliacaoAtualId", ""],
  ["avaliacaoNotaProduto", "5"],
  ["avaliacaoNotaLoja", "5"],
  ["avaliacaoTitulo", "Compra validada manualmente"],
  ["avaliacaoComentario", "Fluxo manual validado com sucesso."],
  ["avaliacaoRecomendaProduto", "true"],
  ["mediaPath", "C:\\\\caminho\\\\para\\\\imagem.png"],
  ["vendedorEmail", ""],
  ["vendedorPassword", ""],
  ["vendedorUsuarioId", ""],
  ["vendedorEnderecoId", ""],
  ["vendedorEnderecoCep", ""],
  ["vendedorEnderecoCidade", ""],
  ["vendedorEnderecoUf", ""],
  ["vendedorTelefoneId", ""],
  ["vendedorLojaId", ""],
  ["vendedorLojaSlug", ""],
  ["vendedorProdutoId", ""],
  ["vendedorEntregaOpcaoId", ""],
  ["compradorEmail", ""],
  ["compradorPassword", ""],
  ["compradorUsuarioId", ""],
  ["compradorEnderecoId", ""],
  ["compradorEnderecoCep", ""],
  ["compradorEnderecoCidade", ""],
  ["compradorEnderecoUf", ""],
  ["compradorTelefoneId", ""],
  ["adminEmail", ""],
  ["adminPassword", ""],
  ["adminUsuarioId", ""]
];

const makePreRequest = ({ required = [], extra = [] } = {}) => [
  ...runtimeScopeHelpers,
  ...lines(`
const baseUrl = getScopedVar("baseUrl", "${defaultBaseUrl}").replace(/\\/+$/, "");
setScopedVar("baseUrl", baseUrl);
`),
  ...required.map((key) => `requireScopedVar("${key}");`),
  ...extra
];

const withHelpers = (scriptText) => [...runtimeScopeHelpers, ...lines(scriptText)];

const registerBody = `{
  "cpf": "{{cadastroCpf}}",
  "nome": "{{cadastroNome}}",
  "sobrenome": "{{cadastroSobrenome}}",
  "email": "{{cadastroEmail}}",
  "password": "{{cadastroPassword}}",
  "confirmPassword": "{{cadastroConfirmPassword}}",
  "aceitouTermos": {{cadastroAceitouTermos}},
  "telefones": [
    {
      "ddd": "{{cadastroTelefoneDdd}}",
      "numero": "{{cadastroTelefoneNumero}}",
      "isPrincipal": {{cadastroTelefonePrincipal}}
    }
  ],
  "enderecos": [
    {
      "cep": "{{cadastroEnderecoCep}}",
      "tipoLogradouro": "{{cadastroEnderecoTipoLogradouro}}",
      "nomeEndereco": "{{cadastroEnderecoNome}}",
      "numero": "{{cadastroEnderecoNumero}}",
      "complemento": "{{cadastroEnderecoComplemento}}",
      "cidade": "{{cadastroEnderecoCidade}}",
      "uf": "{{cadastroEnderecoUf}}",
      "isPrincipal": {{cadastroEnderecoPrincipal}}
    }
  ]
}`;

const loginBody = `{
  "email": "{{loginEmail}}",
  "password": "{{loginPassword}}"
}`;

const addressBody = `{
  "cep": "{{enderecoCep}}",
  "tipoLogradouro": "{{enderecoTipoLogradouro}}",
  "nomeEndereco": "{{enderecoNome}}",
  "numero": "{{enderecoNumero}}",
  "complemento": "{{enderecoComplemento}}",
  "cidade": "{{enderecoCidade}}",
  "uf": "{{enderecoUf}}",
  "isPrincipal": {{enderecoPrincipal}}
}`;

const phoneBody = `{
  "ddd": "{{telefoneDdd}}",
  "numero": "{{telefoneNumero}}",
  "isPrincipal": {{telefonePrincipal}}
}`;

const storeBody = `{
  "nomeFantasia": "{{lojaNomeFantasia}}",
  "slug": "{{lojaSlug}}",
  "tipoDocumentoFiscal": "{{lojaTipoDocumentoFiscal}}",
  "documentoFiscal": "{{lojaDocumentoFiscal}}",
  "descricao": "{{lojaDescricao}}",
  "emailContato": "{{lojaEmailContato}}",
  "usarEnderecoUsuario": {{lojaUsarEnderecoUsuario}},
  "enderecoUsuarioId": {{lojaEnderecoUsuarioId}},
  "usarTelefoneUsuario": {{lojaUsarTelefoneUsuario}},
  "telefoneUsuarioId": {{lojaTelefoneUsuarioId}},
  "ativa": {{lojaAtiva}}
}`;

const productBody = `{
  "nome": "{{produtoNome}}",
  "categoria": "{{produtoCategoria}}",
  "sku": "{{produtoSku}}",
  "preco": {{produtoPreco}},
  "descricao": "{{produtoDescricao}}",
  "estoque": {{produtoEstoque}},
  "statusPublicacao": "{{produtoStatusPublicacao}}",
  "imagens": {{produtoImagensJson}}
}`;

const stockBody = `{
  "estoque": {{produtoEstoqueAtualizado}}
}`;

const deliveryBody = `{
  "tipoEntregaId": {{entregaTipoId}},
  "nome": "{{entregaNome}}",
  "valorFrete": {{entregaValorFrete}},
  "prazoEntregaDias": {{entregaPrazoEntregaDias}},
  "observacao": "{{entregaObservacao}}",
  "ativa": {{entregaAtiva}}
}`;

const cartBody = `{
  "produtoId": {{compraProdutoId}},
  "quantidade": {{compraQuantidade}}
}`;

const orderBody = `{
  "enderecoId": {{pedidoEnderecoId}},
  "tipoEntregaId": {{pedidoTipoEntregaId}},
  "observacao": "{{pedidoObservacao}}",
  "itens": []
}`;

const paymentBody = `{
  "pedidoId": {{pedidoAtualId}},
  "formaPagamentoId": {{pagamentoFormaPagamentoId}}
}`;

const reviewCreateBody = `{
  "pedidoId": {{pedidoAtualId}},
  "notaProduto": {{avaliacaoNotaProduto}},
  "notaLoja": {{avaliacaoNotaLoja}},
  "titulo": "{{avaliacaoTitulo}}",
  "comentario": "{{avaliacaoComentario}}",
  "recomendaProduto": {{avaliacaoRecomendaProduto}}
}`;

const reviewUpdateBody = `{
  "notaProduto": {{avaliacaoNotaProduto}},
  "notaLoja": {{avaliacaoNotaLoja}},
  "titulo": "{{avaliacaoTitulo}}",
  "comentario": "{{avaliacaoComentario}}",
  "recomendaProduto": {{avaliacaoRecomendaProduto}}
}`;

const collection = collectionInfo(
  "OmniMarket Manual Marketplace",
  [
    "Collection manual do OmniMarket para preencher tudo no Postman.",
    "O fluxo foi atualizado para o modelo atual da API, incluindo loja baseada no endereco/telefone do usuario, checkout simplificado, pagamento fake, recibo em PDF e marcacao de envio via admin.",
    "O login salva tokenAtual automaticamente e as requests de sessao rapida reaproveitam as credenciais salvas."
  ].join("\\n\\n")
);

collection.variable = environmentDefaults.map(([key, value]) => ({
  key,
  value,
  type: "string"
}));

collection.item = [
  folder("01 - Cadastro", [
    requestItem({
      name: "Registrar Usuario Completo Manual",
      method: "POST",
      path: ["api", "usuario", "registrar"],
      body: registerBody,
      description:
        "Cadastra um usuario com 1 telefone e 1 endereco iniciais. Edite as variaveis de cadastro antes de executar.",
      preRequest: makePreRequest({
        required: [
          "cadastroCpf",
          "cadastroNome",
          "cadastroSobrenome",
          "cadastroEmail",
          "cadastroPassword",
          "cadastroConfirmPassword",
          "cadastroTelefoneDdd",
          "cadastroTelefoneNumero",
          "cadastroEnderecoCep",
          "cadastroEnderecoTipoLogradouro",
          "cadastroEnderecoNome",
          "cadastroEnderecoNumero",
          "cadastroEnderecoCidade",
          "cadastroEnderecoUf"
        ]
      }),
      tests: withHelpers(`
pm.test("Cadastro retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (data && data.usuario) {
  maybeSetScopedVar("usuarioAtualId", data.usuario.id);
  maybeSetScopedVar("usuarioAtualEmail", data.usuario.email);
  maybeSetScopedVar("loginEmail", data.usuario.email);
}

setScopedVar("loginPassword", getScopedVar("cadastroPassword"));
unsetScopedVar("tokenAtual");
unsetScopedVar("usuarioAtualRole");
unsetScopedVar("enderecoAtualId");
unsetScopedVar("enderecoAtualCep");
unsetScopedVar("enderecoAtualCidade");
unsetScopedVar("enderecoAtualUf");
unsetScopedVar("telefoneAtualId");
unsetScopedVar("telefoneAtualNumero");
unsetScopedVar("lojaAtualId");
unsetScopedVar("lojaAtualSlug");
unsetScopedVar("produtoAtualId");
unsetScopedVar("entregaOpcaoAtualId");
unsetScopedVar("pedidoAtualId");
unsetScopedVar("planoPagamentoAtualId");
unsetScopedVar("avaliacaoAtualId");
`)
    })
  ]),
  folder("02 - Login e Perfis", [
    requestItem({
      name: "Login Manual",
      method: "POST",
      path: ["api", "auth", "login"],
      body: loginBody,
      description:
        "Faz login com as variaveis loginEmail e loginPassword e salva tokenAtual automaticamente.",
      preRequest: makePreRequest({
        required: ["loginEmail", "loginPassword"]
      }),
      tests: withHelpers(`
pm.test("Login retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (data.token) {
  setScopedVar("tokenAtual", data.token);
}
if (data.usuario && data.usuario.email) {
  setScopedVar("usuarioAtualEmail", data.usuario.email);
}
if (data.usuario && data.usuario.role) {
  setScopedVar("usuarioAtualRole", data.usuario.role);
}
`)
    }),
    requestItem({
      name: "Obter Perfil Atual",
      method: "GET",
      path: ["api", "usuario", "me"],
      authVar: "tokenAtual",
      description:
        "Busca o perfil do usuario autenticado e sincroniza usuarioAtualId, enderecoAtualId e telefoneAtualId.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Perfil retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
maybeSetScopedVar("usuarioAtualId", data.id);
maybeSetScopedVar("usuarioAtualEmail", data.email);
maybeSetScopedVar("usuarioAtualRole", data.role);

if (Array.isArray(data.enderecos) && data.enderecos.length > 0) {
  syncCurrentAddressFromObject(data.enderecos[0]);
}

if (Array.isArray(data.telefones) && data.telefones.length > 0) {
  syncCurrentPhoneFromObject(data.telefones[0]);
}

syncRoleFromCurrentIfMatch("vendedor");
syncRoleFromCurrentIfMatch("comprador");
syncRoleFromCurrentIfMatch("admin");
`)
    }),
    requestItem({
      name: "Salvar Sessao Atual Como Vendedor",
      method: "GET",
      path: ["api", "usuario", "me"],
      authVar: "tokenAtual",
      description:
        "Recarrega o perfil atual e salva email, senha, ids e referencias como sessao do vendedor.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "loginPassword"]
      }),
      tests: withHelpers(`
pm.test("Perfil do vendedor retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
maybeSetScopedVar("usuarioAtualId", data.id);
maybeSetScopedVar("usuarioAtualEmail", data.email);
maybeSetScopedVar("usuarioAtualRole", data.role);

if (Array.isArray(data.enderecos) && data.enderecos.length > 0) {
  syncCurrentAddressFromObject(data.enderecos[0]);
}

if (Array.isArray(data.telefones) && data.telefones.length > 0) {
  syncCurrentPhoneFromObject(data.telefones[0]);
}

saveCurrentSessionAs("vendedor");
`)
    }),
    requestItem({
      name: "Login Rapido Vendedor",
      method: "POST",
      path: ["api", "auth", "login"],
      body: loginBody,
      description:
        "Preenche loginEmail/loginPassword com as credenciais salvas do vendedor e executa o login.",
      preRequest: makePreRequest({
        extra: ["prepareQuickLogin(\"vendedor\");"]
      }),
      tests: withHelpers(`
pm.test("Login rapido do vendedor retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (data.token) {
  setScopedVar("tokenAtual", data.token);
}
if (data.usuario && data.usuario.email) {
  setScopedVar("usuarioAtualEmail", data.usuario.email);
}
if (data.usuario && data.usuario.role) {
  setScopedVar("usuarioAtualRole", data.usuario.role);
}
`)
    }),
    requestItem({
      name: "Salvar Sessao Atual Como Comprador",
      method: "GET",
      path: ["api", "usuario", "me"],
      authVar: "tokenAtual",
      description:
        "Recarrega o perfil atual e salva email, senha, ids e endereco como sessao do comprador.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "loginPassword"]
      }),
      tests: withHelpers(`
pm.test("Perfil do comprador retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
maybeSetScopedVar("usuarioAtualId", data.id);
maybeSetScopedVar("usuarioAtualEmail", data.email);
maybeSetScopedVar("usuarioAtualRole", data.role);

if (Array.isArray(data.enderecos) && data.enderecos.length > 0) {
  syncCurrentAddressFromObject(data.enderecos[0]);
}

if (Array.isArray(data.telefones) && data.telefones.length > 0) {
  syncCurrentPhoneFromObject(data.telefones[0]);
}

saveCurrentSessionAs("comprador");
maybeSetScopedVar("pedidoEnderecoId", getScopedVar("compradorEnderecoId"));
`)
    }),
    requestItem({
      name: "Login Rapido Comprador",
      method: "POST",
      path: ["api", "auth", "login"],
      body: loginBody,
      description:
        "Preenche loginEmail/loginPassword com as credenciais salvas do comprador e executa o login.",
      preRequest: makePreRequest({
        extra: ["prepareQuickLogin(\"comprador\");"]
      }),
      tests: withHelpers(`
pm.test("Login rapido do comprador retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (data.token) {
  setScopedVar("tokenAtual", data.token);
}
if (data.usuario && data.usuario.email) {
  setScopedVar("usuarioAtualEmail", data.usuario.email);
}
if (data.usuario && data.usuario.role) {
  setScopedVar("usuarioAtualRole", data.usuario.role);
}
`)
    }),
    requestItem({
      name: "Salvar Sessao Atual Como Admin",
      method: "GET",
      path: ["api", "usuario", "me"],
      authVar: "tokenAtual",
      description:
        "Recarrega o perfil atual e salva email, senha e ids como sessao do admin.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "loginPassword"]
      }),
      tests: withHelpers(`
pm.test("Perfil do admin retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
maybeSetScopedVar("usuarioAtualId", data.id);
maybeSetScopedVar("usuarioAtualEmail", data.email);
maybeSetScopedVar("usuarioAtualRole", data.role);

if (Array.isArray(data.enderecos) && data.enderecos.length > 0) {
  syncCurrentAddressFromObject(data.enderecos[0]);
}

if (Array.isArray(data.telefones) && data.telefones.length > 0) {
  syncCurrentPhoneFromObject(data.telefones[0]);
}

saveCurrentSessionAs("admin");
`)
    }),
    requestItem({
      name: "Login Rapido Admin",
      method: "POST",
      path: ["api", "auth", "login"],
      body: loginBody,
      description:
        "Preenche loginEmail/loginPassword com as credenciais salvas do admin e executa o login.",
      preRequest: makePreRequest({
        extra: ["prepareQuickLogin(\"admin\");"]
      }),
      tests: withHelpers(`
pm.test("Login rapido do admin retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (data.token) {
  setScopedVar("tokenAtual", data.token);
}
if (data.usuario && data.usuario.email) {
  setScopedVar("usuarioAtualEmail", data.usuario.email);
}
if (data.usuario && data.usuario.role) {
  setScopedVar("usuarioAtualRole", data.usuario.role);
}
`)
    })
  ]),
  folder("03 - Enderecos e Telefones", [
    requestItem({
      name: "Listar Tipos de Logradouro",
      method: "GET",
      path: ["api", "usuarios", "{{usuarioAtualId}}", "enderecos", "tipos-logradouro"],
      authVar: "tokenAtual",
      description:
        "Lista os tipos de logradouro aceitos pelo endpoint de enderecos. Usa usuarioAtualId do perfil atual.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "usuarioAtualId"]
      }),
      tests: withHelpers(`
pm.test("Tipos de logradouro retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});
`)
    }),
    requestItem({
      name: "Adicionar Endereco Manual",
      method: "POST",
      path: ["api", "usuarios", "{{usuarioAtualId}}", "enderecos"],
      authVar: "tokenAtual",
      body: addressBody,
      description:
        "Adiciona um novo endereco ao usuario autenticado usando as variaveis endereco... e salva enderecoAtualId.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "usuarioAtualId",
          "enderecoCep",
          "enderecoTipoLogradouro",
          "enderecoNome",
          "enderecoNumero",
          "enderecoCidade",
          "enderecoUf"
        ]
      }),
      tests: withHelpers(`
pm.test("Endereco criado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(201);
});

const data = readJsonResponse();
syncCurrentAddressFromObject(data);
syncRoleFromCurrentIfMatch("vendedor");
syncRoleFromCurrentIfMatch("comprador");
syncRoleFromCurrentIfMatch("admin");
`)
    }),
    requestItem({
      name: "Listar Enderecos",
      method: "GET",
      path: ["api", "usuarios", "{{usuarioAtualId}}", "enderecos"],
      authVar: "tokenAtual",
      description:
        "Lista os enderecos do usuario logado e reaproveita o primeiro id retornado em enderecoAtualId.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "usuarioAtualId"]
      }),
      tests: withHelpers(`
pm.test("Enderecos retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data) && data.length > 0) {
  syncCurrentAddressFromObject(data[0]);
}

syncRoleFromCurrentIfMatch("vendedor");
syncRoleFromCurrentIfMatch("comprador");
syncRoleFromCurrentIfMatch("admin");
`)
    }),
    requestItem({
      name: "Adicionar Telefone Manual",
      method: "POST",
      path: ["api", "telefones"],
      authVar: "tokenAtual",
      body: phoneBody,
      description:
        "Adiciona um telefone ao usuario autenticado. Depois do POST, sincroniza telefoneAtualId usando a listagem.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "telefoneDdd", "telefoneNumero"]
      }),
      tests: withHelpers(`
pm.test("Telefone criado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const token = getScopedVar("tokenAtual");
const baseUrl = getScopedVar("baseUrl", "${defaultBaseUrl}").replace(/\\/+$/, "");

pm.sendRequest(
  {
    url: baseUrl + "/api/telefones",
    method: "GET",
    header: [
      {
        key: "Authorization",
        value: "Bearer " + token
      }
    ]
  },
  function (error, response) {
    if (error || !response || response.code !== 200) {
      console.log("Nao foi possivel sincronizar telefoneAtualId automaticamente.");
      return;
    }

    try {
      const phones = response.json();
      if (!Array.isArray(phones) || phones.length === 0) {
        return;
      }

      syncCurrentPhoneFromObject(phones[0]);
      syncRoleFromCurrentIfMatch("vendedor");
      syncRoleFromCurrentIfMatch("comprador");
      syncRoleFromCurrentIfMatch("admin");
    } catch (syncError) {
      console.log("Falha ao ler a lista de telefones apos o cadastro.");
    }
  }
);
`)
    }),
    requestItem({
      name: "Listar Telefones",
      method: "GET",
      path: ["api", "telefones"],
      authVar: "tokenAtual",
      description:
        "Lista os telefones do usuario logado e reaproveita o primeiro id retornado em telefoneAtualId.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Telefones retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data) && data.length > 0) {
  syncCurrentPhoneFromObject(data[0]);
}

syncRoleFromCurrentIfMatch("vendedor");
syncRoleFromCurrentIfMatch("comprador");
syncRoleFromCurrentIfMatch("admin");
`)
    })
  ]),
  folder("04 - Loja", [
    requestItem({
      name: "Criar Minha Loja Manual",
      method: "POST",
      path: ["api", "lojas", "minha"],
      authVar: "tokenAtual",
      body: storeBody,
      description:
        "Cria a loja do usuario autenticado reutilizando por padrao o endereco e o telefone do usuario atual.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "lojaNomeFantasia",
          "lojaTipoDocumentoFiscal",
          "lojaUsarEnderecoUsuario",
          "lojaUsarTelefoneUsuario"
        ],
        extra: [
          "const lojaEmail = getScopedVar(\"lojaEmailContato\") || getScopedVar(\"usuarioAtualEmail\");",
          "if (lojaEmail) { setScopedVar(\"lojaEmailContato\", lojaEmail); }",
          "const documento = getScopedVar(\"lojaDocumentoFiscal\") || getScopedVar(\"cadastroCpf\");",
          "if (documento) { setScopedVar(\"lojaDocumentoFiscal\", documento); }",
          "if (getScopedVar(\"lojaUsarEnderecoUsuario\") === \"true\") {",
          "  const enderecoId = getScopedVar(\"lojaEnderecoUsuarioId\") || getScopedVar(\"enderecoAtualId\");",
          "  if (enderecoId) { setScopedVar(\"lojaEnderecoUsuarioId\", enderecoId); }",
          "  requireScopedVar(\"lojaEnderecoUsuarioId\", \"Nao foi possivel determinar o endereco do usuario para a loja.\");",
          "}",
          "if (getScopedVar(\"lojaUsarTelefoneUsuario\") === \"true\") {",
          "  const telefoneId = getScopedVar(\"lojaTelefoneUsuarioId\") || getScopedVar(\"telefoneAtualId\");",
          "  if (telefoneId) { setScopedVar(\"lojaTelefoneUsuarioId\", telefoneId); }",
          "  requireScopedVar(\"lojaTelefoneUsuarioId\", \"Nao foi possivel determinar o telefone do usuario para a loja.\");",
          "}"
        ]
      }),
      tests: withHelpers(`
pm.test("Loja criada com sucesso", function () {
  pm.expect(pm.response.code).to.eql(201);
});

const data = readJsonResponse();
syncCurrentStoreFromObject(data);
`)
    }),
    requestItem({
      name: "Obter Minha Loja",
      method: "GET",
      path: ["api", "lojas", "minha"],
      authVar: "tokenAtual",
      description:
        "Busca a loja vinculada ao usuario autenticado e sincroniza lojaAtualId e lojaAtualSlug.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Minha loja retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
syncCurrentStoreFromObject(data);
`)
    }),
    requestItem({
      name: "Atualizar Minha Loja Manual",
      method: "PUT",
      path: ["api", "lojas", "minha"],
      authVar: "tokenAtual",
      body: storeBody,
      description:
        "Atualiza a loja do usuario autenticado usando o mesmo payload manual da criacao.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "lojaNomeFantasia",
          "lojaTipoDocumentoFiscal",
          "lojaUsarEnderecoUsuario",
          "lojaUsarTelefoneUsuario"
        ],
        extra: [
          "const lojaEmail = getScopedVar(\"lojaEmailContato\") || getScopedVar(\"usuarioAtualEmail\");",
          "if (lojaEmail) { setScopedVar(\"lojaEmailContato\", lojaEmail); }",
          "const documento = getScopedVar(\"lojaDocumentoFiscal\") || getScopedVar(\"cadastroCpf\");",
          "if (documento) { setScopedVar(\"lojaDocumentoFiscal\", documento); }",
          "if (getScopedVar(\"lojaUsarEnderecoUsuario\") === \"true\") {",
          "  const enderecoId = getScopedVar(\"lojaEnderecoUsuarioId\") || getScopedVar(\"enderecoAtualId\");",
          "  if (enderecoId) { setScopedVar(\"lojaEnderecoUsuarioId\", enderecoId); }",
          "  requireScopedVar(\"lojaEnderecoUsuarioId\", \"Nao foi possivel determinar o endereco do usuario para a loja.\");",
          "}",
          "if (getScopedVar(\"lojaUsarTelefoneUsuario\") === \"true\") {",
          "  const telefoneId = getScopedVar(\"lojaTelefoneUsuarioId\") || getScopedVar(\"telefoneAtualId\");",
          "  if (telefoneId) { setScopedVar(\"lojaTelefoneUsuarioId\", telefoneId); }",
          "  requireScopedVar(\"lojaTelefoneUsuarioId\", \"Nao foi possivel determinar o telefone do usuario para a loja.\");",
          "}"
        ]
      }),
      tests: withHelpers(`
pm.test("Loja atualizada com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
syncCurrentStoreFromObject(data);
`)
    }),
    requestItem({
      name: "Obter Loja Atual por Slug",
      method: "GET",
      path: ["api", "lojas", "{{lojaAtualSlug}}"],
      description:
        "Consulta publicamente a loja atual pelo slug salvo em lojaAtualSlug.",
      preRequest: makePreRequest({
        required: ["lojaAtualSlug"]
      }),
      tests: withHelpers(`
pm.test("Loja por slug retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
syncCurrentStoreFromObject(data);
`)
    })
  ]),
  folder("05 - Produto", [
    requestItem({
      name: "Cadastrar Produto Manual",
      method: "POST",
      path: ["api", "produto"],
      authVar: "tokenAtual",
      body: productBody,
      description:
        "Cria um produto manualmente com as variaveis produto... e salva produtoAtualId.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "produtoNome",
          "produtoCategoria",
          "produtoSku",
          "produtoPreco",
          "produtoEstoque",
          "produtoStatusPublicacao"
        ]
      }),
      tests: withHelpers(`
pm.test("Produto criado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(201);
});

const data = readJsonResponse();
syncCurrentProductFromObject(data);
`)
    }),
    requestItem({
      name: "Listar Produtos Publicos",
      method: "GET",
      path: ["api", "produto"],
      description:
        "Lista os produtos publicos. Mantem o produtoAtualId se ele ja existir; caso contrario, usa o primeiro item retornado.",
      preRequest: makePreRequest(),
      tests: withHelpers(`
pm.test("Lista de produtos retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (!getScopedVar("produtoAtualId") && Array.isArray(data) && data.length > 0) {
  syncCurrentProductFromObject(data[0]);
}
`)
    }),
    requestItem({
      name: "Filtrar Produtos Publicos",
      method: "GET",
      path: ["api", "produto", "filtro"],
      query: [
        { key: "nome", value: "{{produtoNome}}" },
        { key: "page", value: "1" },
        { key: "pageSize", value: "10" }
      ],
      description:
        "Consulta a listagem paginada usando o nome do produto atual.",
      preRequest: makePreRequest({
        required: ["produtoNome"]
      }),
      tests: withHelpers(`
pm.test("Filtro de produtos retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
const items = getPageItems(data);
if (items.length > 0) {
  syncCurrentProductFromObject(items[0]);
}
`)
    }),
    requestItem({
      name: "Detalhar Produto Atual",
      method: "GET",
      path: ["api", "produto", "{{produtoAtualId}}"],
      description:
        "Busca o produto atual usando produtoAtualId.",
      preRequest: makePreRequest({
        required: ["produtoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Detalhe do produto retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
syncCurrentProductFromObject(data);
`)
    }),
    requestItem({
      name: "Atualizar Produto Atual",
      method: "PUT",
      path: ["api", "produto", "{{produtoAtualId}}"],
      authVar: "tokenAtual",
      body: productBody,
      description:
        "Atualiza o produto atual usando o mesmo payload manual da criacao.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "produtoAtualId",
          "produtoNome",
          "produtoCategoria",
          "produtoSku",
          "produtoPreco",
          "produtoEstoque",
          "produtoStatusPublicacao"
        ]
      }),
      tests: withHelpers(`
pm.test("Produto atualizado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(204);
});
`)
    }),
    requestItem({
      name: "Atualizar Estoque do Produto Atual",
      method: "PUT",
      path: ["api", "produto", "{{produtoAtualId}}", "estoque"],
      authVar: "tokenAtual",
      body: stockBody,
      description:
        "Atualiza apenas o estoque do produto atual.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "produtoAtualId", "produtoEstoqueAtualizado"]
      }),
      tests: withHelpers(`
pm.test("Estoque atualizado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(204);
});

setScopedVar("produtoEstoque", getScopedVar("produtoEstoqueAtualizado"));
`)
    }),
    requestItem({
      name: "Listar Midias do Produto Atual",
      method: "GET",
      path: ["api", "produtos", "{{produtoAtualId}}", "midias"],
      description:
        "Lista as midias do produto atual e salva a primeira quando existir.",
      preRequest: makePreRequest({
        required: ["produtoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Midias do produto retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data) && data.length > 0) {
  maybeSetScopedVar("produtoMidiaId", data[0].id);
}
`)
    }),
    requestItem({
      name: "Upload Midia do Produto Atual",
      method: "POST",
      path: ["api", "produtos", "{{produtoAtualId}}", "midias"],
      authVar: "tokenAtual",
      formData: [
        {
          key: "arquivos",
          type: "file",
          src: "{{mediaPath}}"
        }
      ],
      description:
        "Opcional. Envie uma imagem para o produto atual preenchendo mediaPath no environment.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "produtoAtualId", "mediaPath"]
      }),
      tests: withHelpers(`
pm.test("Upload de midia retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data) && data.length > 0) {
  maybeSetScopedVar("produtoMidiaId", data[0].id);
}
`)
    })
  ]),
  folder("06 - Entregas da Loja", [
    requestItem({
      name: "Criar Opcao de Entrega Manual",
      method: "POST",
      path: ["api", "lojas", "minha", "entregas"],
      authVar: "tokenAtual",
      body: deliveryBody,
      description:
        "Cria uma opcao de entrega para a loja do vendedor e salva os dados da opcao para o pedido.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "entregaTipoId",
          "entregaNome",
          "entregaValorFrete",
          "entregaPrazoEntregaDias"
        ]
      }),
      tests: withHelpers(`
pm.test("Opcao de entrega criada com sucesso", function () {
  pm.expect(pm.response.code).to.eql(201);
});

const data = readJsonResponse();
syncCurrentDeliveryFromObject(data);
`)
    }),
    requestItem({
      name: "Listar Minhas Entregas",
      method: "GET",
      path: ["api", "lojas", "minha", "entregas"],
      authVar: "tokenAtual",
      description:
        "Lista as opcoes de entrega da loja do usuario autenticado e usa a primeira para o checkout.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Minhas entregas retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data) && data.length > 0) {
  syncCurrentDeliveryFromObject(data[0]);
}
`)
    }),
    requestItem({
      name: "Atualizar Opcao de Entrega Atual",
      method: "PUT",
      path: ["api", "lojas", "minha", "entregas", "{{entregaOpcaoAtualId}}"],
      authVar: "tokenAtual",
      body: deliveryBody,
      description:
        "Atualiza a opcao de entrega atual usando o mesmo payload manual da criacao.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "entregaOpcaoAtualId",
          "entregaTipoId",
          "entregaNome",
          "entregaValorFrete",
          "entregaPrazoEntregaDias"
        ]
      }),
      tests: withHelpers(`
pm.test("Opcao de entrega atualizada com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
syncCurrentDeliveryFromObject(data);
`)
    }),
    requestItem({
      name: "Listar Entregas Publicas da Loja do Vendedor",
      method: "GET",
      path: ["api", "lojas", "{{vendedorLojaId}}", "entregas"],
      query: [
        { key: "cep", value: "{{compradorEnderecoCep}}" },
        { key: "cidade", value: "{{compradorEnderecoCidade}}" },
        { key: "uf", value: "{{compradorEnderecoUf}}" }
      ],
      description:
        "Lista as entregas publicas da loja do vendedor usando o endereco salvo do comprador e seleciona a primeira opcao.",
      preRequest: makePreRequest({
        extra: [
          "const lojaId = getScopedVar(\"vendedorLojaId\") || getScopedVar(\"lojaAtualId\");",
          "if (lojaId) { setScopedVar(\"vendedorLojaId\", lojaId); }",
          "const cep = getScopedVar(\"compradorEnderecoCep\") || getScopedVar(\"enderecoAtualCep\");",
          "const cidade = getScopedVar(\"compradorEnderecoCidade\") || getScopedVar(\"enderecoAtualCidade\");",
          "const uf = getScopedVar(\"compradorEnderecoUf\") || getScopedVar(\"enderecoAtualUf\");",
          "if (cep) { setScopedVar(\"compradorEnderecoCep\", cep); }",
          "if (cidade) { setScopedVar(\"compradorEnderecoCidade\", cidade); }",
          "if (uf) { setScopedVar(\"compradorEnderecoUf\", uf); }",
          "requireScopedVar(\"vendedorLojaId\", \"Defina ou salve primeiro a loja do vendedor.\");",
          "requireScopedVar(\"compradorEnderecoCep\", \"Salve primeiro uma sessao de comprador com endereco valido.\");",
          "requireScopedVar(\"compradorEnderecoCidade\", \"Salve primeiro uma sessao de comprador com endereco valido.\");",
          "requireScopedVar(\"compradorEnderecoUf\", \"Salve primeiro uma sessao de comprador com endereco valido.\");"
        ]
      }),
      tests: withHelpers(`
pm.test("Entregas publicas retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data) && data.length > 0) {
  syncCurrentDeliveryFromObject(data[0]);
}
`)
    })
  ]),
  folder("07 - Comercial Comprador", [
    requestItem({
      name: "Limpar Carrinho Atual",
      method: "DELETE",
      path: ["api", "carrinho"],
      authVar: "tokenAtual",
      description:
        "Limpa o carrinho do comprador atual antes de iniciar um novo teste comercial.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Limpeza do carrinho retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});
`)
    }),
    requestItem({
      name: "Adicionar Produto ao Carrinho",
      method: "POST",
      path: ["api", "carrinho"],
      authVar: "tokenAtual",
      body: cartBody,
      description:
        "Adiciona o produto salvo para compra no carrinho do comprador autenticado.",
      preRequest: makePreRequest({
        extra: [
          "const productId = getScopedVar(\"compraProdutoId\") || getScopedVar(\"vendedorProdutoId\") || getScopedVar(\"produtoAtualId\");",
          "if (productId) { setScopedVar(\"compraProdutoId\", productId); }",
          "requireScopedVar(\"tokenAtual\");",
          "requireScopedVar(\"compraProdutoId\", \"Defina primeiro o produto da venda ou crie um produto do vendedor.\");",
          "requireScopedVar(\"compraQuantidade\");"
        ]
      }),
      tests: withHelpers(`
pm.test("Produto adicionado ao carrinho com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data.itens) && data.itens.length > 0) {
  const first = data.itens[0];
  maybeSetScopedVar("compraProdutoId", first.produtoId);
  maybeSetScopedVar("vendedorLojaId", first.lojaId);
  maybeSetScopedVar("vendedorLojaSlug", first.slugLoja);
}
`)
    }),
    requestItem({
      name: "Ver Carrinho Atual",
      method: "GET",
      path: ["api", "carrinho"],
      authVar: "tokenAtual",
      description:
        "Consulta o carrinho atual do comprador e sincroniza produto e loja da compra.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Carrinho retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data.itens) && data.itens.length > 0) {
  const first = data.itens[0];
  maybeSetScopedVar("compraProdutoId", first.produtoId);
  maybeSetScopedVar("vendedorLojaId", first.lojaId);
  maybeSetScopedVar("vendedorLojaSlug", first.slugLoja);
}
`)
    }),
    requestItem({
      name: "Gerar Pedido do Carrinho",
      method: "POST",
      path: ["api", "pedidos"],
      authVar: "tokenAtual",
      body: orderBody,
      description:
        "Gera um pedido a partir do carrinho atual usando o endereco do comprador e o tipo de entrega atual.",
      preRequest: makePreRequest({
        extra: [
          "const pedidoEnderecoId = getScopedVar(\"pedidoEnderecoId\") || getScopedVar(\"compradorEnderecoId\") || getScopedVar(\"enderecoAtualId\");",
          "if (pedidoEnderecoId) { setScopedVar(\"pedidoEnderecoId\", pedidoEnderecoId); }",
          "const tipoEntrega = getScopedVar(\"pedidoTipoEntregaId\") || getScopedVar(\"entregaAtualTipoId\") || getScopedVar(\"entregaTipoId\");",
          "if (tipoEntrega) { setScopedVar(\"pedidoTipoEntregaId\", tipoEntrega); }",
          "requireScopedVar(\"tokenAtual\");",
          "requireScopedVar(\"pedidoEnderecoId\", \"Salve primeiro uma sessao de comprador com endereco valido.\");",
          "requireScopedVar(\"pedidoTipoEntregaId\", \"Selecione primeiro o tipo de entrega do pedido.\");"
        ]
      }),
      tests: withHelpers(`
pm.test("Pedido criado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
syncCurrentOrderFromObject(data);
`)
    }),
    requestItem({
      name: "Buscar Pedido Atual",
      method: "GET",
      path: ["api", "pedidos", "{{pedidoAtualId}}"],
      authVar: "tokenAtual",
      description:
        "Consulta o pedido atual do comprador para validar os dados gerados no checkout.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "pedidoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Pedido atual retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
syncCurrentOrderFromObject(data);
`)
    }),
    requestItem({
      name: "Iniciar Pagamento",
      method: "POST",
      path: ["api", "financeiro", "pagamentos", "iniciar"],
      authVar: "tokenAtual",
      body: paymentBody,
      description:
        "Cria o plano de pagamento do pedido atual. Por padrao usa Pix (FormaPagamentoId=1).",
      preRequest: makePreRequest({
        required: ["tokenAtual", "pedidoAtualId", "pagamentoFormaPagamentoId"]
      }),
      tests: withHelpers(`
pm.test("Pagamento iniciado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
maybeSetScopedVar("pedidoAtualId", data.pedidoId);
maybeSetScopedVar("planoPagamentoAtualId", data.planoPagamentoId);
maybeSetScopedVar("pedidoValorTotal", data.valorTotal);
`)
    }),
    requestItem({
      name: "Confirmar Pagamento Fake",
      method: "POST",
      path: ["api", "financeiro", "pagamentos", "{{planoPagamentoAtualId}}", "confirmar-fake"],
      authVar: "tokenAtual",
      description:
        "Confirma o pagamento fake do pedido atual para seguir com o fluxo operacional.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "planoPagamentoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Pagamento fake confirmado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
maybeSetScopedVar("planoPagamentoAtualId", data.planoPagamentoId);
maybeSetScopedVar("pedidoAtualId", data.pedidoId);
maybeSetScopedVar("pedidoStatusAtual", data.statusPagamento);
`)
    }),
    requestItem({
      name: "Listar Pedidos do Usuario Atual",
      method: "GET",
      path: ["api", "pedidos", "usuario", "{{usuarioAtualId}}"],
      authVar: "tokenAtual",
      description:
        "Lista os pedidos do comprador atual e usa o primeiro como pedidoAtualId quando necessario.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "usuarioAtualId"]
      }),
      tests: withHelpers(`
pm.test("Lista de pedidos do usuario retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data) && data.length > 0) {
  syncCurrentOrderFromObject(data[0]);
}
`)
    })
  ]),
  folder("08 - Admin Operacional", [
    requestItem({
      name: "Dashboard Admin",
      method: "GET",
      path: ["api", "admin", "dashboard"],
      authVar: "tokenAtual",
      description:
        "Consulta o dashboard agregado da administracao.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Dashboard admin retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});
`)
    }),
    requestItem({
      name: "Listar Usuarios Admin",
      method: "GET",
      path: ["api", "admin", "usuarios"],
      query: [
        { key: "page", value: "1" },
        { key: "pageSize", value: "20" }
      ],
      authVar: "tokenAtual",
      description:
        "Lista usuarios visiveis pelo admin.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Usuarios admin retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
const items = getPageItems(data);
if (items.length > 0 && !getScopedVar("adminUsuarioId")) {
  maybeSetScopedVar("adminUsuarioId", items[0].id);
}
`)
    }),
    requestItem({
      name: "Listar Lojas Admin",
      method: "GET",
      path: ["api", "admin", "lojas"],
      query: [
        { key: "page", value: "1" },
        { key: "pageSize", value: "20" }
      ],
      authVar: "tokenAtual",
      description:
        "Lista lojas visiveis pelo admin.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Lojas admin retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});
`)
    }),
    requestItem({
      name: "Listar Produtos Admin",
      method: "GET",
      path: ["api", "admin", "produtos"],
      query: [
        { key: "page", value: "1" },
        { key: "pageSize", value: "20" }
      ],
      authVar: "tokenAtual",
      description:
        "Lista produtos pelo endpoint administrativo.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Produtos admin retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});
`)
    }),
    requestItem({
      name: "Listar Pedidos Admin",
      method: "GET",
      path: ["api", "admin", "pedidos"],
      query: [
        { key: "page", value: "1" },
        { key: "pageSize", value: "20" }
      ],
      authVar: "tokenAtual",
      description:
        "Lista pedidos pelo endpoint administrativo e usa o primeiro como fallback para pedidoAtualId.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Pedidos admin retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
const items = getPageItems(data);
if (items.length > 0 && !getScopedVar("pedidoAtualId")) {
  maybeSetScopedVar("pedidoAtualId", items[0].id);
}
`)
    }),
    requestItem({
      name: "Marcar Pedido Atual Como Enviado",
      method: "PUT",
      path: ["api", "admin", "pedidos", "{{pedidoAtualId}}", "enviar"],
      authVar: "tokenAtual",
      description:
        "Move o pedido pago para enviado, habilitando a confirmacao de entrega pelo comprador.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "pedidoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Pedido marcado como enviado com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
maybeSetScopedVar("pedidoAtualId", data.pedidoId);
maybeSetScopedVar("pedidoStatusAtual", data.status);
`)
    }),
    requestItem({
      name: "Listar Vendas Financeiras Admin",
      method: "GET",
      path: ["api", "admin", "financeiro", "vendas"],
      query: [
        { key: "page", value: "1" },
        { key: "pageSize", value: "20" }
      ],
      authVar: "tokenAtual",
      description:
        "Lista as vendas financeiras do marketplace.",
      preRequest: makePreRequest({
        required: ["tokenAtual"]
      }),
      tests: withHelpers(`
pm.test("Vendas financeiras retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});
`)
    })
  ]),
  folder("09 - Pos-venda Comprador", [
    requestItem({
      name: "Baixar Recibo do Pedido Atual",
      method: "GET",
      path: ["api", "pedidos", "{{pedidoAtualId}}", "recibo"],
      authVar: "tokenAtual",
      headers: [
        {
          key: "Accept",
          value: "application/pdf"
        }
      ],
      description:
        "Baixa o comprovante de venda em PDF do pedido atual quando ele estiver pago, enviado ou entregue.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "pedidoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Recibo em PDF retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
  pm.expect(pm.response.headers.get("Content-Type")).to.include("application/pdf");
});
`)
    }),
    requestItem({
      name: "Confirmar Entrega do Pedido Atual",
      method: "PUT",
      path: ["api", "pedidos", "{{pedidoAtualId}}", "confirmar-entrega"],
      authVar: "tokenAtual",
      description:
        "Depois que o admin libera o fulfillment, o comprador confirma o recebimento do pedido atual.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "pedidoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Entrega confirmada com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
maybeSetScopedVar("pedidoAtualId", data.pedidoId);
maybeSetScopedVar("pedidoStatusAtual", data.status);
`)
    }),
    requestItem({
      name: "Buscar Pedido Atual Pos-venda",
      method: "GET",
      path: ["api", "pedidos", "{{pedidoAtualId}}"],
      authVar: "tokenAtual",
      description:
        "Reconsulta o pedido atual apos a confirmacao de entrega.",
      preRequest: makePreRequest({
        required: ["tokenAtual", "pedidoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Pedido pos-venda retornou sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
syncCurrentOrderFromObject(data);
`)
    }),
    requestItem({
      name: "Criar Avaliacao do Produto Atual",
      method: "POST",
      path: ["api", "produtos", "{{produtoAtualId}}", "avaliacoes"],
      authVar: "tokenAtual",
      body: reviewCreateBody,
      description:
        "Cria a avaliacao do produto atual para o pedido entregue.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "produtoAtualId",
          "pedidoAtualId",
          "avaliacaoNotaProduto",
          "avaliacaoRecomendaProduto"
        ]
      }),
      tests: withHelpers(`
pm.test("Avaliacao criada com sucesso", function () {
  pm.expect(pm.response.code).to.eql(201);
});

const data = readJsonResponse();
maybeSetScopedVar("avaliacaoAtualId", data.id);
`)
    }),
    requestItem({
      name: "Atualizar Avaliacao Atual",
      method: "PUT",
      path: ["api", "produtos", "{{produtoAtualId}}", "avaliacoes", "{{avaliacaoAtualId}}"],
      authVar: "tokenAtual",
      body: reviewUpdateBody,
      description:
        "Atualiza a avaliacao atual do produto.",
      preRequest: makePreRequest({
        required: [
          "tokenAtual",
          "produtoAtualId",
          "avaliacaoAtualId",
          "avaliacaoNotaProduto",
          "avaliacaoRecomendaProduto"
        ]
      }),
      tests: withHelpers(`
pm.test("Avaliacao atualizada com sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});
`)
    }),
    requestItem({
      name: "Listar Avaliacoes do Produto Atual",
      method: "GET",
      path: ["api", "produtos", "{{produtoAtualId}}", "avaliacoes"],
      query: [
        { key: "page", value: "1" },
        { key: "pageSize", value: "10" }
      ],
      description:
        "Lista as avaliacoes publicas do produto atual.",
      preRequest: makePreRequest({
        required: ["produtoAtualId"]
      }),
      tests: withHelpers(`
pm.test("Avaliacoes do produto retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});

const data = readJsonResponse();
if (Array.isArray(data) && data.length > 0) {
  maybeSetScopedVar("avaliacaoAtualId", data[0].id);
}
`)
    }),
    requestItem({
      name: "Listar Avaliacoes da Loja Atual",
      method: "GET",
      path: ["api", "lojas", "{{lojaAtualSlug}}", "avaliacoes"],
      query: [
        { key: "page", value: "1" },
        { key: "pageSize", value: "10" }
      ],
      description:
        "Lista as avaliacoes publicas da loja atual.",
      preRequest: makePreRequest({
        extra: [
          "const slug = getScopedVar(\"lojaAtualSlug\") || getScopedVar(\"vendedorLojaSlug\");",
          "if (slug) { setScopedVar(\"lojaAtualSlug\", slug); }",
          "requireScopedVar(\"lojaAtualSlug\", \"Defina primeiro o slug da loja atual.\");"
        ]
      }),
      tests: withHelpers(`
pm.test("Avaliacoes da loja retornaram sucesso", function () {
  pm.expect(pm.response.code).to.eql(200);
});
`)
    })
  ])
];

const environment = buildEnvironment(
  "OmniMarket Manual Marketplace Local",
  environmentDefaults
);

await mkdir(outputDir, { recursive: true });

await writeFile(
  `${outputDir}/OmniMarket.manual-marketplace.postman_collection.json`,
  `${JSON.stringify(collection, null, 2)}\n`,
  "utf8"
);

await writeFile(
  `${outputDir}/OmniMarket.manual-marketplace.local.postman_environment.json`,
  `${JSON.stringify(environment, null, 2)}\n`,
  "utf8"
);

console.log("Arquivos manuais do Postman gerados com sucesso em postman/.");

