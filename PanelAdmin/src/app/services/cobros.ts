// src/app/services/cobros.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CobrosService {
  // Apunta a la API de Cobros
  private apiUrl = 'https://localhost:7011/api/Cobros';

  constructor(private http: HttpClient) { }

  /** El admin crea una nueva multa para una propiedad */
  crearMulta(datosMulta: any): Observable<any> {
    // Llama al POST /api/Cobros/crear-multa
    return this.http.post(`${this.apiUrl}/crear-multa`, datosMulta);
  }
}