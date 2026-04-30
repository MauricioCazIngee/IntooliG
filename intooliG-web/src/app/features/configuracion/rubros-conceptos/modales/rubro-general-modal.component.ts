import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RubroGeneralDto } from '../servicios/rubro-general.service';

@Component({
  selector: 'app-rubro-general-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar rubro (catálogo)' : 'Nuevo rubro (catálogo)' }}</h5>
        <div class="mb-2">
          <label>Nombre del rubro</label>
          <input class="form-control" [(ngModel)]="nombre" />
        </div>
        <div class="mb-2" *ngIf="editId">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button type="button" class="btn btn-success btn-new" [disabled]="saving || !nombre.trim()" (click)="guardar()">
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
        z-index: 1260;
      }
      .dialog-card {
        width: min(440px, calc(100% - 24px));
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
export class RubroGeneralModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() set item(v: RubroGeneralDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombre = v.nombreRubro;
      this.activo = v.activo;
    } else {
      this.editId = null;
      this.nombre = '';
      this.activo = true;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{ nombreRubro: string; activo: boolean }>();

  editId: number | null = null;
  nombre = '';
  activo = true;

  guardar(): void {
    this.save.emit({ nombreRubro: this.nombre.trim(), activo: this.activo });
  }
}
