import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { CartService } from '../../services/cart.service';
import { Uniforme } from '../../models/uniforme.model';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  uniforme: Uniforme | null = null;
  loading = true;
  error: string | null = null;
  imagenSeleccionada = 0;
  imagenes: string[] = [];
  imagenZoom = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private apiService: ApiService,
    private cartService: CartService
  ) { }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.cargarProducto(+id);
    }
  }

  get isDisponible(): boolean {
    return this.uniforme?.estado === 'Disponible';
  }

  cargarProducto(id: number) {
    this.loading = true;
    this.error = null;

    this.apiService.getUniforme(id).subscribe({
      next: (data) => {
        this.uniforme = data;
        this.prepararImagenes();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al cargar producto:', err);
        this.error = 'No se pudo cargar el producto';
        this.loading = false;
      }
    });
  }

  prepararImagenes() {
    if (!this.uniforme) return;

    this.imagenes = [];
    if (this.uniforme.imagen1) this.imagenes.push(this.uniforme.imagen1);
    if (this.uniforme.imagen2) this.imagenes.push(this.uniforme.imagen2);
    if (this.uniforme.imagen3) this.imagenes.push(this.uniforme.imagen3);

    if (this.imagenes.length === 0) {
      this.imagenes.push('assets/images/placeholder.png');
    }
  }

  seleccionarImagen(index: number) {
    this.imagenSeleccionada = index;
  }

  abrirZoom() {
    this.imagenZoom = true;
    document.body.style.overflow = 'hidden';
  }

  cerrarZoom() {
    this.imagenZoom = false;
    document.body.style.overflow = 'auto';
  }

  volver() {
    this.router.navigate(['/']);
  }

  showToast(message: string) {
    const toast = document.createElement('div');
    toast.className = 'toast-notification';
    toast.textContent = message;
    document.body.appendChild(toast);

    // Trigger reflow
    toast.offsetHeight;

    toast.classList.add('show');

    setTimeout(() => {
      toast.classList.remove('show');
      setTimeout(() => {
        document.body.removeChild(toast);
      }, 300);
    }, 2000);
  }

  agregarAlCarrito() {
    if (!this.uniforme) return;

    if (this.cartService.isInCart(this.uniforme.idUniforme)) {
      this.showToast('Este producto ya está en tu carrito');
      return;
    }

    this.cartService.addToCart(this.uniforme);
    this.showToast('¡Agregado al carrito!');
  }

  comprarAhora() {
    if (!this.uniforme) return;

    // Si no está en el carrito, agregarlo
    if (!this.cartService.isInCart(this.uniforme.idUniforme)) {
      this.cartService.addToCart(this.uniforme);
    }

    // Ir directo al carrito sin alertas
    this.router.navigate(['/carrito']);
  }

  contactarWhatsApp() {
    if (!this.uniforme) return;

    const mensaje = `Hola! Me interesa el ${this.uniforme.tipoPrenda} ${this.uniforme.marca} - Talle ${this.uniforme.talle} - $${this.uniforme.precio} (ID: ${this.uniforme.idUniforme})`;
    const numeroWhatsApp = '5493513728467';
    const url = `https://wa.me/${numeroWhatsApp}?text=${encodeURIComponent(mensaje)}`;

    window.open(url, '_blank');
  }
}