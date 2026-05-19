import type React from "react";

export type CadastroFormData = {
  nomeCompleto: string;
  email: string;
  senha: string;
  confirmarSenha: string;
  cpf: string;
  telefone: string;
  dataNascimento: string;
  icon: React.ReactNode;
};

export type FormErrors = Partial<Record<keyof CadastroFormData, string>>;