import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../../campanias/confirm-dialog.component';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { PaisDto, PaisService } from '../region-ciudad/servicios/pais.service';
import {
  TipoCambioAnioDto,
  TipoCambioDto,
  TipoCambioMesDto,
  TipoCambioRequestDto,
  TipoCambioService
} from './servicios/tipo-cambio.service';
import { TipoCambioModalComponent } from './modales/tipo-cambio-modal.component';

@Component({
  selector: 'app-tipo-cambio',
  standalone: true,
  imports: [CommonModule, FormsModule, ConfirmDialogComponent, TipoCambioModalComponent],
  templateUrl: './tipo-cambio.component.html',
  styleUrl: './tipo-cambio.component.css'
})
export class TipoCambioComponent implements OnInit {
  rows: TipoCambioDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;
  search = '';
  paisId: number | null = null;
  anio: number | null = null;
  loading = false;

  paisesOptions: PaisDto[] = [];
  aniosOptions: number[] = [];
  mesesOptions: TipoCambioMesDto[] = [];

  showModal = false;
  saving = false;
  editing: TipoCambioDto | null = null;

  showDelete = false;
  deleting = false;
  deleteId = 0;
  deleteLabel = '';

  constructor(
    readonly auth: AuthService,
    private readonly api: TipoCambioService,
    private readonly paisApi: PaisService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadLookups();
    this.loadRows();
  }

  pageNumbers(total: number, pageSize: number): number[] {
    const pages = Math.max(1, Math.ceil(total / pageSize));
    return Array.from({ length: pages }, (_, i) => i + 1);
  }

  loadLookups(): void {
    this.paisApi.list({ page: 1, pageSize: 500, soloActivos: true }).subscribe({
      next: (res) => {
        if (res.success && res.data) this.paisesOptions = res.data.items;
      },
      error: () => {}
    });

    this.api.anios().subscribe({
      next: (res) => {
        const yearsFromApi = (res.success && res.data ? res.data : []).map((x: TipoCambioAnioDto) => x.anio);
        const currentYear = new Date().getFullYear();
        const lastTen = Array.from({ length: 10 }, (_, i) => currentYear - i);
        const merged = new Set<number>([...lastTen, ...yearsFromApi]);
        this.aniosOptions = Array.from(merged).sort((a, b) => b - a);
      },
      error: () => {
        const currentYear = new Date().getFullYear();
        this.aniosOptions = Array.from({ length: 10 }, (_, i) => currentYear - i);
      }
    });

    this.api.meses().subscribe({
      next: (res) => {
        if (res.success && res.data) this.mesesOptions = res.data;
      },
      error: () => {}
    });
  }

  loadRows(): void {
    this.loading = true;
    this.api
      .list({
        search: this.search || undefined,
        paisId: this.paisId,
        anio: this.anio,
        page: this.page,
        pageSize: this.pageSize
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar tipos de cambio.');
            return;
          }
          this.rows = res.data.items;
          this.total = res.data.total;
        },
        error: () => {
          this.loading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  openNew(): void {
    this.editing = null;
    this.showModal = true;
  }

  openEdit(row: TipoCambioDto): void {
    this.editing = { ...row };
    this.showModal = true;
  }

  saveRow(body: TipoCambioRequestDto): void {
    this.saving = true;
    const req = this.editing ? this.api.update(this.editing.id, body) : this.api.create(body);

    req.subscribe({
      next: (res) => {
        this.saving = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editing ? 'Registro actualizado.' : 'Registro creado.');
        this.showModal = false;
        this.loadRows();
        this.loadLookups();
      },
      error: (err: { error?: { message?: string } }) => {
        this.saving = false;
        this.notifications.error(err?.error?.message || 'Error de validación o red.');
      }
    });
  }

  askDelete(row: TipoCambioDto): void {
    this.deleteId = row.id;
    this.deleteLabel = `${row.paisNombre} ${row.anio}-${row.mesNombre}`;
    this.showDelete = true;
  }

  closeDelete(): void {
    this.showDelete = false;
  }

  confirmDelete(): void {
    this.deleting = true;
    this.api.delete(this.deleteId).subscribe({
      next: (res) => {
        this.deleting = false;
        this.showDelete = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success('Registro eliminado.');
        this.loadRows();
      },
      error: () => {
        this.deleting = false;
        this.notifications.error('Error de red.');
      }
    });
  }
}
