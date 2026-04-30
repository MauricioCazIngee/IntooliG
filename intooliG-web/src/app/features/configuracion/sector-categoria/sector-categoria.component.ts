import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../../campanias/confirm-dialog.component';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { SectorService, SectorDto } from './servicios/sector.service';
import { BUDto, BuService } from './servicios/bu.service';
import { CategoriaDto, CategoriaService } from './servicios/categoria.service';
import { MarcaDetailDto, MarcaDto, MarcaService } from './servicios/marca.service';
import { SectorModalComponent } from './modales/sector-modal.component';
import { BuModalComponent } from './modales/bu-modal.component';
import { CategoriaModalComponent } from './modales/categoria-modal.component';
import { MarcaModalComponent } from './modales/marca-modal.component';

type TabId = 'sectores' | 'bus' | 'categorias' | 'marcas';
type DeleteTarget = { kind: TabId; id: number; label: string };

@Component({
  selector: 'app-sector-categoria',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ConfirmDialogComponent,
    SectorModalComponent,
    BuModalComponent,
    CategoriaModalComponent,
    MarcaModalComponent
  ],
  templateUrl: './sector-categoria.component.html',
  styleUrl: './sector-categoria.component.css'
})
export class SectorCategoriaComponent implements OnInit {
  tab: TabId = 'sectores';

  /** Opciones para filtros y modales */
  sectoresOptions: { id: number; nombreSector: string }[] = [];
  busOptions: { id: number; nombreBU: string }[] = [];

  /** Sectores */
  sectores: SectorDto[] = [];
  sectoresTotal = 0;
  sectoresPage = 1;
  sectoresPageSize = 10;
  sectoresSearch = '';
  sectoresLoading = false;
  sectoresError = '';

  /** BU */
  bus: BUDto[] = [];
  busTotal = 0;
  busPage = 1;
  busPageSize = 10;
  busSearch = '';
  busSectorFilter: number | null = null;
  busLoading = false;
  busError = '';

  /** Categorías */
  categorias: CategoriaDto[] = [];
  catTotal = 0;
  catPage = 1;
  catPageSize = 10;
  catSearch = '';
  catBuFilter: number | null = null;
  catLoading = false;
  catError = '';

  /** Marcas */
  marcas: MarcaDto[] = [];
  marcasTotal = 0;
  marcasPage = 1;
  marcasPageSize = 10;
  marcasSearch = '';
  marcasLoading = false;
  marcasError = '';

  /** Modales */
  showSectorModal = false;
  showBuModal = false;
  showCatModal = false;
  showMarcaModal = false;
  savingSector = false;
  savingBu = false;
  savingCat = false;
  savingMarca = false;
  editingSector: SectorDto | null = null;
  editingBu: BUDto | null = null;
  editingCat: CategoriaDto | null = null;
  editingMarca: MarcaDetailDto | null = null;
  buModalSectorInicial: number | null = null;

  /** Eliminar */
  showDelete = false;
  deleting = false;
  deleteTarget: DeleteTarget | null = null;

  constructor(
    readonly auth: AuthService,
    private readonly sectors: SectorService,
    private readonly busApi: BuService,
    private readonly categoriasApi: CategoriaService,
    private readonly marcasApi: MarcaService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadSectoresOptions();
    this.loadBusOptions();
    this.loadSectores();
  }

  setTab(t: TabId): void {
    this.tab = t;
    if (t === 'sectores') this.loadSectores();
    else if (t === 'bus') this.loadBus();
    else if (t === 'categorias') this.loadCategorias();
    else this.loadMarcas();
  }

