// src/app/components/home/home.ts
import { Component, OnInit } from '@angular/core'; // <--- Añadir OnInit
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // <--- Añadir FormsModule
import { AuthService } from '../../services/auth';
import { CobrosService } from '../../services/cobros'; // <--- Importar
import { PagosService } from '../../services/pagos';   // <--- Importar
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule, 
    RouterLink 
  ],
  templateUrl: './home.html',
  styleUrls: ['./home.scss']
})
export class HomeComponent implements OnInit { // <--- Implementar OnInit

  deudas: any[] = []; // Array para las deudas
  mensajeDeudas: string = 'Cargando deudas...';
  
  // Modelo para el formulario de reporte
  reporteData = {
    fechaTransferencia: '',
    numeroReferencia: '',
    montoTotal: 0
  };
  
  archivoComprobante: File | null = null;
  idsDeudasSeleccionadas: number[] = [];
  mensajeReporte: string = '';

  constructor(
    private authService: AuthService,
    private cobrosService: CobrosService, // <--- Inyectar
    private pagosService: PagosService    // <--- Inyectar
  ) {}

  // ngOnInit se ejecuta cuando carga la página
  ngOnInit(): void {
    this.cargarDeudas();
  }

  cargarDeudas(): void {
    this.cobrosService.getMisDeudas().subscribe({
      next: (data) => {
        this.deudas = data.map(deuda => ({ ...deuda, seleccionado: false }));

        // Verificamos si hay *alguna* deuda pendiente
        const hayPendientes = data.some(d => d.estado === 'pendiente');
        if (!hayPendientes && data.length > 0) {
          this.mensajeDeudas = '¡Felicidades! Estás al día con tus pagos.';
        } else if (data.length === 0) {
          this.mensajeDeudas = 'No tienes deudas registradas.';
        } else {
          this.mensajeDeudas = '';
        }
      },
      error: (err) => {
        this.mensajeDeudas = 'Error al cargar las deudas.';
      }
    });
  }

  // Se llama cada vez que se marca/desmarca un checkbox
  actualizarSeleccion(): void {
    this.idsDeudasSeleccionadas = this.deudas
      .filter(d => d.seleccionado)
      .map(d => d.idCobro);

    let total = this.deudas
      .filter(d => d.seleccionado)
      .reduce((sum, deuda) => sum + deuda.monto, 0);

    // Actualiza el monto en el modelo del formulario
    this.reporteData.montoTotal = total;
  }

  // Se llama cuando el usuario selecciona un archivo
  onFileSelected(event: any): void {
    this.archivoComprobante = event.target.files[0] || null;
  }

  // Se llama al enviar el formulario de reporte
  onSubmitReporte(): void {
    if (this.idsDeudasSeleccionadas.length === 0) {
      alert('Debes seleccionar al menos una deuda para pagar.');
      return;
    }
    if (!this.archivoComprobante) {
      alert('Debes adjuntar un comprobante.');
      return;
    }

    this.mensajeReporte = 'Enviando reporte...';

    // 1. Crear el FormData (para enviar el archivo)
    const formData = new FormData();
    
    // 2. Añadir los datos del formulario
    formData.append('fechaTransferencia', this.reporteData.fechaTransferencia);
    formData.append('numeroReferencia', this.reporteData.numeroReferencia);
    formData.append('montoTransferido', this.reporteData.montoTotal.toString());
    formData.append('comprobante', this.archivoComprobante);
    
    // 3. Añadir la lista de IDs (es un array)
    this.idsDeudasSeleccionadas.forEach(id => {
      formData.append('idsCobrosPagados', id.toString());
    });

    // 4. Enviar a la API
    this.pagosService.reportarPago(formData).subscribe({
      next: (res) => {
        alert('¡Pago reportado exitosamente! Queda pendiente de validación.');
        // Limpiar y recargar
        this.reporteData = { fechaTransferencia: '', numeroReferencia: '', montoTotal: 0 };
        this.archivoComprobante = null;
        this.cargarDeudas();
      },
      error: (err) => {
        this.mensajeReporte = 'Error al reportar el pago.';
        alert(err.error?.message || 'Error desconocido.');
      }
    });
  }

  cerrarSesion(): void {
    this.authService.logout();
  }
}