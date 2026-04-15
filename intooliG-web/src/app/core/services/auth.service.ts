import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';

export interface LoginResponse {
  token: string;
  usuarioId: string;
  email: string;
  nombre: string;
  rol: string;
}

const TOKEN_KEY = 'intoolig_token';
const ROL_KEY = 'intoolig_rol';
const NOMBRE_KEY = 'intoolig_nombre';
const EMAIL_KEY = 'intoolig_email';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly base = `${environment.apiUrl}/api/auth`;

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router
  ) {}

  login(email: string, password: string): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${this.base}/login`, { email, password })
      .pipe(
        tap((res) => {
          if (res.success && res.data?.token) {
            localStorage.setItem(TOKEN_KEY, res.data.token);
            localStorage.setItem(ROL_KEY, res.data.rol);
            localStorage.setItem(NOMBRE_KEY, res.data.nombre);
            localStorage.setItem(EMAIL_KEY, res.data.email);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(ROL_KEY);
    localStorage.removeItem(NOMBRE_KEY);
    localStorage.removeItem(EMAIL_KEY);
    void this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getRole(): string | null {
    return localStorage.getItem(ROL_KEY);
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  getDisplayName(): string {
    return localStorage.getItem(NOMBRE_KEY) ?? localStorage.getItem(EMAIL_KEY) ?? 'Usuario';
  }
}
