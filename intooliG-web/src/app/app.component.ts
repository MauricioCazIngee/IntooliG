import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgIf } from '@angular/common';
import { AuthService } from './core/services/auth.service';
import { AppLayoutComponent } from './shared/layout/app-layout.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgIf, AppLayoutComponent],
  template: `
    <app-layout *ngIf="auth.isLoggedIn(); else authScreen" />
    <ng-template #authScreen>
      <router-outlet />
    </ng-template>
  `,
  styles: []
})
export class AppComponent {
  constructor(readonly auth: AuthService) {}
}
