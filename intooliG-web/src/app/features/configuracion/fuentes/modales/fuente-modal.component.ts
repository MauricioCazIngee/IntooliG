import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { FuenteDto } from '../servicios/fuente.service';
import { PaisDto } from '../../region-ciudad/servicios/pais.service';

@Component({
  selector: 'app-fuente-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar fuente' : 'Nueva fuente' }}</h5>
        <div class="mb-2">
          <label>Nombre fuente</label>
          <input class="form-control" [(ngModel)]="nombreFuente" />
        </div>
        <div class="mb-2">
          <label>País</label>
          <select class="form-control" [(ngModel)]="paisId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let p of paisesOptions" [ngValue]="p.id">{{ p.nombrePais }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button type="button" class="btn btn-success btn-new" [disabled]="saving || !nombreFuente.trim() || !paisId" (click)="guardar()">
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
export class FuenteModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() paisesOptions: PaisDto[] = [];

  @Input() set fuente(v: FuenteDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombreFuente = v.nombreFuente;
      this.paisId = v.paisId;
      this.activo = v.activo;
    } else {
      this.editId = null;
      this.nombreFuente = '';
      this.paisId = 0;
      this.activo = true;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{ nombreFuente: string; paisId: number; activo: boolean }>();

  editId: number | null = null;
  nombreFuente = '';
  paisId = 0;
  activo = true;

  guardar(): void {
    this.save.emit({
      nombreFuente: this.nombreFuente.trim(),
      paisId: this.paisId,
      activo: this.activo
    });
  }
}
