import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { CadastraMovimentacaoRequest, Movimentacao, Result } from '../models/api.models';
import { API_URL } from './api.config';

@Injectable({ providedIn: 'root' })
export class MovimentacaoService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_URL}/movimentacoes`;

  /**
   * Lista as movimentações do usuário (mais recentes primeiro).
   * As datas são enviadas como instantes ISO (com fuso), para o filtro respeitar o dia local.
   */
  listar(dataInicial?: Date | null, dataFinal?: Date | null): Observable<Movimentacao[]> {
    let params = new HttpParams();
    if (dataInicial) params = params.set('dataInicial', inicioDoDia(dataInicial).toISOString());
    if (dataFinal) params = params.set('dataFinal', fimDoDia(dataFinal).toISOString());

    return this.http
      .get<Result<Movimentacao[]>>(this.url, { params })
      .pipe(map((r) => r.data ?? []));
  }

  criar(request: CadastraMovimentacaoRequest): Observable<string> {
    return this.http
      .post<Result<{ movimentacaoId: string }>>(this.url, request)
      .pipe(map((r) => r.data!.movimentacaoId));
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}

function inicioDoDia(data: Date): Date {
  return new Date(data.getFullYear(), data.getMonth(), data.getDate(), 0, 0, 0, 0);
}

function fimDoDia(data: Date): Date {
  return new Date(data.getFullYear(), data.getMonth(), data.getDate(), 23, 59, 59, 999);
}
