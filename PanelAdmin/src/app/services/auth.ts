// src/app/services/auth.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router'; // <-- Importar Router

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7011/api/Usuarios';
  private readonly TOKEN_KEY = 'ribera2_token'; // Clave para guardar el token

  constructor(
    private http: HttpClient,
    private router: Router // <-- Inyectar Router
  ) { }

  login(datosLogin: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, datosLogin);
  }

  // --- NUEVOS MÉTODOS ---

  /** Guarda el token JWT en el localStorage */
  guardarToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  /** Obtiene el token JWT del localStorage */
  obtenerToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /** Revisa si el usuario está logueado (si existe un token) */
  estaLogueado(): boolean {
    return !!this.obtenerToken(); // Devuelve true si el token existe
  }

  /** Cierra la sesión del usuario */
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    // Redirige al login
    this.router.navigate(['/login']);
  }
}