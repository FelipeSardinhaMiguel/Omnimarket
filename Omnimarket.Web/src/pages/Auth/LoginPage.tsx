import { Link } from "@tanstack/react-router";
import { useState, type ChangeEvent, type FormEvent } from "react";
import { Botao } from "../../Components/Botao";
import { Input } from "../../Components/Input";
import { PageLayout } from "../../Components/PageLayout";

import { LockIcon, User } from "lucide-react";

import Apple from "../../assets/Icone_apple.png";
import Google2 from "../../assets/Icone_google.webp";

import type { LoginErrors, LoginFormData } from "../../types/LoginFormData";
import { validarLogin } from "../../utils/validators";
import { loginUsuario } from "../../Services/auth/authService";

const initialFormData: LoginFormData = {
  email: "",
  senha: "",
};

export function LoginPage() {
  const [formData, setFormData] = useState<LoginFormData>(initialFormData);
  const [errors, setErrors] = useState<LoginErrors>({});
  const [isLoading, setIsLoading] = useState(false);

  function handleInputChange(event: ChangeEvent<HTMLInputElement>) {
    const { name, value } = event.target;

    setFormData((currentData) => ({
      ...currentData,
      [name]: value,
    }));

    setErrors((currentErrors) => ({
      ...currentErrors,
      [name]: "",
    }));
  }

  //Metodo para fazer a validação (real) do login e chamando-a no form (row 127)
  async function handleLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (isLoading) {
      return;
    }

    const validationErrors = validarLogin(formData);

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {

      setErrors({});
      setIsLoading(true);

      const data = await loginUsuario(
        formData.email,
        formData.senha
      );

      console.log(data);

      localStorage.setItem("token", data.token);

      localStorage.setItem(
        "usuario",
        JSON.stringify(data.usuario)
      );

      alert("Login realizado com sucesso!");

    } catch (error: any) {

      alert(error.message);

    } finally {

      setIsLoading(false);

    }
  }

  return (
    <PageLayout>
      <div className="flex min-h-[calc(100vh-106px)] w-full items-center justify-center px-4 py-8 sm:px-6 bg-[radial-gradient(circle_at_top,_rgba(250,204,21,0.18),_transparent_45%),linear-gradient(180deg,_rgba(255,255,255,0.04),_rgba(255,255,255,0.01))]">
        <div className="grid w-full max-w-6xl overflow-hidden rounded-[32px] border border-white/10 bg-[#080808] shadow-[0_24px_80px_rgba(0,0,0,0.45)] lg:grid-cols-[1fr_0.95fr]">
          <section className="flex flex-col justify-between gap-8  p-6 sm:p-8 lg:p-10">
            <div className="space-y-5">
              <span className="inline-flex rounded-full border border-yellow-400/30 bg-yellow-400/10 px-4 py-1 text-sm font-medium text-yellow-300">
                Acesse sua conta
              </span>

              <div className="space-y-3">
                <h1 className="text-4xl font-bold tracking-tight text-white sm:text-5xl lg:text-6xl">
                  Entre na OmniMarket
                </h1>
                <p className="max-w-xl text-sm leading-6 text-neutral-300 sm:text-base">
                  Gerencie compras, vendas e pedidos em uma experiencia pensada
                  para desktop e celular.
                </p>
              </div>
            </div>

            <div className="grid gap-3 sm:grid-cols-2">
              <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <p className="text-sm font-semibold text-white">Acesso rapido</p>
                <p className="mt-1 text-sm text-neutral-400">
                  Entre com email e senha em um fluxo simples e direto.
                </p>
              </div>

              <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <p className="text-sm font-semibold text-white">Conta conectada</p>
                <p className="mt-1 text-sm text-neutral-400">
                  Continue com provedores externos quando esse fluxo for integrado.
                </p>
              </div>
            </div>
          </section>

          <section className="p-6 sm:p-8 lg:p-10">
            <form className="flex flex-col gap-5" onSubmit={handleLogin}>
              <div className="space-y-2">
                <h2 className="text-2xl font-semibold text-white sm:text-3xl">
                  Bem-vindo de volta
                </h2>
                <p className="text-sm text-neutral-400">
                  Informe seus dados para continuar.
                </p>
              </div>

              <Input
                label="Email"
                id="email"
                name="email"
                type="email"
                placeholder="voce@exemplo.com"
                value={formData.email}
                onChange={handleInputChange}
                error={errors.email}
                icon={<User className="w-5 h-5"/>}
              />

              <Input
                label="Senha"
                id="senha"
                name="senha"
                type="password"
                placeholder="Digite sua senha"
                value={formData.senha}
                onChange={handleInputChange}
                error={errors.senha}
                icon={<LockIcon className="h-5 w-5"/>}
              />

              <div className="flex items-center justify-between gap-4 text-sm">
                <label className="flex items-center gap-2 text-neutral-400">
                  <input type="checkbox" className="cursor-pointer accent-yellow-500" />
                  Lembrar-me
                </label>

                <Link
                  to={"/recuperarSenha"}
                  className="text-yellow-400 transition hover:text-yellow-300 hover:underline">
                  Esqueci minha senha
                </Link>
              </div>

              <Botao
                type="submit"
                className={`h-12 text-sm font-semibold sm:text-base ${
                  isLoading ? "cursor-not-allowed opacity-70" : ""
                }`}
              >
                {isLoading ? "Entrando..." : "Entrar"}
              </Botao>

              <p className="text-center text-sm uppercase tracking-[0.3em] text-neutral-500">
                ────────────── Ou ──────────────
              </p>

              <div className="flex flex-col gap-3">
                <Botao
                  type="button"
                  variant="secondary"
                  className="h-12 border-white/10 bg-white/5 hover:bg-white/10"
                  icon={<img src={Google2} alt="Google" className="h-5 w-5" />}
                >
                  Continuar com Google
                </Botao>

                <Botao
                  type="button"
                  variant="secondary"
                  className="h-12 border-white/10 bg-white/5 hover:bg-white/10"
                  icon={<img src={Apple} alt="Apple" className="h-5 w-5" />}
                >
                  Continuar com Apple
                </Botao>
              </div>

              <p className="text-center text-sm text-neutral-400">
                Ainda nao possui conta?
                <Link
                  to="/cadastro"
                  className="ml-2 font-semibold text-yellow-400 transition hover:text-yellow-300 hover:underline"
                >
                  Criar conta
                </Link>
              </p>
            </form>
          </section>
        </div>
      </div>
    </PageLayout>
  );
}
