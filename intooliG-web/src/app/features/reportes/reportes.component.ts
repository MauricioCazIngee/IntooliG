import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
import { jsPDF } from 'jspdf';
import autoTable from 'jspdf-autotable';
import * as XLSX from 'xlsx';
import { NotificationService } from '../../shared/notifications/notification.service';
import { ReportesKpi, ReportesService } from './reportes.service';

Chart.register(...registerables);

@Component({
  selector: 'app-reportes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reportes.component.html',
  styleUrl: './reportes.component.css'
})
export class ReportesComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('campaniasChart') campaniasChartRef?: ElementRef<HTMLCanvasElement>;
  @ViewChild('usuariosChart') usuariosChartRef?: ElementRef<HTMLCanvasElement>;

  kpis: ReportesKpi | null = null;
  loading = false;
  loadError = '';

  private campaniasChart?: Chart;
  private usuariosChart?: Chart;

  constructor(
    private readonly api: ReportesService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.reload();
  }

  ngAfterViewInit(): void {
    if (this.kpis) {
      this.buildCharts();
    }
  }

  ngOnDestroy(): void {
    this.destroyCharts();
  }

  reload(): void {
    this.loading = true;
    this.loadError = '';
    this.api.kpis().subscribe({
      next: (res) => {
        this.loading = false;
        if (!res.success || !res.data) {
          this.loadError = res.message || 'No fue posible cargar KPIs.';
          return;
        }
        this.kpis = res.data;
        this.buildCharts();
      },
      error: () => {
        this.loading = false;
        this.loadError = 'Error de red al cargar reportes.';
      }
    });
  }

  exportExcel(): void {
    if (!this.kpis) return;

    const data = [
      { indicador: 'Total campañas', valor: this.kpis.totalCampanias },
      { indicador: 'Campañas activas', valor: this.kpis.campaniasActivas },
      { indicador: 'Campañas inactivas', valor: this.kpis.campaniasInactivas },
      { indicador: 'Total usuarios', valor: this.kpis.totalUsuarios },
      { indicador: 'Usuarios admin', valor: this.kpis.usuariosAdmin },
      { indicador: 'Usuarios normales', valor: this.kpis.usuariosNormales }
    ];

    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'KPIs');
    XLSX.writeFile(wb, 'reportes-kpis.xlsx');
    this.notifications.success('Reporte Excel exportado.');
  }

  exportPdf(): void {
    if (!this.kpis) return;
    const doc = new jsPDF();
    doc.text('Reporte de KPIs', 14, 14);
    autoTable(doc, {
      startY: 22,
      head: [['Indicador', 'Valor']],
      body: [
        ['Total campañas', String(this.kpis.totalCampanias)],
        ['Campañas activas', String(this.kpis.campaniasActivas)],
        ['Campañas inactivas', String(this.kpis.campaniasInactivas)],
        ['Total usuarios', String(this.kpis.totalUsuarios)],
        ['Usuarios admin', String(this.kpis.usuariosAdmin)],
        ['Usuarios normales', String(this.kpis.usuariosNormales)]
      ]
    });
    doc.save('reportes-kpis.pdf');
    this.notifications.success('Reporte PDF exportado.');
  }

  private buildCharts(): void {
    if (!this.kpis || !this.campaniasChartRef || !this.usuariosChartRef) {
      return;
    }

    this.destroyCharts();

    const campaniasConfig: ChartConfiguration<'pie'> = {
      type: 'pie',
      data: {
        labels: ['Activas', 'Inactivas'],
        datasets: [
          {
            data: [this.kpis.campaniasActivas, this.kpis.campaniasInactivas],
            backgroundColor: ['#5DC1A5', '#6c757d']
          }
        ]
      }
    };

    const usuariosConfig: ChartConfiguration<'bar'> = {
      type: 'bar',
      data: {
        labels: ['Admin', 'Usuario'],
        datasets: [
          {
            label: 'Usuarios',
            data: [this.kpis.usuariosAdmin, this.kpis.usuariosNormales],
            backgroundColor: ['#1872B6', '#5DC1A5']
          }
        ]
      },
      options: {
        scales: { y: { beginAtZero: true, ticks: { precision: 0 } } }
      }
    };

    this.campaniasChart = new Chart(this.campaniasChartRef.nativeElement, campaniasConfig);
    this.usuariosChart = new Chart(this.usuariosChartRef.nativeElement, usuariosConfig);
  }

  private destroyCharts(): void {
    this.campaniasChart?.destroy();
    this.usuariosChart?.destroy();
    this.campaniasChart = undefined;
    this.usuariosChart = undefined;
  }
}
