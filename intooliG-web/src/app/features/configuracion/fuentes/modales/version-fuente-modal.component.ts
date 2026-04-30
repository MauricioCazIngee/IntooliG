import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { VersionFuenteDto, VersionTVLookupDto } from '../servicios/version-fuente.service';
import { FuenteDto } from '../servicios/fuente.service';
import { BUDto } from '../../sector-categoria/servicios/bu.service';
import { CategoriaDto } from '../../sector-categoria/servicios/categoria.service';
import { MarcaDto } from '../../sector-categoria/servicios/marca.service';
import { ProductoCatalogDto, ProductoCatalogService } from '../servicios/producto-catalog.service';

@Component({
  selector: 'app-version-fuente-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card dialog-wide">
        <h5>{{ editId != null ? 'Editar versión fuente' : 'Nueva versión fuente' }}</h5>
        <div class="mb-2">
          <label>Nombre versión fuente</label>
          <input class="form-control" [(ngModel)]="nombreVersionFuente" />
        </div>
        <div class="mb-2">
          <label>Fuente</label>
          <select class="form-control" [(ngModel)]="fuenteId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let f of fuentesOptions" [ngValue]="f.id">{{ f.nombreFuente }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Categoría</label>
          <select class="form-control" [(ngModel)]="categoriaId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let c of categoriasOptions" [ngValue]="c.id">{{ c.nombreCategoria }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>BU</label>
          <select class="form-control" [(ngModel)]="buId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let b of busOptions" [ngValue]="b.id">{{ b.nombreBU }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Versión TV</label>
          <select class="form-control" [(ngModel)]="versionTVId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let v of versionTvOptions" [ngValue]="v.id">{{ v.nombre }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Marca (para producto)</label>
          <select class="form-control" [(ngModel)]="marcaIdSel" (ngModelChange)="onMarcaChange($event)">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let m of marcasOptions" [ngValue]="m.id">{{ m.nombreMarca }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Producto</label>
          <select class="form-control" [(ngModel)]="productoId" [disabled]="!marcaIdSel || loadingProductos">
            <option [ngValue]="0">{{ loadingProductos ? 'Cargando…' : 'Seleccione…' }}</option>
            <option *ngFor="let p of productosOptions" [ngValue]="p.id">{{ p.nombre }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="
              saving ||
              !nombreVersionFuente.trim() ||
              !fuenteId ||
              !categoriaId ||
              !buId ||
              !versionTVId ||
              !productoId
            "
            (click)="guardar()"
          >
            {{ saving ? 'Guardando...' : 'Guardar' }}
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .dialog-backdrop {
        position: fixed;
        inset: 0;
        background: rgb(0 0 0 / 45%);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 1250;
      }
      .dialog-card {
        background: #fff;
        border-radius: 6px;
        padding: 16px;
        max-height: 90vh;
        overflow: auto;
      }
      .dialog-wide {
        width: min(560px, calc(100% - 24px));
      }
      .actions {
        display: flex;
        justify-content: flex-end;
        gap: 8px;
        margin-top: 12px;
      }
      .btn-new {
        background-color: #5dc1a5 !important;
        border-color: #5dc1a5 !important;
        color: #fff;
      }
    `
  ]
})
export class VersionFuenteModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() saving = false;
  @Input() fuentesOptions: FuenteDto[] = [];
  @Input() categoriasOptions: CategoriaDto[] = [];
  @Input() busOptions: BUDto[] = [];
  @Input() marcasOptions: MarcaDto[] = [];
  @Input() versionTvOptions: VersionTVLookupDto[] = [];
  @Input() row: VersionFuenteDto | null = null;

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{
    nombreVersionFuente: string;
    fuenteId: number;
    categoriaId: number;
    buId: number;
    productoId: number;
    versionTVId: number;
    activo: boolean;
  }>();

  constructor(private readonly productosApi: ProductoCatalogService) {}

  editId: number | null = null;
  nombreVersionFuente = '';
  fuenteId = 0;
  categoriaId = 0;
  buId = 0;
  versionTVId = 0;
  marcaIdSel = 0;
  productoId = 0;
  activo = true;
  productosOptions: ProductoCatalogDto[] = [];
  loadingProductos = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['row'] || changes['visible']) {
      if (!this.visible) return;
      if (this.row) {
        this.editId = this.row.id;
        this.nombreVersionFuente = this.row.nombreVersionFuente;
        this.fuenteId = this.row.fuenteId;
        this.categoriaId = this.row.categoriaId;
        this.buId = this.row.buId;
        this.versionTVId = this.row.versionTVId;
        this.productoId = this.row.productoId;
        this.activo = this.row.activo;
        this.marcaIdSel = this.row.marcaId || 0;
        if (this.marcaIdSel) this.loadProductos(this.marcaIdSel);
      } else {
        this.editId = null;
        this.nombreVersionFuente = '';
        this.fuenteId = 0;
        this.categoriaId = 0;
        this.buId = 0;
        this.versionTVId = 0;
        this.marcaIdSel = 0;
        this.productoId = 0;
        this.activo = true;
        this.productosOptions = [];
      }
    }
  }

  onMarcaChange(mid: number): void {
    this.productoId = 0;
    this.productosOptions = [];
    if (mid) this.loadProductos(mid);
  }

  loadProductos(marcaId: number): void {
    this.loadingProductos = true;
    this.productosApi.listByMarca(marcaId).subscribe({
      next: (res) => {
        this.loadingProductos = false;
        if (res.success && res.data) {
          this.productosOptions = res.data.filter((p) => p.activo);
        }
      },
      error: () => {
        this.loadingProductos = false;
      }
    });
  }

  guardar(): void {
    this.save.emit({
      nombreVersionFuente: this.nombreVersionFuente.trim(),
      fuenteId: this.fuenteId,
      categoriaId: this.categoriaId,
      buId: this.buId,
      productoId: this.productoId,
      versionTVId: this.versionTVId,
      activo: this.activo
    });
  }
}
