import { Component, OnInit } from '@angular/core'; // <-- Añadir OnInit
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UsuariosService } from '../../services/usuarios'; // <-- Importar
import { PropiedadesService } from '../../services/propiedades'; // <-- Importar

@Component({
  selector: 'app-gestion-propiedades',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gestion-propiedades.html',
  styleUrls: ['./gestion-propiedades.scss']
})
export class GestionPropiedadesComponent implements OnInit { // <-- Implementar

  residentes: any[] = []; // Para el dropdown

  nuevaPropiedad = {
    numeroCasa: '',
    idPropietario: null // Empezar en nulo
  };

  mensaje: string = '';

  constructor(
    private usuariosService: UsuariosService,
    private propiedadesService: PropiedadesService
  ) {}

  // Al cargar, llenar el dropdown de residentes
  ngOnInit(): void {
    this.usuariosService.getResidentes().subscribe({
      next: (data) => this.residentes = data,
      error: (err) => this.mensaje = 'Error al cargar residentes.'
    });
  }

  onSubmit(): void {
    this.mensaje = 'Creando propiedad...';
    this.propiedadesService.crearPropiedad(this.nuevaPropiedad).subscribe({
      next: (res) => {
        this.mensaje = '¡Propiedad creada exitosamente!';
        this.nuevaPropiedad = { numeroCasa: '', idPropietario: null };
      },
      error: (err) => {
        this.mensaje = `Error: ${err.error?.message || 'No se pudo crear la propiedad.'}`;
      }
    });
  }
}