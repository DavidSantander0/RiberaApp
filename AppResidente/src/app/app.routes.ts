// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { HomeComponent } from './components/home/home';
import { authGuard } from './guards/auth-guard';

// --- AÑADIR IMPORT ---
import { PerfilComponent } from './components/perfil/perfil';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'home', component: HomeComponent, canActivate: [authGuard] },
  
  // --- AÑADIR RUTA ---
  { path: 'perfil', component: PerfilComponent, canActivate: [authGuard] }, 

  { path: '', redirectTo: '/login', pathMatch: 'full' },
];