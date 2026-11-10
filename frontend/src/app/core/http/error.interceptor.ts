import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { NotificacaoService } from '../notificacao.service';

/**
 * Trata todos os erros HTTP num lugar só: mostra a mensagem e, em 401, encerra a sessão.
 * Os componentes só precisam lidar com o próprio estado (ex.: desligar o "carregando").
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const notificacao = inject(NotificacaoService);

  return next(req).pipe(
    catchError((erro: HttpErrorResponse) => {
      const ehLogin = req.url.endsWith('/auth/login');

      if (erro.status === 401 && !ehLogin) {
        auth.logout();
        notificacao.erro('Sua sessão expirou. Faça login novamente.');
      } else {
        notificacao.erro(extrairMensagem(erro));
      }

      return throwError(() => erro);
    }),
  );
};

/**
 * A API devolve erros em formatos diferentes:
 * - Result.Fail ............. { success: false, error: "..." }
 * - Validação de campos ..... ValidationProblemDetails { errors: { Campo: ["..."] } }
 * - Exceções ................ ProblemDetails { title, detail }
 * - Token inválido .......... texto puro
 */
export function extrairMensagem(erro: HttpErrorResponse): string {
  if (erro.status === 0) return 'Não foi possível conectar à API. Verifique se ela está rodando.';

  const corpo = erro.error;

  if (typeof corpo === 'string' && corpo.trim()) return corpo;

  if (corpo && typeof corpo === 'object') {
    if (typeof corpo.error === 'string' && corpo.error) return corpo.error;

    if (corpo.errors && typeof corpo.errors === 'object') {
      const primeira = Object.values(corpo.errors as Record<string, string[]>).flat()[0];
      if (primeira) return primeira;
    }

    if (typeof corpo.detail === 'string' && corpo.detail) return corpo.detail;
    if (typeof corpo.title === 'string' && corpo.title) return corpo.title;
  }

  return 'Ocorreu um erro inesperado. Tente novamente.';
}
