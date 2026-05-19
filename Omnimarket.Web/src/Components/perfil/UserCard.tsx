import type { UsuarioPerfil } from "../../types/perfil";
import { ProfileSection } from "./ProfileSection";
import { UserInfo } from "./UserInfo";

// Exibe o resumo principal do usuario com avatar, nome e contatos.
interface UserCardProps {
  usuario: UsuarioPerfil | null;
}

// Gera as iniciais para quando a foto real ainda nao estiver disponivel.
function obterIniciais(nome: string | undefined) {
  if (!nome) {
    return "U";
  }

  const partesDoNome = nome.trim().split(" ").filter(Boolean);
  const primeiraParte = partesDoNome[0]?.[0] ?? "";
  const ultimaParte = partesDoNome[1]?.[0] ?? "";

  return `${primeiraParte}${ultimaParte}`.toUpperCase();
}

export function UserCard({ usuario }: UserCardProps) {
  return (
    <ProfileSection className="h-full">
      <div className="flex flex-col gap-6">
        <div className="flex flex-col items-center gap-4 text-center">
          {usuario?.avatarUrl ? (
            <img
              src={usuario.avatarUrl}
              alt={`Avatar de ${usuario.nome}`}
              className="h-24 w-24 rounded-full border-4 border-yellow-400 object-cover"
            />
          ) : (
            <div className="flex h-24 w-24 items-center justify-center rounded-full border-4 border-yellow-400 bg-black text-2xl font-bold text-yellow-400">
              {obterIniciais(usuario?.nome)}
            </div>
          )}

          <div className="space-y-2">
            <p className="text-xs uppercase tracking-[0.32em] text-yellow-400">
              Perfil do usuario
            </p>
            <h1 className="text-2xl font-semibold text-white">
              {usuario?.nome || "Usuario sem nome cadastrado"}
            </h1>
            <p className="text-sm text-neutral-400">
              {usuario?.resumo || "Area pronta para bio, cargo ou descricao curta do usuario."}
            </p>
          </div>

          <div className="inline-flex rounded-full border border-yellow-400/30 bg-yellow-400/10 px-4 py-1 text-xs font-medium uppercase tracking-[0.2em] text-yellow-300">
            {usuario?.contaVerificada ? "Conta verificada" : "Conta em configuracao"}
          </div>
        </div>

        <div className="border-t border-white/10" />

        <UserInfo usuario={usuario} />

        <div className="rounded-2xl border border-dashed border-yellow-400/25 bg-yellow-400/5 px-4 py-3">
          <p className="text-sm font-medium text-white">Edicao futura</p>
          <p className="mt-1 text-sm text-neutral-400">
            Este bloco ja deixa espaco reservado para a acao de editar perfil sem precisar refatorar a pagina.
          </p>
        </div>
      </div>
    </ProfileSection>
  );
}
