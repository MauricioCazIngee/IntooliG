import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment';
import { ApiResponse } from '../../../../shared/models/api-response.model';

export interface ProductoCatalogDto {
  id: number;
  nombre: string;
  activo: boolean;
}

@Injectable({ providedIn: 'root' })
export class ProductoCatalogService {
  private readonly base = `${environment.apiUrl}/api/productos`;

  constructor(private readonly http: HttpClient) {}

  listByMarca(marcaId: number): Observable<ApiResponse<ProductoCatalogDto[]>> {
    const hp = new HttpParams().set('marcaId', String(marcaId));
    return this.http.get<ApiResponse<ProductoCatalogDto[]>>(this.base, { params: hp });
  }
}
