import { Component } from '@angular/core';
import { NgIf } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
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
        <li [class.active]="isActividadPublicitariaSection()" class="dropdown">
          <a style="color: #eeedee; cursor: default">
            <i class="fa fa-area-chart faw fa-2x"></i>
            <span aria-hidden="true">Actividad Publicitaria</span>
          </a>
          <ul class="radar-submenu">
            <li routerLinkActive="active">
              <a routerLink="/actividad-publicitaria/inversiones" style="color: #eeedee">
                <i class="fa fa-line-chart nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Inversiones</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/actividad-publicitaria/presiones" style="color: #eeedee">
                <i class="fa fa-hand-pointer-o nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Presiones</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/actividad-publicitaria/segmento-medios" style="color: #eeedee">
                <i class="fa fa-pie-chart nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Segmento por medios</span>
              </a>
            </li>
          </ul>
        </li>
        <li [class.active]="isInformationLoadSection()" class="dropdown">
          <a style="color: #eeedee; cursor: default">
            <i class="fa fa-upload faw fa-2x"></i>
            <span aria-hidden="true">Information Load</span>
          </a>
          <ul class="radar-submenu">
            <li routerLinkActive="active">
              <a routerLink="/information-load/carga-global" style="color: #eeedee">
                <i class="fa fa-globe nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Carga Global</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/information-load/adspend" style="color: #eeedee">
                <i class="fa fa-money nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Adspend</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/information-load/alcance" style="color: #eeedee">
                <i class="fa fa-bullseye nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Alcance</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/information-load/tv-advertising" style="color: #eeedee">
                <i class="fa fa-television nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">TV Advertising</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/information-load/tv-programs" style="color: #eeedee">
                <i class="fa fa-list-alt nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">TV Programs</span>
              </a>
            </li>
          </ul>
        </li>
        <li [class.active]="isRadarSection()" class="dropdown">
          <a style="color: #eeedee; cursor: default">
            <i class="fa faw fa-2x ingeenius-radar"></i>
            <span aria-hidden="true">Radar de Comunicaciones</span>
          </a>
          <ul class="radar-submenu">
            <li routerLinkActive="active">
              <a routerLink="/generar-evaluacion" style="color: #eeedee">
                <i class="fa fa-magic nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Generar Evaluación</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/radar" style="color: #eeedee">
                <i class="fa fa-line-chart nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Análisis radar</span>
              </a>
            </li>
          </ul>
        </li>
        <li *ngIf="auth.isAdmin()" class="dropdown" routerLinkActive="active">
          <a routerLink="/usuarios" style="color: #eeedee">
            <span class="fa faw fa-2x ingeenius-administracion"></span>Usuarios
          </a>
        </li>
        <li [class.active]="isPowerBiSection()" class="dropdown">
          <a style="color: #eeedee; cursor: default">
            <i class="fa fa-bar-chart faw fa-2x" aria-hidden="true"></i>
            <span aria-hidden="true">Power BI</span>
          </a>
          <ul class="radar-submenu">
            <li routerLinkActive="active">
              <a routerLink="/power-bi/panel" style="color: #eeedee">
                <i class="fa fa-tachometer nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Panel Power BI (informe)</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/power-bi/dashboard" style="color: #eeedee">
                <i class="fa fa-th-large nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Dashboard</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/power-bi/tile" style="color: #eeedee">
                <i class="fa fa-square nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Mosaico</span>
              </a>
            </li>
          </ul>
        </li>
        <li class="dropdown" routerLinkActive="active">
          <a routerLink="/reportes" style="color: #eeedee">
            <span class="fa faw fa-2x ingeenius-configuracion"></span>Reportes
          </a>
        </li>
        <li [class.active]="isConfigSection()" class="dropdown">
          <a style="color: #eeedee; cursor: default">
            <span class="fa faw fa-2x ingeenius-configuracion"></span>Configuración
          </a>
          <ul class="radar-submenu">
            <li>
              <span class="nav-text nav-disabled" style="font-size: 13px; opacity: 0.55; padding-left: 52px; display: inline-block">
                <i class="fa fa-building-o nav-sub-icon" aria-hidden="true"></i>Cliente (próximamente)
              </span>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/configuracion" style="color: #eeedee">
                <i class="fa fa-cog nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">General</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/configuracion/sector-categoria" style="color: #eeedee">
                <i class="fa fa-sitemap nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Sector - Categoría</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/configuracion/rubros-conceptos" style="color: #eeedee">
                <i class="fa fa-tags nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Rubros - Conceptos</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/configuracion/region-ciudad" style="color: #eeedee">
                <i class="fa fa-map-marker nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Region - Ciudad</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/configuracion/medios" style="color: #eeedee">
                <i class="fa fa-bullhorn nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Medios</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/configuracion/fuentes" style="color: #eeedee">
                <i class="fa fa-database nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Fuentes</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/configuracion/tipo-cambio" style="color: #eeedee">
                <i class="fa fa-exchange nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">Tipo Cambio</span>
              </a>
            </li>
            <li routerLinkActive="active">
              <a routerLink="/configuracion/daypart" style="color: #eeedee">
                <i class="fa fa-clock-o nav-sub-icon" aria-hidden="true"></i>
                <span class="nav-text" style="font-size: 13px">DayParts</span>
              </a>
            </li>
            <li>
              <span class="nav-text nav-disabled" style="font-size: 13px; opacity: 0.55; padding-left: 52px; display: inline-block">
                <i class="fa fa-shield nav-sub-icon" aria-hidden="true"></i>Security (próximamente)
              </span>
            </li>
          </ul>
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
  styles: [
    `
      .radar-submenu {
        list-style: none;
        margin: 0;
        padding: 0;
        background: rgba(0, 0, 0, 0.12);
      }
      .main-menu li.dropdown {
        position: relative;
      }
      .main-menu li.dropdown > .radar-submenu {
        opacity: 0;
        visibility: hidden;
        max-height: 0;
        overflow: hidden;
        pointer-events: none;
        transition:
          opacity 0.18s ease,
          max-height 0.2s ease,
          visibility 0s linear 0.3s;
      }
      .main-menu li.dropdown:hover > .radar-submenu,
      .main-menu li.dropdown.active > .radar-submenu {
        opacity: 1;
        visibility: visible;
        max-height: 1000px;
        pointer-events: auto;
        transition:
          opacity 0.15s ease,
          max-height 0.2s ease,
          visibility 0s;
      }
      .main-menu:not(:hover):not(.expanded) li.dropdown:hover > .radar-submenu {
        position: absolute;
        left: 100%;
        top: 0;
        min-width: 250px;
        z-index: 1200;
        background: #1a2238;
        box-shadow: 0 8px 20px rgba(0, 0, 0, 0.28);
      }
      .main-menu:hover li.dropdown:hover > .radar-submenu,
      nav.main-menu.expanded li.dropdown:hover > .radar-submenu {
        position: static;
        left: auto;
        top: auto;
        box-shadow: none;
      }
      .radar-submenu li > a {
        padding-left: 52px !important;
      }
      .radar-submenu .nav-text {
        width: 198px;
      }
      .nav-sub-icon {
        width: 16px;
        margin-right: 8px;
        text-align: center;
        opacity: 0.9;
      }
      .nav-disabled .nav-sub-icon {
        opacity: 0.55;
      }
    `
  ]
})
export class AppLayoutComponent {
  constructor(
    readonly auth: AuthService,
    private readonly router: Router
  ) {}

  isRadarSection(): boolean {
    const path = this.router.url.split('?')[0];
    return path === '/radar' || path === '/generar-evaluacion';
  }

  isConfigSection(): boolean {
    const path = this.router.url.split('?')[0];
    return path === '/configuracion' || path.startsWith('/configuracion/');
  }

  isActividadPublicitariaSection(): boolean {
    const path = this.router.url.split('?')[0];
    return path === '/actividad-publicitaria' || path.startsWith('/actividad-publicitaria/');
  }

  isInformationLoadSection(): boolean {
    const path = this.router.url.split('?')[0];
    return path === '/information-load' || path.startsWith('/information-load/');
  }

  isPowerBiSection(): boolean {
    const path = this.router.url.split('?')[0];
    return path === '/power-bi' || path.startsWith('/power-bi/');
  }
}
