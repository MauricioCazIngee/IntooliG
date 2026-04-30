import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import * as Highcharts from 'highcharts';
import {
  InversionesFiltros,
  InversionesRequest,
  InversionesResult,
  InversionesService,
  InversionesTablaDinamica
} from './inversiones.service';
import { NotificationService } from '../../../shared/notifications/notification.service';

@Component({
  selector: 'app-inversiones',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inversiones.component.html',
  styleUrls: ['./inversiones.component.css']
})
export class InversionesComponent implements OnInit, OnDestroy {
  @ViewChild('chartBarras') chartBarras?: ElementRef<HTMLElement>;
  @ViewChild('pieCategoria') pieCategoria?: ElementRef<HTMLElement>;
  @ViewChild('pieMarca') pieMarca?: ElementRef<HTMLElement>;
  @ViewChild('pieMedio') pieMedio?: ElementRef<HTMLElement>;

  loadingFiltros = true;
  loadingRun = false;
  errorFiltros: string | null = null;

  filtros: InversionesFiltros | null = null;

  periodo = 1;
  fechaFinal = this.toYmd(new Date());
  paisId: number | null = null;
  regionId: number | null = null;
  ciudadId: number | null = null;
  sectorId: number | null = null;
  buId: number | null = null;
  categoriaId: number | null = null;
  marcaId: number | null = null;
  tipoTarifa = 1;
  vista = 1;
  exchangeRate = 1;

  ciudades: { id: number; regionId: number; nombre: string }[] = [];

  resultados: InversionesResult[] = [];
  pivotPeriodos: string[] = [];
  pivotMarcas: string[] = [];
  pivotMap: Map<string, Map<string, number | null>> = new Map();

  datosBarras: InversionesTablaDinamica | null = null;
  datosPieCat: InversionesTablaDinamica | null = null;
  datosPieMarca: InversionesTablaDinamica | null = null;
  datosPieMedio: InversionesTablaDinamica | null = null;

  private charts: Highcharts.Chart[] = [];

  constructor(
    private readonly svc: InversionesService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.cargarFiltros();
  }

  ngOnDestroy(): void {
    this.destroyCharts();
  }

  get regionesFiltradas() {
    if (!this.filtros || this.paisId == null) return this.filtros?.regiones ?? [];
    return this.filtros.regiones.filter((r) => r.paisId === this.paisId);
  }

  get busFiltradas() {
    if (!this.filtros || this.sectorId == null) return this.filtros?.bus ?? [];
    return this.filtros.bus.filter((b) => b.sectorId === this.sectorId);
  }

  onPaisChange(): void {
    this.regionId = null;
    this.ciudadId = null;
    this.ciudades = [];
  }

  onSectorChange(): void {
    this.buId = null;
  }

  onRegionChange(): void {
    this.ciudadId = null;
    this.ciudades = [];
    if (this.regionId == null) return;
    this.svc.getCiudadesPorRegion(this.regionId).subscribe({
      next: (res) => {
        if (res.success && res.data) this.ciudades = res.data;
        else this.notifications.warning(res.message || 'No se pudieron cargar las ciudades.');
      },
      error: () => this.notifications.error('Error al cargar ciudades.')
    });
  }

  run(): void {
    if (!this.filtros) return;
    const req = this.buildRequest();
    this.loadingRun = true;
    this.destroyCharts();
    forkJoin({
      main: this.svc.consultar(req),
      barras: this.svc.getBarras(req),
      pieCat: this.svc.getPieCategoria(req),
      pieMarca: this.svc.getPieMarca(req),
      pieMedio: this.svc.getPieMedio(req)
    }).subscribe({
      next: ({ main, barras, pieCat, pieMarca, pieMedio }) => {
        this.loadingRun = false;
        if (!main.success || !main.data) {
          this.notifications.error(main.message || 'Error en consulta principal.');
          return;
        }
        this.resultados = main.data;
        this.rebuildPivot(main.data);
        this.datosBarras = barras.success && barras.data ? barras.data : null;
        this.datosPieCat = pieCat.success && pieCat.data ? pieCat.data : null;
        this.datosPieMarca = pieMarca.success && pieMarca.data ? pieMarca.data : null;
        this.datosPieMedio = pieMedio.success && pieMedio.data ? pieMedio.data : null;
        if (!barras.success) this.notifications.warning(barras.message || 'Gráfico de barras no disponible.');
        if (!pieCat.success) this.notifications.warning(pieCat.message || 'Gráfico pie categoría no disponible.');
        if (!pieMarca.success) this.notifications.warning(pieMarca.message || 'Gráfico pie marca no disponible.');
        if (!pieMedio.success) this.notifications.warning(pieMedio.message || 'Gráfico pie medio no disponible.');
        setTimeout(() => this.renderCharts(), 0);
        this.notifications.success('Consulta ejecutada.');
      },
      error: (e: { error?: { message?: string } }) => {
        this.loadingRun = false;
        this.notifications.error(e?.error?.message || 'Error al ejecutar la consulta.');
      }
    });
  }

  private cargarFiltros(): void {
    this.loadingFiltros = true;
    this.svc.getFiltros().subscribe({
      next: (res) => {
        this.loadingFiltros = false;
        if (!res.success || !res.data) {
          this.errorFiltros = res.message || 'No se pudieron cargar los filtros.';
          this.notifications.error(this.errorFiltros);
          return;
        }
        this.filtros = res.data;
        if (res.data.paises.length) this.paisId = res.data.paises[0].id;
        if (res.data.sectores.length) this.sectorId = res.data.sectores[0].id;
      },
      error: () => {
        this.loadingFiltros = false;
        this.errorFiltros = 'Error de red al cargar filtros.';
        this.notifications.error(this.errorFiltros);
      }
    });
  }

