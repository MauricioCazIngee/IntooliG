import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DayPartDto, DayPartLookupOptionDto } from '../servicios/daypart.service';

@Component({
  selector: 'app-daypart-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar DayPart' : 'Nuevo DayPart' }}</h5>
        <div class="mb-2">
          <label>País</label>
          <select class="form-control" [(ngModel)]="paisId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let p of paisesOptions" [ngValue]="p.id">{{ p.nombre }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Medio</label>
          <select class="form-control" [(ngModel)]="medioId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let m of mediosOptions" [ngValue]="m.id">{{ m.nombre }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Descripción</label>
          <input class="form-control" [(ngModel)]="descripcion" />
        </div>
        <div class="mb-2">
          <label>Inicio</label>
          <select class="form-control" [(ngModel)]="horaInicio">
            <option [ngValue]="-1">Seleccione…</option>
            <option *ngFor="let h of horasOptions" [ngValue]="h">{{ toHourText(h) }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Fin</label>
          <select class="form-control" [(ngModel)]="horaFin">
            <option [ngValue]="-1">Seleccione…</option>
            <option *ngFor="let h of horasOptions" [ngValue]="h">{{ toHourText(h) }}</option>
          </select>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || !isValid()"
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
export class DayPartModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() paisesOptions: DayPartLookupOptionDto[] = [];
  @Input() mediosOptions: DayPartLookupOptionDto[] = [];
  @Input() horasOptions: number[] = [];

  @Input() set row(v: DayPartDto | null) {
    if (v) {
      this.editId = v.id;
      this.paisId = v.paisId;
      this.medioId = v.medioId;
      this.descripcion = v.descripcion;
      this.horaInicio = v.horaInicio;
      this.horaFin = v.horaFin;
    } else {
      this.editId = null;
      this.paisId = 0;
      this.medioId = 0;
      this.descripcion = '';
      this.horaInicio = -1;
      this.horaFin = -1;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{
    paisId: number;
    medioId: number;
    descripcion: string;
    horaInicio: number;
    horaFin: number;
  }>();

  editId: number | null = null;
  paisId = 0;
  medioId = 0;
  descripcion = '';
  horaInicio = -1;
  horaFin = -1;

  isValid(): boolean {
    return this.paisId > 0 && this.medioId > 0 && this.descripcion.trim().length > 0 && this.horaInicio >= 0 && this.horaFin >= 0;
  }

  guardar(): void {
    this.save.emit({
      paisId: this.paisId,
      medioId: this.medioId,
      descripcion: this.descripcion.trim(),
      horaInicio: this.horaInicio,
      horaFin: this.horaFin
    });
  }

  toHourText(hour: number): string {
    return `${hour.toString().padStart(2, '0')}:00`;
  }
}
