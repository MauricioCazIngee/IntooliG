import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface NotificationItem {
  id: number;
  type: NotificationType;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly itemsSubject = new BehaviorSubject<NotificationItem[]>([]);
  readonly items$ = this.itemsSubject.asObservable();
  private nextId = 1;

  success(message: string): void {
    this.push('success', message);
  }

  error(message: string): void {
    this.push('error', message);
  }

  warning(message: string): void {
    this.push('warning', message);
  }

  info(message: string): void {
    this.push('info', message);
  }

  remove(id: number): void {
    this.itemsSubject.next(this.itemsSubject.value.filter((x) => x.id !== id));
  }

  private push(type: NotificationType, message: string): void {
    const id = this.nextId++;
    const item: NotificationItem = { id, type, message };
    this.itemsSubject.next([...this.itemsSubject.value, item]);

    setTimeout(() => this.remove(id), 3500);
  }
}
