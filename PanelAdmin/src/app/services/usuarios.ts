// src/app/services/usuarios.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UsuariosService {
  // Apunta a la API de Usuarios
  private apiUrl = 'https://localhost:7011/api/Usuarios'; 

  constructor(private http: HttpClient) { }

  /** Obtiene la lista de todos los residentes (para dropdowns) */
  getResidentes(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/residentes`);
  }

  /** Registra un nuevo usuario (el admin crea un residente) */
  registrarUsuario(datosUsuario: any): Observable<any> {
    // Llama al endpoint de registro que ya teníamos
    return this.http.post(`${this.apiUrl}/registro`, datosUsuario);
  }

  eliminarUsuario(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Usuarios/${id}`);
  }
}