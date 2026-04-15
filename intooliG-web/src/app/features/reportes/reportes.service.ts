import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';

export interface ReportesKpi {
  totalCampanias: number;
  campaniasActivas: number;
  campaniasInactivas: number;
  totalUsuarios: number;
  usuariosAdmin: number;
  usuariosNormales: number;
}

@Injectable({ providedIn: 'root' })
export class ReportesService {
  private readonly base = `${environment.apiUrl}/api/reportes`;

  constructor(private readonly http: HttpClient) {}

  kpis(): Observable<ApiResponse<ReportesKpi>> {
    return this.http.get<ApiResponse<ReportesKpi>>(`${this.base}/kpis`);
  }
}
