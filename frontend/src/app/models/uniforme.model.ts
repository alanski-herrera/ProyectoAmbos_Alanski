export interface Uniforme {
  idUniforme: number;
  talle: string;
  precio: number;
  estado: string;
  fechaIngreso: Date;
  descripcion?: string;
  imagen1?: string;
  imagen2?: string;
  imagen3?: string;
  marca: string;
  tipoPrenda: string;
}

export interface UniformeCreateDto {
  talle: string;
  precio: number;
  idMarca: number;
  idTipoPrenda: number;
  descripcion?: string;
  imagen1?: string;
  imagen2?: string;
  imagen3?: string;
}

export interface UniformeUpdateDto {
  idUniforme: number;
  talle: string;
  precio: number;
  idMarca: number;
  idTipoPrenda: number;
  descripcion?: string;
  imagen1?: string;
  imagen2?: string;
  imagen3?: string;
}

export interface UniformeEstadoDto {
  estado: string;
}

export interface FiltrosUniforme {
  talle?: string;
  idMarca?: number;
  idTipoPrenda?: number;
  precioMin?: number;
  precioMax?: number;
  estado?: string;
}