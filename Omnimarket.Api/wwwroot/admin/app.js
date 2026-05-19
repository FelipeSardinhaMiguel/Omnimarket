const storageKeys = {
    token: "omnimarket.admin.token",
    user: "omnimarket.admin.user"
};

const state = {
    token: localStorage.getItem(storageKeys.token),
    user: readUser(),
    currentView: "dashboard"
};

const views = {
    dashboard: "Dashboard",
    usuarios: "Usuarios",
    lojas: "Lojas",
    produtos: "Produtos",
    pedidos: "Pedidos",
    financeiro: "Financeiro"
};

const roles = ["User", "Vendedor", "Admin", "Suporte"];
const produtoStatus = ["Rascunho", "Publicado", "Pausado", "Desativado"];

const loginScreen = document.querySelector("#login-screen");
const adminApp = document.querySelector("#admin-app");
const loginForm = document.querySelector("#login-form");
const loginMessage = document.querySelector("#login-message");
const content = document.querySelector("#content");
const statusBar = document.querySelector("#status-bar");
const viewTitle = document.querySelector("#view-title");
const userLabel = document.querySelector("#user-label");
const logoutButton = document.querySelector("#logout-button");

loginForm.addEventListener("submit", handleLogin);
logoutButton.addEventListener("click", logout);

document.querySelectorAll(".nav-item").forEach((button) => {
    button.addEventListener("click", () => {
        document.querySelectorAll(".nav-item").forEach((item) => item.classList.remove("active"));
        button.classList.add("active");
        loadView(button.dataset.view);
    });
});

boot();

function boot() {
    if (!state.token) {
        showLogin();
        return;
    }

    showApp();
    loadView(state.currentView);
}

async function handleLogin(event) {
    event.preventDefault();
    loginMessage.textContent = "";

    const payload = {
        email: document.querySelector("#email").value.trim(),
        password: document.querySelector("#password").value
    };

    try {
        const response = await fetch("/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            loginMessage.textContent = data.mensagem || "Nao foi possivel entrar.";
            return;
        }

        if (data.usuario?.role !== "Admin") {
            loginMessage.textContent = "Este usuario nao tem permissao admin.";
            return;
        }

        state.token = data.token;
        state.user = data.usuario;
        localStorage.setItem(storageKeys.token, data.token);
        localStorage.setItem(storageKeys.user, JSON.stringify(data.usuario));

        showApp();
        loadView("dashboard");
    } catch {
        loginMessage.textContent = "Falha ao conectar com a API.";
    }
}

function logout() {
    localStorage.removeItem(storageKeys.token);
    localStorage.removeItem(storageKeys.user);
    state.token = null;
    state.user = null;
    showLogin();
}

function showLogin() {
    loginScreen.classList.remove("hidden");
    adminApp.classList.add("hidden");
}

function showApp() {
    loginScreen.classList.add("hidden");
    adminApp.classList.remove("hidden");
    userLabel.textContent = state.user?.email || "";
}

async function loadView(view) {
    state.currentView = view;
    viewTitle.textContent = views[view] || "Dashboard";
    content.innerHTML = "<div class=\"empty-state\">Carregando...</div>";
    hideStatus();

    const loaders = {
        dashboard: renderDashboard,
        usuarios: renderUsuarios,
        lojas: renderLojas,
        produtos: renderProdutos,
        pedidos: renderPedidos,
        financeiro: renderFinanceiro
    };

    await loaders[view]();
}

async function renderDashboard() {
    const data = await apiGet("/api/admin/dashboard");
    if (!data) return;

    const stats = [
        ["Usuarios", data.totalUsuarios],
        ["Admins", data.totalAdmins],
        ["Lojas ativas", `${data.totalLojasAtivas}/${data.totalLojas}`],
        ["Produtos publicados", `${data.produtosPublicados}/${data.totalProdutos}`],
        ["Pedidos", data.totalPedidos],
        ["Pedidos pendentes", data.pedidosPendentes],
        ["Pedidos pagos", data.pedidosPagos],
        ["Receita total", money(data.receitaTotalMarketplace)]
    ];

    content.innerHTML = `
        <section class="stat-grid">
            ${stats.map(([label, value]) => `
                <article class="stat-panel">
                    <p class="stat-label">${escapeHtml(label)}</p>
                    <p class="stat-value">${escapeHtml(value)}</p>
                </article>
            `).join("")}
        </section>
    `;
}

