import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PaisDto } from '../../region-ciudad/servicios/pais.service';
import { TipoCambioDto, TipoCambioMesDto } from '../servicios/tipo-cambio.service';

@Component({
  selector: 'app-tipo-cambio-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar tipo de cambio' : 'Nuevo tipo de cambio' }}</h5>
        <div class="mb-2">
          <label>País</label>
          <select class="form-control" [(ngModel)]="paisId">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let p of paisesOptions" [ngValue]="p.id">{{ p.nombrePais }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Año</label>
          <select class="form-control" [(ngModel)]="anio">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let y of aniosOptions" [ngValue]="y">{{ y }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Mes</label>
          <select class="form-control" [(ngModel)]="mes">
            <option [ngValue]="0">Seleccione…</option>
            <option *ngFor="let m of mesesOptions" [ngValue]="m.mes">{{ m.nombre }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Tipo de Cambio</label>
          <input class="form-control" type="number" min="0" step="0.0001" [(ngModel)]="tipoCambio" />
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || !paisId || !anio || !mes || tipoCambio <= 0"
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
export class TipoCambioModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() paisesOptions: PaisDto[] = [];
  @Input() aniosOptions: number[] = [];
  @Input() mesesOptions: TipoCambioMesDto[] = [];

  @Input() set row(v: TipoCambioDto | null) {
    if (v) {
      this.editId = v.id;
      this.paisId = v.paisId;
      this.anio = v.anio;
      this.mes = v.mes;
      this.tipoCambio = v.tipoCambio;
    } else {
      this.editId = null;
      this.paisId = 0;
      this.anio = 0;
      this.mes = 0;
      this.tipoCambio = 0;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{ paisId: number; anio: number; mes: number; tipoCambio: number }>();

  editId: number | null = null;
  paisId = 0;
  anio = 0;
  mes = 0;
  tipoCambio = 0;

  guardar(): void {
    this.save.emit({
      paisId: this.paisId,
      anio: this.anio,
      mes: this.mes,
      tipoCambio: Number(this.tipoCambio)
    });
  }
}
