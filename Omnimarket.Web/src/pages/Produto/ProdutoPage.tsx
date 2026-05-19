import { useParams } from "@tanstack/react-router";
import { ShoppingCart } from "lucide-react";
import { Botao } from "../../Components/Botao";
import { PageLayout } from "../../Components/PageLayout";
import { produtosMock } from "../../data/produtos";
import { useNavigate } from "@tanstack/react-router";

//import para tornar a parte de quando add um produto ao carrinho
//apareça a mensagem "produto adicionado ao carrinho" de forma mais profissional
import toast from "react-hot-toast";
import { useCart } from "../../context/CartContext";



// Esta pagina exibe os detalhes de um produto especifico.
// A rota recebe um "id" dinamico e, com base nele, procuramos o produto correspondente no mock.
export function ProdutoPage() {
  // O useParams le os parametros da URL.
  // O "strict: false" permite acessar o parametro mesmo sem exigir validacao rigida neste ponto.
  const { id } = useParams({ strict: false });

  // Procuramos no array mockado o produto cujo id seja igual ao id vindo da rota.
  // Hoje os dados sao locais, mas essa mesma ideia sera mantida quando a API existir.
  const produto = produtosMock.find((p) => p.id === id);

  //Propriedade do tanstack para levar a pagina de sucesso e simular uma compra feita para 
  //podermos visualizar o desenvolvimento da pagina de sucesso
  const navigate = useNavigate();

  const { adicionarAoCarrinho } = useCart();
  
  // TODO: integrar com API futuramente para buscar dados reais do produto

  // Se nenhum produto for encontrado, exibimos uma mensagem visualmente mais trabalhada.
  // Isso melhora a experiencia do usuario e evita deixar a tela "seca" ou desorganizada.
  if (!produto) {
    return (
      <PageLayout>
        {/* Este container ocupa a altura toda da tela e centraliza o bloco de feedback. */}
        <div className="flex min-h-screen items-center justify-center px-4 py-10 sm:px-6 lg:px-8">
          {/* Este card serve como caixa de destaque para a mensagem de erro.
              "max-w-xl" limita a largura maxima para o texto nao ficar espalhado demais.
              "rounded-[28px]" cria cantos bem arredondados para combinar com o visual moderno do projeto.
              "border border-yellow-400/20" adiciona uma borda sutil dourada, respeitando a identidade visual.
              "bg-white/5" cria um fundo transparente suave sobre o preto.
              "shadow-[...]" adiciona profundidade sem exagero. */}
          <section className="w-full max-w-xl rounded-[28px] border border-yellow-400/20 bg-white/5 p-8 text-center shadow-[0_24px_80px_rgba(0,0,0,0.45)] backdrop-blur-sm">
            {/* Este pequeno selo ajuda a criar hierarquia visual logo no topo do estado de fallback. */}
            <span className="inline-flex rounded-full border border-yellow-400/30 bg-yellow-400/10 px-4 py-1 text-sm font-medium text-yellow-300">
              Produto indisponivel
            </span>

            {/* Usamos h1 aqui porque esta passa a ser a informacao principal desta tela especifica. */}
            <h1 className="mt-4 text-[clamp(1.75rem,4vw,2.5rem)] font-bold leading-tight text-white">
              Produto nao encontrado
            </h1>

            {/* O texto complementar explica o motivo de forma mais amigavel e clara. */}
            <p className="mt-3 text-sm leading-6 text-neutral-300 sm:text-base">
              O item solicitado nao foi localizado. Verifique se o link esta correto ou
              retorne para a listagem para selecionar outro produto.
            </p>
          </section>
        </div>
      </PageLayout>
    );
  }

  return (
    <PageLayout>
      {/* Este container principal cria um respiro nas bordas da tela.
          "px" controla o espacamento lateral.
          "py" cria espacamento vertical para a pagina nao ficar colada no topo.
          "text-white" define a cor padrao dos textos desta area. */}
      <div className="min-h-screen px-4 py-8 text-white sm:px-6 lg:px-8 lg:py-10">
        {/* Esta div centraliza o conteudo e limita a largura maxima da pagina.
            "max-w-7xl" evita que o layout fique exageradamente largo em monitores grandes.
            "mx-auto" centraliza horizontalmente.
            "flex flex-col" permite empilhar blocos internos com facilidade. */}
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-8">
          {/* Este bloco funciona como o "card principal" da pagina do produto.
              "overflow-hidden" impede que conteudos internos escapem dos cantos arredondados.
              "border" e "bg-[linear-gradient(...)]" criam um acabamento mais premium.
              "shadow-[...]" adiciona profundidade para destacar a area principal da tela. */}
          <section className="overflow-hidden rounded-[32px] border border-white/10 bg-[linear-gradient(180deg,_rgba(255,255,255,0.05),_rgba(255,255,255,0.02))] shadow-[0_24px_80px_rgba(0,0,0,0.45)]">
            {/* Aqui mantemos a estrutura em duas colunas.
                "grid" organiza melhor os dois lados da pagina.
                "lg:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)]" cria duas colunas em telas grandes:
                a primeira para a imagem e a segunda para as informacoes.
                Em telas menores, o grid fica com apenas uma coluna automaticamente. */}
            <div className="grid lg:grid-cols-[minmax(0,1fr)_minmax(0,1.1fr)]">
              {/* COLUNA ESQUERDA:
                  Esta area existe exclusivamente para exibir a imagem do produto com destaque. */}
              <div className="overflow-hidden border-b border-white/10 bg-[radial-gradient(circle_at_top,_rgba(250,204,21,0.12),_transparent_45%),linear-gradient(180deg,_rgba(255,255,255,0.03),_rgba(255,255,255,0.01))] lg:min-h-[36rem] lg:border-b-0 lg:border-r">
                {/* A imagem agora ocupa toda a area visual da coluna esquerda.
                    "block" evita espaco inline indesejado.
                    "h-full w-full" faz a imagem preencher a div.
                    "object-contain" garante que a imagem apareca inteira sem cortes. */}
                <img
                  src={produto.imagem}
                  alt={produto.nome}
                  className="block h-full w-full object-contain rounded-2xl"
                />
              </div>

              {/* COLUNA DIREITA:
                  Esta area agrupa as informacoes do produto, preco, acoes e textos auxiliares. */}
              <div className="flex flex-col justify-between p-6 sm:p-8 lg:p-10">
                {/* Este bloco superior concentra a hierarquia principal da pagina:
                    identificacao do produto, nome e preco. */}
                <div className="space-y-6">
                  {/* Esta div organiza pequenos textos introdutorios acima do nome.
                      "space-y-3" cria distancia vertical consistente entre os elementos. */}
                  <div className="space-y-3">
                    {/* Este selo pequeno ajuda a contextualizar a secao sem competir com o titulo.
                        "inline-flex" faz o elemento se ajustar ao proprio conteudo.
                        "rounded-full" deixa o selo em formato de capsula. */}
                    <span className="inline-flex w-fit rounded-full border border-yellow-400/30 bg-yellow-400/10 px-4 py-1 text-sm font-medium text-yellow-300">
                      Detalhes do produto
                    </span>

                    {/* Este bloco agrupa o titulo e o texto secundario logo abaixo. */}
                    <div className="space-y-3">
                      {/* Usamos h1 apenas para o nome principal do produto.
                          "text-[clamp( tamanho min, tamanho que se adapta ao tamanho da tela, tamanho max )]" cria uma tipografia responsiva:
                          Isso melhora a leitura em celular, notebook e monitor grande.
                          "break-words" evita que nomes longos estourem o layout.
                          "leading-[1.05]" reduz o espacamento entre linhas do titulo para ficar mais elegante. */}
                      <h1 className="w-full break-words text-left text-[clamp(2rem,4vw,4rem)] font-bold leading-[1.05] tracking-tight text-white">
                        {produto.nome}
                      </h1>

                      {/* Este paragrafo mostra o id apenas como informacao secundaria.
                          Em vez de outro h1, usamos p para respeitar a semantica correta. */}
                      <p className="text-sm text-neutral-400 sm:text-base">
                        Codigo do produto: <span className="font-medium text-neutral-200">{id}</span>
                      </p>
                    </div>
                  </div>

                  {/* Esta secao destaca o preco.
                      "rounded-2xl" cria um bloco visual separado.
                      "border border-yellow-400/20" reforca a cor de destaque sem exagerar.
                      "bg-yellow-400/5" adiciona uma camada dourada bem sutil ao fundo. */}
                  <div className="rounded-2xl border border-yellow-400/20 bg-yellow-400/5 p-5 sm:p-6">
                    <p className="text-sm uppercase tracking-[0.24em] text-yellow-200/80">
                      Preco
                    </p>

                    {/* O preco continua dourado e em negrito, como solicitado.
                        "text-[clamp(...)]" deixa o tamanho equilibrado e responsivo. */}
                    <p className="mt-2 text-[clamp(2rem,4vw,3.5rem)] font-bold leading-none text-yellow-400">
                      {produto.preco.toLocaleString("pt-BR", {
                        style: "currency",
                        currency: "BRL",
                      })}
                    </p>
                  </div>

                  {/* Esta div contem os dois botoes de acao.
                      "flex-col" no mobile empilha os botoes para caber melhor em telas pequenas.
                      "sm:flex-row" coloca lado a lado a partir de telas um pouco maiores.
                      "gap-4" cria uma distancia consistente entre eles. */}
                  <div className="flex flex-col gap-4 sm:flex-row">
                    {/* Este wrapper controla o espaco do primeiro botao.
                        "flex-1" faz os botoes dividirem o espaco disponivel de forma equilibrada. */}
                    <div className="flex-1">
                      <Botao className="h-14 w-full text-base font-semibold sm:text-lg" 
                        onClick=
                        {() => navigate
                          ({
                            to: "/paginaPagamento",
                            state: {
                              produto: produto.nome,
                              preco: produto.preco,
                              id: produto.id,
                            },
                          })
                        }>
                        Comprar
                      </Botao>
                    </div>

                    {/* Este wrapper faz o mesmo papel no segundo botao, mantendo simetria visual. */}
                    <div className="flex-1">
                      <Botao
                        icon={<ShoppingCart className="h-5 w-5" />}
                        variant="secondary"
                        className="h-14 w-full text-base font-semibold sm:text-lg"
                        onClick={() => 
                                      {
                                        adicionarAoCarrinho(produto)
                                        console.log("Produto adicionado")
                                        alert("Produto adicionado ao carrinho com sucesso!!") 
                                      }
                                }
                      >
                        Adicionar ao carrinho
                      </Botao>
                      
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </section>
        </div>
      </div>
    </PageLayout>
  );
}
