import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface EstadoDto {
  id: number;
  nombreEstado: string;
  paisId: number;
  nombrePais: string;
  codigoMapaId: number | null;
  codigoMapaNombre: string | null;
  activo: boolean;
}

export interface CodigoMapaLookupDto {
  id: number;
  nombre: string;
}

@Injectable({ providedIn: 'root' })
export class EstadoService {
  private readonly base = `${environment.apiUrl}/api/estados`;

  constructor(private readonly http: HttpClient) {}

  codigosMapa(): Observable<ApiResponse<CodigoMapaLookupDto[]>> {
    return this.http.get<ApiResponse<CodigoMapaLookupDto[]>>(`${this.base}/codigos-mapa`);
  }

  list(params: {
    search?: string;
    paisId?: number | null;
    page?: number;
    pageSize?: number;
    soloActivos?: boolean | null;
  }): Observable<ApiResponse<PagedListResult<EstadoDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.paisId != null) hp = hp.set('paisId', String(params.paisId));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    if (params.soloActivos === true) hp = hp.set('soloActivos', 'true');
    if (params.soloActivos === false) hp = hp.set('soloActivos', 'false');
    return this.http.get<ApiResponse<PagedListResult<EstadoDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<EstadoDto>> {
    return this.http.get<ApiResponse<EstadoDto>>(`${this.base}/${id}`);
  }

  create(body: {
    nombreEstado: string;
    paisId: number;
    codigoMapaId: number | null;
    activo: boolean;
  }): Observable<ApiResponse<EstadoDto>> {
    return this.http.post<ApiResponse<EstadoDto>>(this.base, body);
  }

  update(
    id: number,
    body: { nombreEstado: string; paisId: number; codigoMapaId: number | null; activo: boolean }
  ): Observable<ApiResponse<EstadoDto>> {
    return this.http.put<ApiResponse<EstadoDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
