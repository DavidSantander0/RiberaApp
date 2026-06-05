// src/app/app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

// Asegúrate que el nombre 'routes' coincida con tu 'app.routes.ts'
import { routes } from './app.routes'; 

// Importar
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './interceptors/auth-interceptor'; // <-- El interceptor

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    
    // Proveer HttpClient Y registrar el interceptor
    provideHttpClient(
      withInterceptors([authInterceptor]) 
    )
  ]
};