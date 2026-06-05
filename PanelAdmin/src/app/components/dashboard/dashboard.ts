import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router'; // <-- Importar RouterLinkActive

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, 
    RouterOutlet, 
    RouterLink, 
    RouterLinkActive // <-- Importante para resaltar el menú activo
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss']
})
export class DashboardComponent {
  
  now = new Date(); // Para mostrar la fecha en el header

  constructor(private authService: AuthService) {}

  cerrarSesion(): void {
    // Preguntar antes de salir (opcional, pero recomendado)
    if(confirm('¿Desea cerrar su sesión?')) {
      this.authService.logout();
    }
  }
}