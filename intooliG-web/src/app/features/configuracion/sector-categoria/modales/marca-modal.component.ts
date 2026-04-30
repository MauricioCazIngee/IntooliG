import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MarcaDetailDto } from '../servicios/marca.service';

@Component({
  selector: 'app-marca-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card wide">
        <h5>{{ editId ? 'Editar marca' : 'Nueva marca' }}</h5>
        <div class="mb-2">
          <label>Nombre marca</label>
          <input class="form-control" [(ngModel)]="nombre" />
        </div>
        <div class="mb-2" *ngIf="editId">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="mb-2">
          <label>Logo (opcional)</label>
          <input type="file" accept="image/*" class="form-control" (change)="onFile($event)" />
          <small class="text-muted">Dejar vacío en edición para conservar el logo actual.</small>
        </div>
        <div class="mb-2">
          <label>Productos (uno por línea)</label>
          <textarea class="form-control" rows="5" [(ngModel)]="productosTexto"></textarea>
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
        width: min(520px, calc(100% - 24px));
        background: #fff;
        border-radius: 6px;
        padding: 16px;
      }
      .dialog-card.wide {
        max-height: 90vh;
        overflow-y: auto;
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
export class MarcaModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() set marca(v: MarcaDetailDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombre = v.nombreMarca;
      this.activo = v.activo;
      this.productosTexto = v.productos.map((p) => p.nombre).join('\n');
      this.logoBase64 = null;
      this.logoTouched = false;
    } else {
      this.editId = null;
      this.nombre = '';
      this.activo = true;
      this.productosTexto = '';
      this.logoBase64 = null;
      this.logoTouched = false;
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{
    nombreMarca: string;
    activo: boolean;
    logoBase64: string | null;
    productosNombres: string[];
  }>();

  editId: number | null = null;
  nombre = '';
  activo = true;
  productosTexto = '';
  logoBase64: string | null = null;
  logoTouched = false;

  onFile(ev: Event): void {
    const f = (ev.target as HTMLInputElement).files?.[0];
    this.logoTouched = true;
    if (!f) {
      this.logoBase64 = null;
      return;
    }
    const reader = new FileReader();
    reader.onload = () => {
      const r = reader.result as string;
      const comma = r.indexOf(',');
      this.logoBase64 = comma >= 0 ? r.slice(comma + 1) : r;
    };
    reader.readAsDataURL(f);
  }

  guardar(): void {
    const lines = this.productosTexto
      .split(/\r?\n/)
      .map((x) => x.trim())
      .filter((x) => x.length > 0);
    const logoPayload = this.editId
      ? this.logoTouched
        ? this.logoBase64
        : null
      : this.logoBase64;
    this.save.emit({
      nombreMarca: this.nombre.trim(),
      activo: this.activo,
      logoBase64: logoPayload,
      productosNombres: lines
    });
  }
}
