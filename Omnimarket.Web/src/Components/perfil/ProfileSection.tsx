import type { ReactNode } from "react";

// Encapsula o visual padrao dos cards escuros usados na pagina de perfil.
interface ProfileSectionProps {
  title?: string;
  description?: string;
  children: ReactNode;
  className?: string;
}

export function ProfileSection({
  title,
  description,
  children,
  className = "",
}: ProfileSectionProps) {
  return (
    <section
      className={`rounded-3xl border border-white/10 bg-[#111] p-5 shadow-[0_16px_48px_rgba(0,0,0,0.28)] sm:p-6 ${className}`.trim()}
    >
      {(title || description) && (
        <header className="mb-5 space-y-1">
          {title ? <h2 className="text-lg font-semibold text-white">{title}</h2> : null}
          {description ? (
            <p className="text-sm text-neutral-400">{description}</p>
          ) : null}
        </header>
      )}

      {children}
    </section>
  );
}
