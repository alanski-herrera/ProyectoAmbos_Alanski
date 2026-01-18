import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HeaderComponent } from '../../components/header/header.component';
import { FiltrosComponent } from '../../components/filtros/filtros.component';
import { ProductCardComponent } from '../../components/product-card/product-card.component';
import { ApiService } from '../../services/api.service';
import { Uniforme, FiltrosUniforme } from '../../models/uniforme.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FiltrosComponent, ProductCardComponent, FormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  uniformes: Uniforme[] = [];
  uniformesOriginales: Uniforme[] = [];
  loading = true;
  error: string | null = null;
  ordenSeleccionado: string = 'recientes';

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.cargarUniformes();
  }

  cargarUniformes(filtros?: FiltrosUniforme) {
    this.loading = true;
    this.error = null;

    this.apiService.getUniformes(filtros).subscribe({
      next: (data) => {
        this.uniformesOriginales = data;
        this.aplicarOrdenamiento();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al cargar uniformes:', err);
        this.error = 'Error al cargar los productos. Por favor, intenta de nuevo.';
        this.loading = false;
      }
    });
  }

  onFiltrosChanged(filtros: FiltrosUniforme) {
    this.cargarUniformes(filtros);
  }

  onOrdenChanged() {
    this.aplicarOrdenamiento();
  }

  aplicarOrdenamiento() {
    let uniformesOrdenados = [...this.uniformesOriginales];

    switch (this.ordenSeleccionado) {
      case 'recientes':
        uniformesOrdenados.sort((a, b) =>
          new Date(b.fechaIngreso).getTime() - new Date(a.fechaIngreso).getTime()
        );
        break;
      case 'baratos':
        uniformesOrdenados.sort((a, b) => a.precio - b.precio);
        break;
      case 'caros':
        uniformesOrdenados.sort((a, b) => b.precio - a.precio);
        break;
    }

    this.uniformes = uniformesOrdenados;
  }
}