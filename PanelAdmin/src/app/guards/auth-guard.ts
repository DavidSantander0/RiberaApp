// src/app/guards/auth.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth'; // <-- Importa tu servicio

export const authGuard: CanActivateFn = (route, state) => {

  const authService = inject(AuthService); // <-- Inyecta el servicio
  const router = inject(Router); // <-- Inyecta el router

  // Lógica del portero:
  if (authService.estaLogueado()) {
    return true; // Tiene token, puede pasar
  } else {
    // No tiene token, redirigir al login
    router.navigate(['/login']);
    return false; 
  }
};