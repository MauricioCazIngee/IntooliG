import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface PoblacionDto {
  id: number;
  ciudadId: number;
  nombreCiudad: string;
  estadoId: number;
  nombreEstado: string;
  paisId: number;
  nombrePais: string;
  anio: number;
  cantidad: number;
}

@Injectable({ providedIn: 'root' })
export class PoblacionService {
  private readonly base = `${environment.apiUrl}/api/poblaciones`;

  constructor(private readonly http: HttpClient) {}

  anios(): Observable<ApiResponse<number[]>> {
    return this.http.get<ApiResponse<number[]>>(`${this.base}/anios`);
  }

  list(params: {
    search?: string;
    paisId?: number | null;
    estadoId?: number | null;
    anio?: number | null;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<PagedListResult<PoblacionDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.paisId != null) hp = hp.set('paisId', String(params.paisId));
    if (params.estadoId != null) hp = hp.set('estadoId', String(params.estadoId));
    if (params.anio != null) hp = hp.set('anio', String(params.anio));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<PoblacionDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<PoblacionDto>> {
    return this.http.get<ApiResponse<PoblacionDto>>(`${this.base}/${id}`);
  }

  create(body: { ciudadId: number; anio: number; cantidad: number }): Observable<ApiResponse<PoblacionDto>> {
    return this.http.post<ApiResponse<PoblacionDto>>(this.base, body);
  }

  update(id: number, body: { ciudadId: number; anio: number; cantidad: number }): Observable<ApiResponse<PoblacionDto>> {
    return this.http.put<ApiResponse<PoblacionDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
