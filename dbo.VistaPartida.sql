CREATE view [dbo].[VistaPartida] as
Select e1.Grupo as Grupo, st.nombreEstadio as nombreEstadio, e1.nombreEquipo as Equipo1, e2.nombreEquipo as Equipo2, p.Fecha as fecha, p.Hora as hora 
From Equipo e1 JOIN Partidas p ON e1.idEquipo=p.IDEquipo1 join Equipo e2 ON e2.idEquipo=p.IDEquipo2 join Estadios st ON p.EstadioFK=st.id