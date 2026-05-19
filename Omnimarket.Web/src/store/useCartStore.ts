// Importa a função create da biblioteca Zustand
// Ela é usada para criar uma store global (estado compartilhado)
import { create } from "zustand";


// 🧩 Tipo que representa um PRODUTO vindo do backend
export type Produto = {
  id: string;            // Identificador único do produto
  nome: string;          // Nome do produto
  preco: number;         // Preço do produto
  imagem?: string;       // URL da imagem (opcional)
  descricao?: string;    // Descrição do produto (opcional)
};


// 🛒 Tipo que representa um ITEM dentro do carrinho
// Ele herda tudo de Produto e adiciona a quantidade
export type CartItem = Produto & {
  quantidade: number;    // Quantidade desse produto no carrinho
};


// 🧠 Tipo da STORE (estrutura completa do carrinho)
type CartStore = {
  items: CartItem[];     // Lista de itens no carrinho

  // Funções (ações) para manipular o carrinho
  addItem: (produto: Produto) => void;
  removeItem: (id: string) => void;
  increaseQuantity: (id: string) => void;
  decreaseQuantity: (id: string) => void;
  clearCart: () => void;
  getTotal: () => number;
};


// 🚀 Criação da store global
// "set" → altera o estado
// "get" → lê o estado atual
export const useCartStore = create<CartStore>((set, get) => ({

  // 🟢 Estado inicial
  items: [],


  // 🛒 Adiciona um produto ao carrinho
  addItem: (produto) => {

    // Pega os itens atuais do carrinho
    const items = get().items;

    // Verifica se o produto já existe no carrinho
    const existing = items.find((item) => item.id === produto.id);


    // 🔵 Se já existir → aumenta a quantidade
    if (existing) {
      set({
        items: items.map((item) =>
          item.id === produto.id
            ? { ...item, quantidade: item.quantidade + 1 } // incrementa
            : item
        ),
      });
      return; // encerra a função
    }


    // 🟢 Se NÃO existir → adiciona novo item
    set({
      items: [
        ...items,
        {
          ...produto,
          quantidade: 1, // começa com 1 unidade
        },
      ],
    });
  },


  // ❌ Remove um produto completamente do carrinho
  removeItem: (id) => {
    set({
      items: get().items.filter((item) => item.id !== id),
    });
  },


  // ➕ Aumenta a quantidade de um produto específico
  increaseQuantity: (id) => {
    set({
      items: get().items.map((item) =>
        item.id === id
          ? { ...item, quantidade: item.quantidade + 1 }
          : item
      ),
    });
  },


  // ➖ Diminui a quantidade de um produto
  decreaseQuantity: (id) => {
    set({
      items: get().items

        // Primeiro diminui a quantidade
        .map((item) =>
          item.id === id
            ? { ...item, quantidade: item.quantidade - 1 }
            : item
        )

        // Depois remove automaticamente se chegar em 0
        .filter((item) => item.quantidade > 0),
    });
  },


  // 🧹 Limpa todo o carrinho
  clearCart: () => set({ items: [] }),


  // 💰 Calcula o total da compra
  getTotal: () =>

    // reduce percorre todos os itens somando
    get().items.reduce(

      // total começa em 0
      // soma: preço × quantidade
      (total, item) => total + item.preco * item.quantidade,

      0 // valor inicial
    ),
}));