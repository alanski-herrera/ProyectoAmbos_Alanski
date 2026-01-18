-- =====================================================
-- SCRIPT DE CREACIÓN DE BASE DE DATOS
-- PROYECTO: AMBOS - Venta de Uniformes Médicos
-- =====================================================

-- Eliminar base de datos si existe (para desarrollo)
DROP DATABASE IF EXISTS db_ambos;

-- Crear base de datos
CREATE DATABASE db_ambos CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Usar la base de datos
USE db_ambos;

-- =====================================================
-- TABLA: Administradores
-- =====================================================
CREATE TABLE Administradores (
    id_admin INT AUTO_INCREMENT PRIMARY KEY,
    nombre_admin VARCHAR(50) NOT NULL,
    dni VARCHAR(20) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    contrasena VARCHAR(255) NOT NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ultimo_acceso TIMESTAMP NULL,
    activo BOOLEAN DEFAULT TRUE,
    INDEX idx_email (email),
    INDEX idx_dni (dni)
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Clientes
-- =====================================================
CREATE TABLE Clientes (
    id_cliente INT AUTO_INCREMENT PRIMARY KEY,
    nombre_cliente VARCHAR(80) NOT NULL,
    dni VARCHAR(20) NULL,
    direccion VARCHAR(150) NULL,
    telefono VARCHAR(20) NOT NULL,
    email VARCHAR(100) NULL,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    notas TEXT NULL COMMENT 'Notas adicionales del cliente',
    INDEX idx_telefono (telefono),
    INDEX idx_email (email),
    INDEX idx_dni (dni)
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Marcas
-- =====================================================
CREATE TABLE Marcas (
    id_marca INT AUTO_INCREMENT PRIMARY KEY,
    nombre_marca VARCHAR(50) NOT NULL UNIQUE,
    descripcion VARCHAR(200) NULL,
    activo BOOLEAN DEFAULT TRUE,
    INDEX idx_nombre (nombre_marca)
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Tipos de Prenda
-- =====================================================
CREATE TABLE Tipos_Prenda (
    id_tipo_prenda INT AUTO_INCREMENT PRIMARY KEY,
    nombre_tipo VARCHAR(50) NOT NULL UNIQUE,
    descripcion VARCHAR(200) NULL,
    activo BOOLEAN DEFAULT TRUE,
    INDEX idx_nombre (nombre_tipo)
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Uniformes
-- =====================================================
CREATE TABLE Uniformes (
    id_uniforme INT AUTO_INCREMENT PRIMARY KEY,
    talle VARCHAR(10) NOT NULL,
    precio DECIMAL(10,2) NOT NULL,
    estado ENUM('Disponible', 'Reservado', 'Vendido') DEFAULT 'Disponible',
    fecha_ingreso DATE NOT NULL,
    id_marca INT NOT NULL,
    id_tipo_prenda INT NOT NULL,
    descripcion TEXT NULL COMMENT 'Descripción adicional del uniforme',
    imagen_1 VARCHAR(500) NULL COMMENT 'URL de la primera imagen',
    imagen_2 VARCHAR(500) NULL COMMENT 'URL de la segunda imagen',
    imagen_3 VARCHAR(500) NULL COMMENT 'URL de la tercera imagen',
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (id_marca) REFERENCES Marcas(id_marca) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (id_tipo_prenda) REFERENCES Tipos_Prenda(id_tipo_prenda) ON DELETE RESTRICT ON UPDATE CASCADE,
    INDEX idx_estado (estado),
    INDEX idx_talle (talle),
    INDEX idx_precio (precio),
    INDEX idx_marca (id_marca),
    INDEX idx_tipo_prenda (id_tipo_prenda),
    INDEX idx_fecha_ingreso (fecha_ingreso)
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Reservas
-- =====================================================
CREATE TABLE Reservas (
    id_reserva INT AUTO_INCREMENT PRIMARY KEY,
    id_uniforme INT NOT NULL,
    id_cliente INT NOT NULL,
    fecha_reserva DATE NOT NULL,
    estado_reserva ENUM('Activa', 'Cancelada', 'Confirmada') DEFAULT 'Activa',
    mensaje_whatsapp TEXT NULL COMMENT 'Mensaje enviado por WhatsApp',
    fecha_vencimiento DATE NULL COMMENT 'Fecha límite para confirmar la reserva',
    notas TEXT NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (id_uniforme) REFERENCES Uniformes(id_uniforme) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id_cliente) ON DELETE CASCADE ON UPDATE CASCADE,
    INDEX idx_estado (estado_reserva),
    INDEX idx_fecha_reserva (fecha_reserva),
    INDEX idx_uniforme (id_uniforme),
    INDEX idx_cliente (id_cliente)
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Ventas
-- =====================================================
CREATE TABLE Ventas (
    id_venta INT AUTO_INCREMENT PRIMARY KEY,
    id_uniforme INT NOT NULL,
    id_cliente INT NOT NULL,
    id_reserva INT NULL COMMENT 'Reserva asociada si existe',
    fecha_venta DATE NOT NULL,
    monto_total DECIMAL(10,2) NOT NULL,
    metodo_pago ENUM('Transferencia', 'Efectivo', 'Otro') DEFAULT 'Transferencia',
    comprobante_pago VARCHAR(500) NULL COMMENT 'URL del comprobante de pago',
    confirmado BOOLEAN DEFAULT FALSE,
    fecha_confirmacion TIMESTAMP NULL,
    notas TEXT NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_uniforme) REFERENCES Uniformes(id_uniforme) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id_cliente) ON DELETE RESTRICT ON UPDATE CASCADE,
    FOREIGN KEY (id_reserva) REFERENCES Reservas(id_reserva) ON DELETE SET NULL ON UPDATE CASCADE,
    INDEX idx_fecha_venta (fecha_venta),
    INDEX idx_confirmado (confirmado),
    INDEX idx_uniforme (id_uniforme),
    INDEX idx_cliente (id_cliente)
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Envios
-- =====================================================
CREATE TABLE Envios (
    id_envio INT AUTO_INCREMENT PRIMARY KEY,
    id_venta INT NOT NULL,
    direccion VARCHAR(150) NOT NULL,
    ciudad VARCHAR(50) NOT NULL,
    provincia VARCHAR(50) NOT NULL,
    codigo_postal VARCHAR(10) NULL,
    fecha_envio DATE NULL,
    fecha_entrega_estimada DATE NULL,
    estado_envio ENUM('Pendiente', 'En Camino', 'Entregado', 'Cancelado') DEFAULT 'Pendiente',
    empresa_envio VARCHAR(100) NULL,
    numero_seguimiento VARCHAR(100) NULL,
    costo_envio DECIMAL(10,2) NULL,
    notas TEXT NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (id_venta) REFERENCES Ventas(id_venta) ON DELETE CASCADE ON UPDATE CASCADE,
    INDEX idx_estado (estado_envio),
    INDEX idx_fecha_envio (fecha_envio),
    INDEX idx_venta (id_venta)
) ENGINE=InnoDB;

-- =====================================================
-- DATOS DE PRUEBA
-- =====================================================

-- Insertar administrador de prueba
-- Contraseña: Admin123! (deberás hashearla en tu aplicación)
INSERT INTO Administradores (nombre_admin, dni, email, contrasena) VALUES
('Administrador Principal', '12345678', 'admin@ambos.com', 'Admin123!'),
('Juan Pérez', '87654321', 'juan@ambos.com', 'Juan123!');

-- Insertar marcas populares de ambos médicos
INSERT INTO Marcas (nombre_marca, descripcion) VALUES
('Cherokee', 'Marca líder en uniformes médicos'),
('Greys Anatomy', 'Uniformes de alta calidad y confort'),
('Dickies', 'Marca reconocida mundialmente'),
('WonderWink', 'Diseños modernos y funcionales'),
('Koi', 'Estilo y comodidad'),
('Med Couture', 'Elegancia profesional'),
('Otra Marca', 'Marcas varias');

-- Insertar tipos de prenda
INSERT INTO Tipos_Prenda (nombre_tipo, descripcion) VALUES
('Chaqueta', 'Parte superior del uniforme médico'),
('Pantalón', 'Parte inferior del uniforme médico'),
('Ambo Completo', 'Conjunto completo de chaqueta y pantalón');

-- Insertar clientes de prueba
INSERT INTO Clientes (nombre_cliente, dni, direccion, telefono, email, notas) VALUES
('María González', '30123456', 'Av. Colón 1234, Córdoba', '351-1234567', 'maria@email.com', 'Cliente frecuente'),
('Carlos Rodríguez', '28987654', 'Bv. San Juan 567, Córdoba', '351-7654321', 'carlos@email.com', NULL),
('Ana Martínez', '32456789', 'Av. Vélez Sarsfield 890, Córdoba', '351-9876543', 'ana@email.com', 'Prefiere contacto por WhatsApp');

-- Insertar uniformes de prueba
INSERT INTO Uniformes (talle, precio, estado, fecha_ingreso, id_marca, id_tipo_prenda, descripcion, imagen_1) VALUES
('S', 15000.00, 'Disponible', '2024-12-01', 1, 3, 'Ambo completo Cherokee azul marino, excelente estado', 'https://ejemplo.com/imagen1.jpg'),
('M', 18000.00, 'Disponible', '2024-12-02', 2, 3, 'Ambo completo Greys Anatomy gris, con bolsillos adicionales', 'https://ejemplo.com/imagen2.jpg'),
('L', 16500.00, 'Disponible', '2024-12-03', 3, 3, 'Ambo completo Dickies negro, clásico', 'https://ejemplo.com/imagen3.jpg'),
('XL', 17000.00, 'Reservado', '2024-12-04', 1, 3, 'Ambo completo Cherokee verde quirúrgico', 'https://ejemplo.com/imagen4.jpg'),
('M', 19000.00, 'Disponible', '2024-12-05', 4, 3, 'Ambo completo WonderWink estampado, moderno', 'https://ejemplo.com/imagen5.jpg'),
('S', 20000.00, 'Disponible', '2024-12-06', 5, 3, 'Ambo completo Koi rosa pastel, muy buscado', 'https://ejemplo.com/imagen6.jpg'),
('L', 16000.00, 'Vendido', '2024-12-07', 2, 3, 'Ambo completo Greys Anatomy negro', 'https://ejemplo.com/imagen7.jpg'),
('M', 8500.00, 'Disponible', '2024-12-08', 1, 1, 'Chaqueta Cherokee azul celeste', 'https://ejemplo.com/imagen8.jpg'),
('L', 7500.00, 'Disponible', '2024-12-09', 3, 2, 'Pantalón Dickies negro con elástico', 'https://ejemplo.com/imagen9.jpg'),
('S', 9000.00, 'Disponible', '2024-12-10', 5, 1, 'Chaqueta Koi estampada con flores', 'https://ejemplo.com/imagen10.jpg');

-- Insertar una reserva de prueba
INSERT INTO Reservas (id_uniforme, id_cliente, fecha_reserva, estado_reserva, mensaje_whatsapp, fecha_vencimiento) VALUES
(4, 1, '2024-12-15', 'Activa', 'Hola! Me interesa el ambo Cherokee talle XL verde quirúrgico', '2024-12-18');

-- Insertar una venta de prueba
INSERT INTO Ventas (id_uniforme, id_cliente, fecha_venta, monto_total, metodo_pago, confirmado, fecha_confirmacion) VALUES
(7, 2, '2024-12-10', 16000.00, 'Transferencia', TRUE, '2024-12-10 14:30:00');

-- Insertar un envío de prueba
INSERT INTO Envios (id_venta, direccion, ciudad, provincia, codigo_postal, fecha_envio, estado_envio, empresa_envio) VALUES
(1, 'Bv. San Juan 567', 'Córdoba', 'Córdoba', '5000', '2024-12-11', 'En Camino', 'Correo Argentino');

-- =====================================================
-- VISTAS ÚTILES
-- =====================================================

-- Vista: Uniformes con información completa
CREATE VIEW vista_uniformes_completos AS
SELECT 
    u.id_uniforme,
    u.talle,
    u.precio,
    u.estado,
    u.fecha_ingreso,
    u.descripcion,
    u.imagen_1,
    u.imagen_2,
    u.imagen_3,
    m.nombre_marca,
    m.id_marca,
    tp.nombre_tipo as tipo_prenda,
    tp.id_tipo_prenda
FROM Uniformes u
INNER JOIN Marcas m ON u.id_marca = m.id_marca
INNER JOIN Tipos_Prenda tp ON u.id_tipo_prenda = tp.id_tipo_prenda;

-- Vista: Reservas activas con detalles
CREATE VIEW vista_reservas_activas AS
SELECT 
    r.id_reserva,
    r.fecha_reserva,
    r.fecha_vencimiento,
    r.estado_reserva,
    c.nombre_cliente,
    c.telefono,
    c.email,
    u.id_uniforme,
    u.talle,
    u.precio,
    m.nombre_marca,
    tp.nombre_tipo as tipo_prenda
FROM Reservas r
INNER JOIN Clientes c ON r.id_cliente = c.id_cliente
INNER JOIN Uniformes u ON r.id_uniforme = u.id_uniforme
INNER JOIN Marcas m ON u.id_marca = m.id_marca
INNER JOIN Tipos_Prenda tp ON u.id_tipo_prenda = tp.id_tipo_prenda
WHERE r.estado_reserva = 'Activa';

-- Vista: Ventas con detalles completos
CREATE VIEW vista_ventas_completas AS
SELECT 
    v.id_venta,
    v.fecha_venta,
    v.monto_total,
    v.metodo_pago,
    v.confirmado,
    c.nombre_cliente,
    c.telefono,
    u.id_uniforme,
    u.talle,
    m.nombre_marca,
    tp.nombre_tipo as tipo_prenda,
    CASE 
        WHEN e.id_envio IS NOT NULL THEN e.estado_envio
        ELSE 'Sin Envío'
    END as estado_envio
FROM Ventas v
INNER JOIN Clientes c ON v.id_cliente = c.id_cliente
INNER JOIN Uniformes u ON v.id_uniforme = u.id_uniforme
INNER JOIN Marcas m ON u.id_marca = m.id_marca
INNER JOIN Tipos_Prenda tp ON u.id_tipo_prenda = tp.id_tipo_prenda
LEFT JOIN Envios e ON v.id_venta = e.id_venta;

-- =====================================================
-- PROCEDIMIENTOS ALMACENADOS ÚTILES
-- =====================================================

-- Procedimiento: Obtener uniformes disponibles con filtros
DELIMITER //
CREATE PROCEDURE sp_obtener_uniformes_disponibles(
    IN p_talle VARCHAR(10),
    IN p_marca_id INT,
    IN p_tipo_prenda_id INT,
    IN p_precio_min DECIMAL(10,2),
    IN p_precio_max DECIMAL(10,2)
)
BEGIN
    SELECT 
        u.id_uniforme,
        u.talle,
        u.precio,
        u.descripcion,
        u.imagen_1,
        u.imagen_2,
        u.imagen_3,
        u.fecha_ingreso,
        m.nombre_marca,
        tp.nombre_tipo as tipo_prenda
    FROM Uniformes u
    INNER JOIN Marcas m ON u.id_marca = m.id_marca
    INNER JOIN Tipos_Prenda tp ON u.id_tipo_prenda = tp.id_tipo_prenda
    WHERE u.estado = 'Disponible'
        AND (p_talle IS NULL OR u.talle = p_talle)
        AND (p_marca_id IS NULL OR u.id_marca = p_marca_id)
        AND (p_tipo_prenda_id IS NULL OR u.id_tipo_prenda = p_tipo_prenda_id)
        AND (p_precio_min IS NULL OR u.precio >= p_precio_min)
        AND (p_precio_max IS NULL OR u.precio <= p_precio_max)
    ORDER BY u.fecha_ingreso DESC;
END //
DELIMITER ;

-- Procedimiento: Cambiar estado de uniforme
DELIMITER //
CREATE PROCEDURE sp_cambiar_estado_uniforme(
    IN p_id_uniforme INT,
    IN p_nuevo_estado VARCHAR(20)
)
BEGIN
    UPDATE Uniformes 
    SET estado = p_nuevo_estado 
    WHERE id_uniforme = p_id_uniforme;
END //
DELIMITER ;

-- =====================================================
-- TRIGGERS
-- =====================================================

-- Trigger: Actualizar estado del uniforme al crear venta confirmada
DELIMITER //
CREATE TRIGGER trg_venta_confirmada 
AFTER UPDATE ON Ventas
FOR EACH ROW
BEGIN
    IF NEW.confirmado = TRUE AND OLD.confirmado = FALSE THEN
        UPDATE Uniformes 
        SET estado = 'Vendido' 
        WHERE id_uniforme = NEW.id_uniforme;
    END IF;
END //
DELIMITER ;

-- Trigger: Actualizar estado del uniforme al crear reserva
DELIMITER //
CREATE TRIGGER trg_reserva_creada 
AFTER INSERT ON Reservas
FOR EACH ROW
BEGIN
    IF NEW.estado_reserva = 'Activa' THEN
        UPDATE Uniformes 
        SET estado = 'Reservado' 
        WHERE id_uniforme = NEW.id_uniforme;
    END IF;
END //
DELIMITER ;

-- Trigger: Liberar uniforme al cancelar reserva
DELIMITER //
CREATE TRIGGER trg_reserva_cancelada 
AFTER UPDATE ON Reservas
FOR EACH ROW
BEGIN
    IF NEW.estado_reserva = 'Cancelada' AND OLD.estado_reserva != 'Cancelada' THEN
        UPDATE Uniformes 
        SET estado = 'Disponible' 
        WHERE id_uniforme = NEW.id_uniforme;
    END IF;
END //
DELIMITER ;

-- =====================================================
-- CONSULTAS DE VERIFICACIÓN
-- =====================================================

-- Ver todos los uniformes con sus marcas
-- SELECT * FROM vista_uniformes_completos;

-- Ver reservas activas
-- SELECT * FROM vista_reservas_activas;

-- Ver ventas completas
-- SELECT * FROM vista_ventas_completas;

-- Estadísticas generales
-- SELECT 
--     (SELECT COUNT(*) FROM Uniformes WHERE estado = 'Disponible') as uniformes_disponibles,
--     (SELECT COUNT(*) FROM Uniformes WHERE estado = 'Reservado') as uniformes_reservados,
--     (SELECT COUNT(*) FROM Uniformes WHERE estado = 'Vendido') as uniformes_vendidos,
--     (SELECT COUNT(*) FROM Reservas WHERE estado_reserva = 'Activa') as reservas_activas,
--     (SELECT COUNT(*) FROM Ventas WHERE confirmado = TRUE) as ventas_confirmadas;

-- =====================================================
-- FIN DEL SCRIPT
-- =====================================================