import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-information-load-pronto',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="pronto-card">
      <h4 class="mb-2">{{ title }}</h4>
      <p class="text-muted mb-0">Pantalla en preparación para el módulo Information Load.</p>
    </div>
  `,
  styles: [
    `
      .pronto-card {
        max-width: 700px;
        margin: 24px auto;
        padding: 28px;
        background: #fff;
        border: 1px solid #e3e6ef;
        border-radius: 8px;
        text-align: center;
      }
    `
  ]
})
export class InformationLoadProntoComponent {
  private readonly route = inject(ActivatedRoute);
  readonly title = (this.route.snapshot.data as { title?: string })?.title ?? 'Information Load';
}
