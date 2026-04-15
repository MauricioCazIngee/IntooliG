import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';

export interface Campania {
  id: string;
  codigo: string;
  nombre: string;
  descripcion: string | null;
  fechaInicio: string | null;
  fechaFin: string | null;
  activa: boolean;
  fechaCreacionUtc: string;
}

export interface CreateCampaniaRequest {
  codigo: string;
  nombre: string;
  descripcion?: string | null;
  fechaInicio?: string | null;
  fechaFin?: string | null;
  activa: boolean;
}

export type SaveCampaniaRequest = CreateCampaniaRequest;

@Injectable({ providedIn: 'root' })
export class CampaniasService {
  private readonly base = `${environment.apiUrl}/api/campanias`;

  constructor(private readonly http: HttpClient) {}

  list(): Observable<ApiResponse<Campania[]>> {
    return this.http.get<ApiResponse<Campania[]>>(this.base);
  }

  create(body: CreateCampaniaRequest): Observable<ApiResponse<Campania>> {
    return this.http.post<ApiResponse<Campania>>(this.base, body);
  }

  update(id: string, body: SaveCampaniaRequest): Observable<ApiResponse<Campania>> {
    return this.http.put<ApiResponse<Campania>>(`${this.base}/${id}`, body);
  }

  delete(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/${id}`);
  }
}
