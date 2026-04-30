import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../../campanias/confirm-dialog.component';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { ClienteLookupDto, ClienteLookupService } from '../rubros-conceptos/servicios/cliente-lookup.service';
import { MedioDto, MedioLookupDto, MedioService } from './servicios/medio.service';
import { MedioClienteDto, MedioClienteService } from './servicios/medio-cliente.service';
import { MedioModalComponent } from './modales/medio-modal.component';
import { MedioClienteModalComponent } from './modales/medio-cliente-modal.component';

type TabId = 'medio' | 'medioCliente';
type DeleteKind = 'medio' | 'medioCliente';

@Component({
  selector: 'app-medios',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ConfirmDialogComponent,
    MedioModalComponent,
    MedioClienteModalComponent
  ],
  templateUrl: './medios.component.html',
  styleUrl: './medios.component.css'
})
export class MediosComponent implements OnInit {
  tab: TabId = 'medio';

  mediosLookup: MedioLookupDto[] = [];
  clientesOptions: ClienteLookupDto[] = [];

  /** Tab medio */
  medios: MedioDto[] = [];
  mediosTotal = 0;
  mediosPage = 1;
  mediosPageSize = 10;
  mediosSearch = '';
  mediosLoading = false;
  showMedioModal = false;
  savingMedio = false;
  editingMedio: MedioDto | null = null;

  /** Tab medio-cliente */
  asignaciones: MedioClienteDto[] = [];
  mcTotal = 0;
  mcPage = 1;
  mcPageSize = 10;
  mcSearch = '';
  mcClienteFilter: number | null = null;
  mcMedioFilter: number | null = null;
  mcLoading = false;
  showMcModal = false;
  savingMc = false;
  editingMc: MedioClienteDto | null = null;

  showDelete = false;
  deleting = false;
  deleteKind: DeleteKind | null = null;
  deleteMedioId = 0;
  deleteClienteId = 0;
  deleteLabel = '';

  constructor(
    readonly auth: AuthService,
    private readonly medioApi: MedioService,
    private readonly mcApi: MedioClienteService,
    private readonly clientesApi: ClienteLookupService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadLookups();
    this.loadMedios();
  }

  loadLookups(): void {
    this.medioApi.lookup().subscribe({
      next: (res) => {
        if (res.success && res.data) this.mediosLookup = res.data;
      },
      error: () => {}
    });
    this.clientesApi.list().subscribe({
      next: (res) => {
        if (res.success && res.data) this.clientesOptions = res.data.filter((c) => c.activo);
      },
      error: () => {}
    });
  }

  setTab(t: TabId): void {
    this.tab = t;
    if (t === 'medio') this.loadMedios();
    else {
      this.loadLookups();
      this.loadMedioCliente();
    }
  }

  pageNumbers(total: number, pageSize: number): number[] {
    const pages = Math.max(1, Math.ceil(total / pageSize));
    return Array.from({ length: pages }, (_, i) => i + 1);
  }

