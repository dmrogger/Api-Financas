import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

/** Só deixa entrar quem está logado; os demais vão para /login */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  if (auth.estaAutenticado()) return true;

  auth.logout(false); // limpa token expirado, se houver
  return inject(Router).createUrlTree(['/login']);
};

/** Telas de login/cadastro: quem já está logado vai direto para o início */
export const visitanteGuard: CanActivateFn = () => {
  return inject(AuthService).estaAutenticado() ? inject(Router).createUrlTree(['/']) : true;
};
