CREATE TABLE [dbo].[Equipo] (
    [idEquipo]     INT           IDENTITY (1, 1) NOT NULL,
    [Grupo]        NVARCHAR (50) NULL,
    [nombreEquipo] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Equipo] PRIMARY KEY CLUSTERED ([idEquipo] ASC),
    CONSTRAINT [FK_Equipo_Equipo] FOREIGN KEY ([idEquipo]) REFERENCES [dbo].[Equipo] ([idEquipo])
);

