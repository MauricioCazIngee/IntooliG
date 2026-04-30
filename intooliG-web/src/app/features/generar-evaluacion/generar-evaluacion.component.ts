import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../shared/notifications/notification.service';
import {
  AdministracionCampaniaDto,
  CampaniaEvaluacionDto,
  GenerarEvaluacionService
} from './generar-evaluacion.service';

@Component({
  selector: 'app-generar-evaluacion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './generar-evaluacion.component.html',
  styleUrl: './generar-evaluacion.component.css'
})
export class GenerarEvaluacionComponent implements OnInit {
  activeTab: 'campanias' | 'administracion' = 'campanias';

  campanias: CampaniaEvaluacionDto[] = [];
  administraciones: AdministracionCampaniaDto[] = [];

  campPage = 1;
  campPageSize = 10;
  campTotal = 0;
  campTotalPages = 0;

  admPage = 1;
  admPageSize = 10;
  admTotal = 0;
  admTotalPages = 0;

  campAnio: number | null = null;
  campCategoriaId: number | null = null;
  campActivo: boolean | null = null;
  campSearch = '';

  admAnio: number | null = null;
  admSectorId: number | null = null;
  admSearch = '';

  loading = false;
  anios: number[] = [];
  private administracionTabCargada = false;

  constructor(
    private readonly api: GenerarEvaluacionService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    const y = new Date().getFullYear();
    this.anios = [y, y - 1, y - 2, y - 3, y - 4];
    this.campAnio = y;
    this.admAnio = y;
    this.cargarCampanias();
  }

  cambiarTab(tab: 'campanias' | 'administracion'): void {
    this.activeTab = tab;
    if (tab === 'administracion' && !this.administracionTabCargada) {
      this.administracionTabCargada = true;
      this.cargarAdministraciones();
    }
  }

  cargarCampanias(): void {
    this.loading = true;
    this.api
      .getCampaniasEvaluacion({
        anio: this.campAnio,
        categoriaId: this.campCategoriaId,
        activo: this.campActivo,
        search: this.campSearch || null,
        page: this.campPage,
        pageSize: this.campPageSize
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'No fue posible cargar campañas.');
            return;
          }
          this.campanias = res.data.items ?? [];
          this.campTotal = res.data.total;
          this.campTotalPages = res.data.totalPages;
        },
        error: () => {
          this.loading = false;
          this.notifications.error('Error de red al cargar campañas.');
        }
      });
  }

  cargarAdministraciones(): void {
    this.loading = true;
    this.api
      .getAdministracionCampanias({
        anio: this.admAnio,
        sectorId: this.admSectorId,
        search: this.admSearch || null,
        page: this.admPage,
        pageSize: this.admPageSize
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'No fue posible cargar administración.');
            return;
          }
          this.administraciones = res.data.items ?? [];
          this.admTotal = res.data.total;
          this.admTotalPages = res.data.totalPages;
        },
        error: () => {
          this.loading = false;
          this.notifications.error('Error de red al cargar administración de campañas.');
        }
      });
  }

  aplicarFiltrosCampanias(): void {
    this.campPage = 1;
    this.cargarCampanias();
  }

  aplicarFiltrosAdministraciones(): void {
    this.admPage = 1;
    this.cargarAdministraciones();
  }

  campPaginaAnterior(): void {
    if (this.campPage > 1) {
      this.campPage--;
      this.cargarCampanias();
    }
  }

  campPaginaSiguiente(): void {
    if (this.campTotalPages > 0 && this.campPage < this.campTotalPages) {
      this.campPage++;
      this.cargarCampanias();
    }
  }

  campCambiarPagina(n: number): void {
    if (n >= 1 && n <= this.campTotalPages && n !== this.campPage) {
      this.campPage = n;
      this.cargarCampanias();
    }
  }

  admPaginaAnterior(): void {
    if (this.admPage > 1) {
      this.admPage--;
      this.cargarAdministraciones();
    }
  }

  admPaginaSiguiente(): void {
    if (this.admTotalPages > 0 && this.admPage < this.admTotalPages) {
      this.admPage++;
      this.cargarAdministraciones();
    }
  }

  admCambiarPagina(n: number): void {
    if (n >= 1 && n <= this.admTotalPages && n !== this.admPage) {
      this.admPage = n;
      this.cargarAdministraciones();
    }
  }

  campNumerosPagina(): number[] {
    return this.numerosPagina(this.campPage, this.campTotalPages);
  }

  admNumerosPagina(): number[] {
    return this.numerosPagina(this.admPage, this.admTotalPages);
  }

  private numerosPagina(current: number, total: number): number[] {
    if (total <= 0) return [];
    const pages: number[] = [];
    let start = Math.max(1, current - 2);
    let end = Math.min(total, start + 6);
    if (end - start < 6) start = Math.max(1, end - 6);
    for (let i = start; i <= end; i++) pages.push(i);
    return pages;
  }
}
