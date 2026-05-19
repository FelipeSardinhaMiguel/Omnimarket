import { AlertCircle, Inbox } from "lucide-react";

// Exibe mensagens de erro ou estado vazio em um formato consistente.
interface ProfileFeedbackProps {
  title: string;
  description: string;
  variant: "empty" | "error";
}

export function ProfileFeedback({
  title,
  description,
  variant,
}: ProfileFeedbackProps) {
  const Icon = variant === "error" ? AlertCircle : Inbox;
  const accentClass =
    variant === "error"
      ? "border-red-500/20 bg-red-500/10 text-red-300"
      : "border-white/10 bg-black/45 text-neutral-300";

  return (
    <div className={`rounded-2xl border px-5 py-8 text-center ${accentClass}`.trim()}>
      <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full border border-current/20">
        <Icon size={20} />
      </div>
      <h3 className="mt-4 text-lg font-semibold text-white">{title}</h3>
      <p className="mt-2 text-sm text-neutral-400">{description}</p>
    </div>
  );
}
