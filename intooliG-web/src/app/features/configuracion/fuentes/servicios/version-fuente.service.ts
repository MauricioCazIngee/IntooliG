import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface VersionFuenteDto {
  id: number;
  nombreVersionFuente: string;
  fuenteId: number;
  nombreFuente: string;
  categoriaId: number;
  nombreCategoria: string;
  marcaId: number;
  nombreMarca: string;
  activo: boolean;
  productoId: number;
  nombreProducto: string;
  versionTVId: number;
  nombreVersionTV: string;
  buId: number;
  nombreBU: string;
}

export interface VersionTVLookupDto {
  id: number;
  nombre: string;
}

@Injectable({ providedIn: 'root' })
export class VersionFuenteService {
  private readonly base = `${environment.apiUrl}/api/versiones-fuente`;

  constructor(private readonly http: HttpClient) {}

  versionTvLookup(): Observable<ApiResponse<VersionTVLookupDto[]>> {
    return this.http.get<ApiResponse<VersionTVLookupDto[]>>(`${this.base}/version-tv/lookup`);
  }

  porFuente(fuenteId: number): Observable<ApiResponse<VersionFuenteDto[]>> {
    return this.http.get<ApiResponse<VersionFuenteDto[]>>(`${this.base}/por-fuente/${fuenteId}`);
  }

  list(params: {
    search?: string;
    fuenteId?: number | null;
    categoriaId?: number | null;
    page?: number;
    pageSize?: number;
    soloActivos?: boolean | null;
  }): Observable<ApiResponse<PagedListResult<VersionFuenteDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.fuenteId != null) hp = hp.set('fuenteId', String(params.fuenteId));
    if (params.categoriaId != null) hp = hp.set('categoriaId', String(params.categoriaId));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    if (params.soloActivos === true) hp = hp.set('soloActivos', 'true');
    if (params.soloActivos === false) hp = hp.set('soloActivos', 'false');
    return this.http.get<ApiResponse<PagedListResult<VersionFuenteDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<VersionFuenteDto>> {
    return this.http.get<ApiResponse<VersionFuenteDto>>(`${this.base}/${id}`);
  }

  create(body: {
    nombreVersionFuente: string;
    fuenteId: number;
    categoriaId: number;
    buId: number;
    productoId: number;
    versionTVId: number;
    activo: boolean;
  }): Observable<ApiResponse<VersionFuenteDto>> {
    return this.http.post<ApiResponse<VersionFuenteDto>>(this.base, body);
  }

  update(
    id: number,
    body: {
      nombreVersionFuente: string;
      fuenteId: number;
      categoriaId: number;
      buId: number;
      productoId: number;
      versionTVId: number;
      activo: boolean;
    }
  ): Observable<ApiResponse<VersionFuenteDto>> {
    return this.http.put<ApiResponse<VersionFuenteDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
