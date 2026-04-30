import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoriaDto } from '../servicios/categoria.service';

@Component({
  selector: 'app-categoria-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar categoría' : 'Nueva categoría' }}</h5>
        <p class="text-muted small" *ngIf="!editId">
          La relación con BU no está disponible en la base actual (sin columna FiBUid en tbCatCategoria).
        </p>
        <div class="mb-2">
          <label>Nombre categoría</label>
          <input class="form-control" [(ngModel)]="nombre" />
        </div>
        <div class="mb-2">
          <label>Nombre corto (opcional)</label>
          <input class="form-control" [(ngModel)]="corto" />
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
        z-index: 1250;
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
export class CategoriaModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() set cat(v: CategoriaDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombre = v.nombreCategoria;
      this.corto = v.nombreCorto ?? '';
      this.activo = v.activo;
    } else {
      this.editId = null;
      this.nombre = '';
      this.corto = '';
      this.activo = true;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{ nombreCategoria: string; nombreCorto: string | null; activo: boolean }>();

  editId: number | null = null;
  nombre = '';
  corto = '';
  activo = true;

  guardar(): void {
    const c = this.corto.trim();
    this.save.emit({
      nombreCategoria: this.nombre.trim(),
      nombreCorto: c.length ? c : null,
      activo: this.activo
    });
  }
}
