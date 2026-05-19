import { ArrowRight, BadgePercent } from "lucide-react";

// Define os dados minimos necessarios para o banner de destaque.
type BannerProps = {
  titulo: string;
  descricao: string;
  imagem: string;
};

// Exibe o destaque principal da Home.
// O layout foi preparado para receber campanhas reais da API no futuro.
export function Banner({ titulo, descricao, imagem }: BannerProps) {
  return (
    <section className="grid overflow-hidden rounded-[32px] border border-white/10 bg-[radial-gradient(circle_at_top_left,_rgba(250,204,21,0.18),_transparent_34%),linear-gradient(135deg,_rgba(255,255,255,0.05),_rgba(255,255,255,0.02))] shadow-[0_24px_80px_rgba(0,0,0,0.45)] lg:grid-cols-[1.1fr_0.9fr]">
      {/* Bloco textual com a mensagem promocional da Home. */}
      <div className="flex flex-col justify-between gap-8 p-6 sm:p-8 lg:p-10">
        <div className="space-y-4">
          <span className="inline-flex w-fit items-center gap-2 rounded-full border border-yellow-400/30 bg-yellow-400/10 px-4 py-1 text-sm font-medium text-yellow-300">
            <BadgePercent className="h-4 w-4" />
            Oferta em destaque
          </span>

          <div className="space-y-3">
            <h1 className="max-w-2xl text-3xl font-bold tracking-tight text-white sm:text-4xl lg:text-5xl">
              {titulo}
            </h1>
            <p className="max-w-xl text-sm leading-6 text-neutral-300 sm:text-base">
              {descricao}
            </p>
          </div>
        </div>

        {/* Acoes visuais para deixar o mock mais proximo de um marketplace real. */}
        <div className="flex flex-col gap-3 sm:flex-row">
          <button
            type="button"
            className="inline-flex items-center justify-center gap-2 rounded-full bg-yellow-400 px-5 py-3 text-sm font-semibold text-black transition hover:bg-yellow-300"
          >
            Comprar agora
            <ArrowRight className="h-4 w-4" />
          </button>

          <button
            type="button"
            className="rounded-full border border-white/10 bg-white/5 px-5 py-3 text-sm font-semibold text-white transition hover:border-yellow-400/40 hover:bg-white/10"
          >
            Ver detalhes
          </button>
        </div>
      </div>

      {/* Area visual do banner usando imagem mock para manter a pagina independente de backend. */}
      <div className="relative min-h-[280px] border-t border-white/10 bg-black/30 lg:min-h-full lg:border-l lg:border-t-0">
        <img
          src={imagem}
          alt="Produto em destaque no banner principal"
          className="h-full w-full object-cover"
        />
        <div className="pointer-events-none absolute inset-0 bg-gradient-to-t from-black via-black/10 to-transparent lg:bg-gradient-to-r lg:from-transparent lg:via-transparent lg:to-black/20" />
      </div>
    </section>
  );
}
