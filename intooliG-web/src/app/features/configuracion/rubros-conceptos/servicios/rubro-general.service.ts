import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface RubroGeneralDto {
  id: number;
  nombreRubro: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class RubroGeneralService {
  private readonly base = `${environment.apiUrl}/api/rubros-generales`;

  constructor(private readonly http: HttpClient) {}

  list(params: { search?: string; page?: number; pageSize?: number }): Observable<ApiResponse<PagedListResult<RubroGeneralDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<RubroGeneralDto>>>(this.base, { params: hp });
  }

  create(body: { nombreRubro: string }): Observable<ApiResponse<RubroGeneralDto>> {
    return this.http.post<ApiResponse<RubroGeneralDto>>(this.base, body);
  }

  update(id: number, body: { nombreRubro: string; activo: boolean }): Observable<ApiResponse<RubroGeneralDto>> {
    return this.http.put<ApiResponse<RubroGeneralDto>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}`);
  }
}
