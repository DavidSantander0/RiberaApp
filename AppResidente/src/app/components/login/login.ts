// src/app/components/login/login.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true, 
  imports: [
    FormsModule,  // Importar para [(ngModel)]
    CommonModule  // Importar para *ngIf
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.scss']
})
export class LoginComponent {
  
  loginData = {
    email: '', // Datos de prueba para el Residente
    password: ''
  };

  errorMensaje: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  onSubmit(): void {
    this.errorMensaje = ''; 

    this.authService.login(this.loginData).subscribe({
      next: (respuesta) => {
        console.log('Login Residente exitoso', respuesta);
        
        // 1. Guardar el token
        this.authService.guardarToken(respuesta.token);
        
        // 2. Redirigir al "home"
        this.router.navigate(['/home']); 
      },
      error: (err) => {
        this.errorMensaje = 'Error: Email o contraseña incorrectos.';
      }
    });
  }
}