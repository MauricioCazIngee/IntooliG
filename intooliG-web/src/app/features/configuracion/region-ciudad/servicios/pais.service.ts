import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface PaisDto {
  id: number;
  nombrePais: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class PaisService {
  private readonly base = `${environment.apiUrl}/api/paises`;

  constructor(private readonly http: HttpClient) {}

  list(params: {
    search?: string;
    page?: number;
    pageSize?: number;
    soloActivos?: boolean | null;
  }): Observable<ApiResponse<PagedListResult<PaisDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    if (params.soloActivos === true) hp = hp.set('soloActivos', 'true');
    if (params.soloActivos === false) hp = hp.set('soloActivos', 'false');
    return this.http.get<ApiResponse<PagedListResult<PaisDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<PaisDto>> {
    return this.http.get<ApiResponse<PaisDto>>(`${this.base}/${id}`);
  }

  create(body: { nombrePais: string; activo: boolean }): Observable<ApiResponse<PaisDto>> {
    return this.http.post<ApiResponse<PaisDto>>(this.base, body);
  }

  update(id: number, body: { nombrePais: string; activo: boolean }): Observable<ApiResponse<PaisDto>> {
    return this.http.put<ApiResponse<PaisDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
