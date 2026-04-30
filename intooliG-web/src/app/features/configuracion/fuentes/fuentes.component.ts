import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../../campanias/confirm-dialog.component';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { PaisDto, PaisService } from '../region-ciudad/servicios/pais.service';
import { MarcaDto, MarcaService } from '../sector-categoria/servicios/marca.service';
import { BuService, BUDto } from '../sector-categoria/servicios/bu.service';
import { CategoriaDto, CategoriaService } from '../sector-categoria/servicios/categoria.service';
import { FuenteDto, FuenteService } from './servicios/fuente.service';
import { MarcaFuenteDto, MarcaFuenteService } from './servicios/marca-fuente.service';
import { VersionFuenteDto, VersionFuenteService, VersionTVLookupDto } from './servicios/version-fuente.service';
import { FuenteModalComponent } from './modales/fuente-modal.component';
import { MarcaFuenteModalComponent } from './modales/marca-fuente-modal.component';
import { VersionFuenteModalComponent } from './modales/version-fuente-modal.component';

type TabId = 'fuente' | 'marcaFuente' | 'versionFuente';
type DeleteKind = 'fuente' | 'marcaFuente' | 'versionFuente';

@Component({
  selector: 'app-fuentes',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ConfirmDialogComponent,
    FuenteModalComponent,
    MarcaFuenteModalComponent,
    VersionFuenteModalComponent
  ],
  templateUrl: './fuentes.component.html',
  styleUrl: './fuentes.component.css'
})
export class FuentesComponent implements OnInit {
  tab: TabId = 'fuente';

  paisesOptions: PaisDto[] = [];
  fuentesOptions: FuenteDto[] = [];
  marcasOptions: MarcaDto[] = [];
  busOptions: BUDto[] = [];
  categoriasOptions: CategoriaDto[] = [];
  versionTvOptions: VersionTVLookupDto[] = [];

  /** Tab fuentes */
  fuentes: FuenteDto[] = [];
  fuentesTotal = 0;
  fuentesPage = 1;
  fuentesPageSize = 10;
  fuentesSearch = '';
  fuentesPaisFilter: number | null = null;
  fuentesLoading = false;
  showFuenteModal = false;
  savingFuente = false;
  editingFuente: FuenteDto | null = null;

  /** Tab marca fuente */
  marcasFuente: MarcaFuenteDto[] = [];
  mfTotal = 0;
  mfPage = 1;
  mfPageSize = 10;
  mfSearch = '';
  mfFuenteFilter: number | null = null;
  mfLoading = false;
  showMfModal = false;
  savingMf = false;
  editingMf: MarcaFuenteDto | null = null;

  /** Tab version fuente */
  versiones: VersionFuenteDto[] = [];
  vfTotal = 0;
  vfPage = 1;
  vfPageSize = 10;
  vfSearch = '';
  vfFuenteFilter: number | null = null;
  vfCategoriaFilter: number | null = null;
  vfLoading = false;
  showVfModal = false;
  savingVf = false;
  editingVf: VersionFuenteDto | null = null;

  showDelete = false;
  deleting = false;
  deleteKind: DeleteKind | null = null;
  deleteFuenteId = 0;
  deleteMarcaFuenteId = 0;
  deleteVersionId = 0;
  deleteLabel = '';

  constructor(
    readonly auth: AuthService,
    private readonly fuenteApi: FuenteService,
    private readonly mfApi: MarcaFuenteService,
    private readonly vfApi: VersionFuenteService,
    private readonly paisApi: PaisService,
    private readonly marcaApi: MarcaService,
    private readonly buApi: BuService,
    private readonly catApi: CategoriaService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadLookups();
    this.loadFuentes();
  }

