export interface Reserva {
    idReserva: number;
    idUniforme: number;
    idCliente: number;
    fechaReserva: string;
    estadoReserva: string; // Activa, Cancelada, Confirmada
    mensajeWhatsapp?: string;
    fechaVencimiento?: string;
    notas?: string;
    fechaCreacion: string;
    fechaModificacion: string;
}

export interface ReservaCreateDto {
    idUniforme: number;
    idCliente: number;
    mensajeWhatsapp?: string;
    fechaVencimiento?: string;
    notas?: string;
}

export interface ReservaRapidaDto {
    idUniforme: number;
    nombreCliente: string;
    telefono: string;
    dni?: string;
    email?: string;
    direccion?: string;
    mensajeWhatsapp?: string;
    notas?: string;
}

export interface ReservaResponseDto {
    idReserva: number;
    fechaReserva: string;
    fechaVencimiento?: string;
    estadoReserva: string;
    mensajeWhatsapp?: string;
    notas?: string;
    cliente: {
        idCliente: number;
        nombreCliente: string;
        telefono: string;
        email: string;
        direccion?: string;
    };
    uniforme: {
        idUniforme: number;
        talle: string;
        precio: number;
        marca: string;
        tipoPrenda: string;
        imagen1?: string;
    };
}
