// src/app/services/propiedades.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PropiedadesService {
  // Apunta a la API de Propiedades
  private apiUrl = 'https://localhost:7011/api/Propiedades';

  constructor(private http: HttpClient) { }

  /** Obtiene la lista de todas las propiedades (para dropdowns) */
  getPropiedades(): Observable<any[]> {
    // Llama al GET /api/Propiedades
    return this.http.get<any[]>(this.apiUrl);
  }

  /** Crea una nueva propiedad y la asigna a un residente */
  crearPropiedad(datosPropiedad: any): Observable<any> {
    // Llama al POST /api/Propiedades
    return this.http.post(this.apiUrl, datosPropiedad);
  }
}