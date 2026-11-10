// Interfaces que espelham os DTOs da API (ApiFinancas/Src/Application/DTOs).
// A API serializa em camelCase, então os nomes aqui seguem esse padrão.

/** Envelope padrão de resposta da API */
export interface Result<T> {
  success: boolean;
  error: string | null;
  data: T | null;
}

/** ETipoMovimentacao: 1 = Entrada, 2 = Saída */
export enum TipoMovimentacao {
  Entrada = 1,
  Saida = 2,
}

// ---------- Autenticação / Usuário ----------

export interface LoginRequest {
  email: string;
  senha: string;
}

export interface LoginResponse {
  idUsuario: string;
  nome: string;
  email: string;
  token: string;
}

export interface CriaUsuarioRequest {
  nome: string;
  email: string;
  senha: string;
}

export interface Usuario {
  idUsuario: string;
  nome: string;
  email: string;
}

export interface EditaUsuarioRequest {
  senhaAtual: string;
  nome?: string | null;
  senhaNova?: string | null;
}

// ---------- Categorias ----------

export interface Categoria {
  categoriaId: string;
  nome: string;
  /** true = categoria do sistema (não pode ser editada nem excluída) */
  padrao: boolean;
}

// ---------- Movimentações ----------

export interface Movimentacao {
  movimentacaoId: string;
  valor: number;
  /** Data em ISO 8601 (UTC) */
  dataTransacao: string;
  tipo: TipoMovimentacao;
  descricao: string | null;
  categoriaId: string | null;
}

export interface CadastraMovimentacaoRequest {
  valor: number;
  tipo: TipoMovimentacao;
  /** ISO 8601. Se omitida, a API usa a data atual */
  dataOperacao?: string | null;
  descricao?: string | null;
  categoriaId?: string | null;
}

// ---------- Resumos ----------

export interface ResumoCategoria {
  /** null = "Sem categoria" */
  categoriaId: string | null;
  nome: string;
  totalEntradas: number;
  totalSaidas: number;
}

export interface ResumoMensal {
  ano: number;
  mes: number;
  totalEntradas: number;
  totalSaidas: number;
  saldo: number;
  categorias: ResumoCategoria[];
}
