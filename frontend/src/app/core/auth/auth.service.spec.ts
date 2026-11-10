import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AuthService } from './auth.service';

/** Monta um JWT falso (sem assinatura válida) só com o campo exp */
function tokenExpirandoEm(segundosAPartirDeAgora: number): string {
  const payload = btoa(JSON.stringify({ exp: Math.floor(Date.now() / 1000) + segundosAPartirDeAgora }));
  return `cabecalho.${payload}.assinatura`;
}

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    });
    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('começa deslogado', () => {
    expect(service.estaAutenticado()).toBe(false);
    expect(service.token).toBeNull();
  });

  it('guarda a sessão após o login', () => {
    const token = tokenExpirandoEm(3600);
    service.login({ email: 'a@a.com', senha: 'x' }).subscribe();

    const req = http.expectOne('/api/v1/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush({
      success: true,
      error: null,
      data: { idUsuario: '1', nome: 'Ana Souza', email: 'a@a.com', token },
    });

    expect(service.estaAutenticado()).toBe(true);
    expect(service.nomeUsuario()).toBe('Ana Souza');
    expect(localStorage.getItem('financas.sessao')).toContain(token);
  });

  it('considera token expirado como deslogado', () => {
    service.login({ email: 'a@a.com', senha: 'x' }).subscribe();
    http.expectOne('/api/v1/auth/login').flush({
      success: true,
      error: null,
      data: { idUsuario: '1', nome: 'Ana', email: 'a@a.com', token: tokenExpirandoEm(-10) },
    });

    expect(service.estaAutenticado()).toBe(false);
  });

  it('logout limpa a sessão', () => {
    service.login({ email: 'a@a.com', senha: 'x' }).subscribe();
    http.expectOne('/api/v1/auth/login').flush({
      success: true,
      error: null,
      data: { idUsuario: '1', nome: 'Ana', email: 'a@a.com', token: tokenExpirandoEm(3600) },
    });

    service.logout(false);

    expect(service.estaAutenticado()).toBe(false);
    expect(localStorage.getItem('financas.sessao')).toBeNull();
  });
});
