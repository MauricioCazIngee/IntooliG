import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { rolesGuard } from './core/guards/roles.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'campanias' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login.component').then((m) => m.LoginComponent)
  },
  {
    path: 'campanias',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/campanias/campanias.component').then((m) => m.CampaniasComponent)
  },
  {
    path: 'radar',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/radar/radar.component').then((m) => m.RadarComponent)
  },
  {
    path: 'usuarios',
    canActivate: [authGuard, rolesGuard(['Admin'])],
    loadComponent: () =>
      import('./features/usuarios/usuarios.component').then((m) => m.UsuariosComponent)
  },
  {
    path: 'reportes',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/reportes/reportes.component').then((m) => m.ReportesComponent)
  },
  {
    path: 'configuracion',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/configuracion/configuracion.component').then((m) => m.ConfiguracionComponent)
  },
  { path: '**', redirectTo: 'campanias' }
];
