import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../../campanias/confirm-dialog.component';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { PaisDto, PaisService } from './servicios/pais.service';
import { EstadoDto, EstadoService } from './servicios/estado.service';
import { RegionDto, RegionService } from './servicios/region.service';
import { CiudadDto, CiudadRegionService } from './servicios/ciudad.service';
import { PoblacionDto, PoblacionService } from './servicios/poblacion.service';
import { PaisModalComponent } from './modales/pais-modal.component';
import { EstadoModalComponent } from './modales/estado-modal.component';
import { RegionModalComponent } from './modales/region-modal.component';
import { CiudadRegionModalComponent } from './modales/ciudad-modal.component';
import { PoblacionModalComponent } from './modales/poblacion-modal.component';

type TabId = 'paises' | 'estados' | 'regiones' | 'ciudades' | 'poblacion';
type DeleteKind = 'pais' | 'estado' | 'region' | 'ciudad' | 'poblacion';

@Component({
  selector: 'app-region-ciudad',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ConfirmDialogComponent,
    PaisModalComponent,
    EstadoModalComponent,
    RegionModalComponent,
    CiudadRegionModalComponent,
    PoblacionModalComponent
  ],
  templateUrl: './region-ciudad.component.html',
  styleUrl: './region-ciudad.component.css'
})
export class RegionCiudadComponent implements OnInit {
  tab: TabId = 'paises';

  paisesOptions: { id: number; nombrePais: string }[] = [];
  codigosMapaOptions: { id: number; nombre: string }[] = [];
  aniosOptions: number[] = [];

  /** País */
  paises: PaisDto[] = [];
  paisesTotal = 0;
  paisesPage = 1;
  paisesPageSize = 10;
  paisesSearch = '';
  paisesLoading = false;
  showPaisModal = false;
  savingPais = false;
  editingPais: PaisDto | null = null;

  /** Estado */
  estados: EstadoDto[] = [];
  estadosTotal = 0;
  estadosPage = 1;
  estadosPageSize = 10;
  estadosSearch = '';
  estadosPaisFilter: number | null = null;
  estadosLoading = false;
  showEstadoModal = false;
  savingEstado = false;
  editingEstado: EstadoDto | null = null;
  estadosForModal: { id: number; nombreEstado: string }[] = [];

  /** Región */
  regiones: RegionDto[] = [];
  regionesTotal = 0;
  regionesPage = 1;
  regionesPageSize = 10;
  regionesSearch = '';
  regionesPaisFilter: number | null = null;
  regionesLoading = false;
  showRegionModal = false;
  savingRegion = false;
  editingRegion: RegionDto | null = null;
  regionCiudadOptions: { id: number; nombre: string }[] = [];

  /** Ciudad */
  ciudades: CiudadDto[] = [];
  ciudadesTotal = 0;
  ciudadesPage = 1;
  ciudadesPageSize = 10;
  ciudadesSearch = '';
  ciudadesPaisFilter: number | null = null;
  ciudadesEstadoFilter: number | null = null;
  ciudadesLoading = false;
  showCiudadModal = false;
  savingCiudad = false;
  editingCiudad: CiudadDto | null = null;

  /** Población */
  poblaciones: PoblacionDto[] = [];
  poblacionesTotal = 0;
  poblacionesPage = 1;
  poblacionesPageSize = 10;
  poblacionesSearch = '';
  poblacionesPaisFilter: number | null = null;
  poblacionesEstadoFilter: number | null = null;
  poblacionesAnioFilter: number | null = null;
  poblacionesLoading = false;
  showPoblacionModal = false;
  savingPoblacion = false;
  editingPoblacion: PoblacionDto | null = null;
  poblacionCiudadesOptions: { id: number; nombre: string }[] = [];
  poblacionEstadosOptions: { id: number; nombreEstado: string }[] = [];
  /** Sincroniza selector ciudad en modal población (país actual del modal). */
  poblacionModalPais: number | null = null;

  showDelete = false;
  deleting = false;
  deleteKind: DeleteKind | null = null;
  deleteId = 0;
  deleteLabel = '';

  constructor(
    readonly auth: AuthService,
    private readonly paisApi: PaisService,
    private readonly estadoApi: EstadoService,
    private readonly regionApi: RegionService,
    private readonly ciudadApi: CiudadRegionService,
    private readonly poblacionApi: PoblacionService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadPaisesOptions();
    this.loadCodigosMapa();
    this.loadAnios();
    this.loadPaises();
  }

