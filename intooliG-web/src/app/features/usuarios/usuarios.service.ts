import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';

export interface Usuario {
  id: number;
  clienteId: number;
  email: string;
  nombre: string;
  rol: 'Admin' | 'Usuario';
  fechaCreacionUtc: string;
}

export interface CreateUsuarioRequest {
  email: string;
  nombre: string;
  rol: string;
  password: string;
  clienteId: number;
}

export interface UpdateUsuarioRequest {
  email: string;
  nombre: string;
  rol: string;
  clienteId: number;
}

export interface ChangeRolRequest {
  rol: string;
}

export interface ResetPasswordRequest {
  newPassword: string;
}

@Injectable({ providedIn: 'root' })
export class UsuariosService {
  private readonly base = `${environment.apiUrl}/api/usuarios`;

  constructor(private readonly http: HttpClient) {}

  list(): Observable<ApiResponse<Usuario[]>> {
    return this.http.get<ApiResponse<Usuario[]>>(this.base);
  }

  create(body: CreateUsuarioRequest): Observable<ApiResponse<Usuario>> {
    return this.http.post<ApiResponse<Usuario>>(this.base, body);
  }

  update(id: number, body: UpdateUsuarioRequest): Observable<ApiResponse<Usuario>> {
    return this.http.put<ApiResponse<Usuario>>(`${this.base}/${id}`, body);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.base}/${id}`);
  }

  changeRol(id: number, body: ChangeRolRequest): Observable<ApiResponse<Usuario>> {
    return this.http.patch<ApiResponse<Usuario>>(`${this.base}/${id}/rol`, body);
  }

  resetPassword(id: number, body: ResetPasswordRequest): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.base}/${id}/reset-password`, body);
  }
}
