import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';
import { PagedListResult } from '../../../../shared/models/paged-list.model';

export interface MedioClienteDto {
  medioId: number;
  clienteId: number;
  nombreMedio: string;
  nombreCliente: string;
  esNacional: boolean;
}

export interface MedioClientePorClienteDto {
  medioId: number;
  nombreMedio: string;
  esNacional: boolean;
}

@Injectable({ providedIn: 'root' })
export class MedioClienteService {
  private readonly base = `${environment.apiUrl}/api/medios-cliente`;

  constructor(private readonly http: HttpClient) {}

  porCliente(clienteId: number): Observable<ApiResponse<MedioClientePorClienteDto[]>> {
    return this.http.get<ApiResponse<MedioClientePorClienteDto[]>>(`${this.base}/por-cliente/${clienteId}`);
  }

  list(params: {
    search?: string;
    clienteId?: number | null;
    medioId?: number | null;
    page?: number;
    pageSize?: number;
  }): Observable<ApiResponse<PagedListResult<MedioClienteDto>>> {
    let hp = new HttpParams();
    if (params.search) hp = hp.set('search', params.search);
    if (params.clienteId != null) hp = hp.set('clienteId', String(params.clienteId));
    if (params.medioId != null) hp = hp.set('medioId', String(params.medioId));
    if (params.page != null) hp = hp.set('page', String(params.page));
    if (params.pageSize != null) hp = hp.set('pageSize', String(params.pageSize));
    return this.http.get<ApiResponse<PagedListResult<MedioClienteDto>>>(this.base, { params: hp });
  }

  getByKey(medioId: number, clienteId: number): Observable<ApiResponse<MedioClienteDto>> {
    return this.http.get<ApiResponse<MedioClienteDto>>(`${this.base}/${medioId}/${clienteId}`);
  }

  create(body: { medioId: number; clienteId: number; esNacional: boolean }): Observable<ApiResponse<MedioClienteDto>> {
    return this.http.post<ApiResponse<MedioClienteDto>>(this.base, body);
  }

  update(medioId: number, clienteId: number, body: { esNacional: boolean }): Observable<ApiResponse<MedioClienteDto>> {
    return this.http.put<ApiResponse<MedioClienteDto>>(`${this.base}/${medioId}/${clienteId}`, body);
  }

  delete(medioId: number, clienteId: number): Observable<ApiResponse<unknown>> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${medioId}/${clienteId}`);
  }
}
