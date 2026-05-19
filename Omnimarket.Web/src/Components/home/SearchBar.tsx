import { Search, X } from "lucide-react";

// Define os dados que a barra de busca precisa receber da pagina.
type SearchBarProps = {
  value: string;
  onChange: (value: string) => void;
};

// Renderiza o campo de busca principal da Home.
// Hoje ele filtra os mocks localmente e, no futuro, pode disparar consultas para a API.
export function SearchBar({ value, onChange }: SearchBarProps) {
  return (
    <div className="flex items-center gap-3 rounded-[28px] border border-white/10 bg-white/5 px-4 py-3 shadow-[0_16px_40px_rgba(0,0,0,0.35)] backdrop-blur-sm">
      {/* Icone fixo para reforcar o comportamento de busca. */}
      <Search className="h-5 w-5 shrink-0 text-neutral-400" />

      {/* Campo controlado pela HomePage para centralizar estado e futura integracao com API. */}
      <input
        type="text"
        value={value}
        onChange={(event) => onChange(event.target.value)}
        placeholder="Buscar produtos, marcas ou categorias"
        className="w-full bg-transparent text-sm text-white outline-none placeholder:text-neutral-500 sm:text-base"
      />

      {/* Botao simples para limpar o termo digitado e simular uma UX de marketplace real. */}
      {value ? (
        <button
          type="button"
          onClick={() => onChange("")}
          className="rounded-full border border-white/10 bg-white/5 p-2 text-neutral-300 transition hover:border-yellow-400/40 hover:text-yellow-300"
          aria-label="Limpar busca"
        >
          <X className="h-4 w-4" />
        </button>
      ) : null}
    </div>
  );
}
