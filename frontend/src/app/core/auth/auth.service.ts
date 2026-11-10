import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable, tap } from 'rxjs';
import { API_URL } from '../api/api.config';
import { LoginRequest, LoginResponse, Result } from '../models/api.models';

const CHAVE_SESSAO = 'financas.sessao';

/**
 * Guarda a sessão (dados do usuário + token JWT) no localStorage e expõe o estado como signals.
 * O token emitido pela API expira em 2 horas e não há refresh token: ao expirar, o usuário refaz o login.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly sessaoAtual = signal<LoginResponse | null>(lerSessao());

  /** Usuário logado (ou null) */
  readonly usuario = this.sessaoAtual.asReadonly();
  readonly nomeUsuario = computed(() => this.sessaoAtual()?.nome ?? '');

  get token(): string | null {
    return this.sessaoAtual()?.token ?? null;
  }

  login(request: LoginRequest): Observable<void> {
    return this.http.post<Result<LoginResponse>>(`${API_URL}/auth/login`, request).pipe(
      tap((r) => this.salvarSessao(r.data!)),
      map(() => undefined),
    );
  }

  /** true se existe token e ele ainda não expirou */
  estaAutenticado(): boolean {
    const token = this.token;
    if (!token) return false;

    const expiraEm = lerExpiracao(token);
    return expiraEm !== null && expiraEm > Date.now();
  }

  /** Atualiza o nome exibido após editar o perfil */
  atualizarNome(nome: string): void {
    const sessao = this.sessaoAtual();
    if (sessao) this.salvarSessao({ ...sessao, nome });
  }

  logout(redirecionar = true): void {
    try {
      localStorage.removeItem(CHAVE_SESSAO);
    } catch {
      // localStorage indisponível (ex.: modo privado restrito): a sessão some ao recarregar
    }
    this.sessaoAtual.set(null);
    if (redirecionar) this.router.navigate(['/login']);
  }

  private salvarSessao(sessao: LoginResponse): void {
    try {
      localStorage.setItem(CHAVE_SESSAO, JSON.stringify(sessao));
    } catch {
      // segue só em memória
    }
    this.sessaoAtual.set(sessao);
  }
}

function lerSessao(): LoginResponse | null {
  try {
    const bruto = localStorage.getItem(CHAVE_SESSAO);
    return bruto ? (JSON.parse(bruto) as LoginResponse) : null;
  } catch {
    return null;
  }
}

/** Lê o campo "exp" (segundos desde 1970) do payload do JWT e devolve em milissegundos */
function lerExpiracao(token: string): number | null {
  try {
    const payloadBase64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
    const payload = JSON.parse(atob(payloadBase64)) as { exp?: number };
    return payload.exp ? payload.exp * 1000 : null;
  } catch {
    return null;
  }
}
