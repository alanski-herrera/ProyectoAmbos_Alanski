import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Uniforme } from '../../models/uniforme.model';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
  @Input() uniforme!: Uniforme;

  constructor(private router: Router) {}

  verDetalle() {
    this.router.navigate(['/producto', this.uniforme.idUniforme]);
  }

  get imagenPrincipal(): string {
    return this.uniforme.imagen1 || 'assets/images/placeholder.png';
  }
}