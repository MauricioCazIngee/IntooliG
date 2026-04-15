import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Campania, SaveCampaniaRequest } from './campanias.service';

type ModalForm = {
  codigo: string;
  nombre: string;
  descripcion: string;
  fechaInicio: string;
  fechaFin: string;
  activa: boolean;
};

@Component({
  selector: 'app-campanias-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './campanias-modal.component.html',
  styleUrl: './campanias-modal.component.css'
})
export class CampaniasModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() saving = false;
  @Input() campania: Campania | null = null;
  @Input() existingCodes: string[] = [];

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<SaveCampaniaRequest>();

  form: ModalForm = this.emptyForm();
  error = '';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible) {
      this.error = '';
      this.form = this.campania
        ? {
            codigo: this.campania.codigo,
            nombre: this.campania.nombre,
            descripcion: this.campania.descripcion || '',
            fechaInicio: this.toInputDate(this.campania.fechaInicio),
            fechaFin: this.toInputDate(this.campania.fechaFin),
            activa: this.campania.activa
          }
        : this.emptyForm();
    }
  }

  title(): string {
    return this.campania ? 'Editar campaña' : 'Nueva campaña';
  }

  submit(): void {
    this.error = '';
    if (!this.form.codigo.trim()) {
      this.error = 'El código es requerido.';
      return;
    }
    if (!this.form.nombre.trim()) {
      this.error = 'El nombre es requerido.';
      return;
    }

    const duplicateCode = this.existingCodes.some(
      (x) => x.toLowerCase() === this.form.codigo.trim().toLowerCase()
    );
    if (duplicateCode) {
      this.error = 'El código ya existe. Debe ser único.';
      return;
    }

    const payload: SaveCampaniaRequest = {
      codigo: this.form.codigo.trim(),
      nombre: this.form.nombre.trim(),
      descripcion: this.form.descripcion.trim() || null,
      fechaInicio: this.form.fechaInicio || null,
      fechaFin: this.form.fechaFin || null,
      activa: this.form.activa
    };

    this.save.emit(payload);
  }

  cancel(): void {
    if (!this.saving) {
      this.close.emit();
    }
  }

  private emptyForm(): ModalForm {
    return {
      codigo: '',
      nombre: '',
      descripcion: '',
      fechaInicio: '',
      fechaFin: '',
      activa: true
    };
  }

  private toInputDate(value: string | null): string {
    if (!value) {
      return '';
    }
    return value.substring(0, 10);
  }
}
