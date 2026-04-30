import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RubroCombinacionDto } from '../servicios/rubro.service';
import { ClienteLookupDto } from '../servicios/cliente-lookup.service';

@Component({
  selector: 'app-rubro-combo-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar combinación rubro' : 'Nueva combinación rubro' }}</h5>
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
        <div class="mb-2">
          <label>Valor</label>
          <input type="number" class="form-control" step="0.0001" [(ngModel)]="valorRubro" />
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="mb-2" *ngIf="clientes.length">
          <label>Cliente (opcional)</label>
          <select class="form-control" [(ngModel)]="clienteId">
            <option [ngValue]="null">Predeterminado (sesión)</option>
            <option *ngFor="let cl of clientes" [ngValue]="cl.id">{{ cl.nombre }}</option>
          </select>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || rubroGeneralId <= 0 || categoriaId <= 0"
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
export class RubroComboModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() rubrosGenerales: { id: number; nombreRubro: string }[] = [];
  @Input() categorias: { id: number; nombreCategoria: string }[] = [];
  @Input() clientes: ClienteLookupDto[] = [];

  @Input() set combo(v: RubroCombinacionDto | null) {
    if (v) {
      this.editId = v.id;
      this.rubroGeneralId = v.rubroGeneralId;
      this.categoriaId = v.categoriaId;
      this.valorRubro = v.valorRubro;
      this.activo = v.activo;
      this.clienteId = null;
    } else {
      this.editId = null;
      this.rubroGeneralId = 0;
      this.categoriaId = 0;
      this.valorRubro = 0;
      this.activo = true;
      this.clienteId = null;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{
    rubroGeneralId: number;
    categoriaId: number;
    valorRubro: number;
    activo: boolean;
    clienteId: number | null;
  }>();

  editId: number | null = null;
  rubroGeneralId = 0;
  categoriaId = 0;
  valorRubro = 0;
  activo = true;
  clienteId: number | null = null;

  guardar(): void {
    this.save.emit({
      rubroGeneralId: this.rubroGeneralId,
      categoriaId: this.categoriaId,
      valorRubro: Number(this.valorRubro),
      activo: this.activo,
      clienteId: this.clienteId
    });
  }
}
