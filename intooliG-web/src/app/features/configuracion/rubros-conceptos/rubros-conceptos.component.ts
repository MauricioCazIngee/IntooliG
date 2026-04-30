import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmDialogComponent } from '../../campanias/confirm-dialog.component';
import { NotificationService } from '../../../shared/notifications/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { CategoriaService } from '../sector-categoria/servicios/categoria.service';
import { RubroGeneralService, RubroGeneralDto } from './servicios/rubro-general.service';
import { RubroCatalogoService, RubroCombinacionDto } from './servicios/rubro.service';
import { ConceptoCatalogoService, ConceptoDetailDto, ConceptoListItemDto } from './servicios/concepto.service';
import { ClienteLookupDto, ClienteLookupService } from './servicios/cliente-lookup.service';
import { RubroGeneralModalComponent } from './modales/rubro-general-modal.component';
import { RubroComboModalComponent } from './modales/rubro-combo-modal.component';
import { ConceptoCatalogoModalComponent } from './modales/concepto-modal.component';

type TabId = 'rubros' | 'conceptos';
type DeleteKind = 'rubroCombo' | 'concepto';

@Component({
  selector: 'app-rubros-conceptos',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ConfirmDialogComponent,
    RubroGeneralModalComponent,
    RubroComboModalComponent,
    ConceptoCatalogoModalComponent
  ],
  templateUrl: './rubros-conceptos.component.html',
  styleUrl: './rubros-conceptos.component.css'
})
export class RubrosConceptosComponent implements OnInit {
  tab: TabId = 'rubros';

  /** Catálogo completo (nombres de rubro) para tablas y combos */
  rubrosGeneralesFull: RubroGeneralDto[] = [];
  categoriasOptions: { id: number; nombreCategoria: string }[] = [];
  clientesOptions: ClienteLookupDto[] = [];

  /** Tab rubros (combinaciones) */
  rubros: RubroCombinacionDto[] = [];
  rubrosTotal = 0;
  rubrosPage = 1;
  rubrosPageSize = 10;
  rubrosSearch = '';
  rubrosCatFilter: number | null = null;
  rubrosActivoFilter: boolean | null = null;
  rubrosLoading = false;
  rubrosError = '';

  /** Tab conceptos */
  conceptos: ConceptoListItemDto[] = [];
  conceptosTotal = 0;
  conceptosPage = 1;
  conceptosPageSize = 10;
  conceptosSearch = '';
  conceptosCatFilter: number | null = null;
  conceptosRubroFilter: number | null = null;
  conceptosActivoFilter: boolean | null = null;
  conceptosTopFilter: boolean | null = null;
  conceptosLoading = false;
  conceptosError = '';

  /** Modales */
  showRgModal = false;
  showComboModal = false;
  showConceptoModal = false;
  savingRg = false;
  savingCombo = false;
  savingConcepto = false;
  editingRg: RubroGeneralDto | null = null;
  editingCombo: RubroCombinacionDto | null = null;
  editingConcepto: ConceptoDetailDto | null = null;

  showDelete = false;
  deleting = false;
  deleteKind: DeleteKind | null = null;
  deleteId = 0;
  deleteLabel = '';

  constructor(
    readonly auth: AuthService,
    private readonly rgApi: RubroGeneralService,
    private readonly rubroApi: RubroCatalogoService,
    private readonly conceptoApi: ConceptoCatalogoService,
    private readonly categoriasApi: CategoriaService,
    private readonly clientesApi: ClienteLookupService,
    private readonly notifications: NotificationService
  ) {}

  ngOnInit(): void {
    this.loadSelectOptions();
    this.loadRubros();
  }

