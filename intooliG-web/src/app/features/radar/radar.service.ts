import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';

export interface RadarEstado {
  fcNombreCategoria: string;
  semana: number;
  fcNombreEstado: string;
  fcNombreCodigoMapa: string;
  fnPrecio: number | null;
}

export interface RadarInversionMarcaMedios {
  fcNombreCategoria: string;
  anio: number;
  semana: number;
  fcNombreMedio: string;
  fnPrecio: number | null;
}

export interface RadarInversionMarcaMediosVersion {
  fcNombreCliente: string;
  fcNombreMedio: string;
  fcNombreVersionFuente: string;
  ins: number;
}

export interface RadarMarcaFactoresRiesgo {
  fiAnio: number;
  fiSemana: number;
  fcNombreMarca: string;
  valor: number | null;
}

export interface RadarMarcaValoresAgregados {
  fiRubroid: number;
  fcNombreConcepto: string;
  fbEstatus: number;
}

export interface RadarMrcaWeeks {
  fechaOrden: string;
  anio: number;
  semana: number;
  fcNombreMarca: string;
  valor: number | null;
}

export interface RadarTop5 {
  fcNombreConcepto: string;
}

export interface RadarBaseParams {
  anio: number;
  semana: number;
  categoria: number;
  marca: string;
  pais: number;
}

export interface RadarInversionVersionParams extends RadarBaseParams {
  medio: string;
}

export interface RadarFactoresParams extends RadarBaseParams {
  rubro: number;
}

@Injectable({ providedIn: 'root' })
export class RadarService {
  private readonly base = `${environment.apiUrl}/api/radar`;

  constructor(private readonly http: HttpClient) {}

  estados(params: RadarBaseParams): Observable<ApiResponse<RadarEstado[]>> {
    const query = new HttpParams()
      .set('anio', params.anio)
      .set('semana', params.semana)
      .set('categoria', params.categoria)
      .set('marca', params.marca || '')
      .set('pais', params.pais);

    return this.http.get<ApiResponse<RadarEstado[]>>(`${this.base}/estados`, { params: query });
  }

  inversionMarcaMedios(params: RadarBaseParams): Observable<ApiResponse<RadarInversionMarcaMedios[]>> {
    const query = new HttpParams()
      .set('anio', params.anio)
      .set('semana', params.semana)
      .set('categoria', params.categoria)
      .set('marca', params.marca || '')
      .set('pais', params.pais);
    return this.http.get<ApiResponse<RadarInversionMarcaMedios[]>>(`${this.base}/inversion-marca-medios`, { params: query });
  }

  inversionMarcaMediosVersion(
    params: RadarInversionVersionParams
  ): Observable<ApiResponse<RadarInversionMarcaMediosVersion[]>> {
    const query = new HttpParams()
      .set('anio', params.anio)
      .set('semana', params.semana)
      .set('categoria', params.categoria)
      .set('marca', params.marca || '')
      .set('medio', params.medio || '')
      .set('pais', params.pais);
    return this.http.get<ApiResponse<RadarInversionMarcaMediosVersion[]>>(
      `${this.base}/inversion-marca-medios-version`,
      { params: query }
    );
  }

  marcaFactoresRiesgo(params: RadarFactoresParams): Observable<ApiResponse<RadarMarcaFactoresRiesgo[]>> {
    const query = new HttpParams()
      .set('anio', params.anio)
      .set('semana', params.semana)
      .set('categoria', params.categoria)
      .set('rubro', params.rubro)
      .set('marca', params.marca || '')
      .set('pais', params.pais);
    return this.http.get<ApiResponse<RadarMarcaFactoresRiesgo[]>>(`${this.base}/marca-factores-riesgo`, { params: query });
  }

  marcaValoresAgregados(params: RadarBaseParams): Observable<ApiResponse<RadarMarcaValoresAgregados[]>> {
    const query = new HttpParams()
      .set('anio', params.anio)
      .set('semana', params.semana)
      .set('categoria', params.categoria)
      .set('marca', params.marca || '')
      .set('pais', params.pais);
    return this.http.get<ApiResponse<RadarMarcaValoresAgregados[]>>(`${this.base}/marca-valores-agregados`, { params: query });
  }

  mrcaWeeks(params: RadarBaseParams): Observable<ApiResponse<RadarMrcaWeeks[]>> {
    const query = new HttpParams()
      .set('anio', params.anio)
      .set('semana', params.semana)
      .set('categoria', params.categoria)
      .set('pais', params.pais);
    return this.http.get<ApiResponse<RadarMrcaWeeks[]>>(`${this.base}/mrca-weeks`, { params: query });
  }

  top5(params: RadarBaseParams): Observable<ApiResponse<RadarTop5[]>> {
    const query = new HttpParams()
      .set('anio', params.anio)
      .set('semana', params.semana)
      .set('categoria', params.categoria);
    return this.http.get<ApiResponse<RadarTop5[]>>(`${this.base}/top5`, { params: query });
  }
}