  private buildRequest(): InversionesRequest {
    return {
      periodo: this.periodo,
      fechaFinal: this.fechaFinal,
      paisId: this.paisId,
      categoriaId: this.categoriaId,
      tipoTarifa: this.tipoTarifa,
      vista: this.vista,
      regionId: this.regionId,
      ciudadId: this.ciudadId,
      exchangeRate: this.exchangeRate,
      marcaId: this.marcaId,
      sectorId: this.sectorId,
      buId: this.buId
    };
  }

  private rebuildPivot(rows: InversionesResult[]): void {
    const periodos = [...new Set(rows.map((r) => r.mesAnio).filter((x) => x))].sort();
    const marcas = [...new Set(rows.map((r) => r.marca).filter((x) => x))].sort();
    const map = new Map<string, Map<string, number | null>>();
    for (const m of marcas) {
      const inner = new Map<string, number | null>();
      for (const p of periodos) inner.set(p, null);
      map.set(m, inner);
    }
    for (const r of rows) {
      if (!map.has(r.marca)) {
        const inner = new Map<string, number | null>();
        for (const p of periodos) inner.set(p, null);
        map.set(r.marca, inner);
      }
      map.get(r.marca)!.set(r.mesAnio, r.total);
    }
    this.pivotMap = map;
    this.pivotPeriodos = periodos;
    this.pivotMarcas = marcas;
  }

  formatNum(v: number | null | undefined): string {
    if (v === null || v === undefined) return '—';
    return new Intl.NumberFormat('es-MX', { maximumFractionDigits: 2 }).format(v);
  }

  cell(marca: string, periodo: string): number | null {
    return this.pivotMap.get(marca)?.get(periodo) ?? null;
  }

  private toYmd(d: Date): string {
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
  }

  private renderCharts(): void {
    this.destroyCharts();
    if (this.datosBarras?.filas?.length && this.chartBarras) {
      this.renderColumnOrBar(this.datosBarras, this.chartBarras.nativeElement, 'Inversión (barras)');
    }
    if (this.datosPieCat?.filas?.length && this.pieCategoria) {
      this.renderPie(this.datosPieCat, this.pieCategoria.nativeElement, 'Categoría');
    }
    if (this.datosPieMarca?.filas?.length && this.pieMarca) {
      this.renderPie(this.datosPieMarca, this.pieMarca.nativeElement, 'Marca');
    }
    if (this.datosPieMedio?.filas?.length && this.pieMedio) {
      this.renderPie(this.datosPieMedio, this.pieMedio.nativeElement, 'Medio');
    }
  }

  private renderPie(tab: InversionesTablaDinamica, el: HTMLElement, title: string): void {
    const c = tab.columnas;
    if (c.length < 2) return;
    const nameK = c[0];
    const valK = c.find((x, i) => i > 0 && this.isNumericColumn(x, tab)) ?? c[1];
    const data = tab.filas.map((row, i) => ({
      name: String((row as Record<string, unknown>)[nameK] ?? `Item ${i + 1}`),
      y: this.toNumber((row as Record<string, unknown>)[valK])
    }));
    const ch = Highcharts.chart(el, {
      chart: { type: 'pie', backgroundColor: 'transparent' },
      title: { text: title },
      credits: { enabled: false },
      tooltip: { pointFormat: '{point.name}: <b>{point.y:,.2f}</b>' },
      series: [{ type: 'pie', name: title, data }]
    });
    this.charts.push(ch);
  }

  private renderColumnOrBar(tab: InversionesTablaDinamica, el: HTMLElement, title: string): void {
    const c = tab.columnas;
    if (c.length < 2) return;
    const catKey = c[0];
    const valueKeys = c.slice(1);
    const categories = tab.filas.map((row) => String((row as Record<string, unknown>)[catKey] ?? ''));
    const series: Highcharts.SeriesOptionsType[] = valueKeys.map((vk) => ({
      type: 'column',
      name: vk,
      data: tab.filas.map((row) => this.toNumber((row as Record<string, unknown>)[vk]))
    }));
    const ch = Highcharts.chart(el, {
      chart: { type: 'column', backgroundColor: 'transparent' },
      title: { text: title },
      credits: { enabled: false },
      xAxis: { categories, crosshair: true },
      yAxis: { min: 0, title: { text: null } },
      series
    });
    this.charts.push(ch);
  }

  private isNumericColumn(name: string, tab: InversionesTablaDinamica): boolean {
    if (!tab.filas[0]) return false;
    const v = (tab.filas[0] as Record<string, unknown>)[name];
    return typeof v === 'number' || (typeof v === 'string' && !isNaN(Number(v)));
  }

  private toNumber(v: unknown): number {
    if (v === null || v === undefined) return 0;
    if (typeof v === 'number' && !isNaN(v)) return v;
    const n = Number(v);
    return isNaN(n) ? 0 : n;
  }

  private destroyCharts(): void {
    for (const c of this.charts) {
      try {
        c.destroy();
      } catch {
        // ignore
      }
    }
    this.charts = [];
  }
}
