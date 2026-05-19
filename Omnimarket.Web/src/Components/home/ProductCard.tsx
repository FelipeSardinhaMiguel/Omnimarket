import { Star } from "lucide-react";
import type { HomeProduct } from "../../types/home";
import { Link } from "@tanstack/react-router";

// Define a estrutura esperada para um card individual de produto.
type ProductCardProps = {
  produto: HomeProduct;
};

// Renderiza um card de produto com foco em imagem, preco e avaliacao.
export function ProductCard({ produto }: ProductCardProps) {
  return (
    /*este Link serve para redirecionar o user por uma rota dinamica para uma tela especifica do produto clicado com base no id do produto. Depois envia esse id para o ProdutoPage*/
    <Link
      to="/produto/$id"
      params={{ id: produto.id }}
    >

    <article className="group overflow-hidden rounded-[28px] border border-white/10 bg-[linear-gradient(180deg,_rgba(255,255,255,0.05),_rgba(255,255,255,0.02))] shadow-[0_18px_50px_rgba(0,0,0,0.35)] transition hover:-translate-y-1 hover:border-yellow-400/30">
      {/* Area principal da imagem com selo opcional de destaque. */}
      <div className="relative aspect-[4/3] overflow-hidden bg-neutral-950">
        <img
          src={produto.imagem}
          alt={produto.nome}
          className="h-full w-full object-cover transition duration-500 group-hover:scale-105"
        />

        {produto.destaque ? (
          <span className="absolute left-4 top-4 rounded-full border border-yellow-400/30 bg-black/65 px-3 py-1 text-xs font-medium text-yellow-300 backdrop-blur-sm">
            {produto.destaque}
          </span>
        ) : null}
      </div>

      {/* Informacoes principais do produto. */}
      <div className="space-y-4 p-5">
        <div className="space-y-2">
          <h3 className="min-h-[3.5rem] text-base font-semibold leading-7 text-white">
            {produto.nome}
          </h3>

          <div className="flex items-center gap-2 text-sm text-neutral-400">
            <Star className="h-4 w-4 fill-yellow-400 text-yellow-400" />
            <span>{produto.avaliacao.toFixed(1)}</span>
          </div>
        </div>

        <p className="text-2xl font-bold tracking-tight text-yellow-400">
          {produto.preco.toLocaleString("pt-BR", {
            style: "currency",
            currency: "BRL",
          })}
        </p>
      </div>
    </article>
    </Link>
  );
}
