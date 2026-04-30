import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConceptoDetailDto } from '../servicios/concepto.service';

@Component({
  selector: 'app-concepto-catalogo-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar concepto' : 'Nuevo concepto' }}</h5>
        <p class="text-muted small">
          Requiere una combinación Rubro+Categoría existente en la pestaña anterior.
        </p>
        <div class="mb-2">
          <label>Concepto</label>
          <input class="form-control" [(ngModel)]="nombre" />
        </div>
        <div class="mb-2">
          <label>Rubro (catálogo)</label>
          <select class="form-control" [(ngModel)]="rubroGeneralId">
            <option [ngValue]="0">Seleccione...</option>
            <option *ngFor="let r of rubrosGenerales" [ngValue]="r.id">{{ r.nombreRubro }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Categoría</label>
          <select class="form-control" [(ngModel)]="categoriaId">
            <option [ngValue]="0">Seleccione...</option>
            <option *ngFor="let c of categorias" [ngValue]="c.id">{{ c.nombreCategoria }}</option>
          </select>
        </div>
        <div class="row">
          <div class="col-6 mb-2">
            <label>Posición</label>
            <input type="number" class="form-control" [(ngModel)]="posicion" />
          </div>
          <div class="col-6 mb-2">
            <label>Valor</label>
            <input type="number" class="form-control" step="0.0001" [(ngModel)]="valor" />
          </div>
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="top" /> Top</label>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || !nombre.trim() || rubroGeneralId <= 0 || categoriaId <= 0"
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
        width: min(520px, calc(100% - 24px));
        max-height: 90vh;
        overflow-y: auto;
        background: #fff;
        border-radius: 6px;
        padding: 16px;
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
export class ConceptoCatalogoModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() rubrosGenerales: { id: number; nombreRubro: string }[] = [];
  @Input() categorias: { id: number; nombreCategoria: string }[] = [];

  @Input() set concepto(v: ConceptoDetailDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombre = v.nombreConcepto;
      this.rubroGeneralId = v.rubroGeneralId;
      this.categoriaId = v.categoriaId;
      this.posicion = v.posicion;
      this.valor = v.valor;
      this.activo = v.activo;
      this.top = v.top;
    } else {
      this.editId = null;
      this.nombre = '';
      this.rubroGeneralId = 0;
      this.categoriaId = 0;
      this.posicion = 1;
      this.valor = 0;
      this.activo = true;
      this.top = false;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{
    nombreConcepto: string;
    rubroGeneralId: number;
    categoriaId: number;
    posicion: number;
    valor: number;
    activo: boolean;
    top: boolean;
  }>();

  editId: number | null = null;
  nombre = '';
  rubroGeneralId = 0;
  categoriaId = 0;
  posicion = 1;
  valor = 0;
  activo = true;
  top = false;

  guardar(): void {
    this.save.emit({
      nombreConcepto: this.nombre.trim(),
      rubroGeneralId: this.rubroGeneralId,
      categoriaId: this.categoriaId,
      posicion: Number(this.posicion),
      valor: Number(this.valor),
      activo: this.activo,
      top: this.top
    });
  }
}
