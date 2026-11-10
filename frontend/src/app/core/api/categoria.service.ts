import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Categoria, Result } from '../models/api.models';
import { API_URL } from './api.config';

@Injectable({ providedIn: 'root' })
export class CategoriaService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_URL}/categorias`;

  /** Categorias padrão + as do usuário autenticado */
  listar(): Observable<Categoria[]> {
    return this.http.get<Result<Categoria[]>>(this.url).pipe(map((r) => r.data ?? []));
  }

  criar(nome: string): Observable<Categoria> {
    return this.http.post<Result<Categoria>>(this.url, { nome }).pipe(map((r) => r.data!));
  }

  editar(id: string, nome: string): Observable<Categoria> {
    return this.http
      .put<Result<Categoria>>(`${this.url}/${id}`, { nome })
      .pipe(map((r) => r.data!));
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
