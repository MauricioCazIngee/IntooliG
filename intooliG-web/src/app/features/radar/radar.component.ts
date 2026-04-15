import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { NotificationService } from '../../shared/notifications/notification.service';
import {
  RadarEstado,
  RadarInversionMarcaMedios,
  RadarInversionMarcaMediosVersion,
  RadarMarcaFactoresRiesgo,
  RadarMarcaValoresAgregados,
  RadarMrcaWeeks,
  RadarService,
  RadarTop5
} from './radar.service';

@Component({
  selector: 'app-radar',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './radar.component.html',
  styleUrl: './radar.component.css'
})
export class RadarComponent {
  loading = false;
  activeTab: 'estados' | 'inversion' | 'factores' | 'evolucion' | 'top5' = 'estados';

  estadosRows: RadarEstado[] = [];
  inversionRows: RadarInversionMarcaMedios[] = [];
  inversionVersionRows: RadarInversionMarcaMediosVersion[] = [];
  factoresRows: RadarMarcaFactoresRiesgo[] = [];
  valoresAgregadosRows: RadarMarcaValoresAgregados[] = [];
  evolucionRows: RadarMrcaWeeks[] = [];
  top5Rows: RadarTop5[] = [];

  filtros = {
    anio: new Date().getFullYear(),
    semana: 1,
    categoria: 1,
    marca: '',
    pais: 1,
    medio: '',
    rubro: 1
  };

  constructor(
    private readonly radar: RadarService,
    private readonly notifications: NotificationService
  ) {}

  consultar(): void {
    if (this.filtros.anio <= 0 || this.filtros.semana <= 0 || this.filtros.categoria <= 0 || this.filtros.pais <= 0) {
      this.notifications.warning('Completa filtros válidos para consultar Radar.');
      return;
    }

    this.loading = true;
    forkJoin({
      estados: this.radar.estados(this.filtros),
      inversion: this.radar.inversionMarcaMedios(this.filtros),
      inversionVersion: this.radar.inversionMarcaMediosVersion(this.filtros),
      factores: this.radar.marcaFactoresRiesgo(this.filtros),
      valoresAgregados: this.radar.marcaValoresAgregados(this.filtros),
      evolucion: this.radar.mrcaWeeks(this.filtros),
      top5: this.radar.top5(this.filtros)
    }).subscribe({
      next: (result) => {
        this.loading = false;

        this.estadosRows = result.estados.success && result.estados.data ? result.estados.data : [];
        this.inversionRows = result.inversion.success && result.inversion.data ? result.inversion.data : [];
        this.inversionVersionRows =
          result.inversionVersion.success && result.inversionVersion.data ? result.inversionVersion.data : [];
        this.factoresRows = result.factores.success && result.factores.data ? result.factores.data : [];
        this.valoresAgregadosRows =
          result.valoresAgregados.success && result.valoresAgregados.data ? result.valoresAgregados.data : [];
        this.evolucionRows = result.evolucion.success && result.evolucion.data ? result.evolucion.data : [];
        this.top5Rows = result.top5.success && result.top5.data ? result.top5.data : [];

        const total =
          this.estadosRows.length +
          this.inversionRows.length +
          this.inversionVersionRows.length +
          this.factoresRows.length +
          this.valoresAgregadosRows.length +
          this.evolucionRows.length +
          this.top5Rows.length;

        if (total === 0) {
          this.notifications.info('No hay datos para los filtros seleccionados.');
        } else {
          this.notifications.success(`Radar cargado: ${total} registros en 7 SPs.`);
        }
      },
      error: (err: { error?: { message?: string } }) => {
        this.loading = false;
        this.clearData();
        this.notifications.error(err?.error?.message || 'Error consultando Radar.');
      }
    });
  }

  setTab(tab: 'estados' | 'inversion' | 'factores' | 'evolucion' | 'top5'): void {
    this.activeTab = tab;
  }

  private clearData(): void {
    this.estadosRows = [];
    this.inversionRows = [];
    this.inversionVersionRows = [];
    this.factoresRows = [];
    this.valoresAgregadosRows = [];
    this.evolucionRows = [];
    this.top5Rows = [];
  }
}
