import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface CiudadDto {
  id: number;
  nombreCiudad: string;
  nombreCorto: string | null;
  estadoId: number;
  nombreEstado: string;
  paisId: number;
  nombrePais: string;
  ciudadPrincipal: boolean;
  activo: boolean;
  poblacion: number | null;
}

export interface CiudadSelectorItemDto {
  id: number;
  nombre: string;
}

@Injectable({ providedIn: 'root' })
export class CiudadRegionService {
  private readonly base = `${environment.apiUrl}/api/ciudades`;

  constructor(private readonly http: HttpClient) {}

  selector(paisId?: number | null, estadoId?: number | null): Observable<ApiResponse<CiudadSelectorItemDto[]>> {
    let hp = new HttpParams();
    if (paisId != null) hp = hp.set('paisId', String(paisId));
    if (estadoId != null) hp = hp.set('estadoId', String(estadoId));
    return this.http.get<ApiResponse<CiudadSelectorItemDto[]>>(`${this.base}/selector`, { params: hp });
  }

  list(params: {
    search?: string;
    paisId?: number | null;
    estadoId?: number | null;
    page?: number;
    pageSize?: number;
    soloActivos?: boolean | null;
  }): Observable<ApiResponse<PagedListResult<CiudadDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.paisId != null) hp = hp.set('paisId', String(params.paisId));
    if (params.estadoId != null) hp = hp.set('estadoId', String(params.estadoId));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    if (params.soloActivos === true) hp = hp.set('soloActivos', 'true');
    if (params.soloActivos === false) hp = hp.set('soloActivos', 'false');
    return this.http.get<ApiResponse<PagedListResult<CiudadDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<CiudadDto>> {
    return this.http.get<ApiResponse<CiudadDto>>(`${this.base}/${id}`);
  }

  create(body: {
    nombreCiudad: string;
    nombreCorto: string | null;
    estadoId: number;
    ciudadPrincipal: boolean;
    activo: boolean;
    poblacion: number | null;
  }): Observable<ApiResponse<CiudadDto>> {
    return this.http.post<ApiResponse<CiudadDto>>(this.base, body);
  }

  update(
    id: number,
    body: {
      nombreCiudad: string;
      nombreCorto: string | null;
      estadoId: number;
      ciudadPrincipal: boolean;
      activo: boolean;
      poblacion: number | null;
    }
  ): Observable<ApiResponse<CiudadDto>> {
    return this.http.put<ApiResponse<CiudadDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
