import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface CategoriaDto {
  id: number;
  nombreCategoria: string;
  nombreCorto: string | null;
  buNombre: string | null;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class CategoriaService {
  private readonly base = `${environment.apiUrl}/api/categorias`;

  constructor(private readonly http: HttpClient) {}

  list(params: { buId?: number | null; search?: string; page?: number; pageSize?: number }): Observable<ApiResponse<PagedListResult<CategoriaDto>>> {
    let hp = new HttpParams();
    if (params.buId != null) hp = hp.set('buId', String(params.buId));
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<CategoriaDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<CategoriaDto>> {
    return this.http.get<ApiResponse<CategoriaDto>>(`${this.base}/${id}`);
  }

  create(body: { nombreCategoria: string; nombreCorto?: string | null }): Observable<ApiResponse<CategoriaDto>> {
    return this.http.post<ApiResponse<CategoriaDto>>(this.base, body);
  }

  update(id: number, body: { nombreCategoria: string; nombreCorto?: string | null; activo: boolean }): Observable<ApiResponse<CategoriaDto>> {
    return this.http.put<ApiResponse<CategoriaDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
