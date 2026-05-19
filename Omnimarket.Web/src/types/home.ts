// Define a estrutura de uma categoria exibida na Home.
export type HomeCategory = {
  id: string;
  nome: string;
  icone: "smartphone" | "shirt" | "gamepad" | "sofa" | "headphones" | "watch";
};

// Define o formato de um produto usado no mock visual da Home.
export type HomeProduct = {
  id: string;
  nome: string;
  preco: number;
  avaliacao: number;
  categoriaId: string;
  imagem: string;
  destaque?: string;
};
