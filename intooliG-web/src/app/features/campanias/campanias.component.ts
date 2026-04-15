import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../shared/notifications/notification.service';
import {
  Campania,
  CampaniasService,
  SaveCampaniaRequest
} from './campanias.service';
import { CampaniasModalComponent } from './campanias-modal.component';
import { ConfirmDialogComponent } from './confirm-dialog.component';

type SortColumn =
  | 'codigo'
  | 'nombre'
  | 'descripcion'
  | 'activa'
  | 'fechaInicio'
  | 'fechaFin';
type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-campanias',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CampaniasModalComponent,
    ConfirmDialogComponent
  ],
  templateUrl: './campanias.component.html',
  styleUrl: './campanias.component.css'
})
export class CampaniasComponent implements OnInit {
  allItems: Campania[] = [];
  visibleItems: Campania[] = [];

  loading = false;
  loadError = '';

  query = '';
  page = 1;
  readonly pageSize = 10;

  sortColumn: SortColumn = 'codigo';
  sortDirection: SortDirection = 'asc';

  showModal = false;
  modalSaving = false;
  editingItem: Campania | null = null;
  existingCodigos: string[] = [];

  showDeleteDialog = false;
  deleting = false;
  selectedToDelete: Campania | null = null;

  constructor(
    private readonly api: CampaniasService,
    readonly auth: AuthService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.reload();
  }

  get totalCampanias(): number {
    return this.allItems.length;
  }

  get totalActivas(): number {
    return this.allItems.filter((x) => x.activa).length;
  }

  get totalInactivas(): number {
    return this.totalCampanias - this.totalActivas;
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.visibleItems.length / this.pageSize));
  }

  get pagedItems(): Campania[] {
    const start = (this.page - 1) * this.pageSize;
    return this.visibleItems.slice(start, start + this.pageSize);
  }

  get pageNumbers(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  reload(): void {
    this.loading = true;
    this.loadError = '';
    this.api.list().subscribe({
      next: (res) => {
        this.loading = false;
        if (!res.success || !res.data) {
          this.loadError = res.message || 'No fue posible cargar campañas.';
          this.notifications.error(this.loadError);
          return;
        }
        this.allItems = res.data;
        this.applyFilters();
      },
      error: () => {
        this.loading = false;
        this.loadError = 'Error de red al cargar campañas.';
        this.notifications.error(this.loadError);
      }
    });
  }

  onSearchChange(): void {
    this.page = 1;
    this.applyFilters();
  }

  sortBy(column: SortColumn): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }
    this.applyFilters();
  }

  sortIcon(column: SortColumn): string {
    if (this.sortColumn !== column) {
      return '↕';
    }
    return this.sortDirection === 'asc' ? '↑' : '↓';
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) {
      return;
    }
    this.page = page;
  }

  openNew(): void {
    this.editingItem = null;
    this.existingCodigos = this.allItems.map((x) => x.codigo);
    this.showModal = true;
  }

  openEdit(item: Campania): void {
    this.editingItem = item;
    this.existingCodigos = this.allItems
      .filter((x) => x.id !== item.id)
      .map((x) => x.codigo);
    this.showModal = true;
  }

  closeModal(): void {
    if (!this.modalSaving) {
      this.showModal = false;
    }
  }

  saveFromModal(payload: SaveCampaniaRequest): void {
    this.modalSaving = true;
    const save$ = this.editingItem
      ? this.api.update(this.editingItem.id, payload)
      : this.api.create(payload);

    save$.subscribe({
      next: (res) => {
        this.modalSaving = false;
        if (!res.success) {
          this.notifications.error(res.message || 'No fue posible guardar.');
          return;
        }
        this.showModal = false;
        this.notifications.success(
          this.editingItem ? 'Campaña actualizada correctamente.' : 'Campaña creada correctamente.'
        );
        this.reload();
      },
      error: (err: { error?: { message?: string } }) => {
        this.modalSaving = false;
        this.notifications.error(err?.error?.message || 'Ocurrió un error al guardar la campaña.');
      }
    });
  }

  askDelete(item: Campania): void {
    this.selectedToDelete = item;
    this.showDeleteDialog = true;
  }

  closeDeleteDialog(): void {
    if (!this.deleting) {
      this.showDeleteDialog = false;
      this.selectedToDelete = null;
    }
  }

  confirmDelete(): void {
    if (!this.selectedToDelete) {
      return;
    }

    this.deleting = true;
    this.api.delete(this.selectedToDelete.id).subscribe({
      next: (res) => {
        this.deleting = false;
        this.showDeleteDialog = false;
        if (res.success) {
          this.notifications.success('Campaña eliminada correctamente.');
          this.reload();
          return;
        }
        this.notifications.error(res.message || 'No fue posible eliminar la campaña.');
      },
      error: () => {
        this.deleting = false;
        this.notifications.error('No fue posible eliminar la campaña.');
      }
    });
  }

  private applyFilters(): void {
    const term = this.query.trim().toLowerCase();
    const filtered = this.allItems.filter((item) => {
      if (!term) {
        return true;
      }
      return item.codigo.toLowerCase().includes(term) || item.nombre.toLowerCase().includes(term);
    });

    filtered.sort((a, b) => this.compareForSort(a, b));
    this.visibleItems = filtered;
    this.page = Math.min(this.page, this.totalPages);
  }

  private compareForSort(a: Campania, b: Campania): number {
    const factor = this.sortDirection === 'asc' ? 1 : -1;
    const left = this.sortValue(a, this.sortColumn);
    const right = this.sortValue(b, this.sortColumn);

    if (left < right) {
      return -1 * factor;
    }
    if (left > right) {
      return 1 * factor;
    }
    return 0;
  }

  private sortValue(item: Campania, column: SortColumn): string | number {
    switch (column) {
      case 'codigo':
        return item.codigo.toLowerCase();
      case 'nombre':
        return item.nombre.toLowerCase();
      case 'descripcion':
        return (item.descripcion || '').toLowerCase();
      case 'activa':
        return item.activa ? 1 : 0;
      case 'fechaInicio':
        return item.fechaInicio ? new Date(item.fechaInicio).getTime() : 0;
      case 'fechaFin':
        return item.fechaFin ? new Date(item.fechaFin).getTime() : 0;
      default:
        return '';
    }
  }
}
