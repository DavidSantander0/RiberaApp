import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsuariosService } from '../../services/usuarios';
import { RouterLink, Router } from '@angular/router';

@Component({
  selector: 'app-lista-residentes',
  standalone: true,
  imports: [CommonModule, RouterLink], // Importante: RouterLink para el botón de crear
  templateUrl: './lista-residentes.html',
  styleUrls: ['./lista-residentes.scss']
})
export class ListaResidentesComponent implements OnInit {
  
  residentes: any[] = [];
  mensaje: string = 'Cargando...';

  constructor(
    private usuariosService: UsuariosService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargarResidentes();
  }

  cargarResidentes(): void {
    this.usuariosService.getResidentes().subscribe({
      next: (data) => {
        this.residentes = data;
        this.mensaje = '';
      },
      error: (err) => {
        this.mensaje = 'Error al cargar la lista.';
        console.error(err);
      }
    });
  }

  // --- FUNCIÓN ELIMINAR ---
  onEliminar(id: number, nombre: string): void {
    if (confirm(`¿Estás seguro de que deseas eliminar a "${nombre}"? Esta acción es irreversible.`)) {
      
      this.usuariosService.eliminarUsuario(id).subscribe({
        next: () => {
          alert('Usuario eliminado correctamente.');
          this.cargarResidentes(); // Recargar la tabla
        },
        error: (err) => {
          alert('No se pudo eliminar. Verifica que el usuario no tenga propiedades o deudas activas.');
          console.error(err);
        }
      });
    }
  }

  // --- FUNCIÓN EDITAR ---
  onEditar(id: number): void {
    // Si tienes tiempo de hacer la pantalla de edición, descomenta esto:
    // this.router.navigate(['/dashboard/residentes/editar', id]);
    
    // Si NO tienes tiempo, deja este alert para la presentación:
    alert(`Funcionalidad de Edición para el ID ${id}: \n(Aquí se abriría el formulario con los datos cargados)`);
  }
}