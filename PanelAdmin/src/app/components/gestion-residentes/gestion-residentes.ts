import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // <-- Importar
import { UsuariosService } from '../../services/usuarios'; // <-- Importar

@Component({
  selector: 'app-gestion-residentes',
  standalone: true,
  imports: [CommonModule, FormsModule], // <-- Añadir
  templateUrl: './gestion-residentes.html',
  styleUrls: ['./gestion-residentes.scss']
})
export class GestionResidentesComponent {

  // Modelo para el formulario
  nuevoResidente = {
    nombreCompleto: '',
    email: '',
    password: '',
    rol: 'residente' // Rol fijo
  };

  mensaje: string = '';

  constructor(private usuariosService: UsuariosService) {}

  onSubmit(): void {
    this.mensaje = 'Creando residente...';
    this.usuariosService.registrarUsuario(this.nuevoResidente).subscribe({
      next: (res) => {
        this.mensaje = '¡Residente creado exitosamente!';
        // Limpiar formulario
        this.nuevoResidente = { nombreCompleto: '', email: '', password: '', rol: 'residente' };
      },
      error: (err) => {
        this.mensaje = `Error: ${err.error?.message || 'No se pudo crear el residente.'}`;
      }
    });
  }
}