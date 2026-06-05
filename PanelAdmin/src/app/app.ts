// src/app/app.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

// --- AÑADIR IMPORT ---
import { LoginComponent } from './components/login/login'; // <--- Usamos tu ruta 'login'

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    
  ],
  templateUrl: './app.html', // <--- Usamos tu nombre de archivo
  styleUrl: './app.scss' // <--- Usamos tu nombre de archivo
})
export class AppComponent {
  title = 'PanelAdmin';
}