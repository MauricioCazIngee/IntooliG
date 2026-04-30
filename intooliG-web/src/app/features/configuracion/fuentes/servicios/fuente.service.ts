import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface FuenteDto {
  id: number;
  nombreFuente: string;
  paisId: number;
  nombrePais: string;
  activo: boolean;
}

export interface FuenteLookupDto {
  id: number;
  nombre: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class FuenteService {
  private readonly base = `${environment.apiUrl}/api/fuentes`;

  constructor(private readonly http: HttpClient) {}

  lookup(): Observable<ApiResponse<FuenteLookupDto[]>> {
    return this.http.get<ApiResponse<FuenteLookupDto[]>>(`${this.base}/lookup`);
  }

  porPais(paisId: number): Observable<ApiResponse<FuenteDto[]>> {
    return this.http.get<ApiResponse<FuenteDto[]>>(`${this.base}/por-pais/${paisId}`);
  }

  list(params: {
    search?: string;
    paisId?: number | null;
    page?: number;
    pageSize?: number;
    soloActivos?: boolean | null;
  }): Observable<ApiResponse<PagedListResult<FuenteDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.paisId != null) hp = hp.set('paisId', String(params.paisId));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    if (params.soloActivos === true) hp = hp.set('soloActivos', 'true');
    if (params.soloActivos === false) hp = hp.set('soloActivos', 'false');
    return this.http.get<ApiResponse<PagedListResult<FuenteDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<FuenteDto>> {
    return this.http.get<ApiResponse<FuenteDto>>(`${this.base}/${id}`);
  }

  create(body: { nombreFuente: string; paisId: number; activo: boolean }): Observable<ApiResponse<FuenteDto>> {
    return this.http.post<ApiResponse<FuenteDto>>(this.base, body);
  }

  update(id: number, body: { nombreFuente: string; paisId: number; activo: boolean }): Observable<ApiResponse<FuenteDto>> {
    return this.http.put<ApiResponse<FuenteDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
