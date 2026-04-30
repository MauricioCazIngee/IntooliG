import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/models/api-response.model';

export type PowerBiEmbedKind = 'report' | 'dashboard' | 'tile';

export interface PowerBiEmbedData {
  embedToken: string;
  tokenExpiry: string;
  reportId: string;
  id: string;
  embedUrl: string;
  type: PowerBiEmbedKind;
  dashboardId?: string | null;
  isPaginatedReport?: boolean;
}

@Injectable({ providedIn: 'root' })
export class PowerBiService {
  private readonly base = `${environment.apiUrl}/api/power-bi`;

  constructor(private readonly http: HttpClient) {}

  /** Mismo criterio que <c>GET /api/power-bi/embed</c> (rol PBI, RLS, informe de config). */
  getEmbed(rdl = false): Observable<ApiResponse<PowerBiEmbedData>> {
    const params = new HttpParams();
    if (rdl) {
      return this.http.get<ApiResponse<PowerBiEmbedData>>(`${this.base}/embed`, { params: params.set('rdl', 'true') });
    }
    return this.http.get<ApiResponse<PowerBiEmbedData>>(`${this.base}/embed`);
  }

  getEmbedReport(reportId?: string, rdl = false): Observable<ApiResponse<PowerBiEmbedData>> {
    let p = new HttpParams();
    if (reportId) p = p.set('reportId', reportId);
    if (rdl) p = p.set('rdl', 'true');
    return this.http.get<ApiResponse<PowerBiEmbedData>>(`${this.base}/embed-report`, { params: p });
  }

  getDashboardEmbed(dashboardId?: string): Observable<ApiResponse<PowerBiEmbedData>> {
    const p = dashboardId ? new HttpParams().set('dashboardId', dashboardId) : undefined;
    return this.http.get<ApiResponse<PowerBiEmbedData>>(`${this.base}/embed-dashboard`, p ? { params: p } : {});
  }

  getTileEmbed(dashboardId?: string, tileId?: string): Observable<ApiResponse<PowerBiEmbedData>> {
    let p = new HttpParams();
    if (dashboardId) p = p.set('dashboardId', dashboardId);
    if (tileId) p = p.set('tileId', tileId);
    return this.http.get<ApiResponse<PowerBiEmbedData>>(`${this.base}/embed-tile`, { params: p });
  }
}
