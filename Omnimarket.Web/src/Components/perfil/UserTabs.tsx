import type { PerfilTabId } from "../../types/perfil";

// Define o rotulo de cada aba em um lugar unico para facilitar manutencao.
const TABS: Array<{ id: PerfilTabId; label: string }> = [
  { id: "produtos", label: "Produtos" },
  { id: "vendas", label: "Vendas" },
  { id: "compras", label: "Compras" },
];

// Renderiza a navegacao entre os blocos de conteudo do perfil.
interface UserTabsProps {
  abaAtiva: PerfilTabId;
  onChange: (aba: PerfilTabId) => void;
}

export function UserTabs({ abaAtiva, onChange }: UserTabsProps) {
  return (
    <div className="flex flex-wrap gap-3 border-b border-white/10 pb-4">
      {TABS.map((tab) => {
        const isAtiva = tab.id === abaAtiva;

        return (
          <button
            key={tab.id}
            type="button"
            onClick={() => onChange(tab.id)}
            className={`rounded-full border px-4 py-2 text-sm font-medium transition ${
              isAtiva
                ? "border-yellow-400/40 bg-yellow-400/10 text-yellow-300"
                : "border-white/10 bg-black text-neutral-400 hover:border-white/20 hover:text-white"
            }`.trim()}
          >
            {tab.label}
          </button>
        );
      })}
    </div>
  );
}
