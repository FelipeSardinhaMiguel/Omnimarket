using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Admin;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = RolesSistema.Admin)]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminDashboardService _adminDashboardService;
        private readonly AdminUsuarioService _adminUsuarioService;
        private readonly AdminLojaService _adminLojaService;
        private readonly AdminProdutoService _adminProdutoService;
        private readonly AdminPedidoService _adminPedidoService;
        private readonly AdminVendaService _adminVendaService;
        private readonly PedidoService _pedidoService;

        public AdminController(
            AdminDashboardService adminDashboardService,
            AdminUsuarioService adminUsuarioService,
            AdminLojaService adminLojaService,
            AdminProdutoService adminProdutoService,
            AdminPedidoService adminPedidoService,
            AdminVendaService adminVendaService,
            PedidoService pedidoService)
        {
            _adminDashboardService = adminDashboardService;
            _adminUsuarioService = adminUsuarioService;
            _adminLojaService = adminLojaService;
            _adminProdutoService = adminProdutoService;
            _adminPedidoService = adminPedidoService;
            _adminVendaService = adminVendaService;
            _pedidoService = pedidoService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> ObterDashboard()
            => Ok(await _adminDashboardService.ObterDashboardAsync());

        [HttpGet("usuarios")]
        public async Task<IActionResult> ListarUsuarios(
            [FromQuery] string? busca,
            [FromQuery] string? role,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var usuarios = await _adminUsuarioService.ListarUsuariosAsync(busca, role, page, pageSize);
            return Ok(usuarios);
        }

        [HttpPut("usuarios/{usuarioId:int}/role")]
        public async Task<IActionResult> AtualizarRoleUsuario(
            int usuarioId,
            [FromBody] AdminAtualizarRoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var adminId = User.GetUserId();
                var usuario = await _adminUsuarioService.AtualizarRoleUsuarioAsync(usuarioId, dto, adminId);

                if (usuario == null)
                    return NotFound(new { mensagem = "Usuario nao encontrado." });

                return Ok(usuario);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("lojas")]
        public async Task<IActionResult> ListarLojas(
            [FromQuery] string? busca,
            [FromQuery] bool? ativa,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var lojas = await _adminLojaService.ListarLojasAsync(busca, ativa, page, pageSize);
            return Ok(lojas);
        }

        [HttpPut("lojas/{lojaId:int}/status")]
        public async Task<IActionResult> AtualizarStatusLoja(
            int lojaId,
            [FromBody] AdminAtualizarStatusLojaDto dto)
        {
            var loja = await _adminLojaService.AtualizarStatusLojaAsync(lojaId, dto);

            if (loja == null)
                return NotFound(new { mensagem = "Loja nao encontrada." });

            return Ok(loja);
        }

        [HttpGet("produtos")]
        public async Task<IActionResult> ListarProdutos(
            [FromQuery] string? busca,
            [FromQuery] StatusProduto? status,
            [FromQuery] int? lojaId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var produtos = await _adminProdutoService.ListarProdutosAsync(busca, status, lojaId, page, pageSize);
            return Ok(produtos);
        }

        [HttpPut("produtos/{produtoId:int}/status")]
        public async Task<IActionResult> AtualizarStatusProduto(
            int produtoId,
            [FromBody] AdminAtualizarStatusProdutoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var produto = await _adminProdutoService.AtualizarStatusProdutoAsync(produtoId, dto);

                if (produto == null)
                    return NotFound(new { mensagem = "Produto nao encontrado." });

                return Ok(produto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("pedidos")]
        public async Task<IActionResult> ListarPedidos(
            [FromQuery] string? busca,
            [FromQuery] StatusPedido? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var pedidos = await _adminPedidoService.ListarPedidosAsync(busca, status, page, pageSize);
            return Ok(pedidos);
        }

        [HttpPut("pedidos/{pedidoId:int}/enviar")]
        public async Task<IActionResult> MarcarPedidoComoEnviado(int pedidoId)
        {
            try
            {
                var pedido = await _pedidoService.MarcarPedidoComoEnviadoAsync(pedidoId);

                if (pedido == null)
                    return NotFound(new { mensagem = "Pedido nao encontrado." });

                return Ok(new
                {
                    mensagem = "Pedido marcado como enviado com sucesso!",
                    pedidoId = pedido.Id,
                    status = pedido.StatusPedidosId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("financeiro/vendas")]
        public async Task<IActionResult> ListarVendasFinanceiro(
            [FromQuery] StatusVenda? statusVenda,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var vendas = await _adminVendaService.ListarVendasAsync(statusVenda, page, pageSize);
            return Ok(vendas);
        }
    }
}
