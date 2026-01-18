import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Uniforme, FiltrosUniforme, UniformeCreateDto, UniformeUpdateDto, UniformeEstadoDto } from '../models/uniforme.model';
import { Venta, VentaCreateDto, VentaRapidaDto } from '../models/venta.model';
import { Marca } from '../models/marca.model';
import { TipoPrenda } from '../models/tipo-prenda.model';
import { Reserva, ReservaCreateDto, ReservaResponseDto, ReservaRapidaDto } from '../models/reserva.model';
import { Envio, EnvioCreateDto, EnvioUpdateDto, EnvioResponseDto, EnvioEstadoDto } from '../models/envio.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  // ===== UNIFORMES =====
  getUniformes(filtros?: FiltrosUniforme): Observable<Uniforme[]> {
    let params = new HttpParams();

    if (filtros) {
      if (filtros.talle) params = params.set('talle', filtros.talle);
      if (filtros.idMarca) params = params.set('idMarca', filtros.idMarca.toString());
      if (filtros.idTipoPrenda) params = params.set('idTipoPrenda', filtros.idTipoPrenda.toString());
      if (filtros.precioMin) params = params.set('precioMin', filtros.precioMin.toString());
      if (filtros.precioMax) params = params.set('precioMax', filtros.precioMax.toString());
      if (filtros.estado) params = params.set('estado', filtros.estado);
    }

    return this.http.get<Uniforme[]>(`${this.apiUrl}/Uniformes`, { params });
  }

  getUniforme(id: number): Observable<Uniforme> {
    return this.http.get<Uniforme>(`${this.apiUrl}/Uniformes/${id}`);
  }

  getTallesDisponibles(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/Uniformes/talles`);
  }

  // ===== MARCAS =====
  getMarcas(): Observable<Marca[]> {
    return this.http.get<Marca[]>(`${this.apiUrl}/Marcas`);
  }

  // ===== TIPOS DE PRENDA =====
  getTiposPrenda(): Observable<TipoPrenda[]> {
    return this.http.get<TipoPrenda[]>(`${this.apiUrl}/TiposPrenda`);
  }

  // ===== ADMIN - CRUD UNIFORMES =====
  getUniformesAdmin(filtros?: FiltrosUniforme): Observable<Uniforme[]> {
    // Para admin, si no se especifica estado, obtenemos todos los estados
    // Como el backend filtra por "Disponible" por defecto, hacemos múltiples llamadas
    if (!filtros?.estado) {
      const estados = ['Disponible', 'Reservado', 'Vendido'];
      const observables = estados.map(estado =>
        this.getUniformes({ ...filtros, estado })
      );

      // Combinar todos los resultados usando forkJoin
      return forkJoin(observables).pipe(
        map(results => {
          // Aplanar el array de arrays y eliminar duplicados
          const allUniformes = results.flat();
          return allUniformes.filter((uniforme, idx, self) =>
            idx === self.findIndex(u => u.idUniforme === uniforme.idUniforme)
          );
        })
      );
    } else {
      // Si hay filtro de estado, usar el método normal
      return this.getUniformes(filtros);
    }
  }

  createUniforme(dto: UniformeCreateDto): Observable<Uniforme> {
    return this.http.post<Uniforme>(`${this.apiUrl}/Uniformes`, dto);
  }

  updateUniforme(id: number, dto: UniformeUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Uniformes/${id}`, dto);
  }

  deleteUniforme(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Uniformes/${id}`);
  }

  cambiarEstadoUniforme(id: number, estado: string): Observable<void> {
    const dto: UniformeEstadoDto = { estado };
    return this.http.patch<void>(`${this.apiUrl}/Uniformes/${id}/estado`, dto);
  }

  // ===== UPLOAD DE IMÁGENES =====
  uploadImage(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(`${this.apiUrl}/Uniformes/upload`, formData);
  }

  // ===== VENTAS =====
  getVentas(confirmado?: boolean): Observable<any[]> {
    let params = new HttpParams();
    if (confirmado !== undefined) {
      params = params.set('confirmado', confirmado.toString());
    }
    return this.http.get<any[]>(`${this.apiUrl}/Ventas`, { params });
  }

  createVenta(dto: VentaCreateDto): Observable<Venta> {
    return this.http.post<Venta>(`${this.apiUrl}/Ventas`, dto);
  }

  createVentaRapida(dto: VentaRapidaDto): Observable<Venta> {
    return this.http.post<Venta>(`${this.apiUrl}/Ventas/rapida`, dto);
  }

  confirmarVenta(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/Ventas/${id}/confirmar`, {});
  }

  // ===== RESERVAS =====
  getReservas(estado?: string): Observable<ReservaResponseDto[]> {
    let params = new HttpParams();
    if (estado) {
      params = params.set('estado', estado);
    }
    return this.http.get<ReservaResponseDto[]>(`${this.apiUrl}/Reservas`, { params });
  }

  getReserva(id: number): Observable<ReservaResponseDto> {
    return this.http.get<ReservaResponseDto>(`${this.apiUrl}/Reservas/${id}`);
  }

  createReserva(dto: ReservaCreateDto): Observable<ReservaResponseDto> {
    return this.http.post<ReservaResponseDto>(`${this.apiUrl}/Reservas`, dto);
  }

  createReservaRapida(dto: ReservaRapidaDto): Observable<ReservaResponseDto> {
    return this.http.post<ReservaResponseDto>(`${this.apiUrl}/Reservas/rapida`, dto);
  }

  confirmarReserva(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/Reservas/${id}/confirmar`, {});
  }

  cancelarReserva(id: number): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/Reservas/${id}/cancelar`, {});
  }

  deleteReserva(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Reservas/${id}`);
  }

  sincronizarReservas(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Reservas/sincronizar`, {});
  }

  // ===== ENVIOS =====
  getEnvios(estado?: string): Observable<EnvioResponseDto[]> {
    let params = new HttpParams();
    if (estado) {
      params = params.set('estado', estado);
    }
    return this.http.get<EnvioResponseDto[]>(`${this.apiUrl}/Envios`, { params });
  }

  getEnvio(id: number): Observable<EnvioResponseDto> {
    return this.http.get<EnvioResponseDto>(`${this.apiUrl}/Envios/${id}`);
  }

  createEnvio(dto: EnvioCreateDto): Observable<Envio> {
    return this.http.post<Envio>(`${this.apiUrl}/Envios`, dto);
  }

  updateEnvio(id: number, dto: EnvioUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Envios/${id}`, dto);
  }

  cambiarEstadoEnvio(id: number, estado: string): Observable<void> {
    const dto: EnvioEstadoDto = { estado };
    return this.http.patch<void>(`${this.apiUrl}/Envios/${id}/estado`, dto);
  }

  deleteEnvio(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Envios/${id}`);
  }

  // ===== CONFIGURACION =====
  getShippingCost(): Observable<{ costoEnvio: number }> {
    return this.http.get<{ costoEnvio: number }>(`${this.apiUrl}/Configuracion/envio`);
  }

  updateShippingCost(costo: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/Configuracion/envio`, { costoEnvio: costo });
  }
}