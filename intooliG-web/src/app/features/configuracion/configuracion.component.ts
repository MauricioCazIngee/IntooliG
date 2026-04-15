import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-configuracion',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container">
      <div class="card card-custom">
        <div class="card-header card-header-custom">
          <h4 style="margin: 0">Configuración</h4>
        </div>
        <div class="card-body card-body-custom">
          <p class="mb-0">En construcción.</p>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .card-custom {
        border: 1px solid #ddd;
        border-radius: 4px;
        margin-bottom: 20px;
        box-shadow: 0 1px 3px rgb(0 0 0 / 10%);
      }
      .card-header-custom {
        background-color: #f5f5f5;
        border-bottom: 1px solid #ddd;
        padding: 15px;
      }
      .card-body-custom {
        padding: 15px;
      }
    `
  ]
})
export class ConfiguracionComponent {}
