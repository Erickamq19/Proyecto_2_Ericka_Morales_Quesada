
IF DB_ID(N'Proyecto_2_DB') IS NULL
BEGIN
    CREATE DATABASE Proyecto_2_DB;
END;
GO

USE Proyecto_2_DB;
GO


CREATE TABLE dbo.Roles
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE,
    Descripcion NVARCHAR(150) NULL
);
GO

SET IDENTITY_INSERT dbo.Roles ON;

INSERT INTO dbo.Roles (Id, Nombre, Descripcion)
VALUES
(1, N'Administrador', N'Gestión de usuarios, tareas y proyectos'),
(2, N'Usuario', N'Gestión de los proyectos y tareas autorizadas');

SET IDENTITY_INSERT dbo.Roles OFF;
GO


CREATE TABLE dbo.Usuarios
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    ContrasenaHash NVARCHAR(255) NOT NULL,
    IdRol INT NOT NULL
        CONSTRAINT DF_Usuarios_Rol DEFAULT 2,
    FechaRegistro DATETIME2 NOT NULL
        DEFAULT SYSDATETIME(),
    Activo BIT NOT NULL
        DEFAULT 1,

    CONSTRAINT FK_Usuarios_Roles
        FOREIGN KEY (IdRol)
        REFERENCES dbo.Roles(Id)
);
GO


CREATE TABLE dbo.Proyectos
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NULL,
    IdUsuarioCreador INT NOT NULL,
    FechaCreacion DATETIME2 NOT NULL
        DEFAULT SYSDATETIME(),
    Activo BIT NOT NULL
        DEFAULT 1,

    CONSTRAINT FK_Proyectos_Usuarios
        FOREIGN KEY (IdUsuarioCreador)
        REFERENCES dbo.Usuarios(Id),

    CONSTRAINT CK_Proyectos_Fechas
        CHECK (FechaFin IS NULL OR FechaFin >= FechaInicio)
);
GO


CREATE TABLE dbo.Estados
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(40) NOT NULL UNIQUE,
    Descripcion NVARCHAR(150) NULL
);
GO

SET IDENTITY_INSERT dbo.Estados ON;

INSERT INTO dbo.Estados (Id, Nombre, Descripcion)
VALUES
(1, N'Pendiente', N'Tarea registrada y pendiente de iniciar'),
(2, N'En Proceso', N'Tarea en desarrollo'),
(3, N'Completada', N'Tarea finalizada'),
(4, N'Cancelada', N'No requiere ejecución');

SET IDENTITY_INSERT dbo.Estados OFF;
GO


CREATE TABLE dbo.Prioridades
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(30) NOT NULL UNIQUE,
    Descripcion NVARCHAR(150) NULL
);
GO

SET IDENTITY_INSERT dbo.Prioridades ON;

INSERT INTO dbo.Prioridades (Id, Nombre, Descripcion)
VALUES
(1, N'Baja', N'Requiere de poca urgencia'),
(2, N'Media', N'Requiere de atención normal'),
(3, N'Alta', N'Requiere de atención prioritaria'),
(4, N'Urgente', N'Requiere de atención inmediata');

SET IDENTITY_INSERT dbo.Prioridades OFF;
GO


CREATE TABLE dbo.Tareas
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(150) NOT NULL,
    Descripcion NVARCHAR(MAX) NULL,

    IdProyecto INT NOT NULL,
    IdUsuarioCreador INT NOT NULL,
    IdUsuarioAsignado INT NULL,
    IdEstado INT NOT NULL
        CONSTRAINT DF_Tareas_Estado DEFAULT 1,
    IdPrioridad INT NOT NULL
        CONSTRAINT DF_Tareas_Prioridad DEFAULT 2,

    FechaCreacion DATETIME2 NOT NULL
        DEFAULT SYSDATETIME(),
    FechaActualizacion DATETIME2 NULL,
    FechaLimite DATE NULL,
    FechaCompletada DATETIME2 NULL,

    CONSTRAINT FK_Tareas_Proyectos
        FOREIGN KEY (IdProyecto)
        REFERENCES dbo.Proyectos(Id),

    CONSTRAINT FK_Tareas_Creador
        FOREIGN KEY (IdUsuarioCreador)
        REFERENCES dbo.Usuarios(Id),

    CONSTRAINT FK_Tareas_Asignado
        FOREIGN KEY (IdUsuarioAsignado)
        REFERENCES dbo.Usuarios(Id),

    CONSTRAINT FK_Tareas_Estados
        FOREIGN KEY (IdEstado)
        REFERENCES dbo.Estados(Id),

    CONSTRAINT FK_Tareas_Prioridades
        FOREIGN KEY (IdPrioridad)
        REFERENCES dbo.Prioridades(Id)
);
GO

SELECT
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO
