import { Mail, MapPin, Phone } from "lucide-react";

import type { UsuarioPerfil } from "../../types/perfil";

// Renderiza as informacoes textuais do usuario em linhas reutilizaveis.
interface UserInfoProps {
  usuario: UsuarioPerfil | null;
}

// Mantem a definicao visual dos campos em um array para evitar repeticao.
const INFO_ITEMS = [
  {
    key: "email",
    label: "Email",
    icon: Mail,
  },
  {
    key: "telefone",
    label: "Telefone",
    icon: Phone,
  },
  {
    key: "endereco",
    label: "Endereco",
    icon: MapPin,
  },
] as const;

export function UserInfo({ usuario }: UserInfoProps) {
  return (
    <div className="space-y-3">
      {INFO_ITEMS.map(({ key, label, icon: Icon }) => (
        <div
          key={key}
          className="flex items-start gap-3 rounded-2xl border border-white/8 bg-black/40 px-4 py-3"
        >
          <span className="mt-0.5 rounded-full border border-yellow-400/25 bg-yellow-400/10 p-2 text-yellow-400">
            <Icon size={16} />
          </span>

          <div className="min-w-0">
            <p className="text-xs uppercase tracking-[0.24em] text-neutral-500">{label}</p>
            <p className="truncate text-sm text-neutral-100">
              {usuario?.[key] || "Informacao indisponivel"}
            </p>
          </div>
        </div>
      ))}
    </div>
  );
}
