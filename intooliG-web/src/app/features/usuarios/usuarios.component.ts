import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../campanias/confirm-dialog.component';
import { NotificationService } from '../../shared/notifications/notification.service';
import {
  ChangeRolRequest,
  CreateUsuarioRequest,
  UpdateUsuarioRequest,
  Usuario,
  UsuariosService
} from './usuarios.service';
import { UsuariosModalComponent, UsuarioFormPayload } from './usuarios-modal.component';

@Component({
  selector: 'app-usuarios',
  standalone: true,
  imports: [CommonModule, FormsModule, UsuariosModalComponent, ConfirmDialogComponent],
  templateUrl: './usuarios.component.html',
  styleUrl: './usuarios.component.css'
})
export class UsuariosComponent implements OnInit {
  allItems: Usuario[] = [];
  visibleItems: Usuario[] = [];

  query = '';
  page = 1;
  readonly pageSize = 10;
  loading = false;
  loadError = '';

  showModal = false;
  saving = false;
  editingItem: Usuario | null = null;
  existingEmails: string[] = [];

  showDeleteDialog = false;
  deleting = false;
  selectedToDelete: Usuario | null = null;

  showResetDialog = false;
  resetting = false;
  selectedToReset: Usuario | null = null;
  newPassword = '';

  constructor(
    private readonly api: UsuariosService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.reload();
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.visibleItems.length / this.pageSize));
  }

  get pagedItems(): Usuario[] {
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
          this.loadError = res.message || 'No fue posible cargar usuarios.';
          this.notifications.error(this.loadError);
          return;
        }
        this.allItems = res.data;
        this.applyFilter();
      },
      error: () => {
        this.loading = false;
        this.loadError = 'Error de red al cargar usuarios.';
      }
    });
  }

  onSearchChange(): void {
    this.page = 1;
    this.applyFilter();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.page = page;
  }

  openNew(): void {
    this.editingItem = null;
    this.existingEmails = this.allItems.map((x) => x.email);
    this.showModal = true;
  }

  openEdit(item: Usuario): void {
    this.editingItem = item;
    this.existingEmails = this.allItems.filter((x) => x.id !== item.id).map((x) => x.email);
    this.showModal = true;
  }

  closeModal(): void {
    if (!this.saving) this.showModal = false;
  }

  saveFromModal(payload: UsuarioFormPayload): void {
    this.saving = true;
    const requestCreate: CreateUsuarioRequest = {
      email: payload.email,
      nombre: payload.nombre,
      rol: payload.rol,
      password: payload.password || '',
      clienteId: payload.clienteId
    };
    const requestUpdate: UpdateUsuarioRequest = {
      email: payload.email,
      nombre: payload.nombre,
      rol: payload.rol,
      clienteId: payload.clienteId
    };

    const save$ = this.editingItem
      ? this.api.update(this.editingItem.id, requestUpdate)
      : this.api.create(requestCreate);

    save$.subscribe({
      next: (res) => {
        this.saving = false;
        if (!res.success) {
          this.notifications.error(res.message || 'No fue posible guardar el usuario.');
          return;
        }
        this.showModal = false;
        this.notifications.success(this.editingItem ? 'Usuario actualizado.' : 'Usuario creado.');
        this.reload();
      },
      error: (err: { error?: { message?: string } }) => {
        this.saving = false;
        this.notifications.error(err?.error?.message || 'Error al guardar usuario.');
      }
    });
  }

  changeRol(item: Usuario, rol: string): void {
    const body: ChangeRolRequest = { rol };
    this.api.changeRol(item.id, body).subscribe({
      next: (res) => {
        if (res.success) {
          this.notifications.success('Rol actualizado.');
          this.reload();
        } else {
          this.notifications.error(res.message || 'No se pudo actualizar rol.');
        }
      },
      error: () => this.notifications.error('No se pudo actualizar rol.')
    });
  }

  askDelete(item: Usuario): void {
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
    if (!this.selectedToDelete) return;
    this.deleting = true;
    this.api.delete(this.selectedToDelete.id).subscribe({
      next: (res) => {
        this.deleting = false;
        this.showDeleteDialog = false;
        if (res.success) {
          this.notifications.success('Usuario eliminado.');
          this.reload();
        } else {
          this.notifications.error(res.message || 'No se pudo eliminar.');
        }
      },
      error: () => {
        this.deleting = false;
        this.notifications.error('No se pudo eliminar.');
      }
    });
  }

  openResetPassword(item: Usuario): void {
    this.selectedToReset = item;
    this.newPassword = '';
    this.showResetDialog = true;
  }

  closeResetDialog(): void {
    if (!this.resetting) {
      this.showResetDialog = false;
      this.selectedToReset = null;
      this.newPassword = '';
    }
  }

  confirmResetPassword(): void {
    if (!this.selectedToReset) return;
    if (!this.newPassword || this.newPassword.length < 6) {
      this.notifications.warning('La nueva contraseña debe tener al menos 6 caracteres.');
      return;
    }

    this.resetting = true;
    this.api.resetPassword(this.selectedToReset.id, { newPassword: this.newPassword }).subscribe({
      next: (res) => {
        this.resetting = false;
        this.showResetDialog = false;
        if (res.success) {
          this.notifications.success('Contraseña restablecida.');
        } else {
          this.notifications.error(res.message || 'No se pudo restablecer la contraseña.');
        }
      },
      error: () => {
        this.resetting = false;
        this.notifications.error('No se pudo restablecer la contraseña.');
      }
    });
  }

  private applyFilter(): void {
    const term = this.query.trim().toLowerCase();
    this.visibleItems = this.allItems.filter((u) => {
      if (!term) return true;
      return u.email.toLowerCase().includes(term) || u.nombre.toLowerCase().includes(term);
    });
    this.page = Math.min(this.page, this.totalPages);
  }
}
