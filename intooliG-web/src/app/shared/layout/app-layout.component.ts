import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationCenterComponent } from '../notifications/notification-center.component';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [NgIf, RouterLink, RouterLinkActive, RouterOutlet, NotificationCenterComponent],
  template: `
    <div class="toast-default-section">
      <div class="control_wrapper">
        <app-notification-center />
      </div>
    </div>

    <nav class="main-menu">
      <ul>
        <li>
          <img src="assets/logoVMI_Head.svg" alt="Logo" class="img-responsive" style="width: 243px; height: 50px" />
        </li>
        <li routerLinkActive="active">
          <a routerLink="/campanias" style="color: #eeedee">
            <i class="fa faw fa-2x ingeenius-radar"></i>
            <span aria-hidden="true">Campañas</span>
          </a>
        </li>
        <li routerLinkActive="active">
          <a routerLink="/radar" style="color: #eeedee">
            <i class="fa faw fa-2x ingeenius-radar"></i>
            <span aria-hidden="true">Radar de Comunicaciones</span>
          </a>
        </li>
        <li *ngIf="auth.isAdmin()" class="dropdown" routerLinkActive="active">
          <a routerLink="/usuarios" style="color: #eeedee">
            <span class="fa faw fa-2x ingeenius-administracion"></span>Usuarios
          </a>
        </li>
        <li class="dropdown" routerLinkActive="active">
          <a routerLink="/reportes" style="color: #eeedee">
            <span class="fa faw fa-2x ingeenius-configuracion"></span>Reportes
          </a>
        </li>
        <li class="dropdown" routerLinkActive="active">
          <a routerLink="/configuracion" style="color: #eeedee">
            <span class="fa faw fa-2x ingeenius-configuracion"></span>Configuración
          </a>
        </li>
      </ul>

      <ul class="logout">
        <li>
          <a (click)="auth.logout()">
            <i class="fa fa-power-off fa-2x" style="width: 60px"></i>
            <span class="nav-text" style="font-size: small">{{ auth.getDisplayName() }}</span>
          </a>
        </li>
      </ul>
    </nav>

    <div class="jumbotron">
      <div>
        <router-outlet />
      </div>
    </div>

    <footer class="page-footer ml-3 font-small blue pt-3">
      <div class="footer-copyright text-right py-3">
        Intooligence ©
        <a href="https://ingeeniusmcr.com" style="color: black" target="_blank" title="Ir al sitio web de Ingeenius Media"
          >Ingeenius Media</a
        >
      </div>
    </footer>
  `,
  styles: []
})
export class AppLayoutComponent {
  constructor(readonly auth: AuthService) {}
}
