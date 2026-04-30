import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface SectorDto {
  id: number;
  nombreSector: string;
  clienteNombre: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class SectorService {
  private readonly base = `${environment.apiUrl}/api/sectores`;

  constructor(private readonly http: HttpClient) {}

  list(params: { search?: string; page?: number; pageSize?: number }): Observable<ApiResponse<PagedListResult<SectorDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<SectorDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<SectorDto>> {
    return this.http.get<ApiResponse<SectorDto>>(`${this.base}/${id}`);
  }

  create(body: { nombreSector: string }): Observable<ApiResponse<SectorDto>> {
    return this.http.post<ApiResponse<SectorDto>>(this.base, body);
  }

  update(id: number, body: { nombreSector: string; activo: boolean }): Observable<ApiResponse<SectorDto>> {
    return this.http.put<ApiResponse<SectorDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
