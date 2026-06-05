// src/main.ts (CORREGIDO)
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app'; // <--- CORREGIDO

bootstrapApplication(AppComponent, appConfig) // <--- CORREGIDO
  .catch((err) => console.error(err));