import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { lastValueFrom } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { ApiService } from '../../services/api.service';
import { Uniforme } from '../../models/uniforme.model';
import { VentaRapidaDto } from '../../models/venta.model';
import { ReservaRapidaDto } from '../../models/reserva.model';

@Component({
    selector: 'app-cart',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './cart.component.html',
    styleUrl: './cart.component.scss'
})
export class CartComponent implements OnInit {
    items: Uniforme[] = [];
    total = 0;

    cliente = {
        nombre: '',
        telefono: '',
        dni: '',
        calle: '',
        numero: '',
        barrio: ''
    };

    metodoPago = 'Transferencia';
    processing = false;

    // Envío
    conEnvio = false;
    costoEnvioBase = 5000;

    constructor(
        private cartService: CartService,
        private apiService: ApiService,
        private router: Router
    ) { }

    get tieneEnvioGratis(): boolean {
        return this.items.length >= 2;
    }

    get costoEnvioAplicado(): number {
        if (!this.conEnvio) return 0;
        return this.tieneEnvioGratis ? 0 : this.costoEnvioBase;
    }

    get totalFinal(): number {
        return this.total + this.costoEnvioAplicado;
    }

    ngOnInit() {
        this.cartService.items$.subscribe(items => {
            this.items = items;
            this.total = this.cartService.getTotal();
        });

        this.apiService.getShippingCost().subscribe({
            next: (res) => {
                this.costoEnvioBase = res.costoEnvio;
            },
            error: (err) => {
                console.error('Error cargando costo envío', err);
                // Fallback a 5000 si falla
                this.costoEnvioBase = 5000;
            }
        });
    }

    eliminarItem(id: number) {
        this.cartService.removeFromCart(id);
    }

    seguirComprando() {
        this.router.navigate(['/']);
    }

    redirigirWhatsApp(itemsComprados: Uniforme[], totalCompra: number) {
        const numeroWhatsApp = '5493513728467';
        let mensaje = `Hola! Realicé una compra de ${itemsComprados.length} uniforme(s):\n\n`;

        itemsComprados.forEach(item => {
            mensaje += `• [ID: ${item.idUniforme}] ${item.marca} - ${item.tipoPrenda} (Talle ${item.talle}) - $${item.precio}\n`;
        });

        if (this.conEnvio) {
            mensaje += `\nEnvío: ${this.tieneEnvioGratis ? 'GRATIS' : '$' + this.costoEnvioBase}`;
            mensaje += `\nDirección de Envío: ${this.cliente.calle} ${this.cliente.numero}, B° ${this.cliente.barrio}`;
        } else {
            mensaje += `\nEnvío: Retiro en B° Villa los Llanos`;
        }

        mensaje += `\nTotal Final: $${totalCompra}\n`;
        mensaje += `\nQuiero coordinar el pago de mi pedido.\n`;
        mensaje += `Mis datos:\n`;
        mensaje += `Nombre: ${this.cliente.nombre}\n`;
        mensaje += `Teléfono: ${this.cliente.telefono}\n`;
        mensaje += `DNI: ${this.cliente.dni}`;

        const url = `https://wa.me/${numeroWhatsApp}?text=${encodeURIComponent(mensaje)}`;

        window.open(url, '_blank');
        this.router.navigate(['/']);
    }

    async procesarVenta(metodo: 'Transferencia' | 'MercadoPago') {
        if (this.items.length === 0) return;

        this.processing = true;
        this.metodoPago = metodo;
        let ventasExitosas = 0;
        const errores = [];

        // Validar dirección si corresponde envío
        if (this.conEnvio && (!this.cliente.calle || !this.cliente.numero || !this.cliente.barrio)) {
            alert('Por favor completa todos los datos de envío.');
            this.processing = false;
            return;
        }

        const itemsExitosos: Uniforme[] = [];

        for (const item of this.items) {
            let notasVenta = `Venta Web. Envío: ${this.conEnvio ? 'SI' : 'NO'}`;
            if (this.conEnvio) {
                notasVenta += `. Dir: ${this.cliente.calle} ${this.cliente.numero}, ${this.cliente.barrio}`;
            }

            const dto: ReservaRapidaDto = {
                idUniforme: item.idUniforme,
                nombreCliente: this.cliente.nombre,
                telefono: this.cliente.telefono,
                dni: this.cliente.dni,
                direccion: this.conEnvio ? `${this.cliente.calle} ${this.cliente.numero}, B° ${this.cliente.barrio}` : 'Retiro en sucursal',
                notas: notasVenta
            };

            try {
                await lastValueFrom(this.apiService.createReservaRapida(dto));
                ventasExitosas++;
                itemsExitosos.push(item);
            } catch (err: any) {
                console.error(`Error venta item ${item.idUniforme}`, err);
                let errorMsg = 'Error desconocido';
                if (err.error && typeof err.error === 'object' && err.error.message) {
                    errorMsg = err.error.message;
                } else if (typeof err.error === 'string') {
                    errorMsg = err.error;
                } else if (err.status) {
                    errorMsg = `Status: ${err.status}`;
                }
                errores.push(`${item.marca}: ${errorMsg}`);
            }
        }

        this.processing = false;

        if (itemsExitosos.length > 0) {
            // Calcular total solo de lo exitoso
            // El costo de envío se cobra si hay al menos un item exitoso? 
            // Si el envío era gratis por 2 items y ahora queda 1, debería cobrarse?
            // Por simplicidad, mantendremos la lógica original de envío elegida por el usuario
            // pero recalculando el subtotal de items.

            const subtotalExitoso = itemsExitosos.reduce((sum, item) => sum + item.precio, 0);
            const totalFinalExitoso = subtotalExitoso + this.costoEnvioAplicado;

            this.cartService.clearCart();

            if (metodo === 'MercadoPago') {
                // Deprecated
            } else {
                this.redirigirWhatsApp(itemsExitosos, totalFinalExitoso);
            }
        }

        if (errores.length > 0) {
            const msg = '\n' + errores.join('\n');
            alert('Atención: Algunos productos no pudieron procesarse (posiblemente ya no están disponibles):' + msg);
        }
    }

    // Alias para el template
    finalizarCompra() {
        this.procesarVenta('Transferencia');
    }
}
