import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface MedioDto {
  id: number;
  nombreMedio: string;
  nombreMedioGenerico: string;
  activo: boolean;
}

export interface MedioLookupDto {
  id: number;
  nombre: string;
}

@Injectable({ providedIn: 'root' })
export class MedioService {
  private readonly base = `${environment.apiUrl}/api/medios`;

  constructor(private readonly http: HttpClient) {}

  lookup(): Observable<ApiResponse<MedioLookupDto[]>> {
    return this.http.get<ApiResponse<MedioLookupDto[]>>(`${this.base}/lookup`);
  }

  list(params: {
    search?: string;
    page?: number;
    pageSize?: number;
    soloActivos?: boolean | null;
  }): Observable<ApiResponse<PagedListResult<MedioDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    if (params.soloActivos === true) hp = hp.set('soloActivos', 'true');
    if (params.soloActivos === false) hp = hp.set('soloActivos', 'false');
    return this.http.get<ApiResponse<PagedListResult<MedioDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<MedioDto>> {
    return this.http.get<ApiResponse<MedioDto>>(`${this.base}/${id}`);
  }

  create(body: { nombreMedio: string; nombreMedioGenerico: string; activo: boolean }): Observable<ApiResponse<MedioDto>> {
    return this.http.post<ApiResponse<MedioDto>>(this.base, body);
  }

  update(
    id: number,
    body: { nombreMedio: string; nombreMedioGenerico: string; activo: boolean }
  ): Observable<ApiResponse<MedioDto>> {
    return this.http.put<ApiResponse<MedioDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
