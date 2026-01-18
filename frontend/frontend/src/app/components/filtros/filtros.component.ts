import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Marca } from '../../models/marca.model';
import { TipoPrenda } from '../../models/tipo-prenda.model';
import { FiltrosUniforme } from '../../models/uniforme.model';

@Component({
  selector: 'app-filtros',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './filtros.component.html',
  styleUrls: ['./filtros.component.scss']
})
export class FiltrosComponent implements OnInit {
  @Output() filtrosChanged = new EventEmitter<FiltrosUniforme>();

  marcas: Marca[] = [];
  tiposPrenda: TipoPrenda[] = [];
  talles: string[] = [];
  
  filtrosActivos: FiltrosUniforme = {};
  mostrarFiltros = false;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.cargarDatosFiltros();
  }

  cargarDatosFiltros() {
    this.apiService.getMarcas().subscribe(marcas => {
      this.marcas = marcas;
    });

    this.apiService.getTiposPrenda().subscribe(tipos => {
      this.tiposPrenda = tipos;
    });

    this.apiService.getTallesDisponibles().subscribe(talles => {
      this.talles = talles;
    });
  }

  toggleFiltros() {
    this.mostrarFiltros = !this.mostrarFiltros;
  }

  aplicarFiltros() {
    this.filtrosChanged.emit(this.filtrosActivos);
    this.mostrarFiltros = false;
  }

  limpiarFiltros() {
    this.filtrosActivos = {};
    this.filtrosChanged.emit(this.filtrosActivos);
  }

  get cantidadFiltrosActivos(): number {
    let count = 0;
    if (this.filtrosActivos.talle) count++;
    if (this.filtrosActivos.idMarca) count++;
    if (this.filtrosActivos.idTipoPrenda) count++;
    if (this.filtrosActivos.precioMin || this.filtrosActivos.precioMax) count++;
    return count;
  }
}