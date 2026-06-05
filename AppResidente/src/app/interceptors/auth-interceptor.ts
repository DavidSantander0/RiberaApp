// src/app/interceptors/auth.interceptor.ts
import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth'; 

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  
  const authService = inject(AuthService); 
  const token = authService.obtenerToken(); 

  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}` 
      }
    });
    return next(clonedReq);
  }

  return next(req);
};