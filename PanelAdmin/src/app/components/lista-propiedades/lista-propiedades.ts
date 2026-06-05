import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PropiedadesService } from '../../services/propiedades';

@Component({
  selector: 'app-lista-propiedades',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lista-propiedades.html',
})
export class ListaPropiedadesComponent implements OnInit {
  propiedades: any[] = [];
  mensaje: string = 'Cargando...';
  constructor(private propiedadesService: PropiedadesService) {}

  ngOnInit(): void {
    this.propiedadesService.getPropiedades().subscribe({
      next: (data) => {
        this.propiedades = data;
        this.mensaje = data.length === 0 ? 'No se encontraron propiedades.' : '';
      },
      error: (err) => this.mensaje = 'Error al cargar la lista.'
    });
  }
}