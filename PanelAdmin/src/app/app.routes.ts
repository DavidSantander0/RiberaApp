import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login';
import { DashboardComponent } from './components/dashboard/dashboard';
import { authGuard } from './guards/auth-guard';
import { PagosPendientesComponent } from './components/pagos-pendientes/pagos-pendientes';
import { GestionResidentesComponent } from './components/gestion-residentes/gestion-residentes';
import { GestionPropiedadesComponent } from './components/gestion-propiedades/gestion-propiedades';
import { GestionMultasComponent } from './components/gestion-multas/gestion-multas';
import { ListaResidentesComponent } from './components/lista-residentes/lista-residentes';
import { ListaPropiedadesComponent } from './components/lista-propiedades/lista-propiedades';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { 
    path: 'dashboard', 
    component: DashboardComponent,
    canActivate: [authGuard], 
    children: [
      // Pagos
      { path: 'pagos', component: PagosPendientesComponent },
      { path: 'multas/crear', component: GestionMultasComponent },

      // Residentes
      { path: 'residentes/crear', component: GestionResidentesComponent },
      { path: 'residentes/lista', component: ListaResidentesComponent }, // <-- NUEVA

      // Propiedades
      { path: 'propiedades/crear', component: GestionPropiedadesComponent },
      { path: 'propiedades/lista', component: ListaPropiedadesComponent }, // <-- NUEVA

      { path: '', redirectTo: 'pagos', pathMatch: 'full' } 
    ]
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
];