CREATE DATABASE Aeropuerto
USE Aeropuerto
CREATE SCHEMA InfoAerolinea
CREATE SCHEMA InfoPasajero

------------ TABLAS InfoAerolinea ------------
CREATE TABLE InfoAerolinea.Aerolinea
(
    idAerolinea BIGINT IDENTITY(1,1) NOT NULL,
    Nom_Aerolinea VARCHAR(200) NOT NULL,
    FlotaTotal INT NOT NULL,
    AñoFundacion INT NOT NULL,
    NumVuelos INT NOT NULL,
    Logotipo VARBINARY(MAX) NOT NULL,
    CONSTRAINT PK_AEROLINEA PRIMARY KEY (idAerolinea)
)

CREATE TABLE InfoAerolinea.Piloto
(
    idPiloto BIGINT IDENTITY(1,1) NOT NULL,
    Nom_Piloto VARCHAR(200) NOT NULL,
    Genero VARCHAR(20) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    NumLicencia BIGINT NOT NULL,
    CONSTRAINT PK_PILOTO PRIMARY KEY (idPiloto)
)

CREATE TABLE InfoAerolinea.Ciudad
(
    idCiudad BIGINT IDENTITY(1,1) NOT NULL,
    Nom_Ciudad VARCHAR(200) NOT NULL,
    Pais VARCHAR(200) NOT NULL,
    CONSTRAINT PK_CIUDAD PRIMARY KEY (idCiudad)
)

CREATE TABLE InfoAerolinea.Avion
(
    idAvion BIGINT IDENTITY(1,1) NOT NULL,
    idAerolinea BIGINT NOT NULL,
    Capacidad TINYINT NOT NULL,
    Modelo VARCHAR(200) NOT NULL,
    AñoFabricacion INT NOT NULL,
    EstadoUso BIT NOT NULL,
    CONSTRAINT PK_AVION PRIMARY KEY (idAvion),
    CONSTRAINT FK_AEROLINEA FOREIGN KEY (idAerolinea) REFERENCES InfoAerolinea.Aerolinea (idAerolinea)
)

CREATE TABLE InfoAerolinea.Vuelo
(
    idVuelo BIGINT IDENTITY(1,1) NOT NULL,
    idOrigen BIGINT NOT NULL,
    idDestino BIGINT NOT NULL,
    DuracionHoras INT NOT NULL,
    CostoBase MONEY NOT NULL,
    CONSTRAINT PK_VUELO PRIMARY KEY (idVuelo),
    CONSTRAINT FK_ORIGEN FOREIGN KEY (idOrigen) REFERENCES InfoAerolinea.Ciudad (idCiudad),
    CONSTRAINT FK_DESTINO FOREIGN KEY (idDestino) REFERENCES InfoAerolinea.Ciudad (idCiudad)
)
ALTER TABLE InfoAerolinea.Vuelo
ADD CONSTRAINT UQ_IDORGIDDEST UNIQUE (idOrigen, idDestino);

CREATE TABLE InfoAerolinea.Itinerario
(
    idItinerario BIGINT IDENTITY(1,1) NOT NULL,
    idPiloto BIGINT NOT NULL,
    idAvion BIGINT NOT NULL,
    idVuelo BIGINT NOT NULL,
    HoraSalida TIME NOT NULL,
    FechaVuelo DATE NOT NULL,
    CONSTRAINT PK_ITINERARIO PRIMARY KEY (idItinerario),
    CONSTRAINT FK_PILOTO FOREIGN KEY (idPiloto) REFERENCES InfoAerolinea.Piloto (idPiloto),
    CONSTRAINT FK_AVION FOREIGN KEY (idAvion) REFERENCES InfoAerolinea.Avion (idAvion),
    CONSTRAINT FK_VUELO FOREIGN KEY (idVuelo) REFERENCES InfoAerolinea.Vuelo (idVuelo)
)
ALTER TABLE InfoAerolinea.Itinerario
ADD CONSTRAINT UQ_PILOTODIA UNIQUE (idPiloto, FechaVuelo);

