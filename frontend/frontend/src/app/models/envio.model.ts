export interface Envio {
    idEnvio: number;
    idVenta: number;
    direccion: string;
    ciudad: string;
    provincia: string;
    codigoPostal?: string;
    fechaEnvio?: string;
    fechaEntregaEstimada?: string;
    estadoEnvio: string; // Pendiente, En Camino, Entregado, Cancelado
    empresaEnvio?: string;
    numeroSeguimiento?: string;
    costoEnvio?: number;
    notas?: string;
    fechaCreacion: string;
    fechaModificacion: string;
}

export interface EnvioCreateDto {
    idVenta: number;
    direccion: string;
    ciudad: string;
    provincia: string;
    codigoPostal?: string;
    fechaEnvio?: string;
    fechaEntregaEstimada?: string;
    empresaEnvio?: string;
    numeroSeguimiento?: string;
    costoEnvio?: number;
    notas?: string;
}

export interface EnvioUpdateDto {
    idEnvio: number;
    direccion: string;
    ciudad: string;
    provincia: string;
    codigoPostal?: string;
    fechaEnvio?: string;
    fechaEntregaEstimada?: string;
    empresaEnvio?: string;
    numeroSeguimiento?: string;
    costoEnvio?: number;
    notas?: string;
}

export interface EnvioResponseDto {
    idEnvio: number;
    direccion: string;
    ciudad: string;
    provincia: string;
    codigoPostal?: string;
    fechaEnvio?: string;
    fechaEntregaEstimada?: string;
    estadoEnvio: string;
    empresaEnvio?: string;
    numeroSeguimiento?: string;
    costoEnvio?: number;
    notas?: string;
}

export interface EnvioEstadoDto {
    estado: string;
}
