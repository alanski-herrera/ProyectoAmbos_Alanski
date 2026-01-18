import { Reserva } from './reserva.model';
import { Envio } from './envio.model';

export interface Venta {
    idVenta: number;
    idUniforme: number;
    idCliente: number;
    idReserva?: number;
    fechaVenta: string;
    montoTotal: number;
    metodoPago: string;
    comprobantePago?: string;
    confirmado: boolean;
    fechaConfirmacion?: string;
    notas?: string;
    fechaCreacion: string;
    reserva?: Reserva;
    envio?: Envio;
}

export interface VentaCreateDto {
    idUniforme: number;
    idCliente: number;
    idReserva?: number;
    montoTotal: number;
    metodoPago: string;
    comprobantePago?: string;
    notas?: string;
}

export interface VentaRapidaDto {
    idUniforme: number;
    nombreCliente: string;
    telefono: string;
    email: string;
    montoTotal: number;
    metodoPago: string;
    comprobantePago?: string;
    idReserva?: number;
    notas?: string;
}

export interface VentaResponseDto {
    idVenta: number;
    fechaVenta: string;
    montoTotal: number;
    metodoPago: string;
    confirmado: boolean;
    fechaConfirmacion?: string;
    comprobantePago?: string;
    cliente: {
        idCliente: number;
        nombreCliente: string;
        telefono: string;
        email: string;
    };
    uniforme: {
        idUniforme: number;
        talle: string;
        precio: number;
        marca: string;
        tipoPrenda: string;
        imagen1?: string;
    };
    estadoEnvio?: string;
    idReserva?: number;
}
