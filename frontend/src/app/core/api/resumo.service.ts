import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Result, ResumoMensal } from '../models/api.models';
import { API_URL } from './api.config';

@Injectable({ providedIn: 'root' })
export class ResumoService {
  private readonly http = inject(HttpClient);

  /** Totais do mês (mes de 1 a 12) */
  mensal(ano: number, mes: number): Observable<ResumoMensal> {
    const params = new HttpParams().set('ano', ano).set('mes', mes);
    return this.http
      .get<Result<ResumoMensal>>(`${API_URL}/resumos/mensal`, { params })
      .pipe(map((r) => r.data!));
  }
}
