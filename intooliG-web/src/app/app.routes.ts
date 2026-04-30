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
    path: 'generar-evaluacion',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/generar-evaluacion/generar-evaluacion.component').then((m) => m.GenerarEvaluacionComponent)
  },
  {
    path: 'actividad-publicitaria/inversiones',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/actividad-publicitaria/inversiones/inversiones.component').then((m) => m.InversionesComponent)
  },
  {
    path: 'actividad-publicitaria/presiones',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/actividad-publicitaria/actividad-pronto.component').then((m) => m.ActividadProntoComponent),
    data: { title: 'Presiones' }
  },
  {
    path: 'actividad-publicitaria/segmento-medios',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/actividad-publicitaria/actividad-pronto.component').then((m) => m.ActividadProntoComponent),
    data: { title: 'Segmento por medios' }
  },
  {
    path: 'information-load/carga-global',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/information-load/carga-global/carga-global.component').then((m) => m.CargaGlobalComponent)
  },
  {
    path: 'information-load/adspend',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/information-load/information-load-pronto.component').then((m) => m.InformationLoadProntoComponent),
    data: { title: 'Adspend' }
  },
  {
    path: 'information-load/alcance',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/information-load/information-load-pronto.component').then((m) => m.InformationLoadProntoComponent),
    data: { title: 'Alcance' }
  },
  {
    path: 'information-load/tv-advertising',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/information-load/information-load-pronto.component').then((m) => m.InformationLoadProntoComponent),
    data: { title: 'TV Advertising' }
  },
  {
    path: 'information-load/tv-programs',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/information-load/information-load-pronto.component').then((m) => m.InformationLoadProntoComponent),
    data: { title: 'TV Programs' }
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
    path: 'power-bi/panel',
    canActivate: [authGuard],
    data: { powerBiMode: 'report' as const },
    loadComponent: () =>
      import('./features/power-bi/panel-power-bi/panel-power-bi.component').then(
        (m) => m.PanelPowerBiComponent
      )
  },
  {
    path: 'power-bi/dashboard',
    canActivate: [authGuard],
    data: { powerBiMode: 'dashboard' as const },
    loadComponent: () =>
      import('./features/power-bi/panel-power-bi/panel-power-bi.component').then(
        (m) => m.PanelPowerBiComponent
      )
  },
  {
    path: 'power-bi/tile',
    canActivate: [authGuard],
    data: { powerBiMode: 'tile' as const },
    loadComponent: () =>
      import('./features/power-bi/panel-power-bi/panel-power-bi.component').then(
        (m) => m.PanelPowerBiComponent
      )
  },
  {
    path: 'configuracion',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/configuracion/configuracion-shell.component').then((m) => m.ConfiguracionShellComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () =>
          import('./features/configuracion/configuracion.component').then((m) => m.ConfiguracionComponent)
      },
      {
        path: 'sector-categoria',
        loadComponent: () =>
          import('./features/configuracion/sector-categoria/sector-categoria.component').then(
            (m) => m.SectorCategoriaComponent
          )
      },
      {
        path: 'rubros-conceptos',
        loadComponent: () =>
          import('./features/configuracion/rubros-conceptos/rubros-conceptos.component').then((m) => m.RubrosConceptosComponent)
      },
      {
        path: 'region-ciudad',
        loadComponent: () =>
          import('./features/configuracion/region-ciudad/region-ciudad.component').then((m) => m.RegionCiudadComponent)
      },
      {
        path: 'medios',
        loadComponent: () =>
          import('./features/configuracion/medios/medios.component').then((m) => m.MediosComponent)
      },
      {
        path: 'fuentes',
        loadComponent: () =>
          import('./features/configuracion/fuentes/fuentes.component').then((m) => m.FuentesComponent)
      },
      {
        path: 'tipo-cambio',
        loadComponent: () =>
          import('./features/configuracion/tipo-cambio/tipo-cambio.component').then((m) => m.TipoCambioComponent)
      },
      {
        path: 'daypart',
        loadComponent: () =>
          import('./features/configuracion/daypart/daypart.component').then((m) => m.DayPartComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'campanias' }
];
