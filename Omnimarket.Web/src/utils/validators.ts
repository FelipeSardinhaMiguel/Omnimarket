import type { CadastroFormData, FormErrors } from "../types/cadastroFormData";
import type { LoginErrors, LoginFormData } from "../types/LoginFormData";

export function validarEmail(email: string) {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

export function validarCpf(cpf: string) {
  return /^\d{3}\.\d{3}\.\d{3}-\d{2}$/.test(cpf);
}

export function validarFormulario(formData: CadastroFormData): FormErrors {
  const errors: FormErrors = {};

  if (!formData.nomeCompleto.trim()) {
    errors.nomeCompleto = "Informe seu nome completo.";
  }

  if (!formData.email.trim()) {
    errors.email = "Informe seu email.";
  } else if (!validarEmail(formData.email)) {
    errors.email = "Digite um email valido.";
  }

  if (!formData.senha) {
    errors.senha = "Informe uma senha.";
  } else if (formData.senha.length < 6) {
    errors.senha = "A senha precisa ter no minimo 6 caracteres.";
  }

  if (!formData.confirmarSenha) {
    errors.confirmarSenha = "Confirme sua senha.";
  } else if (formData.senha !== formData.confirmarSenha) {
    errors.confirmarSenha = "As senhas precisam coincidir.";
  }

  if (!formData.cpf.trim()) {
    errors.cpf = "Informe seu CPF.";
  } else if (!validarCpf(formData.cpf)) {
    errors.cpf = "Digite um CPF no formato 000.000.000-00.";
  }

  if (!formData.telefone.trim()) {
    errors.telefone = "Informe seu telefone.";
  }

  if (!formData.dataNascimento) {
    errors.dataNascimento = "Informe sua data de nascimento.";
  }

  return errors;
}

export function validarLogin(formData: LoginFormData): LoginErrors {
  const errors: LoginErrors = {};

  if (!formData.email.trim()) {
    errors.email = "Informe seu email.";
  } else if (!validarEmail(formData.email)) {
    errors.email = "Digite um email valido.";
  }

  if (!formData.senha.trim()) {
    errors.senha = "Informe sua senha.";
  }

  return errors;
}
