// src/app/components/pagos-pendientes/pagos-pendientes.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PagosService } from '../../services/pagos'; // <-- Importa el servicio

@Component({
  selector: 'app-pagos-pendientes',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagos-pendientes.html',
  styleUrls: ['./pagos-pendientes.scss']
})
export class PagosPendientesComponent implements OnInit {

  pagos: any[] = []; // Array para guardar la lista
  mensaje: string = '';

  constructor(private pagosService: PagosService) {}

  // ngOnInit se ejecuta automáticamente cuando el componente carga
  ngOnInit(): void {
    this.cargarPagos();
  }

  cargarPagos(): void {
    this.mensaje = 'Cargando...';
    this.pagosService.getPagosPendientes().subscribe({
      next: (data) => {
        this.pagos = data;
        if (this.pagos.length === 0) {
          this.mensaje = 'No hay pagos pendientes de validación.';
        } else {
          this.mensaje = '';
        }
      },
      error: (err) => {
        this.mensaje = 'Error al cargar los pagos.';
        console.error(err);
      }
    });
  }

  // Método para el botón "Validar"
  onValidar(idReporte: number): void {
    if (!confirm('¿Estás seguro de que quieres validar este pago?')) {
      return;
    }

    this.pagosService.validarPago(idReporte).subscribe({
      next: (res) => {
        alert('¡Pago validado exitosamente!');
        // Recargar la lista
        this.cargarPagos(); 
      },
      error: (err) => {
        alert('Error al validar el pago.');
        console.error(err);
      }
    });
  }

  onRechazar(idReporte: number): void {
    if (!confirm('¿Estás seguro de que quieres RECHAZAR este pago? Las deudas se reactivarán.')) {
      return;
    }
    this.pagosService.rechazarPago(idReporte).subscribe({
      next: (res) => {
        alert('¡Pago rechazado exitosamente!');
        this.cargarPagos(); 
      },
      error: (err) => alert('Error al rechazar el pago.')
    });
  }
}