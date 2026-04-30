import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PoblacionDto } from '../servicios/poblacion.service';

@Component({
  selector: 'app-poblacion-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar población' : 'Nueva población' }}</h5>
        <div class="mb-2">
          <label>País</label>
          <select class="form-control" [(ngModel)]="paisId" (ngModelChange)="paisChange.emit($event)">
            <option [ngValue]="null">—</option>
            <option *ngFor="let p of paisesOptions" [ngValue]="p.id">{{ p.nombrePais }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Estado</label>
          <select class="form-control" [(ngModel)]="estadoId" (ngModelChange)="onEstadoChange($event)">
            <option [ngValue]="null">—</option>
            <option *ngFor="let e of estadosOptions" [ngValue]="e.id">{{ e.nombreEstado }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Ciudad</label>
          <select class="form-control" [(ngModel)]="ciudadId">
            <option [ngValue]="null">—</option>
            <option *ngFor="let c of ciudadesOptions" [ngValue]="c.id">{{ c.nombre }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Año</label>
          <input type="number" class="form-control" [(ngModel)]="anio" />
        </div>
        <div class="mb-2">
          <label>Población</label>
          <input type="number" class="form-control" [(ngModel)]="cantidad" />
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || ciudadId == null || !anio"
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
        width: min(480px, calc(100% - 24px));
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
export class PoblacionModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() paisesOptions: { id: number; nombrePais: string }[] = [];
  @Input() estadosOptions: { id: number; nombreEstado: string }[] = [];
  @Input() ciudadesOptions: { id: number; nombre: string }[] = [];

  @Input() set poblacion(v: PoblacionDto | null) {
    if (v) {
      this.editId = v.id;
      this.paisId = v.paisId;
      this.estadoId = v.estadoId;
      this.ciudadId = v.ciudadId;
      this.anio = v.anio;
      this.cantidad = v.cantidad;
    } else {
      this.editId = null;
      this.paisId = null;
      this.estadoId = null;
      this.ciudadId = null;
      this.anio = new Date().getFullYear();
      this.cantidad = 0;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() paisChange = new EventEmitter<number | null>();
  @Output() estadoChange = new EventEmitter<number | null>();
  @Output() save = new EventEmitter<{ ciudadId: number; anio: number; cantidad: number }>();

  onEstadoChange(v: number | null): void {
    this.estadoChange.emit(v);
  }

  editId: number | null = null;
  paisId: number | null = null;
  estadoId: number | null = null;
  ciudadId: number | null = null;
  anio = new Date().getFullYear();
  cantidad = 0;

  guardar(): void {
    if (this.ciudadId == null) return;
    this.save.emit({
      ciudadId: this.ciudadId,
      anio: Number(this.anio),
      cantidad: Number(this.cantidad)
    });
  }
}
