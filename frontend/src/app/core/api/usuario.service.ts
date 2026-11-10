import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { CriaUsuarioRequest, EditaUsuarioRequest, Result, Usuario } from '../models/api.models';
import { API_URL } from './api.config';

@Injectable({ providedIn: 'root' })
export class UsuarioService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_URL}/usuarios`;

  cadastrar(request: CriaUsuarioRequest): Observable<Usuario> {
    return this.http.post<Result<Usuario>>(this.url, request).pipe(map((r) => r.data!));
  }

  consultar(): Observable<Usuario> {
    return this.http.get<Result<Usuario>>(`${this.url}/me`).pipe(map((r) => r.data!));
  }

  editar(request: EditaUsuarioRequest): Observable<void> {
    return this.http.patch<Result<string>>(`${this.url}/me`, request).pipe(map(() => undefined));
  }

  /** Exclui a conta do usuário autenticado (confirmação por senha) */
  excluir(senha: string): Observable<void> {
    return this.http
      .delete<Result<string>>(`${this.url}/me`, { body: { senha } })
      .pipe(map(() => undefined));
  }
}
