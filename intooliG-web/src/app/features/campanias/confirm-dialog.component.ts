import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div *ngIf="visible" class="dialog-backdrop">
      <div class="dialog-card">
        <h5>{{ title }}</h5>
        <p>{{ message }}</p>
        <div class="actions">
          <button type="button" class="btn btn-light" [disabled]="loading" (click)="cancel.emit()">
            {{ cancelText }}
          </button>
          <button type="button" class="btn btn-confirm" [disabled]="loading" (click)="confirm.emit()">
            {{ loading ? 'Procesando...' : confirmText }}
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
        width: min(420px, calc(100% - 24px));
        background: #fff;
        border-radius: 6px;
        padding: 16px;
      }
      .dialog-card h5 {
        margin: 0 0 8px;
      }
      .dialog-card p {
        margin: 0 0 14px;
      }
      .actions {
        display: flex;
        justify-content: end;
        gap: 8px;
      }
      .btn-confirm {
        background: #d8514a;
        border: 1px solid #d43f3a;
        color: #fff;
      }
    `
  ]
})
export class ConfirmDialogComponent {
  @Input() visible = false;
  @Input() loading = false;
  @Input() title = 'Confirmación';
  @Input() message = '¿Desea continuar?';
  @Input() confirmText = 'Confirmar';
  @Input() cancelText = 'Cancelar';

  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
}
