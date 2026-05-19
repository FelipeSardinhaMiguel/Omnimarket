import { useEffect, useState } from "react";

import type {
  PerfilGridItem,
  PerfilPageState,
  PerfilTabContent,
  PerfilTabId,
  UsuarioPerfil,
  UsuarioStatsData,
} from "../types/perfil";

// Define a estrutura esperada das funcoes de API ja existentes no projeto.
interface PerfilUsuarioApi {
  getUsuario?: () => Promise<UsuarioPerfil>;
  getProdutos?: () => Promise<PerfilGridItem[]>;
  getVendas?: () => Promise<PerfilGridItem[]>;
  getCompras?: () => Promise<PerfilGridItem[]>;
  getUsuarioStats?: () => Promise<UsuarioStatsData>;
}

// Expande o objeto global para permitir a injecao das funcoes reais no futuro.
declare global {
  interface Window {
    perfilUsuarioApi?: PerfilUsuarioApi;
  }
}

// Mantem os textos de cada aba centralizados para evitar repeticao.
const TAB_METADATA: Record<PerfilTabId, Omit<PerfilTabContent, "itens">> = {
  produtos: {
    titulo: "Produtos publicados",
    descricao: "Itens atualmente disponiveis na sua vitrine.",
    vazioTitulo: "Nenhum produto encontrado",
    vazioDescricao: "Quando houver produtos cadastrados, eles aparecerao aqui.",
  },
  vendas: {
    titulo: "Historico de vendas",
    descricao: "Acompanhe as vendas concluidas e em andamento.",
    vazioTitulo: "Nenhuma venda encontrada",
    vazioDescricao: "Assim que as vendas forem carregadas, a listagem sera exibida nesta aba.",
  },
  compras: {
    titulo: "Historico de compras",
    descricao: "Visualize os pedidos feitos pela sua conta.",
    vazioTitulo: "Nenhuma compra encontrada",
    vazioDescricao: "As compras vinculadas ao usuario serao exibidas aqui.",
  },
};

// Define um estado inicial vazio para reduzir condicionais na interface.
const INITIAL_STATE: PerfilPageState = {
  isUsuarioLoading: true,
  isConteudoLoading: true,
  usuarioError: "",
  conteudoError: "",
};

// Define um estado inicial para as estatisticas enquanto a API real nao e conectada.
const INITIAL_STATS: UsuarioStatsData = {
  avaliacaoMedia: 0,
  seguidores: 0,
  totalProdutos: 0,
  totalVendas: 0,
  totalCompras: 0,
};

// Define um conteudo vazio padrao para a primeira renderizacao.
const INITIAL_TAB_CONTENT: PerfilTabContent = {
  ...TAB_METADATA.produtos,
  itens: [],
};

// Executa a funcao recebida e, quando ela nao existir, devolve um fallback vazio.
async function executarOuRetornarVazio<T>(
  request: (() => Promise<T>) | undefined,
  fallback: T,
): Promise<T> {
  if (!request) {
    // Simula uma chamada assincrona para manter o comportamento de integracao.
    await new Promise((resolve) => setTimeout(resolve, 400));
    return fallback;
  }

  return request();
}

// Escolhe a funcao correta da API com base na aba ativa.
function obterRequestDaAba(api: PerfilUsuarioApi | undefined, abaAtiva: PerfilTabId) {
  if (abaAtiva === "vendas") {
    return api?.getVendas;
  }

  if (abaAtiva === "compras") {
    return api?.getCompras;
  }

  return api?.getProdutos;
}

// Concentra a logica de carregamento da pagina de perfil em um hook reutilizavel.
export function usePerfilUsuarioData() {
  // Guarda os dados do usuario carregados da API.
  const [usuario, setUsuario] = useState<UsuarioPerfil | null>(null);

  // Guarda as estatisticas principais do perfil.
  const [stats, setStats] = useState<UsuarioStatsData>(INITIAL_STATS);

  // Controla qual aba esta ativa no momento.
  const [abaAtiva, setAbaAtiva] = useState<PerfilTabId>("produtos");

  // Guarda o conteudo exibido abaixo das abas.
  const [tabContent, setTabContent] = useState<PerfilTabContent>(INITIAL_TAB_CONTENT);

  // Centraliza os estados de loading e erro da pagina.
  const [pageState, setPageState] = useState<PerfilPageState>(INITIAL_STATE);

  useEffect(() => {
    let isMounted = true;
    const api = window.perfilUsuarioApi;

    // Carrega os dados fixos do cabecalho do perfil quando a pagina abre.
    async function carregarDadosIniciais() {
      setPageState((currentState) => ({
        ...currentState,
        isUsuarioLoading: true,
        usuarioError: "",
      }));

      try {
        const [usuarioResponse, statsResponse] = await Promise.all([
          executarOuRetornarVazio(api?.getUsuario, null),
          executarOuRetornarVazio(api?.getUsuarioStats, INITIAL_STATS),
        ]);

        if (!isMounted) {
          return;
        }

        setUsuario(usuarioResponse);
        setStats(statsResponse);
      } catch {
        if (!isMounted) {
          return;
        }

        setPageState((currentState) => ({
          ...currentState,
          usuarioError: "Nao foi possivel carregar os dados do perfil.",
        }));
      } finally {
        if (!isMounted) {
          return;
        }

        setPageState((currentState) => ({
          ...currentState,
          isUsuarioLoading: false,
        }));
      }
    }

    carregarDadosIniciais();

    // Evita atualizacao de estado quando a tela e desmontada.
    return () => {
      isMounted = false;
    };
  }, []);

  useEffect(() => {
    let isMounted = true;
    const api = window.perfilUsuarioApi;

    // Recarrega o conteudo principal toda vez que o usuario troca de aba.
    async function carregarConteudoDaAba() {
      setPageState((currentState) => ({
        ...currentState,
        isConteudoLoading: true,
        conteudoError: "",
      }));

      try {
        const itens = await executarOuRetornarVazio(
          obterRequestDaAba(api, abaAtiva),
          [],
        );

        if (!isMounted) {
          return;
        }

        setTabContent({
          ...TAB_METADATA[abaAtiva],
          itens,
        });
      } catch {
        if (!isMounted) {
          return;
        }

        setPageState((currentState) => ({
          ...currentState,
          conteudoError: "Nao foi possivel carregar o conteudo desta aba.",
        }));

        setTabContent({
          ...TAB_METADATA[abaAtiva],
          itens: [],
        });
      } finally {
        if (!isMounted) {
          return;
        }

        setPageState((currentState) => ({
          ...currentState,
          isConteudoLoading: false,
        }));
      }
    }

    carregarConteudoDaAba();

    // Evita atualizacoes de estado apos desmontar o componente.
    return () => {
      isMounted = false;
    };
  }, [abaAtiva]);

  // Exponibiliza apenas os dados e acoes que a interface realmente precisa.
  return {
    usuario,
    stats,
    abaAtiva,
    tabContent,
    ...pageState,
    setAbaAtiva,
  };
}
