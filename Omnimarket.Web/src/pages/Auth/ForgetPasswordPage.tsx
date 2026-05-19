import { useState, type ChangeEvent, type FormEvent } from "react";
import { Botao } from "../../Components/Botao";
import { Input } from "../../Components/Input";
import { PageLayout } from "../../Components/PageLayout";

import { RefreshCw, KeyRound  } from "lucide-react";

export function ForgetPasswordPage() {
  const [formData, setFormData] = useState({
    senha: "",
    confirmarSenha: "",
  });

  const [errors, setErrors] = useState({
    senha: "",
    confirmarSenha: "",
  });

  const [isLoading, setIsLoading] = useState(false);

  function handleInputChange(event: ChangeEvent<HTMLInputElement>) {
    const { name, value } = event.target;

    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));

    setErrors((prev) => ({
      ...prev,
      [name]: "",
    }));
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (isLoading) return;

    const newErrors = {
      senha: "",
      confirmarSenha: "",
    };

    if (formData.senha.length < 6) {
      newErrors.senha = "A senha deve ter no mínimo 6 caracteres";
    }

    if (formData.senha !== formData.confirmarSenha) {
      newErrors.confirmarSenha = "As senhas não coincidem";
    }

    if (newErrors.senha || newErrors.confirmarSenha) {
      setErrors(newErrors);
      return;
    }

    setIsLoading(true);

    await new Promise((resolve) => setTimeout(resolve, 1500));

    setIsLoading(false);

    alert("Senha alterada com sucesso!");
  }

  return (
    <PageLayout>
      <div className="flex min-h-[calc(100vh-106px)] w-full items-center justify-center px-4 py-8 sm:px-6 bg-[radial-gradient(circle_at_top,_rgba(250,204,21,0.18),_transparent_45%),linear-gradient(180deg,_rgba(255,255,255,0.04),_rgba(255,255,255,0.01))]">
        
        <div className="grid w-full max-w-6xl overflow-hidden rounded-[32px] border border-white/10 bg-[#080808] shadow-[0_24px_80px_rgba(0,0,0,0.45)] lg:grid-cols-[1fr_0.95fr]">
          
          {/* LADO ESQUERDO (IGUAL AO LOGIN) */}
          <section className="flex flex-col justify-between gap-8 p-6 sm:p-8 lg:p-10">
            <div className="space-y-5">
              <span className="inline-flex rounded-full border border-yellow-400/30 bg-yellow-400/10 px-4 py-1 text-sm font-medium text-yellow-300">
                Segurança da conta
              </span>

              <div className="space-y-3">
                <h1 className="text-4xl font-bold tracking-tight text-white sm:text-5xl lg:text-6xl">
                  Redefina sua senha
                </h1>
                <p className="max-w-xl text-sm leading-6 text-neutral-300 sm:text-base">
                  Escolha uma nova senha para proteger sua conta e continuar utilizando a plataforma com segurança.
                </p>
              </div>
            </div>

            <div className="grid gap-3 sm:grid-cols-2">
              <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <p className="text-sm font-semibold text-white">Segurança</p>
                <p className="mt-1 text-sm text-neutral-400">
                  Utilize uma senha forte para manter sua conta protegida.
                </p>
              </div>

              <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <p className="text-sm font-semibold text-white">Privacidade</p>
                <p className="mt-1 text-sm text-neutral-400">
                  Nunca compartilhe sua senha com outras pessoas.
                </p>
              </div>
            </div>
          </section>

          {/* LADO DIREITO (FORMULÁRIO) */}
          <section className="p-6 sm:p-8 lg:p-10">
            <form className="flex flex-col gap-5" onSubmit={handleSubmit}>
              
              <div className="space-y-2">
                <h2 className="text-2xl font-semibold text-white sm:text-3xl">
                  Nova senha
                </h2>
                <p className="text-sm text-neutral-400">
                  Digite e confirme sua nova senha.
                </p>
              </div>

              <Input
                label="Nova senha"
                id="senha"
                name="senha"
                type="password"
                placeholder="Digite sua nova senha"
                value={formData.senha}
                onChange={handleInputChange}
                error={errors.senha}
                icon={<KeyRound  className="h-5 w-5"/>}
              />

              <Input
                label="Confirmar senha"
                id="confirmarSenha"
                name="confirmarSenha"
                type="password"
                placeholder="Repita sua nova senha"
                value={formData.confirmarSenha}
                onChange={handleInputChange}
                error={errors.confirmarSenha}
                icon={<RefreshCw  className="h-5 w-5"/>}
              />

              <Botao
                type="submit"
                className={`h-12 text-sm font-semibold sm:text-base ${
                  isLoading ? "cursor-not-allowed opacity-70" : ""
                }`}
              >
                {isLoading ? "Salvando..." : "Alterar senha"}
              </Botao>
            </form>
          </section>

        </div>
      </div>
    </PageLayout>
  );
}