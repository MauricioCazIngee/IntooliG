import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface DayPartDto {
  id: number;
  paisId: number;
  paisNombre: string;
  medioId: number;
  medioNombre: string;
  descripcion: string;
  horaInicio: number;
  horaFin: number;
  inicioTexto: string;
  finTexto: string;
}

export interface DayPartRequestDto {
  paisId: number;
  medioId: number;
  descripcion: string;
  horaInicio: number;
  horaFin: number;
}

export interface DayPartLookupOptionDto {
  id: number;
  nombre: string;
}

export interface DayPartLookupDto {
  paises: DayPartLookupOptionDto[];
  medios: DayPartLookupOptionDto[];
  horas: number[];
}

@Injectable({ providedIn: 'root' })
export class DayPartService {
  private readonly base = `${environment.apiUrl}/api/daypart`;

  constructor(private readonly http: HttpClient) {}

  lookup(): Observable<ApiResponse<DayPartLookupDto>> {
    return this.http.get<ApiResponse<DayPartLookupDto>>(`${this.base}/lookup`);
  }

  list(params: {
    search?: string;
    paisId?: number | null;
    medioId?: number | null;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<PagedListResult<DayPartDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.paisId != null) hp = hp.set('paisId', String(params.paisId));
    if (params.medioId != null) hp = hp.set('medioId', String(params.medioId));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<DayPartDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<DayPartDto>> {
    return this.http.get<ApiResponse<DayPartDto>>(`${this.base}/${id}`);
  }

  create(body: DayPartRequestDto): Observable<ApiResponse<DayPartDto>> {
    return this.http.post<ApiResponse<DayPartDto>>(this.base, body);
  }

  update(id: number, body: DayPartRequestDto): Observable<ApiResponse<DayPartDto>> {
    return this.http.put<ApiResponse<DayPartDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
