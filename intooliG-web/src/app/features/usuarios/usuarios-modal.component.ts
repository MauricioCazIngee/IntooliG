import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from './usuarios.service';

export type UsuarioFormPayload = {
  email: string;
  nombre: string;
  rol: string;
  clienteId: number;
  password?: string;
};

@Component({
  selector: 'app-usuarios-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './usuarios-modal.component.html',
  styleUrl: './usuarios-modal.component.css'
})
export class UsuariosModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() saving = false;
  @Input() usuario: Usuario | null = null;
  @Input() existingEmails: string[] = [];

  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<UsuarioFormPayload>();

  form = {
    email: '',
    nombre: '',
    rol: 'Usuario',
    clienteId: 1,
    password: ''
  };
  error = '';

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible) {
      this.error = '';
      if (this.usuario) {
        this.form = {
          email: this.usuario.email,
          nombre: this.usuario.nombre,
          rol: this.usuario.rol,
          clienteId: this.usuario.clienteId ?? 1,
          password: ''
        };
      } else {
        this.form = { email: '', nombre: '', rol: 'Usuario', clienteId: 1, password: '' };
      }
    }
  }

  get isEdit(): boolean {
    return !!this.usuario;
  }

  submit(): void {
    this.error = '';
    const email = this.form.email.trim();
    const nombre = this.form.nombre.trim();

    if (!email) {
      this.error = 'El email es requerido.';
      return;
    }
    if (!nombre) {
      this.error = 'El nombre es requerido.';
      return;
    }

    if (this.existingEmails.some((x) => x.toLowerCase() === email.toLowerCase())) {
      this.error = 'El email ya existe.';
      return;
    }

    if (!this.isEdit && this.form.password.trim().length < 6) {
      this.error = 'La contraseña inicial debe tener al menos 6 caracteres.';
      return;
    }

    const clienteId = Number(this.form.clienteId);
    if (!Number.isFinite(clienteId) || clienteId <= 0) {
      this.error = 'Cliente (ID) debe ser un número mayor a cero.';
      return;
    }

    this.save.emit({
      email,
      nombre,
      rol: this.form.rol,
      clienteId,
      password: this.isEdit ? undefined : this.form.password
    });
  }

  cancel(): void {
    if (!this.saving) {
      this.close.emit();
    }
  }
}
