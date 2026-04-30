import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface RegionDto {
  id: number;
  clienteId: number;
  nombreRegion: string;
  paisId: number;
  nombrePais: string;
  esNacional: boolean;
  activo: boolean;
  ciudadesResumen: string;
  ciudadIds: number[];
}

@Injectable({ providedIn: 'root' })
export class RegionService {
  private readonly base = `${environment.apiUrl}/api/regiones`;

  constructor(private readonly http: HttpClient) {}

  list(params: {
    search?: string;
    paisId?: number | null;
    page?: number;
    pageSize?: number;
    soloActivos?: boolean | null;
  }): Observable<ApiResponse<PagedListResult<RegionDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.paisId != null) hp = hp.set('paisId', String(params.paisId));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    if (params.soloActivos === true) hp = hp.set('soloActivos', 'true');
    if (params.soloActivos === false) hp = hp.set('soloActivos', 'false');
    return this.http.get<ApiResponse<PagedListResult<RegionDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<RegionDto>> {
    return this.http.get<ApiResponse<RegionDto>>(`${this.base}/${id}`);
  }

  create(body: {
    paisId: number;
    nombreRegion: string;
    esNacional: boolean;
    activo: boolean;
    ciudadIds: number[];
  }): Observable<ApiResponse<RegionDto>> {
    return this.http.post<ApiResponse<RegionDto>>(this.base, body);
  }

  update(
    id: number,
    body: {
      paisId: number;
      nombreRegion: string;
      esNacional: boolean;
      activo: boolean;
      ciudadIds: number[];
    }
  ): Observable<ApiResponse<RegionDto>> {
    return this.http.put<ApiResponse<RegionDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
