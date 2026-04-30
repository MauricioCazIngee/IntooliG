import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MedioClienteDto } from '../servicios/medio-cliente.service';
import { MedioLookupDto } from '../servicios/medio.service';
import { ClienteLookupDto } from '../../rubros-conceptos/servicios/cliente-lookup.service';

@Component({
  selector: 'app-medio-cliente-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editKey ? 'Editar asignación' : 'Nueva asignación' }}</h5>
        <div class="mb-2">
          <label>Medio</label>
          <select class="form-control" [(ngModel)]="medioId" [disabled]="!!editKey">
            <option [ngValue]="null">—</option>
            <option *ngFor="let m of mediosOptions" [ngValue]="m.id">{{ m.nombre }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Cliente</label>
          <select class="form-control" [(ngModel)]="clienteId" [disabled]="!!editKey">
            <option [ngValue]="null">—</option>
            <option *ngFor="let c of clientesOptions" [ngValue]="c.id">{{ c.nombre }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="esNacional" /> Es nacional</label>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || medioId == null || clienteId == null"
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
export class MedioClienteModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() mediosOptions: MedioLookupDto[] = [];
  @Input() clientesOptions: ClienteLookupDto[] = [];

  /** Si hay fila en edición, clave medioId-clienteId */
  @Input() set asignacion(v: MedioClienteDto | null) {
    if (v) {
      this.editKey = `${v.medioId}-${v.clienteId}`;
      this.medioId = v.medioId;
      this.clienteId = v.clienteId;
      this.esNacional = v.esNacional;
    } else {
      this.editKey = null;
      this.medioId = null;
      this.clienteId = null;
      this.esNacional = false;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{
    medioId: number;
    clienteId: number;
    esNacional: boolean;
  }>();

  editKey: string | null = null;
  medioId: number | null = null;
  clienteId: number | null = null;
  esNacional = false;

  guardar(): void {
    if (this.medioId == null || this.clienteId == null) return;
    this.save.emit({
      medioId: this.medioId,
      clienteId: this.clienteId,
      esNacional: this.esNacional
    });
  }
}
