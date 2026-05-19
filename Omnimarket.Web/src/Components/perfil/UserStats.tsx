import type { UsuarioStatsData } from "../../types/perfil";
import { ProfileSection } from "./ProfileSection";

// Define os indicadores exibidos no painel para manter a renderizacao enxuta.
const STATS_CONFIG = [
  {
    key: "avaliacaoMedia",
    label: "Avaliacao media",
    format: (value: number) => (value ? `${value.toFixed(1)} / 5` : "Sem nota"),
  },
  {
    key: "seguidores",
    label: "Seguidores",
    format: (value: number) => `${value}`,
  },
  {
    key: "totalProdutos",
    label: "Produtos",
    format: (value: number) => `${value}`,
  },
  {
    key: "totalVendas",
    label: "Vendas",
    format: (value: number) => `${value}`,
  },
  {
    key: "totalCompras",
    label: "Compras",
    format: (value: number) => `${value}`,
  },
] as const;

// Exibe os principais indicadores do usuario em um grid responsivo.
interface UserStatsProps {
  stats: UsuarioStatsData;
}

export function UserStats({ stats }: UserStatsProps) {
  return (
    <ProfileSection
      title="Minha atividade"
      description="Resumo rapido da conta para apoiar decisoes e futuras integracoes."
    >
      <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
        {STATS_CONFIG.map(({ key, label, format }) => (
          <div
            key={key}
            className="rounded-2xl border border-white/8 bg-black/45 px-4 py-4"
          >
            <p className="text-xs uppercase tracking-[0.22em] text-neutral-500">{label}</p>
            <p className="mt-3 text-xl font-semibold text-white">
              {format(stats[key])}
            </p>
          </div>
        ))}
      </div>
    </ProfileSection>
  );
}
