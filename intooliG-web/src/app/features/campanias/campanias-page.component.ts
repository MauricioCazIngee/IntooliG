import { Component, OnInit } from '@angular/core';
import { NgFor, NgIf, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CampaniasService, Campania } from './campanias.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-campanias-page',
  standalone: true,
  imports: [NgFor, NgIf, DatePipe, FormsModule],
  template: `
    <h1>Campañas</h1>
    <p *ngIf="loadError" class="err">{{ loadError }}</p>

    <section *ngIf="auth.isAdmin()" class="panel">
      <h2>Nueva campaña</h2>
      <div class="grid">
        <input [(ngModel)]="draft.codigo" placeholder="Código" />
        <input [(ngModel)]="draft.nombre" placeholder="Nombre" />
        <input [(ngModel)]="draft.descripcion" placeholder="Descripción (opcional)" />
        <label class="chk"><input type="checkbox" [(ngModel)]="draft.activa" /> Activa</label>
        <button type="button" (click)="create()" [disabled]="saving">Guardar</button>
      </div>
      <p *ngIf="saveError" class="err">{{ saveError }}</p>
    </section>

    <table *ngIf="items.length">
      <thead>
        <tr>
          <th>Código</th>
          <th>Nombre</th>
          <th>Activa</th>
          <th>Creada (UTC)</th>
          <th *ngIf="auth.isAdmin()"></th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let c of items">
          <td>{{ c.codigo }}</td>
          <td>{{ c.nombre }}</td>
          <td>{{ c.activa ? 'Sí' : 'No' }}</td>
          <td>{{ c.fechaCreacionUtc | date : 'medium' }}</td>
          <td *ngIf="auth.isAdmin()">
            <button type="button" class="danger" (click)="remove(c)">Eliminar</button>
          </td>
        </tr>
      </tbody>
    </table>
    <p *ngIf="!items.length && !loadError">No hay campañas.</p>
  `,
  styles: [
    `
      table {
        width: 100%;
        border-collapse: collapse;
        background: white;
        border-radius: 8px;
        overflow: hidden;
        box-shadow: 0 1px 3px rgb(0 0 0 / 0.08);
      }
      th,
      td {
        text-align: left;
        padding: 0.6rem 0.75rem;
        border-bottom: 1px solid #e5e7eb;
      }
      .err {
        color: #b91c1c;
      }
      .panel {
        margin-bottom: 1.25rem;
        padding: 1rem;
        background: white;
        border-radius: 8px;
        box-shadow: 0 1px 3px rgb(0 0 0 / 0.08);
      }
      .grid {
        display: grid;
        gap: 0.5rem;
        grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
        align-items: center;
      }
      input {
        padding: 0.45rem 0.5rem;
        border: 1px solid #d1d5db;
        border-radius: 6px;
      }
      button {
        padding: 0.45rem 0.65rem;
        border-radius: 6px;
        border: none;
        background: #2563eb;
        color: white;
      }
      .danger {
        background: #b91c1c;
      }
      .chk {
        display: flex;
        gap: 0.35rem;
        align-items: center;
      }
    `
  ]
})
export class CampaniasPageComponent implements OnInit {
  items: Campania[] = [];
  loadError = '';
  saveError = '';
  saving = false;

  draft = {
    codigo: '',
    nombre: '',
    descripcion: '',
    activa: true
  };

  constructor(
    private readonly api: CampaniasService,
    readonly auth: AuthService
  ) {}

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.loadError = '';
    this.api.list().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.items = res.data;
        } else {
          this.loadError = res.message || 'No se pudo cargar la lista.';
        }
      },
      error: () => (this.loadError = 'Error de red al cargar campañas.')
    });
  }

  create(): void {
    this.saveError = '';
    this.saving = true;
    this.api
      .create({
        codigo: this.draft.codigo,
        nombre: this.draft.nombre,
        descripcion: this.draft.descripcion || null,
        fechaInicio: null,
        fechaFin: null,
        activa: this.draft.activa
      })
      .subscribe({
        next: (res) => {
          this.saving = false;
          if (res.success) {
            this.draft = { codigo: '', nombre: '', descripcion: '', activa: true };
            this.reload();
          } else {
            this.saveError = res.message || 'No se pudo crear.';
          }
        },
        error: () => {
          this.saving = false;
          this.saveError = 'Error al crear (¿permisos Admin?).';
        }
      });
  }

  remove(c: Campania): void {
    if (!confirm(`¿Eliminar ${c.codigo}?`)) {
      return;
    }
    this.api.delete(c.id).subscribe({
      next: (res) => {
        if (res.success) {
          this.reload();
        }
      }
    });
  }
}