  loadMedios(): void {
    this.mediosLoading = true;
    this.medioApi
      .list({
        search: this.mediosSearch || undefined,
        page: this.mediosPage,
        pageSize: this.mediosPageSize
      })
      .subscribe({
        next: (res) => {
          this.mediosLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar medios.');
            return;
          }
          this.medios = res.data.items;
          this.mediosTotal = res.data.total;
        },
        error: () => {
          this.mediosLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  openMedioNew(): void {
    this.editingMedio = null;
    this.showMedioModal = true;
  }

  openMedioEdit(m: MedioDto): void {
    this.editingMedio = { ...m };
    this.showMedioModal = true;
  }

  saveMedio(body: { nombreMedio: string; nombreMedioGenerico: string; activo: boolean }): void {
    this.savingMedio = true;
    const req = this.editingMedio
      ? this.medioApi.update(this.editingMedio.id, body)
      : this.medioApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingMedio = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingMedio ? 'Medio actualizado.' : 'Medio creado.');
        this.showMedioModal = false;
        this.loadMedios();
        this.loadLookups();
      },
      error: () => {
        this.savingMedio = false;
        this.notifications.error('Error de red.');
      }
    });
  }

  askDeleteMedio(m: MedioDto): void {
    this.deleteKind = 'medio';
    this.deleteMedioId = m.id;
    this.deleteClienteId = 0;
    this.deleteLabel = m.nombreMedio;
    this.showDelete = true;
  }

  loadMedioCliente(): void {
    this.mcLoading = true;
    this.mcApi
      .list({
        search: this.mcSearch || undefined,
        clienteId: this.mcClienteFilter,
        medioId: this.mcMedioFilter,
        page: this.mcPage,
        pageSize: this.mcPageSize
      })
      .subscribe({
        next: (res) => {
          this.mcLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar asignaciones.');
            return;
          }
          this.asignaciones = res.data.items;
          this.mcTotal = res.data.total;
        },
        error: () => {
          this.mcLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  openMcNew(): void {
    this.editingMc = null;
    this.showMcModal = true;
  }

  openMcEdit(row: MedioClienteDto): void {
    this.editingMc = { ...row };
    this.showMcModal = true;
  }

  saveMc(body: { medioId: number; clienteId: number; esNacional: boolean }): void {
    this.savingMc = true;
    if (this.editingMc) {
      this.mcApi.update(body.medioId, body.clienteId, { esNacional: body.esNacional }).subscribe({
        next: (res) => {
          this.savingMc = false;
          if (!res.success) {
            this.notifications.error(res.message || 'Error.');
            return;
          }
          this.notifications.success('Asignación actualizada.');
          this.showMcModal = false;
          this.loadMedioCliente();
        },
        error: () => {
          this.savingMc = false;
          this.notifications.error('Error de red.');
        }
      });
    } else {
      this.mcApi.create(body).subscribe({
        next: (res) => {
          this.savingMc = false;
          if (!res.success) {
            this.notifications.error(res.message || 'Error.');
            return;
          }
          this.notifications.success('Asignación creada.');
          this.showMcModal = false;
          this.loadMedioCliente();
        },
        error: (err: { error?: { message?: string } }) => {
          this.savingMc = false;
          this.notifications.error(err?.error?.message || 'Error de validación o red.');
        }
      });
    }
  }

  askDeleteMc(row: MedioClienteDto): void {
    this.deleteKind = 'medioCliente';
    this.deleteMedioId = row.medioId;
    this.deleteClienteId = row.clienteId;
    this.deleteLabel = `${row.nombreMedio} / ${row.nombreCliente}`;
    this.showDelete = true;
  }

  closeDelete(): void {
    this.showDelete = false;
  }

  confirmDelete(): void {
    this.deleting = true;
    if (this.deleteKind === 'medio') {
      this.medioApi.delete(this.deleteMedioId).subscribe({
        next: (res) => {
          this.deleting = false;
          this.showDelete = false;
          this.deleteKind = null;
          if (!res.success) {
            this.notifications.error(res.message || 'Error.');
            return;
          }
          this.notifications.success('Medio desactivado.');
          this.loadMedios();
          this.loadLookups();
        },
        error: () => {
          this.deleting = false;
          this.notifications.error('Error de red.');
        }
      });
    } else if (this.deleteKind === 'medioCliente') {
      this.mcApi.delete(this.deleteMedioId, this.deleteClienteId).subscribe({
        next: (res) => {
          this.deleting = false;
          this.showDelete = false;
          this.deleteKind = null;
          if (!res.success) {
            this.notifications.error(res.message || 'Error.');
            return;
          }
          this.notifications.success('Asignación eliminada.');
          this.loadMedioCliente();
        },
        error: () => {
          this.deleting = false;
          this.notifications.error('Error de red.');
        }
      });
    }
  }
}
