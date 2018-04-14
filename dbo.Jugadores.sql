CREATE TABLE [dbo].[Jugadores] (
    [idJugador]     INT        IDENTITY (1, 1) NOT NULL,
    [idEquipo]      INT        NOT NULL,
    [nombreJugador] NCHAR (50) NULL,
    [posicion]      NCHAR (50) NULL,
    CONSTRAINT [PK_Jugadores] PRIMARY KEY CLUSTERED ([idJugador] ASC),
    CONSTRAINT [FK_Jugadores_Equipo] FOREIGN KEY ([idEquipo]) REFERENCES [dbo].[Equipo] ([idEquipo])
);