------------ TABLAS InfoPasajero ------------
CREATE TABLE InfoPasajero.Pasajero
(
    idPasajero BIGINT IDENTITY(1,1) NOT NULL,
    Nom_Pasajero VARCHAR(200) NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Edad INT,
    Nacionalidad VARCHAR(50) NOT NULL,
    Genero VARCHAR(20) NOT NULL,
	NumPasaporte VARCHAR(15) NOT NULL,
	Telefono BIGINT NOT NULL,
	ContactoEmergencia BIGINT NOT NULL,
	Email VARCHAR(200) NOT NULL,
    CONSTRAINT PK_PASAJERO PRIMARY KEY (idPasajero)
) 
ALTER TABLE InfoPasajero.Pasajero
ADD CONSTRAINT UQ_NUMPASAPORTEPASAJERO UNIQUE (NumPasaporte);
ALTER TABLE InfoPasajero.Pasajero
ADD CONSTRAINT UQ_NUMTELPASAJERO UNIQUE (Telefono);
ALTER TABLE InfoPasajero.Pasajero
ADD CONSTRAINT UQ_EMAILPASAJERO UNIQUE (Email);

CREATE TABLE InfoPasajero.TarjetaPasajero
(
    idTarjetaPasajero BIGINT IDENTITY(1,1) NOT NULL,
    idPasajero BIGINT NOT NULL,
    NombreTitular VARCHAR(200) NOT NULL,
    Banco VARCHAR(200) NOT NULL,
    NumTarjeta BIGINT NOT NULL,
    FechaVencimiento DATE NOT NULL,
    CVV INT NOT NULL,
    CONSTRAINT PK_TARJETAPASAJERO PRIMARY KEY (idTarjetaPasajero),
    CONSTRAINT FK_PASAJERO1 FOREIGN KEY (idPasajero) REFERENCES InfoPasajero.Pasajero (idPasajero)
)
ALTER TABLE InfoPasajero.TarjetaPasajero
ADD CONSTRAINT UQ_IDPASAJNUMTARJ UNIQUE (idPasajero, NumTarjeta);

CREATE TABLE InfoPasajero.Asiento
(   
    idAsiento BIGINT IDENTITY(1,1) NOT NULL,
    idItinerario BIGINT NOT NULL,
    Num_Asiento TINYINT NOT NULL,
    Letra CHAR(1) NOT NULL,
    Ocupado BIT NOT NULL,
    CONSTRAINT PK_ASIENTO PRIMARY KEY (idAsiento),
    CONSTRAINT FK_ITINERARIO1 FOREIGN KEY (idItinerario) REFERENCES InfoAerolinea.Itinerario (idItinerario)
)
ALTER TABLE InfoPasajero.Asiento
ADD CONSTRAINT UQ_ASIENTO UNIQUE (idItinerario, Num_Asiento, Letra);

CREATE TABLE InfoPasajero.Venta
(
    idVenta BIGINT IDENTITY(1,1) NOT NULL,
    idTarjetaPasajero BIGINT NOT NULL,
    idItinerario BIGINT NOT NULL,
    FechaVenta DATETIME DEFAULT GETDATE(),
    MontoTotal MONEY,
    EstadoPago BIT NOT NULL,
    CONSTRAINT PK_VENTA PRIMARY KEY (idVenta),
    CONSTRAINT FK_TARJETAPASAJERO FOREIGN KEY (idTarjetaPasajero) REFERENCES InfoPasajero.TarjetaPasajero (idTarjetaPasajero),
    CONSTRAINT FK_ITINERARIO2 FOREIGN KEY (idItinerario) REFERENCES InfoAerolinea.Itinerario (idItinerario)
)
ALTER TABLE InfoPasajero.Venta
ADD CONSTRAINT UQ_TJPASAJEROITINERARIO UNIQUE (idTarjetaPasajero, idItinerario);

