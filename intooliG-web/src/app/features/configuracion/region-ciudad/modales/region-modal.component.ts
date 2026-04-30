import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RegionDto } from '../servicios/region.service';

@Component({
  selector: 'app-region-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card dialog-wide">
        <h5>{{ editId ? 'Editar región' : 'Nueva región' }}</h5>
        <div class="mb-2">
          <label>País</label>
          <select class="form-control" [(ngModel)]="paisId" (ngModelChange)="paisChange.emit($event)">
            <option *ngFor="let p of paisesOptions" [ngValue]="p.id">{{ p.nombrePais }}</option>
          </select>
        </div>
        <div class="mb-2">
          <label>Nombre región</label>
          <input class="form-control" [(ngModel)]="nombre" />
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="esNacional" /> Es nacional</label>
        </div>
        <div class="mb-2">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="mb-2">
          <label>Ciudades (seleccione una o varias)</label>
          <div class="ciudad-box">
            <label *ngFor="let c of ciudadOptions" class="d-block small">
              <input type="checkbox" [checked]="selectedIds.has(c.id)" (change)="toggle(c.id, $event)" />
              {{ c.nombre }}
            </label>
            <p *ngIf="!ciudadOptions.length" class="text-muted small mb-0">Cargue ciudades eligiendo país.</p>
          </div>
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
        background: #fff;
        border-radius: 6px;
        padding: 16px;
      }
      .dialog-wide {
        width: min(520px, calc(100% - 24px));
        max-height: 90vh;
        overflow: auto;
      }
      .ciudad-box {
        max-height: 220px;
        overflow: auto;
        border: 1px solid #ddd;
        padding: 8px;
        border-radius: 4px;
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
export class RegionModalComponent {
  @Input() visible = false;
  @Input() saving = false;
  @Input() paisesOptions: { id: number; nombrePais: string }[] = [];
  @Input() ciudadOptions: { id: number; nombre: string }[] = [];

  @Input() set region(v: RegionDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombre = v.nombreRegion;
      this.paisId = v.paisId;
      this.esNacional = v.esNacional;
      this.activo = v.activo;
      this.selectedIds = new Set(v.ciudadIds);
    } else {
      this.editId = null;
      this.nombre = '';
      this.paisId = this.paisesOptions[0]?.id ?? null;
      this.esNacional = false;
      this.activo = true;
      this.selectedIds = new Set();
    }
  }

  @Output() close = new EventEmitter<void>();
  @Output() paisChange = new EventEmitter<number | null>();
  @Output() save = new EventEmitter<{
    paisId: number;
    nombreRegion: string;
    esNacional: boolean;
    activo: boolean;
    ciudadIds: number[];
  }>();

  editId: number | null = null;
  nombre = '';
  paisId: number | null = null;
  esNacional = false;
  activo = true;
  selectedIds = new Set<number>();

  toggle(id: number, ev: Event): void {
    const checked = (ev.target as HTMLInputElement).checked;
    if (checked) this.selectedIds.add(id);
    else this.selectedIds.delete(id);
  }

  guardar(): void {
    if (this.paisId == null) return;
    this.save.emit({
      paisId: this.paisId,
      nombreRegion: this.nombre.trim(),
      esNacional: this.esNacional,
      activo: this.activo,
      ciudadIds: [...this.selectedIds]
    });
  }
}
