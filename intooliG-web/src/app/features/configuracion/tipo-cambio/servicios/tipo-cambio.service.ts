import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface TipoCambioDto {
  id: number;
  paisId: number;
  paisNombre: string;
  anio: number;
  mes: number;
  mesNombre: string;
  tipoCambio: number;
}

export interface TipoCambioRequestDto {
  paisId: number;
  anio: number;
  mes: number;
  tipoCambio: number;
}

export interface TipoCambioAnioDto {
  anio: number;
}

export interface TipoCambioMesDto {
  mes: number;
  nombre: string;
}

@Injectable({ providedIn: 'root' })
export class TipoCambioService {
  private readonly base = `${environment.apiUrl}/api/tipo-cambio`;

  constructor(private readonly http: HttpClient) {}

  list(params: {
    search?: string;
    paisId?: number | null;
    anio?: number | null;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<PagedListResult<TipoCambioDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.paisId != null) hp = hp.set('paisId', String(params.paisId));
    if (params.anio != null) hp = hp.set('anio', String(params.anio));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<TipoCambioDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<TipoCambioDto>> {
    return this.http.get<ApiResponse<TipoCambioDto>>(`${this.base}/${id}`);
  }

  create(body: TipoCambioRequestDto): Observable<ApiResponse<TipoCambioDto>> {
    return this.http.post<ApiResponse<TipoCambioDto>>(this.base, body);
  }

  update(id: number, body: TipoCambioRequestDto): Observable<ApiResponse<TipoCambioDto>> {
    return this.http.put<ApiResponse<TipoCambioDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }

  anios(): Observable<ApiResponse<TipoCambioAnioDto[]>> {
    return this.http.get<ApiResponse<TipoCambioAnioDto[]>>(`${this.base}/anios`);
  }

  meses(): Observable<ApiResponse<TipoCambioMesDto[]>> {
    return this.http.get<ApiResponse<TipoCambioMesDto[]>>(`${this.base}/meses`);
  }
}