CREATE TABLE InfoPasajero.Boleto
(
    idVenta BIGINT NOT NULL,
    idPasajero BIGINT NULL,
    idAsiento BIGINT NOT NULL,
    Impuestos REAL NOT NULL,
    TarifasAdicionales MONEY NOT NULL,
    CostoTotal MONEY NOT NULL,
    Estado BIT NOT NULL,
    CONSTRAINT PK_BOLETO PRIMARY KEY (idVenta, idPasajero, idAsiento),
    CONSTRAINT FK_VENTA FOREIGN KEY (idVenta) REFERENCES InfoPasajero.Venta (idVenta),
    CONSTRAINT FK_PASAJERO2 FOREIGN KEY (idPasajero) REFERENCES InfoPasajero.Pasajero (idPasajero),
    CONSTRAINT FK_ASIENTO FOREIGN KEY (idAsiento) REFERENCES InfoPasajero.Asiento (idAsiento)
)

-- Quitar el Not Null en este atributo
ALTER TABLE InfoPasajero.Boleto
ALTER COLUMN idPasajero BIGINT NULL;

-- Eliminar la restricción de clave primaria anterior
ALTER TABLE InfoPasajero.Boleto
DROP CONSTRAINT PK_BOLETO;

-- Agregar la nueva columna idBoleto como clave primaria con IDENTIDAD
ALTER TABLE InfoPasajero.Boleto
ADD idBoleto BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY;

-------------------------------------- TRIGGERS --------------------------------------------------

-- Trigger 1 --
CREATE TRIGGER TR_CUPOASIENTO
ON InfoPasajero.Asiento
AFTER INSERT, UPDATE
AS
BEGIN
    -- Verifica si la clave foránea se ha insertado más de 8 veces.
    IF ((SELECT COUNT(*) FROM InfoPasajero.Asiento WHERE idItinerario IN (SELECT idItinerario FROM inserted)) > 8)
    BEGIN
        -- Si ya se insertó 8 veces, entonces genera un error.
        THROW 50000, 'No se pueden insertar más de 8 registros con el mismo itinerario', 1;
        ROLLBACK TRANSACTION; -- Deshace la transacción para evitar la inserción.
    END
END;

-- Trigger 2 --
CREATE TRIGGER TR_ACTUALIZAMONTOTOTAL1
ON InfoPasajero.Boleto
AFTER INSERT, UPDATE
AS
BEGIN
    -- Declarar una variable para almacenar el costo del boleto
    DECLARE @CostoBoleto MONEY;

    -- Obtener el costo del boleto del registro recién insertado
    SELECT @CostoBoleto = CostoTotal
    FROM inserted;

	-- Actualizar el campo "MontoTotal" en la tabla "Venta" restando el costo del boleto antiguo
    UPDATE V
    SET MontoTotal = MontoTotal - D.CostoTotal
    FROM InfoPasajero.Venta V
    INNER JOIN deleted D ON V.idVenta = D.idVenta;

    -- Actualizar el campo "MontoTotal" en la tabla "Venta" sumando el costo del boleto nuevo
    UPDATE V
    SET MontoTotal = MontoTotal + @CostoBoleto
    FROM InfoPasajero.Venta V
    INNER JOIN inserted I ON V.idVenta = I.idVenta;
END;

-- Trigger 3 --
CREATE TRIGGER TR_ACTUALIZAMONTOTOTAL2
ON InfoPasajero.Boleto
AFTER DELETE
AS
BEGIN
    -- Declarar una variable para almacenar el costo del boleto
    DECLARE @CostoBoleto MONEY;

    -- Obtener el costo del boleto del registro eliminado
    SELECT @CostoBoleto = CostoTotal
    FROM deleted;

    -- Actualizar el campo "MontoTotal" en la tabla "Venta" restando el costo del boleto
    UPDATE V
    SET MontoTotal = MontoTotal - @CostoBoleto
    FROM InfoPasajero.Venta V
    INNER JOIN deleted D ON V.idVenta = D.idVenta;
END;

