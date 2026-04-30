import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../shared/models/api-response.model';

export interface PaisDto {
  id: number;
  nombre: string;
}

export interface RegionDto {
  id: number;
  paisId: number;
  nombre: string;
}

export interface SectorDto {
  id: number;
  nombre: string;
}

export interface BUDto {
  id: number;
  sectorId: number;
  nombre: string;
}

export interface CategoriaDto {
  id: number;
  nombre: string;
}

export interface MarcaFiltroDto {
  id: number;
  nombre: string;
}

export interface CatalogoItemDto {
  id: number;
  nombre: string;
  clave: string;
}

export interface CiudadFiltroDto {
  id: number;
  regionId: number;
  nombre: string;
}

export interface InversionesFiltros {
  paises: PaisDto[];
  regiones: RegionDto[];
  sectores: SectorDto[];
  bus: BUDto[];
  categorias: CategoriaDto[];
  marcas: MarcaFiltroDto[];
  periodos: CatalogoItemDto[];
  tarifas: CatalogoItemDto[];
  vistas: CatalogoItemDto[];
  exchangeRates: CatalogoItemDto[];
}

export interface InversionesRequest {
  periodo: number;
  fechaFinal: string;
  paisId?: number | null;
  categoriaId?: number | null;
  tipoTarifa: number;
  vista: number;
  regionId?: number | null;
  ciudadId?: number | null;
  exchangeRate: number;
  marcaId?: number | null;
  sectorId?: number | null;
  buId?: number | null;
}

export interface InversionesResult {
  marca: string;
  mesAnio: string;
  total: number | null;
}

export interface InversionesTablaDinamica {
  columnas: string[];
  filas: ReadonlyArray<Readonly<Record<string, unknown>>>;
}

@Injectable({ providedIn: 'root' })
export class InversionesService {
  private readonly base = `${environment.apiUrl}/api/inversiones`;

  constructor(private readonly http: HttpClient) {}

  getFiltros(): Observable<ApiResponse<InversionesFiltros>> {
    return this.http.get<ApiResponse<InversionesFiltros>>(`${this.base}/filtros`);
  }

  getCiudadesPorRegion(regionId: number): Observable<ApiResponse<CiudadFiltroDto[]>> {
    const p = new HttpParams().set('regionId', String(regionId));
    return this.http.get<ApiResponse<CiudadFiltroDto[]>>(`${this.base}/filtros/ciudades`, { params: p });
  }

  consultar(request: InversionesRequest): Observable<ApiResponse<InversionesResult[]>> {
    return this.http.post<ApiResponse<InversionesResult[]>>(`${this.base}/consultar`, request);
  }

  getBarras(request: InversionesRequest): Observable<ApiResponse<InversionesTablaDinamica>> {
    return this.http.post<ApiResponse<InversionesTablaDinamica>>(`${this.base}/barras`, request);
  }

  getPieCategoria(request: InversionesRequest): Observable<ApiResponse<InversionesTablaDinamica>> {
    return this.http.post<ApiResponse<InversionesTablaDinamica>>(`${this.base}/pie-categoria`, request);
  }

  getPieMarca(request: InversionesRequest): Observable<ApiResponse<InversionesTablaDinamica>> {
    return this.http.post<ApiResponse<InversionesTablaDinamica>>(`${this.base}/pie-marca`, request);
  }

  getPieMedio(request: InversionesRequest): Observable<ApiResponse<InversionesTablaDinamica>> {
    return this.http.post<ApiResponse<InversionesTablaDinamica>>(`${this.base}/pie-medio`, request);
  }
}
