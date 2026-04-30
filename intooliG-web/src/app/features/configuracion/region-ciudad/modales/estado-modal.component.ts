import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EstadoDto } from '../servicios/estado.service';

@Component({
  selector: 'app-estado-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar estado' : 'Nuevo estado' }}</h5>
        <div class="mb-2">
          <label>País</label>
          <select class="form-control" [(ngModel)]="paisId">
            <option *ngFor="let p of paisesOptions" [ngValue]="p.id">{{ p.nombrePais }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Nombre estado</label>
          <input class="form-control" [(ngModel)]="nombre" />
        </div>
        <div class="mb-2">
          <label>Código mapa</label>
          <select class="form-control" [(ngModel)]="codigoMapaId">
            <option [ngValue]="null">(ninguno)</option>
            <option *ngFor="let c of codigosOptions" [ngValue]="c.id">{{ c.nombre }}</option>
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
            [disabled]="saving || !nombre.trim() || paisId == null"
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
export class EstadoModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() paisesOptions: { id: number; nombrePais: string }[] = [];
  @Input() codigosOptions: { id: number; nombre: string }[] = [];

  @Input() set estado(v: EstadoDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombre = v.nombreEstado;
      this.paisId = v.paisId;
      this.codigoMapaId = v.codigoMapaId;
      this.activo = v.activo;
    } else {
      this.editId = null;
      this.nombre = '';
      this.paisId = this.paisesOptions[0]?.id ?? null;
      this.codigoMapaId = null;
      this.activo = true;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{
    nombreEstado: string;
    paisId: number;
    codigoMapaId: number | null;
    activo: boolean;
  }>();

  editId: number | null = null;
  nombre = '';
  paisId: number | null = null;
  codigoMapaId: number | null = null;
  activo = true;

  guardar(): void {
    if (this.paisId == null) return;
    this.save.emit({
      nombreEstado: this.nombre.trim(),
      paisId: this.paisId,
      codigoMapaId: this.codigoMapaId,
      activo: this.activo
    });
  }
}
