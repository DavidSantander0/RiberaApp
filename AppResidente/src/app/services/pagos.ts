// src/app/services/pagos.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PagosService {
  // API de Pagos
  private apiUrl = 'https://localhost:7011/api/Pagos'; 

  constructor(private http: HttpClient) { }

  /** Reporta un nuevo pago (sube el formulario con el archivo) */
  reportarPago(formData: FormData): Observable<any> {
    // El interceptor también añade el token aquí
    return this.http.post(`${this.apiUrl}/reportar`, formData);
  }
}