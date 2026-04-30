import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface RubroCombinacionDto {
  id: number;
  rubroGeneralId: number;
  rubroNombre: string;
  categoriaId: number;
  categoriaNombre: string;
  valorRubro: number;
  activo: boolean;
  clienteId: number;
  clienteNombre: string | null;
}

@Injectable({ providedIn: 'root' })
export class RubroCatalogoService {
  private readonly base = `${environment.apiUrl}/api/rubros`;

  constructor(private readonly http: HttpClient) {}

  list(params: {
    categoriaId?: number | null;
    activo?: boolean | null;
    search?: string;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<PagedListResult<RubroCombinacionDto>>> {
    let hp = new HttpParams();
    if (params.categoriaId != null) hp = hp.set('categoriaId', String(params.categoriaId));
    if (params.activo != null) hp = hp.set('activo', String(params.activo));
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<RubroCombinacionDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<RubroCombinacionDto>> {
    return this.http.get<ApiResponse<RubroCombinacionDto>>(`${this.base}/${id}`);
  }

  create(body: {
    rubroGeneralId: number;
    categoriaId: number;
    valorRubro: number;
    activo: boolean;
    clienteId?: number | null;
  }): Observable<ApiResponse<RubroCombinacionDto>> {
    return this.http.post<ApiResponse<RubroCombinacionDto>>(this.base, body);
  }

  update(
    id: number,
    body: {
      rubroGeneralId: number;
      categoriaId: number;
      valorRubro: number;
      activo: boolean;
      clienteId?: number | null;
    }
  ): Observable<ApiResponse<RubroCombinacionDto>> {
    return this.http.put<ApiResponse<RubroCombinacionDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
