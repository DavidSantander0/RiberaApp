// src/app/components/login/login.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth'; // <--- Usamos tu nombre de archivo 'auth'

// --- AÑADIR IMPORTS para Standalone ---
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true, 
  imports: [
    FormsModule,  // <-- Importar para [(ngModel)]
    CommonModule  // <-- Importar para *ngIf
  ],
  templateUrl: './login.html', // <--- Usamos tu nombre de archivo
  styleUrls: ['./login.scss'] // <--- Usamos tu nombre de archivo
})
export class LoginComponent {
  // Objeto para guardar los datos del formulario
  loginData = {
    email: '', // Pre-llenado para pruebas
    password: '' // Pre-llenado para pruebas
  };

  errorMensaje: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onSubmit(): void {
    this.errorMensaje = ''; // Limpiar errores

    this.authService.login(this.loginData).subscribe({
      next: (respuesta) => {
        // --- CAMBIOS AQUÍ ---
        console.log('Login exitoso', respuesta);

        // 1. Guardar el token
        this.authService.guardarToken(respuesta.token);

        // 2. Redirigir al dashboard
        this.router.navigate(['/dashboard']); 
        // --- FIN DE CAMBIOS ---
      },
      error: (err) => {
        // ERROR
        console.error('Error en el login', err);
        this.errorMensaje = 'Error: Email o contraseña incorrectos.';
      }
    });
  }
}