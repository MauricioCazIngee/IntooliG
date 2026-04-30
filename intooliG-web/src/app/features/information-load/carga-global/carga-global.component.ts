import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../shared/notifications/notification.service';
import {
  CargaGlobalEstadoCatalogacionDto,
  CargaGlobalFiltrosDto,
  CargaGlobalLogDto,
  CargaGlobalService
} from './carga-global.service';

@Component({
  selector: 'app-carga-global',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './carga-global.component.html',
  styleUrl: './carga-global.component.css'
})
export class CargaGlobalComponent implements OnInit {
  filtros: CargaGlobalFiltrosDto | null = null;
  estadoCatalogacion: CargaGlobalEstadoCatalogacionDto[] = [];
  ultimoLog: CargaGlobalLogDto | null = null;

  loading = false;
  uploading = false;
  running = false;

  paisId: number | null = null;
  proceso = 'SPWEB_CARGA_GLOBAL_ETL';
  selectedFile: File | null = null;
  ultimoArchivo = '';

  constructor(
    readonly auth: AuthService,
    private readonly api: CargaGlobalService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadFiltros();
  }

  loadFiltros(): void {
    this.loading = true;
    this.api.getFiltros(this.paisId).subscribe({
      next: (res) => {
        this.loading = false;
        if (!res.success || !res.data) {
          this.notifications.error(res.message || 'No fue posible cargar filtros.');
          return;
        }
        this.filtros = res.data;
        if (this.paisId == null && res.data.paises.length) this.paisId = res.data.paises[0].id;
        this.refreshPaneles();
      },
      error: () => {
        this.loading = false;
        this.notifications.error('Error de red al cargar filtros.');
      }
    });
  }

  onPaisChange(): void {
    this.api.getFiltros(this.paisId).subscribe({
      next: (res) => {
        if (res.success && res.data) this.filtros = res.data;
        this.refreshPaneles();
      },
      error: () => this.notifications.error('No fue posible refrescar filtros por país.')
    });
  }

  private refreshPaneles(): void {
    if (!this.paisId) return;

    this.api.getUltimoLog(this.paisId).subscribe({
      next: (res) => {
        if (res.success && res.data) this.ultimoLog = res.data;
      },
      error: () => {
        this.ultimoLog = null;
      }
    });

    this.api.getEstadoCatalogacion(this.paisId).subscribe({
      next: (res) => {
        if (res.success && res.data) this.estadoCatalogacion = res.data;
      },
      error: () => {
        this.estadoCatalogacion = [];
      }
    });
  }

  onFileSelected(ev: Event): void {
    const input = ev.target as HTMLInputElement;
    this.selectedFile = input.files?.[0] ?? null;
  }

  upload(): void {
    if (!this.selectedFile) {
      this.notifications.warning('Seleccione un archivo.');
      return;
    }
    this.uploading = true;
    this.api.upload(this.selectedFile).subscribe({
      next: (res) => {
        this.uploading = false;
        if (!res.success || !res.data) {
          this.notifications.error(res.message || 'No fue posible cargar el archivo.');
          return;
        }
        this.ultimoArchivo = `${res.data.nombreArchivo} (${this.formatBytes(res.data.tamanoBytes)})`;
        this.notifications.success('Archivo cargado correctamente.');
      },
      error: () => {
        this.uploading = false;
        this.notifications.error('Error de red al cargar archivo.');
      }
    });
  }

  runProceso(): void {
    if (!this.paisId) {
      this.notifications.warning('Seleccione un país.');
      return;
    }
    if (!this.proceso.trim()) {
      this.notifications.warning('Indique el nombre del proceso ETL.');
      return;
    }

    this.running = true;
    this.api.ejecutarProceso({ paisId: this.paisId, proceso: this.proceso.trim() }).subscribe({
      next: (res) => {
        this.running = false;
        if (!res.success || !res.data || !res.data.exito) {
          this.notifications.error(res.message || res.data?.mensaje || 'No fue posible ejecutar el proceso.');
          return;
        }
        this.notifications.success(res.data.mensaje || 'Proceso ejecutado.');
        this.refreshPaneles();
      },
      error: () => {
        this.running = false;
        this.notifications.error('Error de red al ejecutar proceso.');
      }
    });
  }

  finalizarCatalogacion(): void {
    if (!this.paisId) {
      this.notifications.warning('Seleccione un país.');
      return;
    }
    this.api.finalizarCatalogacion(this.paisId).subscribe({
      next: (res) => {
        if (!res.success) {
          this.notifications.error(res.message || 'No fue posible finalizar catalogación.');
          return;
        }
        this.notifications.success('Catalogación finalizada para el país.');
        this.refreshPaneles();
      },
      error: () => this.notifications.error('Error de red al finalizar catalogación.')
    });
  }

  private formatBytes(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    const kb = bytes / 1024;
    if (kb < 1024) return `${kb.toFixed(2)} KB`;
    const mb = kb / 1024;
    return `${mb.toFixed(2)} MB`;
  }
}
