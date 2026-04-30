import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface BUDto {
  id: number;
  nombreBU: string;
  sectorId: number;
  sectorNombre: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class BuService {
  private readonly base = `${environment.apiUrl}/api/bus`;

  constructor(private readonly http: HttpClient) {}

  list(params: { sectorId?: number | null; search?: string; page?: number; pageSize?: number }): Observable<ApiResponse<PagedListResult<BUDto>>> {
    let hp = new HttpParams();
    if (params.sectorId != null) hp = hp.set('sectorId', String(params.sectorId));
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<BUDto>>>(this.base, { params: hp });
  }

  getById(id: number): Observable<ApiResponse<BUDto>> {
    return this.http.get<ApiResponse<BUDto>>(`${this.base}/${id}`);
  }

  create(body: { nombreBU: string; sectorId: number }): Observable<ApiResponse<BUDto>> {
    return this.http.post<ApiResponse<BUDto>>(this.base, body);
  }

  update(id: number, body: { nombreBU: string; sectorId: number; activo: boolean }): Observable<ApiResponse<BUDto>> {
    return this.http.put<ApiResponse<BUDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