  loadLookups(): void {
    this.paisApi.list({ page: 1, pageSize: 500, soloActivos: true }).subscribe({
      next: (res) => {
        if (res.success && res.data) this.paisesOptions = res.data.items;
      },
      error: () => {}
    });
    this.fuenteApi.list({ page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (res.success && res.data) this.fuentesOptions = res.data.items;
      },
      error: () => {}
    });
    this.marcaApi.list({ page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (res.success && res.data) this.marcasOptions = res.data.items;
      },
      error: () => {}
    });
    this.buApi.list({ page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (res.success && res.data) this.busOptions = res.data.items;
      },
      error: () => {}
    });
    this.catApi.list({ page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (res.success && res.data) this.categoriasOptions = res.data.items;
      },
      error: () => {}
    });
    this.vfApi.versionTvLookup().subscribe({
      next: (res) => {
        if (res.success && res.data) this.versionTvOptions = res.data;
      },
      error: () => {}
    });
  }

  setTab(t: TabId): void {
    this.tab = t;
    if (t === 'fuente') this.loadFuentes();
    else if (t === 'marcaFuente') {
      this.loadLookups();
      this.loadMarcasFuente();
    } else {
      this.loadLookups();
      this.loadVersiones();
    }
  }

  pageNumbers(total: number, pageSize: number): number[] {
    const pages = Math.max(1, Math.ceil(total / pageSize));
    return Array.from({ length: pages }, (_, i) => i + 1);
  }

  loadFuentes(): void {
    this.fuentesLoading = true;
    this.fuenteApi
      .list({
        search: this.fuentesSearch || undefined,
        paisId: this.fuentesPaisFilter,
        page: this.fuentesPage,
        pageSize: this.fuentesPageSize
      })
      .subscribe({
        next: (res) => {
          this.fuentesLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar fuentes.');
            return;
          }
          this.fuentes = res.data.items;
          this.fuentesTotal = res.data.total;
        },
        error: () => {
          this.fuentesLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  openFuenteNew(): void {
    this.editingFuente = null;
    this.showFuenteModal = true;
  }

  openFuenteEdit(row: FuenteDto): void {
    this.editingFuente = { ...row };
    this.showFuenteModal = true;
  }

  saveFuente(body: { nombreFuente: string; paisId: number; activo: boolean }): void {
    this.savingFuente = true;
    const req = this.editingFuente ? this.fuenteApi.update(this.editingFuente.id, body) : this.fuenteApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingFuente = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingFuente ? 'Fuente actualizada.' : 'Fuente creada.');
        this.showFuenteModal = false;
        this.loadFuentes();
        this.loadLookups();
      },
      error: (err: { error?: { message?: string } }) => {
        this.savingFuente = false;
        this.notifications.error(err?.error?.message || 'Error de validación o red.');
      }
    });
  }

  askDeleteFuente(row: FuenteDto): void {
    this.deleteKind = 'fuente';
    this.deleteFuenteId = row.id;
    this.deleteLabel = row.nombreFuente;
    this.showDelete = true;
  }

  loadMarcasFuente(): void {
    this.mfLoading = true;
    this.mfApi
      .list({
        search: this.mfSearch || undefined,
        fuenteId: this.mfFuenteFilter,
        page: this.mfPage,
        pageSize: this.mfPageSize
      })
      .subscribe({
        next: (res) => {
          this.mfLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar marcas fuente.');
            return;
          }
          this.marcasFuente = res.data.items;
          this.mfTotal = res.data.total;
        },
        error: () => {
          this.mfLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  openMfNew(): void {
    this.editingMf = null;
    this.showMfModal = true;
  }

  openMfEdit(row: MarcaFuenteDto): void {
    this.editingMf = { ...row };
    this.showMfModal = true;
  }

  saveMf(body: { nombreMarcaFuente: string; fuenteId: number; marcaId: number; productoId: number }): void {
    this.savingMf = true;
    const req = this.editingMf ? this.mfApi.update(this.editingMf.id, body) : this.mfApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingMf = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingMf ? 'Marca fuente actualizada.' : 'Marca fuente creada.');
        this.showMfModal = false;
        this.loadMarcasFuente();
      },
      error: (err: { error?: { message?: string } }) => {
        this.savingMf = false;
        this.notifications.error(err?.error?.message || 'Error de validación o red.');
      }
    });
  }

  askDeleteMf(row: MarcaFuenteDto): void {
    this.deleteKind = 'marcaFuente';
    this.deleteMarcaFuenteId = row.id;
    this.deleteLabel = row.nombreMarcaFuente;
    this.showDelete = true;
  }

  loadVersiones(): void {
    this.vfLoading = true;
    this.vfApi
      .list({
        search: this.vfSearch || undefined,
        fuenteId: this.vfFuenteFilter,
        categoriaId: this.vfCategoriaFilter,
        page: this.vfPage,
        pageSize: this.vfPageSize
      })
      .subscribe({
        next: (res) => {
          this.vfLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar versiones.');
            return;
          }
          this.versiones = res.data.items;
          this.vfTotal = res.data.total;
        },
        error: () => {
          this.vfLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  openVfNew(): void {
    this.editingVf = null;
    this.showVfModal = true;
  }

  openVfEdit(row: VersionFuenteDto): void {
    this.editingVf = { ...row };
    this.showVfModal = true;
  }

  saveVf(body: {
    nombreVersionFuente: string;
    fuenteId: number;
    categoriaId: number;
    buId: number;
    productoId: number;
    versionTVId: number;
    activo: boolean;
  }): void {
    this.savingVf = true;
    const req = this.editingVf ? this.vfApi.update(this.editingVf.id, body) : this.vfApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingVf = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingVf ? 'Versión fuente actualizada.' : 'Versión fuente creada.');
        this.showVfModal = false;
        this.loadVersiones();
      },
      error: (err: { error?: { message?: string } }) => {
        this.savingVf = false;
        this.notifications.error(err?.error?.message || 'Error de validación o red.');
      }
    });
  }

  askDeleteVf(row: VersionFuenteDto): void {
    this.deleteKind = 'versionFuente';
    this.deleteVersionId = row.id;
    this.deleteLabel = row.nombreVersionFuente;
    this.showDelete = true;
  }

  closeDelete(): void {
    this.showDelete = false;
  }

  confirmDelete(): void {
    this.deleting = true;
    if (this.deleteKind === 'fuente') {
      this.fuenteApi.delete(this.deleteFuenteId).subscribe({
        next: (res) => {
          this.finishDelete();
          if (!res.success) {
            this.notifications.error(res.message || 'Error.');
            return;
          }
          this.notifications.success('Fuente desactivada.');
          this.loadFuentes();
          this.loadLookups();
        },
        error: () => {
          this.finishDelete();
          this.notifications.error('Error de red.');
        }
      });
    } else if (this.deleteKind === 'marcaFuente') {
      this.mfApi.delete(this.deleteMarcaFuenteId).subscribe({
        next: (res) => {
          this.finishDelete();
          if (!res.success) {
            this.notifications.error(res.message || 'Error.');
            return;
          }
          this.notifications.success('Registro eliminado.');
          this.loadMarcasFuente();
        },
        error: () => {
          this.finishDelete();
          this.notifications.error('Error de red.');
        }
      });
    } else if (this.deleteKind === 'versionFuente') {
      this.vfApi.delete(this.deleteVersionId).subscribe({
        next: (res) => {
          this.finishDelete();
          if (!res.success) {
            this.notifications.error(res.message || 'Error.');
            return;
          }
          this.notifications.success('Versión fuente desactivada.');
          this.loadVersiones();
        },
        error: () => {
          this.finishDelete();
          this.notifications.error('Error de red.');
        }
      });
    }
  }

  private finishDelete(): void {
    this.deleting = false;
    this.showDelete = false;
    this.deleteKind = null;
  }
}
