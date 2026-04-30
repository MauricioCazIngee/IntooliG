import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MarcaFuenteDto } from '../servicios/marca-fuente.service';
import { FuenteDto } from '../servicios/fuente.service';
import { MarcaDto } from '../../sector-categoria/servicios/marca.service';
import { ProductoCatalogDto, ProductoCatalogService } from '../servicios/producto-catalog.service';

@Component({
  selector: 'app-marca-fuente-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card dialog-wide">
        <h5>{{ editId != null ? 'Editar marca fuente' : 'Nueva marca fuente' }}</h5>
        <div class="mb-2">
          <label>Nombre marca fuente</label>
          <input class="form-control" [(ngModel)]="nombreMarcaFuente" />
        </div>
        <div class="mb-2">
          <label>Fuente</label>
          <select class="form-control" [(ngModel)]="fuenteId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let f of fuentesOptions" [ngValue]="f.id">{{ f.nombreFuente }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Marca</label>
          <select class="form-control" [(ngModel)]="marcaId" (ngModelChange)="onMarcaChange($event)">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let m of marcasOptions" [ngValue]="m.id">{{ m.nombreMarca }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Producto</label>
          <select class="form-control" [(ngModel)]="productoId" [disabled]="!marcaId || loadingProductos">
            <option [ngValue]="0">{{ loadingProductos ? 'Cargando…' : 'Seleccione…' }}</option>
            <option *ngFor="let p of productosOptions" [ngValue]="p.id">{{ p.nombre }}</option>
          </select>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || !nombreMarcaFuente.trim() || !fuenteId || !marcaId || !productoId"
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
export class MarcaFuenteModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() saving = false;
  @Input() fuentesOptions: FuenteDto[] = [];
  @Input() marcasOptions: MarcaDto[] = [];
  @Input() row: MarcaFuenteDto | null = null;

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{ nombreMarcaFuente: string; fuenteId: number; marcaId: number; productoId: number }>();

  constructor(private readonly productosApi: ProductoCatalogService) {}

  editId: number | null = null;
  nombreMarcaFuente = '';
  fuenteId = 0;
  marcaId = 0;
  productoId = 0;
  productosOptions: ProductoCatalogDto[] = [];
  loadingProductos = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['row'] || changes['visible']) {
      if (!this.visible) return;
      if (this.row) {
        this.editId = this.row.id;
        this.nombreMarcaFuente = this.row.nombreMarcaFuente;
        this.fuenteId = this.row.fuenteId;
        this.marcaId = this.row.marcaId;
        this.productoId = this.row.productoId;
        if (this.marcaId) this.loadProductos(this.marcaId);
      } else {
        this.editId = null;
        this.nombreMarcaFuente = '';
        this.fuenteId = 0;
        this.marcaId = 0;
        this.productoId = 0;
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
      nombreMarcaFuente: this.nombreMarcaFuente.trim(),
      fuenteId: this.fuenteId,
      marcaId: this.marcaId,
      productoId: this.productoId
    });
  }
}