  loadSelectOptions(): void {
    this.rgApi.list({ page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (res.success && res.data?.items) {
          this.rubrosGeneralesFull = res.data.items;
        }
      },
      error: () => {}
    });
    this.categoriasApi.list({ page: 1, pageSize: 500 }).subscribe({
      next: (res) => {
        if (res.success && res.data?.items) {
          this.categoriasOptions = res.data.items.map((x) => ({ id: x.id, nombreCategoria: x.nombreCategoria }));
        }
      },
      error: () => {}
    });
    this.clientesApi.list().subscribe({
      next: (res) => {
        if (res.success && res.data) this.clientesOptions = res.data;
      },
      error: () => {}
    });
  }

  setTab(t: TabId): void {
    this.tab = t;
    if (t === 'rubros') this.loadRubros();
    else this.loadConceptos();
  }

  pageNumbers(total: number, pageSize: number): number[] {
    const pages = Math.max(1, Math.ceil(total / pageSize));
    return Array.from({ length: pages }, (_, i) => i + 1);
  }

  loadRubros(): void {
    this.rubrosLoading = true;
    this.rubrosError = '';
    this.rubroApi
      .list({
        categoriaId: this.rubrosCatFilter,
        activo: this.rubrosActivoFilter,
        search: this.rubrosSearch || undefined,
        page: this.rubrosPage,
        pageSize: this.rubrosPageSize
      })
      .subscribe({
        next: (res) => {
          this.rubrosLoading = false;
          if (!res.success || !res.data) {
            this.rubrosError = res.message || 'Error al cargar rubros.';
            this.notifications.error(this.rubrosError);
            return;
          }
          this.rubros = res.data.items;
          this.rubrosTotal = res.data.total;
        },
        error: () => {
          this.rubrosLoading = false;
          this.rubrosError = 'Error de red.';
        }
      });
  }

  loadConceptos(): void {
    this.conceptosLoading = true;
    this.conceptosError = '';
    this.conceptoApi
      .list({
        categoriaId: this.conceptosCatFilter,
        rubroGeneralId: this.conceptosRubroFilter,
        activo: this.conceptosActivoFilter,
        top: this.conceptosTopFilter,
        search: this.conceptosSearch || undefined,
        page: this.conceptosPage,
        pageSize: this.conceptosPageSize
      })
      .subscribe({
        next: (res) => {
          this.conceptosLoading = false;
          if (!res.success || !res.data) {
            this.conceptosError = res.message || 'Error al cargar conceptos.';
            this.notifications.error(this.conceptosError);
            return;
          }
          this.conceptos = res.data.items;
          this.conceptosTotal = res.data.total;
        },
        error: () => {
          this.conceptosLoading = false;
          this.conceptosError = 'Error de red.';
        }
      });
  }

  onSearchRubros(): void {
    this.rubrosPage = 1;
    this.loadRubros();
  }

  onSearchConceptos(): void {
    this.conceptosPage = 1;
    this.loadConceptos();
  }

  goRubrosPage(p: number): void {
    const max = Math.max(1, Math.ceil(this.rubrosTotal / this.rubrosPageSize));
    if (p < 1 || p > max) return;
    this.rubrosPage = p;
    this.loadRubros();
  }

  goConceptosPage(p: number): void {
    const max = Math.max(1, Math.ceil(this.conceptosTotal / this.conceptosPageSize));
    if (p < 1 || p > max) return;
    this.conceptosPage = p;
    this.loadConceptos();
  }

  changeRubrosPageSize(): void {
    this.rubrosPage = 1;
    this.loadRubros();
  }

  changeConceptosPageSize(): void {
    this.conceptosPage = 1;
    this.loadConceptos();
  }

  openRgNew(): void {
    this.editingRg = null;
    this.showRgModal = true;
  }

  openRgEdit(row: RubroGeneralDto): void {
    this.editingRg = { ...row };
    this.showRgModal = true;
  }

  closeRgModal(): void {
    if (!this.savingRg) this.showRgModal = false;
  }

  saveRg(payload: { nombreRubro: string; activo: boolean }): void {
    this.savingRg = true;
    if (this.editingRg) {
      this.rgApi.update(this.editingRg.id, payload).subscribe({
        next: (res) => {
          this.savingRg = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error');
            return;
          }
          this.notifications.success('Rubro (catálogo) actualizado.');
          this.showRgModal = false;
          this.loadSelectOptions();
        },
        error: () => {
          this.savingRg = false;
          this.notifications.error('Error de red.');
        }
      });
    } else {
      this.rgApi.create({ nombreRubro: payload.nombreRubro }).subscribe({
        next: (res) => {
          this.savingRg = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error');
            return;
          }
          this.notifications.success('Rubro (catálogo) creado.');
          this.showRgModal = false;
          this.loadSelectOptions();
        },
        error: () => {
          this.savingRg = false;
          this.notifications.error('Error de red.');
        }
      });
    }
  }

  openComboNew(): void {
    this.editingCombo = null;
    this.showComboModal = true;
  }

  openComboEdit(row: RubroCombinacionDto): void {
    this.editingCombo = row;
    this.showComboModal = true;
  }

  closeComboModal(): void {
    if (!this.savingCombo) this.showComboModal = false;
  }

  saveCombo(payload: {
    rubroGeneralId: number;
    categoriaId: number;
    valorRubro: number;
    activo: boolean;
    clienteId: number | null;
  }): void {
    this.savingCombo = true;
    const body = {
      rubroGeneralId: payload.rubroGeneralId,
      categoriaId: payload.categoriaId,
      valorRubro: payload.valorRubro,
      activo: payload.activo,
      clienteId: payload.clienteId
    };
    if (this.editingCombo) {
      this.rubroApi.update(this.editingCombo.id, body).subscribe({
        next: (res) => {
          this.savingCombo = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error');
            return;
          }
          this.notifications.success('Combinación actualizada.');
          this.showComboModal = false;
          this.loadRubros();
        },
        error: () => {
          this.savingCombo = false;
          this.notifications.error('Error de red.');
        }
      });
    } else {
      this.rubroApi.create(body).subscribe({
        next: (res) => {
          this.savingCombo = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error');
            return;
          }
          this.notifications.success('Combinación creada.');
          this.showComboModal = false;
          this.loadRubros();
        },
        error: () => {
          this.savingCombo = false;
          this.notifications.error('Error de red.');
        }
      });
    }
  }

  openConceptoNew(): void {
    this.editingConcepto = null;
    this.showConceptoModal = true;
  }

  openConceptoEdit(row: ConceptoListItemDto): void {
    this.conceptoApi.getById(row.id).subscribe({
      next: (res) => {
        if (!res.success || !res.data) {
          this.notifications.error(res.message || 'No se pudo cargar.');
          return;
        }
        this.editingConcepto = res.data;
        this.showConceptoModal = true;
      },
      error: () => this.notifications.error('Error de red.')
    });
  }

  closeConceptoModal(): void {
    if (!this.savingConcepto) this.showConceptoModal = false;
  }

  saveConcepto(payload: {
    nombreConcepto: string;
    rubroGeneralId: number;
    categoriaId: number;
    posicion: number;
    valor: number;
    activo: boolean;
    top: boolean;
  }): void {
    this.savingConcepto = true;
    if (this.editingConcepto) {
      this.conceptoApi.update(this.editingConcepto.id, payload).subscribe({
        next: (res) => {
          this.savingConcepto = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error');
            return;
          }
          this.notifications.success('Concepto actualizado.');
          this.showConceptoModal = false;
          this.loadConceptos();
        },
        error: () => {
          this.savingConcepto = false;
          this.notifications.error('Error de red.');
        }
      });
    } else {
      this.conceptoApi.create(payload).subscribe({
        next: (res) => {
          this.savingConcepto = false;
          if (!res.success || !res.data) {
            this.notifications.error(res.message || 'Error');
            return;
          }
          this.notifications.success('Concepto creado.');
          this.showConceptoModal = false;
          this.loadConceptos();
        },
        error: () => {
          this.savingConcepto = false;
          this.notifications.error('Error de red.');
        }
      });
    }
  }

  askDelete(kind: DeleteKind, id: number, label: string): void {
    this.deleteKind = kind;
    this.deleteId = id;
    this.deleteLabel = label;
    this.showDelete = true;
  }

  closeDelete(): void {
    if (!this.deleting) {
      this.showDelete = false;
      this.deleteKind = null;
    }
  }

  confirmDelete(): void {
    if (!this.deleteKind) return;
    this.deleting = true;
    const kind = this.deleteKind;
    const id = this.deleteId;
    const done = (): void => {
      this.deleting = false;
      this.showDelete = false;
      this.deleteKind = null;
    };

    if (kind === 'rubroCombo') {
      this.rubroApi.delete(id).subscribe({
        next: (res) => {
          done();
          if (!res.success) {
            this.notifications.error(res.message || 'No se pudo eliminar.');
            return;
          }
          this.notifications.success('Combinación eliminada.');
          this.loadRubros();
        },
        error: () => {
          done();
          this.notifications.error('Error de red.');
        }
      });
    } else if (kind === 'concepto') {
      this.conceptoApi.delete(id).subscribe({
        next: (res) => {
          done();
          if (!res.success) {
            this.notifications.error(res.message || 'No se pudo eliminar.');
            return;
          }
          this.notifications.success('Concepto eliminado.');
          this.loadConceptos();
        },
        error: () => {
          done();
          this.notifications.error('Error de red.');
        }
      });
    }
  }
}
