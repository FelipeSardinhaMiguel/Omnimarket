import { Minus, Plus, Trash2 } from "lucide-react";
import type { CartItem as CartItemType } from "../../store/useCartStore";

type CartItemProps = {
  item: CartItemType;
  onIncrease: (id: string) => void;
  onDecrease: (id: string) => void;
  onRemove: (id: string) => void;
};

const currencyFormatter = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL",
});

export function CartItem({
  item,
  onIncrease,
  onDecrease,
  onRemove,
}: CartItemProps) {
  return (
    <article className="group rounded-[28px] border border-white/10 bg-[linear-gradient(180deg,_rgba(255,255,255,0.05),_rgba(255,255,255,0.02))] p-4 shadow-[0_20px_45px_rgba(0,0,0,0.35)] transition duration-300 hover:-translate-y-0.5 hover:border-yellow-400/20 hover:bg-white/[0.07] sm:p-5">
      <div className="flex items-start gap-4">
        <div className="h-24 w-24 shrink-0 overflow-hidden rounded-2xl border border-white/10 bg-neutral-950 sm:h-28 sm:w-28">
          {item.imagem ? (
            <img
              src={item.imagem}
              alt={item.nome}
              className="h-full w-full object-cover transition duration-500 group-hover:scale-105"
            />
          ) : (
            <div className="flex h-full w-full items-center justify-center bg-[radial-gradient(circle_at_top,_rgba(250,204,21,0.2),_transparent_60%)] text-center text-xs font-medium uppercase tracking-[0.2em] text-neutral-500">
              Sem imagem
            </div>
          )}
        </div>

        <div className="flex min-w-0 flex-1 flex-col gap-4">
          <div className="flex items-start justify-between gap-4">
            <div className="min-w-0 space-y-2">
              <h2 className="text-base font-semibold leading-6 text-white sm:text-lg">
                {item.nome}
              </h2>
              <p className="text-sm leading-6 text-neutral-400">
                {item.descricao ?? "Detalhes do produto indisponiveis no momento."}
              </p>
              <p className="text-lg font-bold tracking-tight text-yellow-400">
                {currencyFormatter.format(item.preco)}
              </p>
            </div>

            <button
              type="button"
              onClick={() => onRemove(item.id)}
              className="inline-flex h-10 w-10 shrink-0 items-center justify-center rounded-full border border-white/10 bg-white/5 text-neutral-400 transition hover:border-red-400/30 hover:bg-red-500/10 hover:text-red-300"
              aria-label={`Remover ${item.nome} do carrinho`}
            >
              <Trash2 className="h-4 w-4" />
            </button>
          </div>

          <div className="flex items-center justify-between gap-4">
            <div className="inline-flex items-center rounded-full border border-white/10 bg-black/40 p-1 shadow-inner shadow-black/40">
              <button
                type="button"
                onClick={() => onDecrease(item.id)}
                className="inline-flex h-10 w-10 items-center justify-center rounded-full text-neutral-200 transition hover:bg-white/10 hover:text-white"
                aria-label={`Diminuir quantidade de ${item.nome}`}
              >
                <Minus className="h-4 w-4" />
              </button>

              <span className="min-w-10 px-2 text-center text-sm font-semibold text-white">
                {item.quantidade}
              </span>

              <button
                type="button"
                onClick={() => onIncrease(item.id)}
                className="inline-flex h-10 w-10 items-center justify-center rounded-full bg-yellow-400 text-black transition hover:bg-yellow-300"
                aria-label={`Aumentar quantidade de ${item.nome}`}
              >
                <Plus className="h-4 w-4" />
              </button>
            </div>

            <p className="text-sm text-neutral-400">
              {currencyFormatter.format(item.preco * item.quantidade)}
            </p>
          </div>
        </div>
      </div>
    </article>
  );
}
