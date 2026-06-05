// src/app/app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes'; 

// Importar
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './interceptors/auth-interceptor'; // <-- Importa tu interceptor

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),

    // --- MODIFICA ESTA LÍNEA ---
    provideHttpClient(
      withInterceptors([authInterceptor]) // <-- Registra el interceptor
    )
  ]
};