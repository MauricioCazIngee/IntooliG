import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, NgIf],
  template: `
    <h1>Iniciar sesión</h1>
    <p class="hint">Usuario demo: <code>admin&#64;demo.local</code> / <code>Demo123!</code></p>
    <form (ngSubmit)="submit()" class="form">
      <label>
        Email
        <input name="email" [(ngModel)]="email" type="email" required autocomplete="username" />
      </label>
      <label>
        Contraseña
        <input
          name="password"
          [(ngModel)]="password"
          type="password"
          required
          autocomplete="current-password"
        />
      </label>
      <button type="submit" [disabled]="loading">Entrar</button>
    </form>
    <p *ngIf="error" class="error">{{ error }}</p>
  `,
  styles: [
    `
      .form {
        display: grid;
        gap: 0.75rem;
        max-width: 320px;
      }
      label {
        display: grid;
        gap: 0.25rem;
        font-size: 0.9rem;
      }
      input {
        padding: 0.5rem 0.6rem;
        border: 1px solid #d1d5db;
        border-radius: 6px;
      }
      button {
        padding: 0.55rem 0.75rem;
        border-radius: 6px;
        border: none;
        background: #2563eb;
        color: white;
        font-weight: 600;
      }
      button:disabled {
        opacity: 0.6;
      }
      .error {
        color: #b91c1c;
      }
      .hint {
        color: #4b5563;
        font-size: 0.9rem;
      }
    `
  ]
})
export class LoginComponent {
  email = 'admin@demo.local';
  password = 'Demo123!';
  error = '';
  loading = false;

  constructor(
    private readonly auth: AuthService,
    private readonly router: Router
  ) {}

  submit(): void {
    this.error = '';
    this.loading = true;
    this.auth.login(this.email, this.password).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success) {
          void this.router.navigate(['/campanias']);
        } else {
          this.error = res.message || 'No se pudo iniciar sesión.';
        }
      },
      error: () => {
        this.loading = false;
        this.error = 'Credenciales inválidas o error de red.';
      }
    });
  }
}
