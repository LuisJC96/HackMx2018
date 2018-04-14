CREATE TABLE [dbo].[Partidas] (
    [idPartida]     INT         IDENTITY (1, 1) NOT NULL,
    [numeroPartida] INT         NULL,
    [EstadioFK]     INT         NULL,
    [Fecha]         DATE        NULL,
    [IDEquipo1]     INT         NOT NULL,
    [IDEquipo2]     INT         NOT NULL,
    [IDGanador]     INT         NULL,
    [Hora]          VARCHAR (5) NOT NULL,
    CONSTRAINT [PK_Partidas] PRIMARY KEY CLUSTERED ([idPartida] ASC),
    CONSTRAINT [FK_Partidas_Equipo] FOREIGN KEY ([IDEquipo1]) REFERENCES [dbo].[Equipo] ([idEquipo]),
    CONSTRAINT [FK_Partidas_Equipo2] FOREIGN KEY ([IDEquipo2]) REFERENCES [dbo].[Equipo] ([idEquipo])
);

