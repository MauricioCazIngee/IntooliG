import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface ConceptoListItemDto {
  id: number;
  nombreConcepto: string;
  categoriaId: number;
  categoriaNombre: string;
  rubroGeneralId: number;
  rubroNombre: string;
  posicion: number;
  valor: number | null;
  activo: boolean;
  top: boolean;
}

export interface ConceptoDetailDto {
  id: number;
  nombreConcepto: string;
  rubroGeneralId: number;
  rubroNombre: string;
  categoriaId: number;
  categoriaNombre: string;
  posicion: number;
  valor: number;
  activo: boolean;
  top: boolean;
}

@Injectable({ providedIn: 'root' })
export class ConceptoCatalogoService {
  private readonly base = `${environment.apiUrl}/api/conceptos`;

  constructor(private readonly http: HttpClient) {}

  list(params: {
    categoriaId?: number | null;
    rubroGeneralId?: number | null;
    activo?: boolean | null;
    top?: boolean | null;
    search?: string;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<PagedListResult<ConceptoListItemDto>>> {
    let hp = new HttpParams();
    if (params.categoriaId != null) hp = hp.set('categoriaId', String(params.categoriaId));
    if (params.rubroGeneralId != null) hp = hp.set('rubroGeneralId', String(params.rubroGeneralId));
    if (params.activo != null) hp = hp.set('activo', String(params.activo));
    if (params.top != null) hp = hp.set('top', String(params.top));
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<ConceptoListItemDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<ConceptoDetailDto>> {
    return this.http.get<ApiResponse<ConceptoDetailDto>>(`${this.base}/${id}`);
  }

  create(body: {
    nombreConcepto: string;
    rubroGeneralId: number;
    categoriaId: number;
    posicion: number;
    valor: number;
    activo: boolean;
    top: boolean;
  }): Observable<ApiResponse<ConceptoDetailDto>> {
    return this.http.post<ApiResponse<ConceptoDetailDto>>(this.base, body);
  }

  update(
    id: number,
    body: {
      nombreConcepto: string;
      rubroGeneralId: number;
      categoriaId: number;
      posicion: number;
      valor: number;
      activo: boolean;
      top: boolean;
    }
  ): Observable<ApiResponse<ConceptoDetailDto>> {
    return this.http.put<ApiResponse<ConceptoDetailDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
