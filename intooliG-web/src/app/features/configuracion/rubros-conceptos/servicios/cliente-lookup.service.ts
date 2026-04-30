import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';

export interface ClienteLookupDto {
  id: number;
  nombre: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class ClienteLookupService {
  private readonly base = `${environment.apiUrl}/api/clientes`;

  constructor(private readonly http: HttpClient) {}

  list(): Observable<ApiResponse<ClienteLookupDto[]>> {
    return this.http.get<ApiResponse<ClienteLookupDto[]>>(this.base);
  }
}
