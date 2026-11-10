/**
 * Ícone e cor de cada categoria, só para exibição.
 * As categorias padrão têm ícones conhecidos; as criadas pelo usuário recebem um ícone genérico.
 * A cor é escolhida pelo nome (sempre a mesma para o mesmo nome).
 */

const ICONES: Record<string, string> = {
  alimentacao: 'restaurant',
  educacao: 'school',
  lazer: 'sports_esports',
  moradia: 'home',
  outros: 'category',
  salario: 'payments',
  saude: 'favorite',
  transporte: 'directions_car',
  pets: 'pets',
  mercado: 'shopping_cart',
  viagem: 'flight',
  investimentos: 'trending_up',
};

/** Cores que funcionam bem tanto no tema claro quanto no escuro */
export const CORES_CATEGORIAS = [
  '#7c5cff',
  '#16b8d4',
  '#ff8a3d',
  '#2fb67c',
  '#ff5c8a',
  '#e0a800',
  '#4c6fff',
  '#b25cff',
];

export const COR_SEM_CATEGORIA = '#8a8796';

function normalizar(nome: string): string {
  return nome
    .normalize('NFD')
    .replace(/[̀-ͯ]/g, '')
    .toLowerCase()
    .trim();
}

export function iconeCategoria(nome: string | null | undefined): string {
  if (!nome) return 'help_outline';
  return ICONES[normalizar(nome)] ?? 'sell';
}

export function corCategoria(nome: string | null | undefined): string {
  if (!nome) return COR_SEM_CATEGORIA;
  let hash = 0;
  for (const letra of normalizar(nome)) hash = (hash * 31 + letra.charCodeAt(0)) >>> 0;
  return CORES_CATEGORIAS[hash % CORES_CATEGORIAS.length];
}
