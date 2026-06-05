// src/main.ts (CORREGIDO)
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app'; // <--- CORREGIDO (era 'App')

bootstrapApplication(AppComponent, appConfig) // <--- CORREGIDO (era 'App')
  .catch((err) => console.error(err));