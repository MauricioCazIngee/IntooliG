import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-actividad-pronto',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="ap-pronto">
      <h4>{{ titulo }}</h4>
      <p class="text-muted mb-0">Módulo en preparación.</p>
    </div>
  `,
  styles: [
    `
      .ap-pronto {
        max-width: 640px;
        margin: 24px auto;
        padding: 32px 24px;
        background: #fff;
        border: 1px solid #e3e6ef;
        border-radius: 8px;
        text-align: center;
      }
    `
  ]
})
export class ActividadProntoComponent {
  private readonly route = inject(ActivatedRoute);
  readonly titulo = (this.route.snapshot.data as { title?: string })?.title ?? 'Actividad publicitaria';
}