  loadPaisesOptions(): void {
    this.paisApi.list({ page: 1, pageSize: 500, soloActivos: true }).subscribe({
      next: (res) => {
        if (res.success && res.data?.items) {
          this.paisesOptions = res.data.items.map((x) => ({ id: x.id, nombrePais: x.nombrePais }));
        }
      },
      error: () => {}
    });
  }

  loadCodigosMapa(): void {
    this.estadoApi.codigosMapa().subscribe({
      next: (res) => {
        if (res.success && res.data) this.codigosMapaOptions = res.data;
      },
      error: () => {}
    });
  }

  loadAnios(): void {
    this.poblacionApi.anios().subscribe({
      next: (res) => {
        if (res.success && res.data) this.aniosOptions = res.data;
      },
      error: () => {}
    });
  }

  setTab(t: TabId): void {
    this.tab = t;
    if (t === 'paises') this.loadPaises();
    else if (t === 'estados') this.loadEstados();
    else if (t === 'regiones') this.loadRegiones();
    else if (t === 'ciudades') {
      if (this.ciudadesPaisFilter != null) this.loadEstadosForPais(this.ciudadesPaisFilter, 'ciudad');
      this.loadCiudades();
    } else this.loadPoblaciones();
  }

  pageNumbers(total: number, pageSize: number): number[] {
    const pages = Math.max(1, Math.ceil(total / pageSize));
    return Array.from({ length: pages }, (_, i) => i + 1);
  }

