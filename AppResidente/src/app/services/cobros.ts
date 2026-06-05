// src/app/services/cobros.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CobrosService {
  // API de Cobros
  private apiUrl = 'https://localhost:7011/api/Cobros'; 

  constructor(private http: HttpClient) { }

  /** Obtiene las deudas pendientes del residente logueado */
  getMisDeudas(): Observable<any[]> {
    // No necesitamos el token, el interceptor lo añade
    return this.http.get<any[]>(`${this.apiUrl}/mis-deudas`);
  }
}