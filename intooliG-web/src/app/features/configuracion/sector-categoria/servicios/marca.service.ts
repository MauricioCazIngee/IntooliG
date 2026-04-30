import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface MarcaDto {
  id: number;
  nombreMarca: string;
  activo: boolean;
  clienteNombre: string;
  tieneLogo: boolean;
  productosResumen: string;
}

export interface ProductoDto {
  id: number;
  nombre: string;
  activo: boolean;
}

export interface MarcaDetailDto {
  id: number;
  nombreMarca: string;
  activo: boolean;
  clienteId: number;
  clienteNombre: string;
  tieneLogo: boolean;
  logoBase64: string | null;
  productos: ProductoDto[];
}

@Injectable({ providedIn: 'root' })
export class MarcaService {
  private readonly base = `${environment.apiUrl}/api/marcas`;

  constructor(private readonly http: HttpClient) {}

  list(params: { search?: string; page?: number; pageSize?: number }): Observable<ApiResponse<PagedListResult<MarcaDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<MarcaDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<MarcaDetailDto>> {
    return this.http.get<ApiResponse<MarcaDetailDto>>(`${this.base}/${id}`);
  }

  create(body: {
    nombreMarca: string;
    activo: boolean;
    logoBase64: string | null;
    productosNombres: string[];
  }): Observable<ApiResponse<MarcaDetailDto>> {
    return this.http.post<ApiResponse<MarcaDetailDto>>(this.base, body);
  }

  update(
    id: number,
    body: {
      nombreMarca: string;
      activo: boolean;
      logoBase64: string | null;
      productosNombres: string[];
    }
  ): Observable<ApiResponse<MarcaDetailDto>> {
    return this.http.put<ApiResponse<MarcaDetailDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