-- Trigger 4 --
CREATE TRIGGER TR_BANCOTARJETA
ON InfoPasajero.TarjetaPasajero
AFTER INSERT, UPDATE
AS
BEGIN
    -- Declarar una variable para almacenar el número de tarjeta insertado
    DECLARE @NumeroTarjeta VARCHAR(16);

    -- Declarar una variable para almacenar el banco
    DECLARE @Banco VARCHAR(50);

    -- Obtener el número de tarjeta insertado
    SELECT @NumeroTarjeta = CAST(NumTarjeta AS VARCHAR(16))
    FROM INSERTED;

	-- Obtener el último dígito del número de tarjeta
    SET @NumeroTarjeta = RIGHT(@NumeroTarjeta, 1);

    -- Analizar los ultimos dígitos para determinar el banco
    IF @NumeroTarjeta = '1'
    BEGIN
        SET @Banco = 'Santander';
    END
    ELSE IF @NumeroTarjeta = '2'
    BEGIN
        SET @Banco = 'Bancomer';
    END
    ELSE IF @NumeroTarjeta = '3'
    BEGIN
        SET @Banco = 'Banorte';
    END
	ELSE IF @NumeroTarjeta = '4'
    BEGIN
        SET @Banco = 'BanBajio';
    END
	ELSE IF @NumeroTarjeta = '5'
    BEGIN
        SET @Banco = 'Scotiabank';
    END
	ELSE IF @NumeroTarjeta = '6'
    BEGIN
        SET @Banco = 'Banco Azteca';
    END
    ELSE
    BEGIN
        SET @Banco = 'Desconocido';
    END

    -- Actualizar el campo Banco en la misma fila de la tabla
    UPDATE InfoPasajero.TarjetaPasajero
    SET Banco = @Banco
    FROM INSERTED
    WHERE InfoPasajero.TarjetaPasajero.idTarjetaPasajero = INSERTED.idTarjetaPasajero;
END;

-- Triggers 5 --
CREATE TRIGGER TR_ACTUALIZARFLOTA1
ON InfoAerolinea.Avion
AFTER INSERT, UPDATE
AS
BEGIN
    IF UPDATE(idAerolinea)
    BEGIN
        -- Restar 1 a la antigua Aerolínea
        UPDATE InfoAerolinea.Aerolinea
        SET FlotaTotal = FlotaTotal - 1,
		NumVuelos = NumVuelos - (SELECT COUNT(*) FROM InfoAerolinea.Itinerario WHERE idAvion = (SELECT idAvion FROM DELETED))
        FROM InfoAerolinea.Aerolinea AS A
        INNER JOIN DELETED AS D ON A.idAerolinea = D.idAerolinea;

        -- Sumar 1 a la nueva Aerolínea
        UPDATE InfoAerolinea.Aerolinea
        SET FlotaTotal = FlotaTotal + 1,
		NumVuelos =  NumVuelos + (SELECT COUNT(*) FROM InfoAerolinea.Itinerario WHERE idAvion = (SELECT idAvion FROM INSERTED))
        FROM InfoAerolinea.Aerolinea AS A
        INNER JOIN INSERTED AS I ON A.idAerolinea = I.idAerolinea;
    END
END;

-- Trigger 6 --
CREATE TRIGGER TR_ACTUALIZARFLOTA2
ON InfoAerolinea.Avion
AFTER DELETE
AS
BEGIN
    UPDATE InfoAerolinea.Aerolinea
    SET FlotaTotal = FlotaTotal - 1,
	NumVuelos = NumVuelos - (SELECT COUNT(*) FROM InfoAerolinea.Itinerario WHERE idAvion = (SELECT idAvion FROM DELETED))
    FROM InfoAerolinea.Aerolinea AS A
    INNER JOIN DELETED AS D ON A.idAerolinea = D.idAerolinea;
END;

