export type LoginFormData = {
  email: string;
  senha: string;
};

export type LoginErrors = Partial<Record<keyof LoginFormData, string>>;
