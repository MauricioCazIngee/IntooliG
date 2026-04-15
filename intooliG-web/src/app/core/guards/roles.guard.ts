import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/** Ejemplo: canActivate: [authGuard, rolesGuard(['Admin'])] */
export function rolesGuard(roles: string[]): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const rol = auth.getRole();
    if (rol && roles.includes(rol)) {
      return true;
    }
    void router.navigate(['/campanias']);
    return false;
  };
}
