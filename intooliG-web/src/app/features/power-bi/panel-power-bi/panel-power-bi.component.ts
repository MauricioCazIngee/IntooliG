import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { AfterViewInit, Component, DestroyRef, ElementRef, OnDestroy, ViewChild, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Data, ParamMap, Router, RouterModule } from '@angular/router';
import { combineLatest } from 'rxjs';
import { factories, models, service } from 'powerbi-client';
import { PowerBiEmbedData, PowerBiService } from '../power-bi.service';

type PanelMode = 'report' | 'dashboard' | 'tile';

function httpErrorToMessage(err: unknown): string {
  if (err instanceof HttpErrorResponse) {
    const b = err.error;
    if (b && typeof b === 'object' && 'message' in b) {
      const m = (b as { message?: string }).message;
      if (m) {
        return m;
      }
    }
    if (typeof b === 'string' && b.length > 0) {
      return b;
    }
    const base = `HTTP ${err.status} ${(err.statusText || '').trim()}${err.url ? ` — ${err.url}` : ''}`.trim();
    if (err.status === 404) {
      return (
        base +
        ' (El backend no expone esta ruta: recompila y reinicia IntooliG.Api, o abre ' +
        '/swagger y comprueba GET /api/power-bi/embed-dashboard o embed-tile.)'
      );
    }
    if (err.status === 0) {
      return 'No hay respuesta del servidor (compruebe que la API esté en marcha y la URL en environment.apiUrl).';
    }
    return err.message && err.message.length > 0 ? err.message : base;
  }
  return (err as Error)?.message || 'Error al conectar con la API.';
}

@Component({
  selector: 'app-panel-power-bi',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './panel-power-bi.component.html',
  styleUrl: './panel-power-bi.component.css'
})
export class PanelPowerBiComponent implements AfterViewInit, OnDestroy {
  @ViewChild('pbiContainer') pbiContainer?: ElementRef<HTMLDivElement>;

  private readonly destroyRef = inject(DestroyRef);

  title = 'Panel Power BI';
  panelMode: PanelMode = 'report';
  private routeMode: PanelMode = 'report';
  private queryRdl = false;

  loading = true;
  error = '';

  private pbi: service.Service | null = null;
  private embedData: PowerBiEmbedData | null = null;

  constructor(
    private readonly powerBi: PowerBiService,
    private readonly route: ActivatedRoute,
    private readonly router: Router
  ) {}

  ngAfterViewInit(): void {
    combineLatest([this.route.data, this.route.queryParamMap])
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ([data, p]) => {
          this.applyRouteData(data, p);
        }
      });
  }

  ngOnDestroy(): void {
    this.teardownEmbed();
  }

  private getModeFromUrl(): PanelMode {
    const path = (this.router.url || '').split('?')[0] || '';
    if (path === '/power-bi/tile' || path.endsWith('/power-bi/tile')) {
      return 'tile';
    }
    if (path === '/power-bi/dashboard' || path.includes('/power-bi/dashboard')) {
      return 'dashboard';
    }
    if (path.includes('power-bi/panel') || path.includes('/power-bi/panel')) {
      return 'report';
    }
    return 'report';
  }

  private applyRouteData(data: Data, p: ParamMap): void {
    this.teardownEmbed();
    this.pbi = null;
    this.embedData = null;
    this.error = '';
    this.loading = true;
    const fromData = (data as Record<string, unknown>)['powerBiMode'] as PanelMode | undefined;
    this.routeMode = fromData ?? this.getModeFromUrl();
    this.panelMode = this.routeMode;
    this.queryRdl = p.get('rdl') === 'true' || p.get('rdl') === '1';
    this.title = this.titlesByMode(this.routeMode);
    this.load();
  }

  private titlesByMode(m: PanelMode): string {
    switch (m) {
      case 'dashboard':
        return 'Panel Power BI — Dashboard';
      case 'tile':
        return 'Panel Power BI — Mosaico';
      default:
        return 'Panel Power BI';
    }
  }

  reload(): void {
    this.teardownEmbed();
    this.error = '';
    this.loading = true;
    this.embedData = null;
    this.load();
  }

  private load(): void {
    const onErr = (err: unknown) => {
      this.loading = false;
      this.error = httpErrorToMessage(err);
    };
    if (this.routeMode === 'dashboard') {
      this.panelMode = 'dashboard';
      this.powerBi.getDashboardEmbed().subscribe({
        next: (res) => this.onPayload(res),
        error: onErr
      });
      return;
    }
    if (this.routeMode === 'tile') {
      this.panelMode = 'tile';
      this.powerBi.getTileEmbed().subscribe({
        next: (res) => this.onPayload(res),
        error: onErr
      });
      return;
    }
    this.panelMode = 'report';
    const rdl = this.queryRdl;
    this.powerBi.getEmbed(rdl).subscribe({
      next: (res) => this.onPayload(res),
      error: onErr
    });
  }

  private onPayload(res: { success: boolean; data?: PowerBiEmbedData; message?: string }): void {
    this.loading = false;
    if (!res.success || !res.data) {
      this.error = res.message || 'No se pudo cargar el contenido de Power BI.';
      return;
    }
    this.embedData = res.data;
    setTimeout(() => this.embed(), 0);
  }

  private embed(): void {
    if (!this.embedData || !this.pbiContainer?.nativeElement) {
      return;
    }
    this.teardownEmbed();
    this.pbi = new service.Service(
      factories.hpmFactory,
      factories.wpmpFactory,
      factories.routerFactory
    );
    const d = this.embedData;
    const id = d.id || d.reportId;
    if (d.type === 'report') {
      const config: models.IReportEmbedConfiguration = {
        type: 'report',
        id,
        embedUrl: d.embedUrl,
        accessToken: d.embedToken,
        tokenType: models.TokenType.Embed
      };
      this.pbi.embed(this.pbiContainer.nativeElement, config);
      return;
    }
    if (d.type === 'dashboard') {
      this.pbi.embed(this.pbiContainer.nativeElement, {
        type: 'dashboard',
        id,
        embedUrl: d.embedUrl,
        accessToken: d.embedToken,
        tokenType: models.TokenType.Embed
      });
      return;
    }
    if (!d.dashboardId) {
      this.error = 'Falta dashboardId para mosaico.';
      return;
    }
    const t: models.ITileEmbedConfiguration = {
      type: 'tile',
      id,
      embedUrl: d.embedUrl,
      accessToken: d.embedToken,
      tokenType: models.TokenType.Embed,
      dashboardId: d.dashboardId
    };
    this.pbi.embed(this.pbiContainer.nativeElement, t);
  }

  private teardownEmbed(): void {
    const el = this.pbiContainer?.nativeElement;
    if (el && this.pbi) {
      this.pbi.reset(el);
    }
    this.pbi = null;
  }
}