-- Trigger 7 --
CREATE TRIGGER TR_ACTUALIZARNUMVUELOS1
ON InfoAerolinea.Itinerario
AFTER INSERT, UPDATE
AS
BEGIN
    IF UPDATE(idAvion)
    BEGIN
        UPDATE InfoAerolinea.Aerolinea
        SET NumVuelos = NumVuelos - 1
        FROM InfoAerolinea.Aerolinea AS T
        INNER JOIN InfoAerolinea.Avion AS S ON T.idAerolinea = S.idAerolinea
        INNER JOIN DELETED AS D ON S.idAvion = D.idAvion;

        UPDATE InfoAerolinea.Aerolinea
        SET NumVuelos = NumVuelos + 1
        FROM InfoAerolinea.Aerolinea AS T
        INNER JOIN InfoAerolinea.Avion AS S ON T.idAerolinea = S.idAerolinea
        INNER JOIN INSERTED AS I ON S.idAvion = I.idAvion;
    END
END;

-- Trigger 8 --
CREATE TRIGGER TR_ACTUALIZARNUMVUELOS2
ON InfoAerolinea.Itinerario
AFTER DELETE
AS
BEGIN
    UPDATE InfoAerolinea.Aerolinea
    SET NumVuelos = NumVuelos - 1
    FROM InfoAerolinea.Aerolinea AS T
    INNER JOIN InfoAerolinea.Avion AS S ON T.idAerolinea = S.idAerolinea
    INNER JOIN DELETED AS D ON S.idAvion = D.idAvion;
END;

-- Trigger 9 --
CREATE TRIGGER TR_CALCULAREDADPASAJERO
ON InfoPasajero.Pasajero
AFTER INSERT, UPDATE
AS
BEGIN
	DECLARE @idpasajero as BIGINT
	BEGIN
		IF EXISTS (SELECT * FROM inserted)
		BEGIN
			SELECT @idpasajero = idPasajero FROM inserted
			UPDATE InfoPasajero.Pasajero
			SET Edad = DATEDIFF(YEAR, FechaNacimiento, GETDATE())-(CASE WHEN DATEADD(YY,DATEDIFF(YEAR,FechaNacimiento,GETDATE()), FechaNacimiento)>GETDATE() THEN 1 ELSE 0 END)
			WHERE idPasajero = @idpasajero
		END
	END
END;

-- Trigger 10 --
CREATE TRIGGER TR_ESTADOASIENTO1
ON InfoPasajero.Boleto
AFTER INSERT, UPDATE
AS
BEGIN
    IF UPDATE(idAsiento)
	BEGIN
		-- Actualizar el estado del asiento a NO ocupado en el itinerario pasado
		UPDATE InfoPasajero.Asiento
		SET Ocupado = 0
		FROM InfoPasajero.Asiento AS A
		INNER JOIN DELETED AS D ON A.idAsiento = D.idAsiento;

		-- Actualizar el estado del asiento a ocupado en el itinerario específico
		UPDATE InfoPasajero.Asiento
		SET Ocupado = 1
		FROM InfoPasajero.Asiento AS A
		INNER JOIN INSERTED AS I ON A.idAsiento = I.idAsiento;
	END
END;

-- Trigger 11 --
CREATE TRIGGER TR_ESTADOASIENTO2
ON InfoPasajero.Boleto
AFTER DELETE
AS
BEGIN
    -- Actualizar el estado del asiento a NO OCUPADO en el itinerario específico
    UPDATE InfoPasajero.Asiento
    SET Ocupado = 0
    FROM InfoPasajero.Asiento AS A
    INNER JOIN DELETED AS
    D ON A.idAsiento = D.idAsiento;
END;

--------------------------------------------- RULES --------------------------------------------------
--RULE 1
-- Crear la regla
CREATE RULE RL_NOM_BANCOS AS @Banco IN ('Santander','Bancomer','Banorte','BanBajio','Scotiabank','Banco Azteca','Desconocido');
-- Vincular la regla a la columna "Banco" en la tabla "TarjetaPasajero" en el esquema "InfoPasajero"
EXEC sp_bindrule 'RL_NOM_BANCOS', 'InfoPasajero.TarjetaPasajero.Banco';

--RULE 2
-- Crear la regla
CREATE RULE RL_EDAD AS @Edad BETWEEN 0 AND 110;
-- Vincular la regla
EXEC sp_bindrule 'RL_EDAD', 'InfoPasajero.Pasajero.Edad';

----------------------------------------------------------------------------------------------------------