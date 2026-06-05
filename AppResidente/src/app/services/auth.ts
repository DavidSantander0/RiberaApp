// src/app/services/auth.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // URL de tu API de .NET
  private apiUrl = 'https://localhost:7011/api/Usuarios';
  private readonly TOKEN_KEY = 'residente_token'; // Clave diferente a la del admin

  constructor(
    private http: HttpClient,
    private router: Router
  ) { }

  login(datosLogin: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, datosLogin);
  }

  guardarToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  obtenerToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  estaLogueado(): boolean {
    return !!this.obtenerToken(); 
  }
  getMiPerfil(): Observable<any> {
    // El interceptor añade el token automáticamente
    return this.http.get(`${this.apiUrl}/perfil`);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.router.navigate(['/login']);
  }
}