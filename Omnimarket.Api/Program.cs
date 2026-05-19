using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Omnimarket.Api.Data;
using Omnimarket.Api.Services;
using Omnimarket.Api.Services.Interfaces;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Permite sobreposicao local de segredos sem versionar no repositorio.
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("ConexaoLocal");
if (string.IsNullOrWhiteSpace(connectionString) ||
    connectionString.Contains("SEU_SERVIDOR", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "Configure ConnectionStrings:ConexaoLocal em appsettings.Local.json, User Secrets ou variavel de ambiente.");
}

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) ||
    jwtKey.Contains("DEFINA_", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        "Configure Jwt:Key em appsettings.Local.json, User Secrets ou variavel de ambiente.");
}

// Chave usada para assinar e validar os tokens JWT da aplicacao.
var key = Encoding.UTF8.GetBytes(jwtKey);

// Evita problemas de permissao em ambiente local mantendo as chaves de DataProtection no proprio projeto.
var dataProtectionPath = Path.Combine(builder.Environment.ContentRootPath, ".dataprotection-keys");
Directory.CreateDirectory(dataProtectionPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath));

// Registra o contexto do Entity Framework apontando para o banco SQL Server.
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Configura a autenticacao da API para usar JWT em todos os endpoints protegidos.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Servicos de negocio que serao injetados nos controllers.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<UsuarioPerfilService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<RegistrarService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<AvaliacaoProdutoService>();
builder.Services.AddScoped<LojaService>();
builder.Services.AddScoped<LojaEntregaService>();
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddScoped<ProdutoMidiaService>();
builder.Services.AddScoped<EnderecoService>();
builder.Services.AddScoped<TelefoneService>();
builder.Services.AddScoped<IGatewayPagamentoService, GatewayPagamentoFakeService>();
builder.Services.AddScoped<FinanceiroService>();
builder.Services.AddScoped<ReciboPedidoService>();
builder.Services.AddScoped<AdminDashboardService>();
builder.Services.AddScoped<AdminUsuarioService>();
builder.Services.AddScoped<AdminLojaService>();
builder.Services.AddScoped<AdminProdutoService>();
builder.Services.AddScoped<AdminPedidoService>();
builder.Services.AddScoped<AdminVendaService>();

// Permite que o front-end local acesse a API durante o desenvolvimento.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendLocal", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configura os controllers e serializa enums como texto no JSON.
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger facilita a exploracao e o teste dos endpoints em desenvolvimento.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await AdminSeedService.AplicarAsync(app.Services);

// Pipeline HTTP da aplicacao.
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("FrontendLocal");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapeia as rotas declaradas nos controllers.
app.MapControllers();

app.Run();
