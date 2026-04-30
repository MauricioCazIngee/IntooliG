import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CiudadDto } from '../servicios/ciudad.service';

@Component({
  selector: 'app-ciudad-region-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar ciudad' : 'Nueva ciudad' }}</h5>
        <div class="mb-2">
          <label>País</label>
          <select class="form-control" [(ngModel)]="paisId" (ngModelChange)="paisChange.emit($event)">
            <option [ngValue]="null">—</option>
            <option *ngFor="let p of paisesOptions" [ngValue]="p.id">{{ p.nombrePais }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Estado</label>
          <select class="form-control" [(ngModel)]="estadoId">
            <option [ngValue]="null">—</option>
            <option *ngFor="let e of estadosOptions" [ngValue]="e.id">{{ e.nombreEstado }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Nombre ciudad</label>
          <input class="form-control" [(ngModel)]="nombre" />
        </div>
        <div class="mb-2">
          <label>Nombre corto</label>
          <input class="form-control" [(ngModel)]="nombreCorto" />
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="ciudadPrincipal" /> Ciudad principal</label>
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="mb-2">
          <label>Población (FnPoblacion)</label>
          <input type="number" class="form-control" [(ngModel)]="poblacion" />
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || !nombre.trim() || estadoId == null"
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
export class CiudadRegionModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() paisesOptions: { id: number; nombrePais: string }[] = [];
  @Input() estadosOptions: { id: number; nombreEstado: string }[] = [];

  @Input() set ciudad(v: CiudadDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombre = v.nombreCiudad;
      this.nombreCorto = v.nombreCorto ?? '';
      this.paisId = v.paisId;
      this.estadoId = v.estadoId;
      this.ciudadPrincipal = v.ciudadPrincipal;
      this.activo = v.activo;
      this.poblacion = v.poblacion;
    } else {
      this.editId = null;
      this.nombre = '';
      this.nombreCorto = '';
      this.paisId = null;
      this.estadoId = null;
      this.ciudadPrincipal = false;
      this.activo = true;
      this.poblacion = null;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() paisChange = new EventEmitter<number | null>();
  @Output() save = new EventEmitter<{
    nombreCiudad: string;
    nombreCorto: string | null;
    estadoId: number;
    ciudadPrincipal: boolean;
    activo: boolean;
    poblacion: number | null;
  }>();

  editId: number | null = null;
  nombre = '';
  nombreCorto = '';
  paisId: number | null = null;
  estadoId: number | null = null;
  ciudadPrincipal = false;
  activo = true;
  poblacion: number | null = null;

  guardar(): void {
    if (this.estadoId == null) return;
    this.save.emit({
      nombreCiudad: this.nombre.trim(),
      nombreCorto: this.nombreCorto.trim() || null,
      estadoId: this.estadoId,
      ciudadPrincipal: this.ciudadPrincipal,
      activo: this.activo,
      poblacion: this.poblacion
    });
  }
}
