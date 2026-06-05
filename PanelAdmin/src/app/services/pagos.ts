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

  /**
   * Obtiene la lista de pagos pendientes de validación (solo Admin)
   */
  getPagosPendientes(): Observable<any[]> {
    // No necesitamos añadir el token, ¡el interceptor lo hace!
    return this.http.get<any[]>(`${this.apiUrl}/pendientes`);
  }

  /**
   * Valida un pago (solo Admin)
   * @param idReporte El ID del reporte a validar
   */
  validarPago(idReporte: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/validar/${idReporte}`, {});
  }
  rechazarPago(idReporte: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/rechazar/${idReporte}`, {});
  }
}