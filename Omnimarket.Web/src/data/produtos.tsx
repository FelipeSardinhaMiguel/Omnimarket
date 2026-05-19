//nessa classe (data/produtos) vai ter o array dos produtos chumbados que será usada pela HomePage e ProdutoPage e a função que cria as imagens deles

import type { HomeProduct } from "../types/home";


export function createMockImage(label: string, colorA: string, colorB: string) {
  const svg = `
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 800 600">
      <defs>
        <linearGradient id="bg" x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" stop-color="${colorA}" />
          <stop offset="100%" stop-color="${colorB}" />
        </linearGradient>
      </defs>
      <rect width="800" height="600" fill="url(#bg)" />
      <circle cx="650" cy="140" r="110" fill="rgba(255,255,255,0.08)" />
      <circle cx="180" cy="470" r="150" fill="rgba(0,0,0,0.18)" />
      <text x="70" y="310" fill="#ffffff" font-family="Arial, sans-serif" font-size="56" font-weight="700">
        ${label}
      </text>
    </svg>
  `;

  return `data:image/svg+xml;charset=UTF-8,${encodeURIComponent(svg)}`;
}

export const produtosMock: HomeProduct[] = [
  {
    id: "1",
    nome: "Smartphone Orion Pro 256GB com camera tripla",
    preco: 2499.9,
    avaliacao: 4.8,
    categoriaId: "eletronicos",
    imagem: createMockImage("Orion Pro", "#f59e0b", "#111827"),
    destaque: "Frete gratis",
  },
  {
    id: "2",
    nome: "Jaqueta urban tech resistente a agua",
    preco: 289.9,
    avaliacao: 4.6,
    categoriaId: "roupas",
    imagem: createMockImage("Urban Tech", "#334155", "#0f172a"),
    destaque: "Mais vendido",
  },
  {
    id: "3",
    nome: "Console NovaPlay com controle sem fio",
    preco: 3599.0,
    avaliacao: 4.9,
    categoriaId: "games",
    imagem: createMockImage("NovaPlay", "#1d4ed8", "#020617"),
  },
  {
    id: "4",
    nome: "Poltrona lounge para sala e home office",
    preco: 899.9,
    avaliacao: 4.5,
    categoriaId: "casa",
    imagem: createMockImage("Lounge Seat", "#78716c", "#111827"),
  },
  {
    id: "5",
    nome: "Fone bluetooth pulse com cancelamento de ruido",
    preco: 649.9,
    avaliacao: 4.7,
    categoriaId: "audio",
    imagem: createMockImage("Pulse Audio", "#0f766e", "#022c22"),
    destaque: "Entrega hoje",
  },
  {
    id: "6",
    nome: "Smartwatch Axis com monitoramento avancado",
    preco: 799.9,
    avaliacao: 4.4,
    categoriaId: "acessorios",
    imagem: createMockImage("Axis Watch", "#7c3aed", "#111827"),
  },
  {
    id: "7",
    nome: "Notebook Vision 15 para estudo e produtividade",
    preco: 4299.9,
    avaliacao: 4.8,
    categoriaId: "eletronicos",
    imagem: createMockImage("Vision 15", "#f97316", "#1f2937"),
  },
  {
    id: "8",
    nome: "Moletom essencial premium com modelagem ampla",
    preco: 159.9,
    avaliacao: 4.3,
    categoriaId: "roupas",
    imagem: createMockImage("Essencial", "#475569", "#111827"),
  },
  {
    id: "9",
    nome: "Kit gamer com teclado mecanico e mouse RGB",
    preco: 449.9,
    avaliacao: 4.6,
    categoriaId: "games",
    imagem: createMockImage("Kit Gamer", "#9333ea", "#111827"),
  },
];