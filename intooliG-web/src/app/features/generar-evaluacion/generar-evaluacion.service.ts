import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';

export interface CampaniaEvaluacionDto {
  campaniaId: number;
  campania: string;
  marca: string;
  categoria: string;
  bu: string;
  sector: string;
  anio: number;
  activo: boolean;
}

export interface AdministracionCampaniaDto {
  administracionId: number;
  sector: string;
  unidadNegocio: string;
  producto: string;
  marca: string;
  vehiculo: string;
  campania: string;
  anio: number;
  semanas: string;
  activo: boolean;
}

export interface PagedListResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CampaniasEvaluacionParams {
  anio?: number | null;
  categoriaId?: number | null;
  activo?: boolean | null;
  search?: string | null;
  page?: number;
  pageSize?: number;
}

export interface AdministracionCampaniasParams {
  anio?: number | null;
  sectorId?: number | null;
  search?: string | null;
  page?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class GenerarEvaluacionService {
  private readonly base = `${environment.apiUrl}/api/GenerarEvaluacion`;

  constructor(private readonly http: HttpClient) {}

  getCampaniasEvaluacion(
    params: CampaniasEvaluacionParams
  ): Observable<ApiResponse<PagedListResult<CampaniaEvaluacionDto>>> {
    let hp = new HttpParams();
    const p = params;
    if (p.anio != null) hp = hp.set('anio', String(p.anio));
    if (p.categoriaId != null) hp = hp.set('categoriaId', String(p.categoriaId));
    if (p.activo !== null && p.activo !== undefined) hp = hp.set('activo', String(p.activo));
    if (p.search != null && p.search.trim() !== '') hp = hp.set('search', p.search.trim());
    if (p.page != null) hp = hp.set('page', String(p.page));
    if (p.pageSize != null) hp = hp.set('pageSize', String(p.pageSize));
    return this.http.get<ApiResponse<PagedListResult<CampaniaEvaluacionDto>>>(`${this.base}/campanias`, {
      params: hp
    });
  }

  getAdministracionCampanias(
    params: AdministracionCampaniasParams
  ): Observable<ApiResponse<PagedListResult<AdministracionCampaniaDto>>> {
    let hp = new HttpParams();
    const p = params;
    if (p.anio != null) hp = hp.set('anio', String(p.anio));
    if (p.sectorId != null) hp = hp.set('sectorId', String(p.sectorId));
    if (p.search != null && p.search.trim() !== '') hp = hp.set('search', p.search.trim());
    if (p.page != null) hp = hp.set('page', String(p.page));
    if (p.pageSize != null) hp = hp.set('pageSize', String(p.pageSize));
    return this.http.get<ApiResponse<PagedListResult<AdministracionCampaniaDto>>>(
      `${this.base}/administracion`,
      { params: hp }
    );
  }
}
