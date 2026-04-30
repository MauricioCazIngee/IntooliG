import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/** Contenedor para rutas hijas bajo `/configuracion/*`. */
@Component({
  selector: 'app-configuracion-shell',
  standalone: true,
  imports: [RouterOutlet],
  template: '<router-outlet />'
})
export class ConfiguracionShellComponent {}
