import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      const isLogin = req.url.includes('/api/auth/login');
      if (err.status === 401 && !isLogin) {
        auth.logout();
        void router.navigate(['/login']);
      }
      return throwError(() => err);
    })
  );
};
