import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';

export interface PaisDto {
  id: number;
  nombre: string;
}

export interface FormatoDto {
  id: number;
  nombre: string;
}

export interface VersionTVDto {
  id: number;
  nombre: string;
}

export interface MarcaDto {
  id: number;
  nombre: string;
}

export interface ProductoDto {
  id: number;
  nombre: string;
}

export interface CiudadDto {
  id: number;
  nombre: string;
  paisId?: number | null;
}

export interface VehiculoDto {
  id: number;
  nombre: string;
}

export interface TargetDto {
  id: number;
  nombre: string;
}

export interface MedicionDto {
  id: number;
  nombre: string;
}

export interface CargaGlobalFiltrosDto {
  paises: PaisDto[];
  formatos: FormatoDto[];
  versionesTV: VersionTVDto[];
  marcas: MarcaDto[];
  productos: ProductoDto[];
  ciudades: CiudadDto[];
  vehiculos: VehiculoDto[];
  targets: TargetDto[];
  mediciones: MedicionDto[];
}

export interface CargaGlobalLogDto {
  id?: number | null;
  estatus: string;
  mensaje: string;
  fechaProceso?: string | null;
}

export interface CargaGlobalUploadResultDto {
  nombreArchivo: string;
  rutaTemporal: string;
  tamanoBytes: number;
  fechaCargaUtc: string;
}

export interface CargaGlobalProcessRequestDto {
  paisId: number;
  proceso: string;
}

export interface CargaGlobalProcessResultDto {
  exito: boolean;
  mensaje: string;
  fechaEjecucionUtc: string;
}

export interface CargaGlobalEstadoCatalogacionDto {
  paisId: number;
  catalogo: string;
  completado: boolean;
  fechaActualizacion?: string | null;
}

@Injectable({ providedIn: 'root' })
export class CargaGlobalService {
  private readonly base = `${environment.apiUrl}/api/information-load/carga-global`;

  constructor(private readonly http: HttpClient) {}

  getFiltros(paisId?: number | null): Observable<ApiResponse<CargaGlobalFiltrosDto>> {
    let params = new HttpParams();
    if (paisId != null) params = params.set('paisId', String(paisId));
    return this.http.get<ApiResponse<CargaGlobalFiltrosDto>>(`${this.base}/filtros`, { params });
  }

  getUltimoLog(paisId: number): Observable<ApiResponse<CargaGlobalLogDto>> {
    const params = new HttpParams().set('paisId', String(paisId));
    return this.http.get<ApiResponse<CargaGlobalLogDto>>(`${this.base}/log`, { params });
  }

  getEstadoCatalogacion(paisId: number): Observable<ApiResponse<CargaGlobalEstadoCatalogacionDto[]>> {
    const params = new HttpParams().set('paisId', String(paisId));
    return this.http.get<ApiResponse<CargaGlobalEstadoCatalogacionDto[]>>(`${this.base}/estado-catalogacion`, { params });
  }

  upload(file: File): Observable<ApiResponse<CargaGlobalUploadResultDto>> {
    const form = new FormData();
    form.append('file', file, file.name);
    return this.http.post<ApiResponse<CargaGlobalUploadResultDto>>(`${this.base}/upload`, form);
  }

  ejecutarProceso(request: CargaGlobalProcessRequestDto): Observable<ApiResponse<CargaGlobalProcessResultDto>> {
    return this.http.post<ApiResponse<CargaGlobalProcessResultDto>>(`${this.base}/ejecutar-proceso`, request);
  }

  finalizarCatalogacion(paisId: number): Observable<ApiResponse<unknown>> {
    return this.http.post<ApiResponse<unknown>>(`${this.base}/finalizar-catalogacion/${paisId}`, {});
  }
}
