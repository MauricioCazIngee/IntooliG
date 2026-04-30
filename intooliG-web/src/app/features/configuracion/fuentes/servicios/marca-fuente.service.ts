import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface MarcaFuenteDto {
  id: number;
  nombreMarcaFuente: string;
  fuenteId: number;
  nombreFuente: string;
  marcaId: number;
  nombreMarca: string;
  productoId: number;
  nombreProducto: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class MarcaFuenteService {
  private readonly base = `${environment.apiUrl}/api/marcas-fuente`;

  constructor(private readonly http: HttpClient) {}

  list(params: {
    search?: string;
    fuenteId?: number | null;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<PagedListResult<MarcaFuenteDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.fuenteId != null) hp = hp.set('fuenteId', String(params.fuenteId));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<MarcaFuenteDto>>>(this.base, { params: hp });
  }

  porFuente(fuenteId: number): Observable<ApiResponse<MarcaFuenteDto[]>> {
    return this.http.get<ApiResponse<MarcaFuenteDto[]>>(`${this.base}/por-fuente/${fuenteId}`);
  }

  getById(id: number): Observable<ApiResponse<MarcaFuenteDto>> {
    return this.http.get<ApiResponse<MarcaFuenteDto>>(`${this.base}/${id}`);
  }

  create(body: { nombreMarcaFuente: string; fuenteId: number; marcaId: number; productoId: number }): Observable<ApiResponse<MarcaFuenteDto>> {
    return this.http.post<ApiResponse<MarcaFuenteDto>>(this.base, body);
  }

  update(
    id: number,
    body: { nombreMarcaFuente: string; fuenteId: number; marcaId: number; productoId: number }
  ): Observable<ApiResponse<MarcaFuenteDto>> {
    return this.http.put<ApiResponse<MarcaFuenteDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
