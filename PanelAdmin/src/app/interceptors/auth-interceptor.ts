// src/app/interceptors/auth.interceptor.ts
import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth'; // <-- Importa tu servicio

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const authService = inject(AuthService); // <-- Inyecta el servicio
  const token = authService.obtenerToken(); // <-- Obtiene el token guardado

  // Si el token existe, clona la petición y añade el Header de Autorización
  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}` // <-- ¡La magia!
      }
    });
    return next(clonedReq);
  }

  // Si no hay token, deja pasar la petición sin modificar (para el Login)
  return next(req);
};