  /** Catálogo para combos (primera página amplia) */
  loadSectoresOptions(): void {
    this.sectors.list({ page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (res.success && res.data?.items) {
          this.sectoresOptions = res.data.items.map((x) => ({ id: x.id, nombreSector: x.nombreSector }));
        }
      },
      error: () => {}
    });
  }

  loadBusOptions(): void {
    this.busApi.list({ page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (res.success && res.data?.items) {
          this.busOptions = res.data.items.map((x) => ({ id: x.id, nombreBU: x.nombreBU }));
        }
      },
      error: () => {}
    });
  }

  loadSectores(): void {
    this.sectoresLoading = true;
    this.sectoresError = '';
    this.sectors.list({ search: this.sectoresSearch || undefined, page: this.sectoresPage, pageSize: this.sectoresPageSize }).subscribe({
      next: (res) => {
        this.sectoresLoading = false;
        if (!res.success || !res.data) {
          this.sectoresError = res.message || 'No fue posible cargar sectores.';
          this.notifications.error(this.sectoresError);
          return;
        }
        this.sectores = res.data.items;
        this.sectoresTotal = res.data.total;
      },
      error: () => {
        this.sectoresLoading = false;
        this.sectoresError = 'Error de red.';
      }
    });
  }

  loadBus(): void {
    this.busLoading = true;
    this.busError = '';
    this.busApi
      .list({
        sectorId: this.busSectorFilter,
        search: this.busSearch || undefined,
        page: this.busPage,
        pageSize: this.busPageSize
      })
      .subscribe({
        next: (res) => {
          this.busLoading = false;
          if (!res.success || !res.data) {
            this.busError = res.message || 'No fue posible cargar BU.';
            this.notifications.error(this.busError);
            return;
          }
          this.bus = res.data.items;
          this.busTotal = res.data.total;
        },
        error: () => {
          this.busLoading = false;
          this.busError = 'Error de red.';
        }
      });
  }

  loadCategorias(): void {
    this.catLoading = true;
    this.catError = '';
    this.categoriasApi
      .list({
        buId: this.catBuFilter,
        search: this.catSearch || undefined,
        page: this.catPage,
        pageSize: this.catPageSize
      })
      .subscribe({
        next: (res) => {
          this.catLoading = false;
          if (!res.success || !res.data) {
            this.catError = res.message || 'No fue posible cargar categorías.';
            this.notifications.error(this.catError);
            return;
          }
          this.categorias = res.data.items;
          this.catTotal = res.data.total;
        },
        error: () => {
          this.catLoading = false;
          this.catError = 'Error de red.';
        }
      });
  }

  loadMarcas(): void {
    this.marcasLoading = true;
    this.marcasError = '';
    this.marcasApi
      .list({
        search: this.marcasSearch || undefined,
        page: this.marcasPage,
        pageSize: this.marcasPageSize
      })
      .subscribe({
        next: (res) => {
          this.marcasLoading = false;
          if (!res.success || !res.data) {
            this.marcasError = res.message || 'No fue posible cargar marcas.';
            this.notifications.error(this.marcasError);
            return;
          }
          this.marcas = res.data.items;
          this.marcasTotal = res.data.total;
        },
        error: () => {
          this.marcasLoading = false;
          this.marcasError = 'Error de red.';
        }
      });
  }

  onSearchSectores(): void {
    this.sectoresPage = 1;
    this.loadSectores();
  }

  onSearchBus(): void {
    this.busPage = 1;
    this.loadBus();
  }

  onSearchCat(): void {
    this.catPage = 1;
    this.loadCategorias();
  }

  onSearchMarcas(): void {
    this.marcasPage = 1;
    this.loadMarcas();
  }

  pageNumbers(total: number, pageSize: number): number[] {
    const pages = Math.max(1, Math.ceil(total / pageSize));
    return Array.from({ length: pages }, (_, i) => i + 1);
  }

  goSectoresPage(p: number): void {
    const max = Math.max(1, Math.ceil(this.sectoresTotal / this.sectoresPageSize));
    if (p < 1 || p > max) return;
    this.sectoresPage = p;
    this.loadSectores();
  }

  goBusPage(p: number): void {
    const max = Math.max(1, Math.ceil(this.busTotal / this.busPageSize));
    if (p < 1 || p > max) return;
    this.busPage = p;
    this.loadBus();
  }

  goCatPage(p: number): void {
    const max = Math.max(1, Math.ceil(this.catTotal / this.catPageSize));
    if (p < 1 || p > max) return;
    this.catPage = p;
    this.loadCategorias();
  }

  goMarcasPage(p: number): void {
    const max = Math.max(1, Math.ceil(this.marcasTotal / this.marcasPageSize));
    if (p < 1 || p > max) return;
    this.marcasPage = p;
    this.loadMarcas();
  }

  changeSectoresPageSize(): void {
    this.sectoresPage = 1;
    this.loadSectores();
  }

  changeBusPageSize(): void {
    this.busPage = 1;
    this.loadBus();
  }

  changeCatPageSize(): void {
    this.catPage = 1;
    this.loadCategorias();
  }

  changeMarcasPageSize(): void {
    this.marcasPage = 1;
    this.loadMarcas();
  }

  openSectorNew(): void {
    this.editingSector = null;
    this.showSectorModal = true;
  }

  openSectorEdit(row: SectorDto): void {
    this.editingSector = row;
    this.showSectorModal = true;
  }

  closeSectorModal(): void {
    if (!this.savingSector) this.showSectorModal = false;
  }

  saveSector(payload: { nombreSector: string; activo: boolean }): void {
    this.savingSector = true;
    if (this.editingSector) {
      this.sectors.update(this.editingSector.id, { nombreSector: payload.nombreSector, activo: payload.activo }).subscribe({
        next: (res) => {
          this.savingSector = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al guardar.');
            return;
          }
          this.notifications.success('Sector actualizado.');
          this.showSectorModal = false;
          this.loadSectores();
          this.loadSectoresOptions();
        },
        error: () => {
          this.savingSector = false;
          this.notifications.error('Error de red.');
        }
      });
    } else {
      this.sectors.create({ nombreSector: payload.nombreSector }).subscribe({
        next: (res) => {
          this.savingSector = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al crear.');
            return;
          }
          this.notifications.success('Sector creado.');
          this.showSectorModal = false;
          this.loadSectores();
          this.loadSectoresOptions();
        },
        error: () => {
          this.savingSector = false;
          this.notifications.error('Error de red.');
        }
      });
    }
  }

  openBuNew(): void {
    this.editingBu = null;
    this.buModalSectorInicial = this.busSectorFilter;
    this.showBuModal = true;
  }

  openBuEdit(row: BUDto): void {
    this.editingBu = row;
    this.buModalSectorInicial = null;
    this.showBuModal = true;
  }

  closeBuModal(): void {
    if (!this.savingBu) this.showBuModal = false;
  }

  saveBu(payload: { nombreBU: string; sectorId: number; activo: boolean }): void {
    this.savingBu = true;
    if (this.editingBu) {
      this.busApi.update(this.editingBu.id, payload).subscribe({
        next: (res) => {
          this.savingBu = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al guardar.');
            return;
          }
          this.notifications.success('BU actualizado.');
          this.showBuModal = false;
          this.loadBus();
          this.loadBusOptions();
        },
        error: () => {
          this.savingBu = false;
          this.notifications.error('Error de red.');
        }
      });
    } else {
      this.busApi.create({ nombreBU: payload.nombreBU, sectorId: payload.sectorId }).subscribe({
        next: (res) => {
          this.savingBu = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al crear.');
            return;
          }
          this.notifications.success('BU creado.');
          this.showBuModal = false;
          this.loadBus();
          this.loadBusOptions();
        },
        error: () => {
          this.savingBu = false;
          this.notifications.error('Error de red.');
        }
      });
    }
  }

  openCatNew(): void {
    this.editingCat = null;
    this.showCatModal = true;
  }

  openCatEdit(row: CategoriaDto): void {
    this.editingCat = row;
    this.showCatModal = true;
  }

  closeCatModal(): void {
    if (!this.savingCat) this.showCatModal = false;
  }

  saveCat(payload: { nombreCategoria: string; nombreCorto: string | null; activo: boolean }): void {
    this.savingCat = true;
    if (this.editingCat) {
      this.categoriasApi
        .update(this.editingCat.id, {
          nombreCategoria: payload.nombreCategoria,
          nombreCorto: payload.nombreCorto,
          activo: payload.activo
        })
        .subscribe({
          next: (res) => {
            this.savingCat = false;
            if (!res.success || !res.data) {
              this.notifications.error(res.message || 'Error al guardar.');
              return;
            }
            this.notifications.success('Categoría actualizada.');
            this.showCatModal = false;
            this.loadCategorias();
          },
          error: () => {
            this.savingCat = false;
            this.notifications.error('Error de red.');
          }
        });
    } else {
      this.categoriasApi
        .create({ nombreCategoria: payload.nombreCategoria, nombreCorto: payload.nombreCorto })
        .subscribe({
          next: (res) => {
            this.savingCat = false;
            if (!res.success || !res.data) {
              this.notifications.error(res.message || 'Error al crear.');
              return;
            }
            this.notifications.success('Categoría creada.');
            this.showCatModal = false;
            this.loadCategorias();
          },
          error: () => {
            this.savingCat = false;
            this.notifications.error('Error de red.');
          }
        });
    }
  }

  openMarcaNew(): void {
    this.editingMarca = null;
    this.showMarcaModal = true;
  }

  openMarcaEdit(row: MarcaDto): void {
    this.marcasApi.getById(row.id).subscribe({
      next: (res) => {
        if (!res.success || !res.data) {
          this.notifications.error(res.message || 'No se pudo cargar la marca.');
          return;
        }
        this.editingMarca = res.data;
        this.showMarcaModal = true;
      },
      error: () => this.notifications.error('Error de red.')
    });
  }

  closeMarcaModal(): void {
    if (!this.savingMarca) this.showMarcaModal = false;
  }

  saveMarca(payload: {
    nombreMarca: string;
    activo: boolean;
    logoBase64: string | null;
    productosNombres: string[];
  }): void {
    this.savingMarca = true;
    if (this.editingMarca) {
      this.marcasApi.update(this.editingMarca.id, payload).subscribe({
        next: (res) => {
          this.savingMarca = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al guardar.');
            return;
          }
          this.notifications.success('Marca actualizada.');
          this.showMarcaModal = false;
          this.loadMarcas();
        },
        error: () => {
          this.savingMarca = false;
          this.notifications.error('Error de red.');
        }
      });
    } else {
      this.marcasApi
        .create({
          nombreMarca: payload.nombreMarca,
          activo: payload.activo,
          logoBase64: payload.logoBase64,
          productosNombres: payload.productosNombres
        })
        .subscribe({
          next: (res) => {
            this.savingMarca = false;
            if (!res.success || !res.data) {
              this.notifications.error(res.message || 'Error al crear.');
              return;
            }
            this.notifications.success('Marca creada.');
            this.showMarcaModal = false;
            this.loadMarcas();
          },
          error: () => {
            this.savingMarca = false;
            this.notifications.error('Error de red.');
          }
        });
    }
  }

  askDelete(kind: TabId, id: number, label: string): void {
    this.deleteTarget = { kind, id, label };
    this.showDelete = true;
  }

  closeDelete(): void {
    if (!this.deleting) {
      this.showDelete = false;
      this.deleteTarget = null;
    }
  }

  confirmDelete(): void {
    if (!this.deleteTarget) return;
    this.deleting = true;
    const t = this.deleteTarget;
    const done = () => {
      this.deleting = false;
      this.showDelete = false;
      this.deleteTarget = null;
    };
    if (t.kind === 'sectores') {
      this.sectors.delete(t.id).subscribe({
        next: (res) => {
          done();
          if (!res.success) {
            this.notifications.error(res.message || 'No se pudo eliminar.');
            return;
          }
          this.notifications.success('Sector eliminado.');
          this.loadSectores();
          this.loadSectoresOptions();
        },
        error: () => {
          done();
          this.notifications.error('Error de red.');
        }
      });
    } else if (t.kind === 'bus') {
      this.busApi.delete(t.id).subscribe({
        next: (res) => {
          done();
          if (!res.success) {
            this.notifications.error(res.message || 'No se pudo eliminar.');
            return;
          }
          this.notifications.success('BU eliminado.');
          this.loadBus();
          this.loadBusOptions();
        },
        error: () => {
          done();
          this.notifications.error('Error de red.');
        }
      });
    } else if (t.kind === 'categorias') {
      this.categoriasApi.delete(t.id).subscribe({
        next: (res) => {
          done();
          if (!res.success) {
            this.notifications.error(res.message || 'No se pudo eliminar.');
            return;
          }
          this.notifications.success('Categoría eliminada.');
          this.loadCategorias();
        },
        error: () => {
          done();
          this.notifications.error('Error de red.');
        }
      });
    } else {
      this.marcasApi.delete(t.id).subscribe({
        next: (res) => {
          done();
          if (!res.success) {
            this.notifications.error(res.message || 'No se pudo eliminar.');
            return;
          }
          this.notifications.success('Marca eliminada.');
          this.loadMarcas();
        },
        error: () => {
          done();
          this.notifications.error('Error de red.');
        }
      });
    }
  }
}
