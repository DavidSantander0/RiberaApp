import { Component, OnInit } from '@angular/core'; // <-- Añadir OnInit
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PropiedadesService } from '../../services/propiedades'; // <-- Importar
import { CobrosService } from '../../services/cobros'; // <-- Importar

@Component({
  selector: 'app-gestion-multas',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gestion-multas.html',
  styleUrls: ['./gestion-multas.scss']
})
export class GestionMultasComponent implements OnInit { // <-- Implementar

  propiedades: any[] = []; // Para el dropdown

  nuevaMulta = {
    idPropiedad: null,
    monto: 0.00,
    descripcion: ''
  };

  mensaje: string = '';

  constructor(
    private propiedadesService: PropiedadesService,
    private cobrosService: CobrosService
  ) {}

  // Al cargar, llenar el dropdown de propiedades
  ngOnInit(): void {
    this.propiedadesService.getPropiedades().subscribe({
      next: (data) => this.propiedades = data,
      error: (err) => this.mensaje = 'Error al cargar propiedades.'
    });
  }

  onSubmit(): void {
    this.mensaje = 'Creando multa...';
    this.cobrosService.crearMulta(this.nuevaMulta).subscribe({
      next: (res) => {
        this.mensaje = '¡Multa creada exitosamente!';
        this.nuevaMulta = { idPropiedad: null, monto: 0.00, descripcion: '' };
      },
      error: (err) => {
        this.mensaje = `Error: ${err.error?.message || 'No se pudo crear la multa.'}`;
      }
    });
  }
}