  // --- País ---
  loadPaises(): void {
    this.paisesLoading = true;
    this.paisApi
      .list({
        search: this.paisesSearch || undefined,
        page: this.paisesPage,
        pageSize: this.paisesPageSize
      })
      .subscribe({
        next: (res) => {
          this.paisesLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar países.');
            return;
          }
          this.paises = res.data.items;
          this.paisesTotal = res.data.total;
        },
        error: () => {
          this.paisesLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  openPaisNew(): void {
    this.editingPais = null;
    this.showPaisModal = true;
  }

  openPaisEdit(p: PaisDto): void {
    this.editingPais = { ...p };
    this.showPaisModal = true;
  }

  savePais(body: { nombrePais: string; activo: boolean }): void {
    this.savingPais = true;
    const req = this.editingPais
      ? this.paisApi.update(this.editingPais.id, body)
      : this.paisApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingPais = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingPais ? 'País actualizado.' : 'País creado.');
        this.showPaisModal = false;
        this.loadPaises();
        this.loadPaisesOptions();
      },
      error: () => {
        this.savingPais = false;
        this.notifications.error('Error de red.');
      }
    });
  }

  askDeletePais(p: PaisDto): void {
    this.deleteKind = 'pais';
    this.deleteId = p.id;
    this.deleteLabel = p.nombrePais;
    this.showDelete = true;
  }

  // --- Estado ---
  loadEstados(): void {
    this.estadosLoading = true;
    this.estadoApi
      .list({
        search: this.estadosSearch || undefined,
        paisId: this.estadosPaisFilter,
        page: this.estadosPage,
        pageSize: this.estadosPageSize
      })
      .subscribe({
        next: (res) => {
          this.estadosLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar estados.');
            return;
          }
          this.estados = res.data.items;
          this.estadosTotal = res.data.total;
        },
        error: () => {
          this.estadosLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  openEstadoNew(): void {
    this.editingEstado = null;
    this.showEstadoModal = true;
  }

  openEstadoEdit(e: EstadoDto): void {
    this.editingEstado = { ...e };
    this.showEstadoModal = true;
  }

  saveEstado(body: {
    nombreEstado: string;
    paisId: number;
    codigoMapaId: number | null;
    activo: boolean;
  }): void {
    this.savingEstado = true;
    const req = this.editingEstado
      ? this.estadoApi.update(this.editingEstado.id, body)
      : this.estadoApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingEstado = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingEstado ? 'Estado actualizado.' : 'Estado creado.');
        this.showEstadoModal = false;
        this.loadEstados();
      },
      error: () => {
        this.savingEstado = false;
        this.notifications.error('Error de red.');
      }
    });
  }

  askDeleteEstado(e: EstadoDto): void {
    this.deleteKind = 'estado';
    this.deleteId = e.id;
    this.deleteLabel = e.nombreEstado;
    this.showDelete = true;
  }

  // --- Región ---
  loadRegiones(): void {
    this.regionesLoading = true;
    this.regionApi
      .list({
        search: this.regionesSearch || undefined,
        paisId: this.regionesPaisFilter,
        page: this.regionesPage,
        pageSize: this.regionesPageSize
      })
      .subscribe({
        next: (res) => {
          this.regionesLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar regiones.');
            return;
          }
          this.regiones = res.data.items;
          this.regionesTotal = res.data.total;
        },
        error: () => {
          this.regionesLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  onRegionModalPais(paisId: number | null): void {
    if (paisId == null) {
      this.regionCiudadOptions = [];
      return;
    }
    this.ciudadApi.selector(paisId, null).subscribe({
      next: (res) => {
        if (res.success && res.data) this.regionCiudadOptions = res.data.map((x) => ({ id: x.id, nombre: x.nombre }));
      },
      error: () => {}
    });
  }

  openRegionNew(): void {
    this.editingRegion = null;
    this.regionCiudadOptions = [];
    this.showRegionModal = true;
    const pid = this.paisesOptions[0]?.id ?? null;
    if (pid != null) this.onRegionModalPais(pid);
  }

  openRegionEdit(r: RegionDto): void {
    this.editingRegion = { ...r };
    this.showRegionModal = true;
    this.onRegionModalPais(r.paisId);
  }

  saveRegion(body: {
    paisId: number;
    nombreRegion: string;
    esNacional: boolean;
    activo: boolean;
    ciudadIds: number[];
  }): void {
    this.savingRegion = true;
    const req = this.editingRegion
      ? this.regionApi.update(this.editingRegion.id, body)
      : this.regionApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingRegion = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingRegion ? 'Región actualizada.' : 'Región creada.');
        this.showRegionModal = false;
        this.loadRegiones();
      },
      error: () => {
        this.savingRegion = false;
        this.notifications.error('Error de red.');
      }
    });
  }

  askDeleteRegion(r: RegionDto): void {
    this.deleteKind = 'region';
    this.deleteId = r.id;
    this.deleteLabel = r.nombreRegion;
    this.showDelete = true;
  }

  // --- Ciudad ---
  loadEstadosForPais(paisId: number | null, target: 'ciudad' | 'poblacion'): void {
    if (paisId == null) {
      if (target === 'ciudad') this.estadosForModal = [];
      else this.poblacionEstadosOptions = [];
      return;
    }
    this.estadoApi.list({ paisId, page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (!res.success || !res.data) return;
        const rows = res.data.items.map((x) => ({ id: x.id, nombreEstado: x.nombreEstado }));
        if (target === 'ciudad') this.estadosForModal = rows;
        else this.poblacionEstadosOptions = rows;
      },
      error: () => {}
    });
  }

  loadCiudades(): void {
    this.ciudadesLoading = true;
    this.ciudadApi
      .list({
        search: this.ciudadesSearch || undefined,
        paisId: this.ciudadesPaisFilter,
        estadoId: this.ciudadesEstadoFilter,
        page: this.ciudadesPage,
        pageSize: this.ciudadesPageSize
      })
      .subscribe({
        next: (res) => {
          this.ciudadesLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar ciudades.');
            return;
          }
          this.ciudades = res.data.items;
          this.ciudadesTotal = res.data.total;
        },
        error: () => {
          this.ciudadesLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  onCiudadModalPais(paisId: number | null): void {
    this.loadEstadosForPais(paisId, 'ciudad');
  }

  openCiudadNew(): void {
    this.editingCiudad = null;
    this.estadosForModal = [];
    this.showCiudadModal = true;
  }

  openCiudadEdit(c: CiudadDto): void {
    this.loadEstadosForPais(c.paisId, 'ciudad');
    this.editingCiudad = { ...c };
    this.showCiudadModal = true;
  }

  saveCiudad(body: {
    nombreCiudad: string;
    nombreCorto: string | null;
    estadoId: number;
    ciudadPrincipal: boolean;
    activo: boolean;
    poblacion: number | null;
  }): void {
    this.savingCiudad = true;
    const req = this.editingCiudad
      ? this.ciudadApi.update(this.editingCiudad.id, body)
      : this.ciudadApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingCiudad = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingCiudad ? 'Ciudad actualizada.' : 'Ciudad creada.');
        this.showCiudadModal = false;
        this.loadCiudades();
      },
      error: () => {
        this.savingCiudad = false;
        this.notifications.error('Error de red.');
      }
    });
  }

  askDeleteCiudad(c: CiudadDto): void {
    this.deleteKind = 'ciudad';
    this.deleteId = c.id;
    this.deleteLabel = c.nombreCiudad;
    this.showDelete = true;
  }

  // --- Población ---
  loadPoblaciones(): void {
    this.poblacionesLoading = true;
    this.poblacionApi
      .list({
        search: this.poblacionesSearch || undefined,
        paisId: this.poblacionesPaisFilter,
        estadoId: this.poblacionesEstadoFilter,
        anio: this.poblacionesAnioFilter,
        page: this.poblacionesPage,
        pageSize: this.poblacionesPageSize
      })
      .subscribe({
        next: (res) => {
          this.poblacionesLoading = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error al cargar población.');
            return;
          }
          this.poblaciones = res.data.items;
          this.poblacionesTotal = res.data.total;
        },
        error: () => {
          this.poblacionesLoading = false;
          this.notifications.error('Error de red.');
        }
      });
  }

  onPoblacionModalPais(paisId: number | null): void {
    this.poblacionModalPais = paisId;
    this.poblacionEstadosOptions = [];
    this.poblacionCiudadesOptions = [];
    this.loadEstadosForPais(paisId, 'poblacion');
  }

  onPoblacionModalEstadoId(estadoId: number | null): void {
    if (estadoId == null || this.poblacionModalPais == null) {
      this.poblacionCiudadesOptions = [];
      return;
    }
    this.ciudadApi.selector(this.poblacionModalPais, estadoId).subscribe({
      next: (res) => {
        if (res.success && res.data) this.poblacionCiudadesOptions = res.data.map((x) => ({ id: x.id, nombre: x.nombre }));
      },
      error: () => {}
    });
  }

  openPoblacionNew(): void {
    this.editingPoblacion = null;
    this.poblacionModalPais = null;
    this.poblacionEstadosOptions = [];
    this.poblacionCiudadesOptions = [];
    this.showPoblacionModal = true;
  }

  openPoblacionEdit(p: PoblacionDto): void {
    this.editingPoblacion = { ...p };
    this.poblacionModalPais = p.paisId;
    this.loadEstadosForPais(p.paisId, 'poblacion');
    this.ciudadApi.selector(p.paisId, p.estadoId).subscribe({
      next: (res) => {
        if (res.success && res.data) this.poblacionCiudadesOptions = res.data.map((x) => ({ id: x.id, nombre: x.nombre }));
      },
      error: () => {}
    });
    this.showPoblacionModal = true;
  }

  savePoblacion(body: { ciudadId: number; anio: number; cantidad: number }): void {
    this.savingPoblacion = true;
    const req = this.editingPoblacion
      ? this.poblacionApi.update(this.editingPoblacion.id, body)
      : this.poblacionApi.create(body);
    req.subscribe({
      next: (res) => {
        this.savingPoblacion = false;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(this.editingPoblacion ? 'Población actualizada.' : 'Población creada.');
        this.showPoblacionModal = false;
        this.loadPoblaciones();
        this.loadAnios();
      },
      error: (err: { error?: { message?: string } }) => {
        this.savingPoblacion = false;
        this.notifications.error(err?.error?.message || 'Error de validación o red.');
      }
    });
  }

  askDeletePoblacion(p: PoblacionDto): void {
    this.deleteKind = 'poblacion';
    this.deleteId = p.id;
    this.deleteLabel = `${p.nombreCiudad} ${p.anio}`;
    this.showDelete = true;
  }

  closeDelete(): void {
    this.showDelete = false;
  }

  confirmDelete(): void {
    if (this.deleteKind == null) return;
    this.deleting = true;
    const kind = this.deleteKind;
    const id = this.deleteId;
    const obs =
      kind === 'pais'
        ? this.paisApi.delete(id)
        : kind === 'estado'
          ? this.estadoApi.delete(id)
          : kind === 'region'
            ? this.regionApi.delete(id)
            : kind === 'ciudad'
              ? this.ciudadApi.delete(id)
              : this.poblacionApi.delete(id);
    obs.subscribe({
      next: (res) => {
        this.deleting = false;
        this.showDelete = false;
        this.deleteKind = null;
        if (!res.success) {
          this.notifications.error(res.message || 'Error.');
          return;
        }
        this.notifications.success(kind === 'poblacion' ? 'Registro eliminado.' : 'Registro desactivado.');
        if (kind === 'pais') {
          this.loadPaises();
          this.loadPaisesOptions();
        } else if (kind === 'estado') this.loadEstados();
        else if (kind === 'region') this.loadRegiones();
        else if (kind === 'ciudad') this.loadCiudades();
        else this.loadPoblaciones();
      },
      error: () => {
        this.deleting = false;
        this.notifications.error('Error de red.');
      }
    });
  }
}
