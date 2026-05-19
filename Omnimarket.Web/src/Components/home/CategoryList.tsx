import {
  Gamepad2,
  Headphones,
  Shirt,
  Smartphone,
  Sofa,
  Watch,
} from "lucide-react";
import type { HomeCategory } from "../../types/home";

// Define os dados de controle da lista horizontal de categorias.
type CategoryListProps = {
  categorias: HomeCategory[];
  categoriaAtiva: string;
  onSelect: (categoriaId: string) => void;
};

// Mapeia o nome do icone informado nos mocks para o componente visual correspondente.
const iconMap = {
  smartphone: Smartphone,
  shirt: Shirt,
  gamepad: Gamepad2,
  sofa: Sofa,
  headphones: Headphones,
  watch: Watch,
};

// Renderiza a faixa horizontal de categorias, preparada para scroll em telas menores.
export function CategoryList({
  categorias,
  categoriaAtiva,
  onSelect,
}: CategoryListProps) {
  return (
    <section className="space-y-4">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h2 className="text-xl font-semibold text-white sm:text-2xl">Categorias</h2>
          <p className="text-sm text-neutral-400">
            Navegue por colecoes como em um marketplace real.
          </p>
        </div>
      </div>

      {/* O container horizontal usa overflow para manter a UX fluida no mobile. */}
      <div className="flex gap-3 overflow-x-auto pb-2 [scrollbar-width:none] [&::-webkit-scrollbar]:hidden">
        {categorias.map((categoria) => {
          const Icone = iconMap[categoria.icone];
          const isAtiva = categoriaAtiva === categoria.id;

          return (
            <button
              key={categoria.id}
              type="button"
              onClick={() => onSelect(categoria.id)}
              className={`flex min-w-[140px] items-center gap-3 rounded-[24px] border px-4 py-4 text-left transition ${
                isAtiva
                  ? "border-yellow-400/50 bg-yellow-400/10 text-yellow-300"
                  : "border-white/10 bg-white/5 text-white hover:border-white/20 hover:bg-white/10"
              }`}
            >
              <span
                className={`flex h-11 w-11 items-center justify-center rounded-2xl ${
                  isAtiva ? "bg-yellow-400/15" : "bg-black/40"
                }`}
              >
                <Icone className="h-5 w-5" />
              </span>

              <span className="text-sm font-medium">{categoria.nome}</span>
            </button>
          );
        })}
      </div>
    </section>
  );
}
