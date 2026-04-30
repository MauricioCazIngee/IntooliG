import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../../campanias/confirm-dialog.component';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { DayPartDto, DayPartLookupDto, DayPartRequestDto, DayPartService } from './servicios/daypart.service';
import { DayPartModalComponent } from './modales/daypart-modal.component';

@Component({
  selector: 'app-daypart',
  standalone: true,
  imports: [CommonModule, FormsModule, ConfirmDialogComponent, DayPartModalComponent],
  templateUrl: './daypart.component.html',
  styleUrl: './daypart.component.css'
})
export class DayPartComponent implements OnInit {
  rows: DayPartDto[] = [];
  total = 0;
  page = 1;
  pageSize = 10;
  search = '';
  paisId: number | null = null;
  medioId: number | null = null;
  loading = false;

  lookup: DayPartLookupDto = { paises: [], medios: [], horas: [] };

  showModal = false;
  saving = false;
  editing: DayPartDto | null = null;

  showDelete = false;
  deleting = false;
  deleteId = 0;
  deleteLabel = '';

  constructor(
    readonly auth: AuthService,
    private readonly api: DayPartService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadLookup();
    this.loadRows();
  }

  pageNumbers(total: number, pageSize: number): number[] {
    const pages = Math.max(1, Math.ceil(total / pageSize));
    return Array.from({ length: pages }, (_, i) => i + 1);
  }

  loadLookup(): void {
    this.api.lookup().subscribe({
      next: (res) => {
        if (res.success && res.data) this.lookup = res.data;
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
        medioId: this.medioId,
        page: this.page,
        pageSize: this.pageSize
      })
      .subscribe({
        next: (res) => {
          this.loading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar DayParts.');
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

  openEdit(row: DayPartDto): void {
    this.editing = { ...row };
    this.showModal = true;
  }

  saveRow(body: DayPartRequestDto): void {
    this.saving = true;
    const req = this.editing ? this.api.update(this.editing.id, body) : this.api.create(body);
    req.subscribe({
      next: (res) => {
        this.saving = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editing ? 'DayPart actualizado.' : 'DayPart creado.');
        this.showModal = false;
        this.loadRows();
      },
      error: (err: { error?: { message?: string } }) => {
        this.saving = false;
        this.notifications.error(err?.error?.message || 'Error de validación o red.');
      }
    });
  }

  askDelete(row: DayPartDto): void {
    this.deleteId = row.id;
    this.deleteLabel = row.descripcion;
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
        this.notifications.success('DayPart eliminado.');
        this.loadRows();
      },
      error: () => {
        this.deleting = false;
        this.notifications.error('Error de red.');
      }
    });
  }
}
