import { Spotlight } from "../../Components/home/SpotLight";
import { PageLayout } from "../../Components/PageLayout";
import { ProductGrid } from "../../Components/perfil/ProductGrid";
import { ProfileFeedback } from "../../Components/perfil/ProfileFeedback";
import { ProfileSection } from "../../Components/perfil/ProfileSection";
import { ProfileSkeleton } from "../../Components/perfil/ProfileSkeleton";
import { UserCard } from "../../Components/perfil/UserCard";
import { UserStats } from "../../Components/perfil/UserStats";
import { UserTabs } from "../../Components/perfil/UserTabs";
import { usePerfilUsuarioData } from "../../hooks/usePerfilUsuarioData";

export function PerfilUsuarioPage() {
  // Consome toda a logica do perfil em um hook separado da interface.
  const {
    usuario,
    stats,
    abaAtiva,
    tabContent,
    isUsuarioLoading,
    isConteudoLoading,
    usuarioError,
    conteudoError,
    setAbaAtiva,
  } = usePerfilUsuarioData();

  return (
    <PageLayout>
      <div className="min-h-screen bg-black px-4 py-6 text-white sm:px-6 lg:px-8">
        <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
          {/* Apresenta o cabecalho principal da pagina com a mesma linguagem visual escura do projeto. */}
          <Spotlight>
            <section className="relative overflow-hidden rounded-[32px] border border-white/10 bg-[radial-gradient(circle_at_top,_rgba(250,204,21,0.14),_transparent_42%),linear-gradient(180deg,_rgba(255,255,255,0.04),_rgba(255,255,255,0.01))] p-6 shadow-[0_24px_80px_rgba(0,0,0,0.45)] sm:p-8">
              <div className="space-y-3">
                <span className="inline-flex rounded-full border border-yellow-400/30 bg-yellow-400/10 px-4 py-1 text-sm font-medium text-yellow-300">
                  Central do perfil
                </span>
                <h1 className="text-3xl font-bold tracking-tight text-white sm:text-4xl">
                  Meu perfil
                </h1>
                <p className="max-w-2xl text-sm leading-6 text-neutral-400 sm:text-base">
                  Pagina preparada para receber dados reais da API, com componentes reutilizaveis,
                  abas dinamicas e estados visuais de carregamento, vazio e erro.
                </p>
              </div>
            </section>
          </Spotlight>

          <div className="grid gap-6 xl:grid-cols-[360px_minmax(0,1fr)]">
            <div className="space-y-6">
              {/* Decide se a coluna esquerda mostra loading, erro ou os dados do usuario. */}
              {isUsuarioLoading ? (
                <ProfileSection>
                  <ProfileSkeleton lines={4} cardCount={1} />
                </ProfileSection>
              ) : usuarioError ? (
                <ProfileSection>
                  <ProfileFeedback
                    variant="error"
                    title="Erro ao carregar perfil"
                    description={usuarioError}
                  />
                </ProfileSection>
              ) : (
                <UserCard usuario={usuario} />
              )}
            </div>

            <div className="space-y-6">
              {/* Exibe o resumo numerico da conta mesmo quando o usuario ainda nao possui dados completos. */}
              <UserStats stats={stats} />

              <ProfileSection title={tabContent.titulo} description={tabContent.descricao}>
                {/* Controla a troca de abas e o recarregamento dinamico do conteudo. */}
                <div className="space-y-5">
                  <UserTabs abaAtiva={abaAtiva} onChange={setAbaAtiva} />

                  {/* Renderiza feedback visual adequado para cada estado da listagem. */}
                  {isConteudoLoading ? (
                    <ProfileSkeleton />
                  ) : conteudoError ? (
                    <ProfileFeedback
                      variant="error"
                      title="Erro ao carregar conteudo"
                      description={conteudoError}
                    />
                  ) : tabContent.itens.length === 0 ? (
                    <ProfileFeedback
                      variant="empty"
                      title={tabContent.vazioTitulo}
                      description={tabContent.vazioDescricao}
                    />
                  ) : (
                    <ProductGrid itens={tabContent.itens} />
                  )}
                </div>
              </ProfileSection>
            </div>
          </div>
        </div>
      </div>
    </PageLayout>
  );
}