async function renderUsuarios() {
    content.innerHTML = renderToolbar("usuarios", `
        <label>Busca <input id="usuarios-busca" type="search" placeholder="Nome, email ou CPF"></label>
        <label>Role
            <select id="usuarios-role">
                <option value="">Todas</option>
                ${roles.map((role) => `<option value="${role}">${role}</option>`).join("")}
            </select>
        </label>
        <button class="primary-action" type="button" id="usuarios-filtrar">Filtrar</button>
    `) + renderPanel("Usuarios", "usuarios-table");

    document.querySelector("#usuarios-filtrar").addEventListener("click", carregarUsuarios);
    await carregarUsuarios();
}

async function carregarUsuarios() {
    const busca = document.querySelector("#usuarios-busca").value;
    const role = document.querySelector("#usuarios-role").value;
    const data = await apiGet(`/api/admin/usuarios?pageSize=50&busca=${encode(busca)}&role=${encode(role)}`);
    if (!data) return;

    const rows = data.items.map((usuario) => `
        <tr>
            <td>${usuario.id}</td>
            <td>${escapeHtml(usuario.nome)}</td>
            <td>${escapeHtml(usuario.email)}</td>
            <td>${escapeHtml(usuario.cpf)}</td>
            <td>${renderRoleSelect(usuario.id, usuario.role)}</td>
            <td>${usuario.possuiLoja ? renderBadge(usuario.lojaAtiva ? "Loja ativa" : "Loja inativa", usuario.lojaAtiva ? "ok" : "warn") : renderBadge("Sem loja")}</td>
            <td>${usuario.totalProdutos}</td>
            <td><button class="table-action" type="button" data-user-role="${usuario.id}">Salvar</button></td>
        </tr>
    `).join("");

    fillTable("usuarios-table", ["Id", "Nome", "Email", "CPF", "Role", "Loja", "Produtos", "Acao"], rows);

    document.querySelectorAll("[data-user-role]").forEach((button) => {
        button.addEventListener("click", async () => {
            const id = button.dataset.userRole;
            const role = document.querySelector(`[data-role-select="${id}"]`).value;
            const ok = await apiSend(`/api/admin/usuarios/${id}/role`, "PUT", { role });
            if (ok) showStatus("Role atualizada.");
        });
    });
}

async function renderLojas() {
    content.innerHTML = renderToolbar("lojas", `
        <label>Busca <input id="lojas-busca" type="search" placeholder="Loja, slug ou responsavel"></label>
        <label>Status
            <select id="lojas-status">
                <option value="">Todas</option>
                <option value="true">Ativas</option>
                <option value="false">Inativas</option>
            </select>
        </label>
        <button class="primary-action" type="button" id="lojas-filtrar">Filtrar</button>
    `) + renderPanel("Lojas", "lojas-table");

    document.querySelector("#lojas-filtrar").addEventListener("click", carregarLojas);
    await carregarLojas();
}

async function carregarLojas() {
    const busca = document.querySelector("#lojas-busca").value;
    const ativa = document.querySelector("#lojas-status").value;
    const data = await apiGet(`/api/admin/lojas?pageSize=50&busca=${encode(busca)}&ativa=${encode(ativa)}`);
    if (!data) return;

    const rows = data.items.map((loja) => `
        <tr>
            <td>${loja.id}</td>
            <td>${escapeHtml(loja.nomeFantasia)}</td>
            <td>${escapeHtml(loja.slug)}</td>
            <td>${escapeHtml(loja.nomeResponsavel)}</td>
            <td>${loja.ativa ? renderBadge("Ativa", "ok") : renderBadge("Inativa", "warn")}</td>
            <td>${loja.totalProdutos}</td>
            <td>${loja.produtosPublicados}</td>
            <td><button class="table-action" type="button" data-store-status="${loja.id}" data-active="${loja.ativa}">${loja.ativa ? "Desativar" : "Ativar"}</button></td>
        </tr>
    `).join("");

    fillTable("lojas-table", ["Id", "Loja", "Slug", "Responsavel", "Status", "Produtos", "Publicados", "Acao"], rows);

    document.querySelectorAll("[data-store-status]").forEach((button) => {
        button.addEventListener("click", async () => {
            const id = button.dataset.storeStatus;
            const ativa = button.dataset.active !== "true";
            const ok = await apiSend(`/api/admin/lojas/${id}/status`, "PUT", { ativa });
            if (ok) {
                showStatus("Status da loja atualizado.");
                await carregarLojas();
            }
        });
    });
}

