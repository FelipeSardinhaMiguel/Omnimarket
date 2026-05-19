import { LoaderCircle, PackageSearch } from "lucide-react";
import { ProductCard } from "./ProductCard";
import type { HomeProduct } from "../../types/home";

// Define os estados visuais que a grade precisa controlar.
type ProductGridProps = {
  produtos: HomeProduct[];
  isLoading: boolean;
  termoBusca: string;
  categoriaAtivaNome: string;
  visibleCount: number;
};

// Renderiza a grade de produtos com suporte a loading, vazio e estrutura para paginacao futura.
export function ProductGrid({
  produtos,
  isLoading,
  termoBusca,
  categoriaAtivaNome,
  visibleCount,
}: ProductGridProps) {
  // Calcula quantos itens estao visiveis no mock atual.
  // Esse numero sera util quando a API trouxer paginação ou infinite scroll.
  const quantidadeVisivel = Math.min(produtos.length, visibleCount);

  if (isLoading) {
    return (
      <section className="space-y-4">
        <div className="flex items-center gap-3 text-sm text-neutral-400">
          <LoaderCircle className="h-4 w-4 animate-spin text-yellow-400" />
          <span>Carregando produtos simulados...</span>
        </div>

        {/* Skeleton simples para comunicar carregamento antes dos dados mockados aparecerem. */}
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
          {Array.from({ length: 6 }).map((_, index) => (
            <div
              key={`skeleton-${index}`}
              className="overflow-hidden rounded-[28px] border border-white/10 bg-white/[0.03]"
            >
              <div className="aspect-[4/3] animate-pulse bg-white/10" />
              <div className="space-y-3 p-5">
                <div className="h-4 w-3/4 animate-pulse rounded-full bg-white/10" />
                <div className="h-4 w-1/2 animate-pulse rounded-full bg-white/10" />
                <div className="h-6 w-1/3 animate-pulse rounded-full bg-yellow-400/20" />
              </div>
            </div>
          ))}
        </div>
      </section>
    );
  }

  if (produtos.length === 0) {
    return (
      <section className="rounded-[28px] border border-dashed border-white/10 bg-white/[0.03] p-8 text-center">
        <div className="mx-auto flex max-w-md flex-col items-center gap-4">
          <span className="flex h-14 w-14 items-center justify-center rounded-full bg-yellow-400/10 text-yellow-300">
            <PackageSearch className="h-6 w-6" />
          </span>

          <div className="space-y-2">
            <h3 className="text-xl font-semibold text-white">Nenhum produto encontrado</h3>
            <p className="text-sm leading-6 text-neutral-400">
              Nenhum item corresponde aos filtros atuais para{" "}
              <span className="font-medium text-white">{categoriaAtivaNome}</span>
              {termoBusca ? ` com a busca "${termoBusca}"` : ""}.
            </p>
          </div>
        </div>
      </section>
    );
  }

  return (
    <section className="space-y-5">
      {/* Resumo visual da listagem para aproximar o mock do comportamento de um ecommerce. */}
      <div className="flex flex-col gap-2 text-sm text-neutral-400 sm:flex-row sm:items-center sm:justify-between">
        <p>
          Exibindo <span className="font-semibold text-white">{quantidadeVisivel}</span> de{" "}
          <span className="font-semibold text-white">{produtos.length}</span> produtos
        </p>

        {/* Aviso estrutural para o proximo passo de evolucao com API e infinite scroll. */}
        <p className="rounded-full border border-white/10 bg-white/5 px-3 py-1 text-xs uppercase tracking-[0.24em] text-neutral-500">
          Estrutura pronta para infinite scroll
        </p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
        {produtos.slice(0, visibleCount).map((produto) => (
          <ProductCard key={produto.id} produto={produto} />
        ))}
      </div>
    </section>
  );
}
