// src/app/components/perfil/perfil.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';
import { RouterLink } from '@angular/router'; // Importante para el botón volver

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [
    CommonModule, 
    RouterLink 
  ], 
  templateUrl: './perfil.html',
  styleUrls: ['./perfil.scss']
})
export class PerfilComponent implements OnInit {

  perfil: any = null;
  mensaje: string = 'Cargando perfil...';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.getMiPerfil().subscribe({
      next: (data) => {
        this.perfil = data;
        this.mensaje = '';
      },
      error: (err) => {
        this.mensaje = 'Error al cargar el perfil.';
        console.error(err);
      }
    });
  }

  // --- SOLUCIÓN AL ERROR ---
  // Creamos esta propiedad para calcular la inicial aquí, en lugar de en el HTML
  get iniciales(): string {
    // Si existe el perfil y tiene nombre, toma la 1ra letra. Si no, usa 'U'.
    return this.perfil?.nombreCompleto?.charAt(0).toUpperCase() || 'U';
  }
}