async function renderProdutos() {
    content.innerHTML = renderToolbar("produtos", `
        <label>Busca <input id="produtos-busca" type="search" placeholder="Nome, SKU, loja ou categoria"></label>
        <label>Status
            <select id="produtos-status">
                <option value="">Todos</option>
                ${produtoStatus.map((status) => `<option value="${status}">${status}</option>`).join("")}
            </select>
        </label>
        <button class="primary-action" type="button" id="produtos-filtrar">Filtrar</button>
    `) + renderPanel("Produtos", "produtos-table");

    document.querySelector("#produtos-filtrar").addEventListener("click", carregarProdutos);
    await carregarProdutos();
}

async function carregarProdutos() {
    const busca = document.querySelector("#produtos-busca").value;
    const status = document.querySelector("#produtos-status").value;
    const data = await apiGet(`/api/admin/produtos?pageSize=50&busca=${encode(busca)}&status=${encode(status)}`);
    if (!data) return;

    const rows = data.items.map((produto) => `
        <tr>
            <td>${produto.id}</td>
            <td>${escapeHtml(produto.nome)}</td>
            <td>${escapeHtml(produto.sku)}</td>
            <td>${escapeHtml(produto.nomeLoja || produto.nomeVendedor)}</td>
            <td>${money(produto.preco)}</td>
            <td>${produto.estoque}</td>
            <td>${renderProdutoStatusSelect(produto.id, produto.statusPublicacao)}</td>
            <td><button class="table-action" type="button" data-product-status="${produto.id}">Salvar</button></td>
        </tr>
    `).join("");

    fillTable("produtos-table", ["Id", "Produto", "SKU", "Loja", "Preco", "Estoque", "Status", "Acao"], rows);

    document.querySelectorAll("[data-product-status]").forEach((button) => {
        button.addEventListener("click", async () => {
            const id = button.dataset.productStatus;
            const statusPublicacao = document.querySelector(`[data-product-select="${id}"]`).value;
            const ok = await apiSend(`/api/admin/produtos/${id}/status`, "PUT", { statusPublicacao });
            if (ok) showStatus("Status do produto atualizado.");
        });
    });
}

async function renderPedidos() {
    content.innerHTML = renderToolbar("pedidos", `
        <label>Busca <input id="pedidos-busca" type="search" placeholder="Cliente ou cidade"></label>
        <button class="primary-action" type="button" id="pedidos-filtrar">Filtrar</button>
    `) + renderPanel("Pedidos", "pedidos-table");

    document.querySelector("#pedidos-filtrar").addEventListener("click", carregarPedidos);
    await carregarPedidos();
}

async function carregarPedidos() {
    const busca = document.querySelector("#pedidos-busca").value;
    const data = await apiGet(`/api/admin/pedidos?pageSize=50&busca=${encode(busca)}`);
    if (!data) return;

    const rows = data.items.map((pedido) => `
        <tr>
            <td>${pedido.id}</td>
            <td>${escapeHtml(pedido.nomeCliente)}</td>
            <td>${renderBadge(pedido.status, badgeClass(pedido.status))}</td>
            <td>${pedido.quantidadeItens}</td>
            <td>${money(pedido.valorTotalPedido)}</td>
            <td>${escapeHtml(pedido.cidadeEntrega)} / ${escapeHtml(pedido.ufEntrega)}</td>
            <td>${date(pedido.dataPedido)}</td>
        </tr>
    `).join("");

    fillTable("pedidos-table", ["Id", "Cliente", "Status", "Itens", "Total", "Entrega", "Data"], rows);
}

