import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { Uniforme, UniformeCreateDto } from '../../models/uniforme.model';
import { Marca } from '../../models/marca.model';
import { TipoPrenda } from '../../models/tipo-prenda.model';
import { ReservaResponseDto, ReservaRapidaDto } from '../../models/reserva.model';
import { VentaCreateDto } from '../../models/venta.model';
import imageCompression from 'browser-image-compression';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {
  uniformes: Uniforme[] = [];
  marcas: Marca[] = [];
  tiposPrenda: TipoPrenda[] = [];
  reservas: ReservaResponseDto[] = [];

  viewMode: 'inventario' | 'reservas' = 'inventario';

  loading = true;
  showModal = false;
  modoEdicion = false;

  uniformeActual: any = {
    idUniforme: 0,
    talle: '',
    precio: 0,
    idMarca: undefined,
    idTipoPrenda: undefined,
    descripcion: '',
    imagen1: '',
    imagen2: '',
    imagen3: ''
  };

  archivosImagen: { [key: string]: File | null } = {
    imagen1: null,
    imagen2: null,
    imagen3: null
  };

  confirmarEliminar = false;
  uniformeAEliminar: Uniforme | null = null;

  filtroEstado: string = '';
  searchId: string = '';

  // Configuración
  costoEnvio: number = 0;
  showConfigModal = false;

  // Search Reservas
  searchReservaId: string = '';

  constructor(
    private apiService: ApiService,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit() {
    this.cargarDatos();
    this.cargarReservas();
    this.loadShippingCost();
  }

  cargarDatos() {
    this.loading = true;

    // Cargar uniformes (todos los estados para el admin)
    this.apiService.getUniformesAdmin({ estado: this.filtroEstado || undefined }).subscribe({
      next: (data) => {
        this.uniformes = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error al cargar uniformes:', err);
        this.loading = false;
      }
    });

    // Cargar marcas
    this.apiService.getMarcas().subscribe({
      next: (data) => {
        this.marcas = data;
      }
    });

    // Cargar tipos de prenda
    this.apiService.getTiposPrenda().subscribe({
      next: (data) => {
        this.tiposPrenda = data;
      }
    });
    this.apiService.getTiposPrenda().subscribe({
      next: (data) => {
        this.tiposPrenda = data;
      }
    });
  }

  cargarReservas() {
    this.apiService.getReservas('Activa').subscribe({
      next: (data) => {
        this.reservas = data;
      },
      error: (err) => console.error('Error al cargar reservas:', err)
    });
  }

  get uniformesFiltrados(): Uniforme[] {
    let resultado = this.uniformes;

    if (this.filtroEstado) {
      resultado = resultado.filter(u => u.estado === this.filtroEstado);
    }

    if (this.searchId) {
      const searchIdNum = this.searchId.toString();
      resultado = resultado.filter(u =>
        u.idUniforme.toString().includes(searchIdNum)
      );
    }

    return resultado;
  }

  get reservasFiltradas(): ReservaResponseDto[] {
    let resultado = this.reservas;

    if (this.searchReservaId) {
      const searchIdNum = this.searchReservaId.toString();
      resultado = resultado.filter(r =>
        r.uniforme.idUniforme.toString().includes(searchIdNum)
      );
    }

    return resultado;
  }

  abrirModalCrear() {
    this.modoEdicion = false;
    this.uniformeActual = {
      idUniforme: 0,
      talle: '',
      precio: 0,
      idMarca: undefined,
      idTipoPrenda: undefined,
      descripcion: '',
      imagen1: '',
      imagen2: '',
      imagen3: ''
    };
    this.archivosImagen = {
      imagen1: null,
      imagen2: null,
      imagen3: null
    };
    this.showModal = true;
  }

  abrirModalEditar(uniforme: Uniforme) {
    this.modoEdicion = true;

    // Buscar IDs de marca y tipo prenda
    const marca = this.marcas.find(m => m.nombreMarca === uniforme.marca);
    const tipoPrenda = this.tiposPrenda.find(t => t.nombreTipo === uniforme.tipoPrenda);

    this.uniformeActual = {
      idUniforme: uniforme.idUniforme,
      talle: uniforme.talle,
      precio: uniforme.precio,
      idMarca: marca?.idMarca,
      idTipoPrenda: tipoPrenda?.idTipoPrenda,
      descripcion: uniforme.descripcion || '',
      imagen1: uniforme.imagen1 || '',
      imagen2: uniforme.imagen2 || '',
      imagen3: uniforme.imagen3 || ''
    };
    this.archivosImagen = {
      imagen1: null,
      imagen2: null,
      imagen3: null
    };
    this.showModal = true;
  }

  cerrarModal() {
    this.showModal = false;
    this.showConfigModal = false;
    this.uniformeActual = {};
    this.archivosImagen = {
      imagen1: null,
      imagen2: null,
      imagen3: null
    };
  }

  // ===== CONFIGURACION =====
  loadShippingCost() {
    this.apiService.getShippingCost().subscribe({
      next: (res) => {
        this.costoEnvio = res.costoEnvio;
      },
      error: (err) => console.error('Error cargando costo envío', err)
    });
  }

  abrirConfigModal() {
    this.showConfigModal = true;
  }

  guardarConfiguracion() {
    this.apiService.updateShippingCost(this.costoEnvio).subscribe({
      next: () => {
        alert('Costo de envío actualizado correctamente');
        this.cerrarModal();
      },
      error: (err) => {
        console.error('Error actualizando costo envío', err);
        alert('Error al actualizar costo de envío');
      }
    });
  }

  async onFileSelected(event: Event, campo: string) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];

      // Validar que sea una imagen
      if (!file.type.startsWith('image/')) {
        alert('Por favor selecciona un archivo de imagen válido');
        return;
      }

      // Validar tamaño antes de comprimir (máximo 8MB para casos extremos)
      if (file.size > 8 * 1024 * 1024) {
        alert('La imagen es demasiado grande. Máximo 8MB');
        return;
      }

      try {
        // Configuración de compresión
        const options = {
          maxSizeMB: 1, // Comprimir a máximo 1MB
          maxWidthOrHeight: 1920, // Resolución máxima razonable para web
          useWebWorker: true, // Usar web worker para no bloquear la UI
          fileType: file.type // Mantener el tipo de archivo original
        };

        // Comprimir la imagen
        console.log(`Comprimiendo ${campo}... Tamaño original: ${(file.size / 1024 / 1024).toFixed(2)}MB`);
        const compressedBlob = await imageCompression(file, options);
        console.log(`Compresión completada. Nuevo tamaño: ${(compressedBlob.size / 1024 / 1024).toFixed(2)}MB`);

        // Convertir el blob comprimido a File con el nombre original
        const compressedFile = new File([compressedBlob], file.name, {
          type: compressedBlob.type,
          lastModified: Date.now()
        });

        this.archivosImagen[campo] = compressedFile;

        // Mostrar vista previa mientras se sube
        const reader = new FileReader();
        reader.onload = (e: any) => {
          this.uniformeActual[campo] = e.target.result; // Vista previa temporal
        };
        reader.readAsDataURL(compressedFile);

        // Subir la imagen comprimida al servidor
        this.subirImagen(compressedFile, campo);
      } catch (error) {
        console.error('Error al comprimir la imagen:', error);
        alert('Error al procesar la imagen. Por favor intenta con otra imagen.');
      }
    }
  }

  subirImagen(file: File, campo: string) {
    // Mostrar indicador de carga
    const loadingMessage = `Subiendo ${campo}...`;
    console.log(loadingMessage);

    this.apiService.uploadImage(file).subscribe({
      next: (response) => {
        // Reemplazar la vista previa base64 con la URL del servidor
        this.uniformeActual[campo] = response.url;
        console.log(`${campo} subida exitosamente:`, response.url);
      },
      error: (err) => {
        console.error(`Error al subir ${campo}:`, err);
        alert(`Error al subir la imagen ${campo}. Por favor intenta de nuevo o usa una URL.`);
        // Limpiar el campo si falla
        this.uniformeActual[campo] = '';
        this.archivosImagen[campo] = null;
        const input = document.getElementById(`file-${campo}`) as HTMLInputElement;
        if (input) {
          input.value = '';
        }
      }
    });
  }


  eliminarImagen(campo: string) {
    this.uniformeActual[campo] = '';
    this.archivosImagen[campo] = null;

    // Resetear el input file
    const input = document.getElementById(`file-${campo}`) as HTMLInputElement;
    if (input) {
      input.value = '';
    }
  }

  onUrlChange(campo: string) {
    // Si se pega una URL, limpiar el archivo seleccionado
    if (this.uniformeActual[campo] && !this.uniformeActual[campo].startsWith('data:')) {
      this.archivosImagen[campo] = null;
      const input = document.getElementById(`file-${campo}`) as HTMLInputElement;
      if (input) {
        input.value = '';
      }
    }
  }

  guardarUniforme() {
    // Preparar datos
    const datosUniforme: any = {
      talle: this.uniformeActual.talle,
      precio: this.uniformeActual.precio,
      idMarca: this.uniformeActual.idMarca,
      idTipoPrenda: this.uniformeActual.idTipoPrenda,
      descripcion: this.uniformeActual.descripcion,
      imagen1: this.uniformeActual.imagen1 || undefined,
      imagen2: this.uniformeActual.imagen2 || undefined,
      imagen3: this.uniformeActual.imagen3 || undefined
    };

    // Si hay imágenes en base64 (vista previa temporal), esperar a que se suban
    // o usar solo URLs
    ['imagen1', 'imagen2', 'imagen3'].forEach(campo => {
      if (datosUniforme[campo] && datosUniforme[campo].startsWith('data:')) {
        // Si todavía es base64, significa que la subida no terminó
        if (this.archivosImagen[campo]) {
          alert(`La imagen ${campo} aún se está subiendo. Por favor espera un momento e intenta de nuevo.`);
          throw new Error('Imagen aún subiendo');
        }
        // Si no hay archivo pero hay base64, limpiar (no debería pasar)
        datosUniforme[campo] = undefined;
      }
    });

    if (this.modoEdicion) {
      // Actualizar
      datosUniforme.idUniforme = this.uniformeActual.idUniforme;
      this.apiService.updateUniforme(this.uniformeActual.idUniforme, datosUniforme).subscribe({
        next: () => {
          this.cerrarModal();
          this.cargarDatos();
        },
        error: (err) => {
          console.error('Error completo al actualizar:', err);
          console.error('Status:', err.status);
          console.error('Error body:', err.error);
          console.error('Headers:', err.headers);

          let mensaje = 'Error al actualizar el uniforme';
          if (err.error?.message) {
            mensaje += ': ' + err.error.message;
          } else if (err.status === 413) {
            mensaje += '. La imagen es demasiado grande. Intenta con una imagen más pequeña o usa una URL.';
          } else if (err.status === 400) {
            mensaje += '. Error de validación. Verifica que todos los campos sean correctos.';
            if (err.error?.errors) {
              console.error('Errores de validación:', err.error.errors);
            }
          } else if (err.status === 415) {
            mensaje += '. El formato de imagen no es compatible. Intenta con otra imagen.';
          } else if (err.status === 0) {
            mensaje += '. No se pudo conectar con el servidor. Verifica tu conexión.';
          }
          alert(mensaje);
        }
      });
    } else {
      // Crear
      this.apiService.createUniforme(datosUniforme).subscribe({
        next: () => {
          this.cerrarModal();
          this.cargarDatos();
        },
        error: (err) => {
          console.error('Error completo al crear:', err);
          console.error('Status:', err.status);
          console.error('Error body:', err.error);
          console.error('Headers:', err.headers);

          // Log del tamaño de los datos enviados
          const datosSize = JSON.stringify(datosUniforme).length;
          console.error('Tamaño del request (caracteres):', datosSize);
          console.error('Tamaño del request (KB):', (datosSize / 1024).toFixed(2));

          let mensaje = 'Error al crear el uniforme';
          if (err.error?.message) {
            mensaje += ': ' + err.error.message;
          } else if (err.status === 413) {
            mensaje += '. La imagen es demasiado grande. Intenta con una imagen más pequeña o usa una URL.';
          } else if (err.status === 400) {
            mensaje += '. Error de validación. Verifica que todos los campos sean correctos.';
            if (err.error?.errors) {
              console.error('Errores de validación:', err.error.errors);
            }
          } else if (err.status === 415) {
            mensaje += '. El formato de imagen no es compatible. Intenta con otra imagen.';
          } else if (err.status === 0) {
            mensaje += '. No se pudo conectar con el servidor. Verifica tu conexión.';
          }
          alert(mensaje);
        }
      });
    }
  }

  confirmarEliminacion(uniforme: Uniforme) {
    this.uniformeAEliminar = uniforme;
    this.confirmarEliminar = true;
  }

  eliminarUniforme() {
    if (!this.uniformeAEliminar) return;

    this.apiService.deleteUniforme(this.uniformeAEliminar.idUniforme).subscribe({
      next: () => {
        this.confirmarEliminar = false;
        this.uniformeAEliminar = null;
        this.cargarDatos();
      },
      error: (err) => {
        console.error('Error al eliminar:', err);
        alert('Error al eliminar el uniforme');
      }
    });
  }

  cancelarEliminacion() {
    this.confirmarEliminar = false;
    this.uniformeAEliminar = null;
  }



  cambiarEstado(uniforme: Uniforme, nuevoEstado: string) {
    if (nuevoEstado === 'Reservado') {
      this.crearReservaManual(uniforme);
      return;
    }

    this.apiService.cambiarEstadoUniforme(uniforme.idUniforme, nuevoEstado).subscribe({
      next: () => {
        this.cargarDatos();
      },
      error: (err) => {
        console.error('Error al cambiar estado:', err);
        alert('Error al cambiar el estado');
      }
    });
  }

  crearReservaManual(uniforme: Uniforme) {
    const nombre = prompt('Ingrese nombre del cliente para la reserva:');
    if (!nombre) {
      this.cargarDatos(); // Revertir visualmente si cancela
      return;
    }

    const reservaDto: ReservaRapidaDto = {
      idUniforme: uniforme.idUniforme,
      nombreCliente: nombre,
      telefono: '',
      email: '',
      notas: 'Reserva manual desde panel administrador'
    };

    this.apiService.createReservaRapida(reservaDto).subscribe({
      next: () => {
        alert('Reserva creada exitosamente');
        this.cargarDatos();
        this.cargarReservas();
      },
      error: (err) => {
        console.error('Error al crear reserva manual:', err);
        alert('Error al crear la reserva');
        this.cargarDatos();
      }
    });
  }

  sincronizarReservas() {
    this.apiService.sincronizarReservas().subscribe({
      next: (res) => {
        if (res.cantidad > 0) {
          alert(`Se recuperaron ${res.cantidad} reservas de sistema anterior.`);
          this.cargarReservas();
        }
      },
      error: (err) => console.error('Error sincronizando:', err)
    });
  }

  // ===== RESERVAS =====
  confirmarVentaReserva(reserva: ReservaResponseDto) {
    if (!confirm('¿Confirmar venta de esta reserva? Se generará una venta y se marcará como pagada.')) {
      return;
    }

    const ventaDto: VentaCreateDto = {
      idUniforme: reserva.uniforme.idUniforme,
      idCliente: reserva.cliente.idCliente,
      idReserva: reserva.idReserva,
      montoTotal: reserva.uniforme.precio,
      metodoPago: 'Transferencia',
      notas: 'Venta generada desde reserva'
    };

    this.apiService.createVenta(ventaDto).subscribe({
      next: (venta) => {
        this.processConfirmarVenta(venta.idVenta);
      },
      error: (err) => {
        console.error('Error al crear venta:', err);
        alert('Error al confirmar la venta');
      }
    });
  }

  private processConfirmarVenta(idVenta: number) {
    this.apiService.confirmarVenta(idVenta).subscribe({
      next: () => {
        alert('Venta confirmada exitosamente');
        this.cargarReservas();
        this.cargarDatos();
      },
      error: (err) => {
        console.error('Error al confirmar venta:', err);
        alert('Venta creada pero hubo error al confirmar el estado');
      }
    });
  }

  cancelarReserva(reserva: ReservaResponseDto) {
    if (!confirm('¿Descartar esta reserva? El uniforme volverá a estar disponible.')) {
      return;
    }

    this.apiService.cancelarReserva(reserva.idReserva).subscribe({
      next: () => {
        // Notificación visual
        alert('Reserva cancelada');
        this.cargarReservas();
        this.cargarDatos(); // Para actualizar contadores y lista de uniformes
      },
      error: (err) => {
        console.error('Error al cancelar reserva:', err);
        alert('Error al cancelar la reserva');
      }
    });
  }

  volverInicio() {
    this.router.navigate(['/']);
  }

  get estadisticas() {
    return {
      total: this.uniformes.length,
      disponibles: this.uniformes.filter(u => u.estado === 'Disponible').length,
      reservados: this.uniformes.filter(u => u.estado === 'Reservado').length,
      vendidos: this.uniformes.filter(u => u.estado === 'Vendido').length
    };
  }
}