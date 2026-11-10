import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { API_URL } from '../api/api.config';
import { AuthService } from './auth.service';

/** Adiciona "Authorization: Bearer <token>" nas chamadas à API */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).token;

  if (!token || !req.url.startsWith(API_URL)) return next(req);

  return next(req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
};