async function renderFinanceiro() {
    content.innerHTML = renderPanel("Vendas", "financeiro-table");
    const data = await apiGet("/api/admin/financeiro/vendas?pageSize=50");
    if (!data) return;

    const rows = data.items.map((venda) => `
        <tr>
            <td>${venda.id}</td>
            <td>${venda.pedidoId}</td>
            <td>${escapeHtml(venda.nomeVendedor)}</td>
            <td>${money(venda.valorBruto)}</td>
            <td>${money(venda.valorComissao)}</td>
            <td>${money(venda.valorLiquido)}</td>
            <td>${renderBadge(venda.statusVenda, badgeClass(venda.statusVenda))}</td>
            <td>${date(venda.dataCriacao)}</td>
        </tr>
    `).join("");

    fillTable("financeiro-table", ["Venda", "Pedido", "Vendedor", "Bruto", "Comissao", "Liquido", "Status", "Data"], rows);
}

async function apiGet(path) {
    return apiRequest(path, { method: "GET" });
}

async function apiSend(path, method, body) {
    const data = await apiRequest(path, {
        method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    return Boolean(data);
}

async function apiRequest(path, options) {
    try {
        const response = await fetch(path, {
            ...options,
            headers: {
                ...(options.headers || {}),
                Authorization: `Bearer ${state.token}`
            }
        });

        if (response.status === 401 || response.status === 403) {
            logout();
            loginMessage.textContent = "Sessao expirada ou sem permissao.";
            return null;
        }

        const data = await response.json().catch(() => ({}));

        if (!response.ok) {
            showStatus(data.mensagem || "Operacao nao concluida.", true);
            return null;
        }

        return data;
    } catch {
        showStatus("Falha ao conectar com a API.", true);
        return null;
    }
}

function renderToolbar(name, innerHtml) {
    return `<section class="toolbar" aria-label="Filtros de ${name}">${innerHtml}</section>`;
}

function renderPanel(title, id) {
    return `
        <section class="section-panel">
            <div class="section-header">
                <h2 class="section-title">${escapeHtml(title)}</h2>
            </div>
            <div class="table-wrap" id="${id}"></div>
        </section>
    `;
}

function fillTable(targetId, headers, rows) {
    const target = document.querySelector(`#${targetId}`);
    if (!rows) {
        target.innerHTML = "<div class=\"empty-state\">Nenhum registro encontrado.</div>";
        return;
    }

    target.innerHTML = `
        <table>
            <thead>
                <tr>${headers.map((header) => `<th>${escapeHtml(header)}</th>`).join("")}</tr>
            </thead>
            <tbody>${rows}</tbody>
        </table>
    `;
}

function renderRoleSelect(id, currentRole) {
    return `
        <select data-role-select="${id}">
            ${roles.map((role) => `<option value="${role}" ${role === currentRole ? "selected" : ""}>${role}</option>`).join("")}
        </select>
    `;
}

function renderProdutoStatusSelect(id, currentStatus) {
    return `
        <select data-product-select="${id}">
            ${produtoStatus.map((status) => `<option value="${status}" ${status === currentStatus ? "selected" : ""}>${status}</option>`).join("")}
        </select>
    `;
}

function renderBadge(text, variant = "") {
    return `<span class="badge ${variant}">${escapeHtml(text)}</span>`;
}

function showStatus(message, isError = false) {
    statusBar.textContent = message;
    statusBar.style.borderLeftColor = isError ? "var(--danger)" : "var(--accent)";
    statusBar.classList.remove("hidden");
}

function hideStatus() {
    statusBar.textContent = "";
    statusBar.classList.add("hidden");
}

function badgeClass(status) {
    if (["Pago", "Entregue", "Repassado", "Publicado", "Aprovado"].includes(status)) return "ok";
    if (["Pendente", "AguardandoSplit", "AguardandoRepasse", "EmAnalise", "Pausado"].includes(status)) return "warn";
    if (["Cancelado", "Estornado", "Desativado", "Recusado"].includes(status)) return "danger";
    return "";
}

function money(value) {
    return Number(value || 0).toLocaleString("pt-BR", {
        style: "currency",
        currency: "BRL"
    });
}

function date(value) {
    if (!value) return "";
    return new Date(value).toLocaleDateString("pt-BR");
}

function encode(value) {
    return encodeURIComponent(value || "");
}

function readUser() {
    try {
        return JSON.parse(localStorage.getItem(storageKeys.user));
    } catch {
        return null;
    }
}

function escapeHtml(value) {
    return String(value ?? "")
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll("\"", "&quot;")
        .replaceAll("'", "&#039;");
}
