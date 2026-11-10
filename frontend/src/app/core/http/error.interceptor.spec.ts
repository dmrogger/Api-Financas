import { HttpErrorResponse } from '@angular/common/http';
import { extrairMensagem } from './error.interceptor';

function erro(status: number, corpo: unknown): HttpErrorResponse {
  return new HttpErrorResponse({ status, error: corpo });
}

describe('extrairMensagem', () => {
  it('lê o campo error do Result da API', () => {
    expect(extrairMensagem(erro(409, { success: false, error: 'E-mail já cadastrado', data: null }))).toBe(
      'E-mail já cadastrado',
    );
  });

  it('lê a primeira mensagem de ValidationProblemDetails', () => {
    const corpo = { title: 'One or more validation errors occurred.', errors: { Senha: ['Senha fraca'] } };
    expect(extrairMensagem(erro(400, corpo))).toBe('Senha fraca');
  });

  it('lê o detail de ProblemDetails', () => {
    expect(extrairMensagem(erro(500, { title: 'Erro interno', detail: 'Tente mais tarde' }))).toBe(
      'Tente mais tarde',
    );
  });

  it('aceita texto puro', () => {
    expect(extrairMensagem(erro(401, 'Token inválido ou incompleto!'))).toBe('Token inválido ou incompleto!');
  });

  it('avisa quando a API está fora do ar', () => {
    expect(extrairMensagem(erro(0, null))).toContain('Não foi possível conectar');
  });
});
