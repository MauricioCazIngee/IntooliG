import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BUDto } from '../servicios/bu.service';

@Component({
  selector: 'app-bu-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ editId ? 'Editar BU' : 'Nuevo BU' }}</h5>
        <div class="mb-2">
          <label>Nombre BU</label>
          <input class="form-control" [(ngModel)]="nombre" />
        </div>
        <div class="mb-2">
          <label>Sector</label>
          <select class="form-control" [(ngModel)]="sectorId">
            <option [ngValue]="0">Seleccione...</option>
            <option *ngFor="let s of sectores" [ngValue]="s.id">{{ s.nombreSector }}</option>
          </select>
        </div>
        <div class="mb-2" *ngIf="editId">
          <label><input type="checkbox" [(ngModel)]="activo" /> Activo</label>
        </div>
        <div class="actions">
          <button type="button" class="btn btn-light" (click)="close.emit()">Cancelar</button>
          <button
            type="button"
            class="btn btn-success btn-new"
            [disabled]="saving || !nombre.trim() || sectorId <= 0"
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
export class BuModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() saving = false;
  @Input() sectores: { id: number; nombreSector: string }[] = [];
  @Input() sectorIdInicial: number | null = null;
  @Input() set bu(v: BUDto | null) {
    if (v) {
      this.editId = v.id;
      this.nombre = v.nombreBU;
      this.activo = v.activo;
      this.sectorId = v.sectorId;
    } else {
      this.editId = null;
      this.nombre = '';
      this.activo = true;
      this.sectorId = 0;
      this.applySectorInicial();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['sectorIdInicial'] && !this.editId) {
      this.applySectorInicial();
    }
  }

  private applySectorInicial(): void {
    const v = this.sectorIdInicial;
    if (v != null && v > 0) this.sectorId = v;
  }

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<{ nombreBU: string; sectorId: number; activo: boolean }>();

  editId: number | null = null;
  nombre = '';
  activo = true;
  sectorId = 0;

  guardar(): void {
    this.save.emit({ nombreBU: this.nombre.trim(), sectorId: this.sectorId, activo: this.activo });
  }
}
