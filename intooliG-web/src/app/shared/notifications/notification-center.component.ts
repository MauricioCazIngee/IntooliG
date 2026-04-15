import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { NotificationService } from './notification.service';

@Component({
  selector: 'app-notification-center',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      <div
        class="toast-item"
        *ngFor="let item of notifications.items$ | async"
        [class.e-toast-success]="item.type === 'success'"
        [class.e-toast-danger]="item.type === 'error'"
        [class.e-toast-warning]="item.type === 'warning'"
        [class.e-toast-info]="item.type === 'info'"
      >
        <span class="title">{{ getTitle(item.type) }}</span>
        <span class="message">{{ item.message }}</span>
        <button type="button" class="close-btn" (click)="notifications.remove(item.id)">x</button>
      </div>
    </div>
  `,
  styles: [
    `
      .toast-container {
        position: fixed;
        top: 14px;
        right: 14px;
        z-index: 2000;
        display: grid;
        gap: 8px;
      }
      .toast-item {
        min-width: 260px;
        max-width: 360px;
        border-radius: 4px;
        color: #fff;
        padding: 9px 34px 9px 10px;
        position: relative;
        box-shadow: 0 2px 8px rgb(0 0 0 / 20%);
      }
      .toast-item.e-toast-success {
        background: #28a745;
      }
      .toast-item.e-toast-danger {
        background: #d9534f;
      }
      .toast-item.e-toast-warning {
        background: #f0ad4e;
      }
      .toast-item.e-toast-info {
        background: #1872b6;
      }
      .title {
        font-weight: 700;
        margin-right: 6px;
      }
      .close-btn {
        position: absolute;
        right: 8px;
        top: 4px;
        border: none;
        background: transparent;
        color: inherit;
      }
    `
  ]
})
export class NotificationCenterComponent {
  constructor(readonly notifications: NotificationService) {}

  getTitle(type: string): string {
    switch (type) {
      case 'success':
        return 'Success!';
      case 'error':
        return 'Error!';
      case 'warning':
        return 'Warning!';
      default:
        return 'Info!';
    }
  }
}
