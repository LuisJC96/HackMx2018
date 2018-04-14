CREATE TABLE [dbo].[Estadios] (
    [nombreEstadio] NVARCHAR (50) NOT NULL,
    [sede]          NVARCHAR (50) NOT NULL,
    [capacidad]     INT           NULL,
    [id]            INT           IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_Estadios] PRIMARY KEY CLUSTERED ([id] ASC)
);

