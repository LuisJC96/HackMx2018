using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using Microsoft.Bot.Builder.FormFlow;

namespace FootballOne.Dialogs
{
    [Serializable]
    [LuisModel(
        modelID: "1d72cab2-9168-4601-bcd3-cdd2005e3885",
        subscriptionKey: "4ecdb5b07513435796578ae6d5217f06",
        apiVersion: LuisApiVersion.V2,
        domain: "southcentralus.api.cognitive.microsoft.com")]

    public class LuisHandler : LuisDialog<object>
    {
        /*************************************
         * Métodos para el manejo de intents
         * **********************************/
        private EntityRecommendation entityRec;
        //Intent no reconocido
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            var act = await activity as Activity;
            string message = $"Perdón no entendí qué quisiste decir con {act.Text}, todavía estoy aprendiendo pero me puedes preguntar sobre los grupos, estadios o fechas de partidos";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            return;
        }
        [LuisIntent("Ejemplo")]
        public async Task EjemploIntent(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            int a = 2;
            string message = $"hola a vale {a}, este es el intent de EJEMPLO";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            return;
        }
        [LuisIntent("NombreJefeInmediato")]
        public async Task NombreJefeInmediatoIntent(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            Models.NewStartersDBEntities1 DB = new Models.NewStartersDBEntities1();
            HeroCard card = new HeroCard();
            List<CardImage> cardImages = new List<CardImage>();
            card.Title = "Jefe";
            //Mis datos es literalmente todo lo de mi mismo (Bruno) se accede de la forma misDatos[0].foto
            var misDatos = ( from yo in DB.Empleado
                             where yo.empleadoID == 6
                             select new
                             {
                                 nombre = yo.nombre,
                                 apellido = yo.apellido,
                                 jefeId = yo.jefeInmediatoID,
                                 puesto = yo.puesto,
                                 sede = yo.sede,
                                 departamentoID = yo .departamento,
                                 foto = yo.fotoURL,
                                 jerarquia = yo.jerarquia
                             }
                ).ToList();
            int id = (int)misDatos[0].jefeId;
            var datosJefe = ( from jefe in DB.Empleado
                          where jefe.empleadoID == id
                          select new
                          {
                              nombre = jefe.nombre,
                              apellido = jefe.apellido,
                              sede = jefe.sede,
                              foto = jefe.fotoURL
                          }
                ).ToList();
            cardImages.Add(new CardImage(datosJefe[0].foto));
            card.Images = cardImages;
            message += $"Tu jefe es {datosJefe[0].nombre} {datosJefe[0].apellido}";
            card.Subtitle = $"{datosJefe[0].nombre} {datosJefe[0].apellido}";
            card.Text = $"Tu jefe es {datosJefe[0].nombre}";
            Attachment att = card.ToAttachment();
            IMessageActivity reply = context.MakeMessage();
            reply.Attachments.Add(att);
            await context.PostAsync(reply);
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            return;
        }
        [LuisIntent("Sublevados")]
        public async Task NombreSublevados(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            Models.NewStartersDBEntities1 DB = new Models.NewStartersDBEntities1();
            //Mis datos es literalmente todo lo de mi mismo (Bruno) se accede de la forma misDatos[0].foto
            var misDatos = (from yo in DB.Empleado
                            where yo.empleadoID == 6
                            select new
                            {
                                nombre = yo.nombre,
                                apellido = yo.apellido,
                                jefeId = yo.jefeInmediatoID,
                                puesto = yo.puesto,
                                sede = yo.sede,
                                departamentoID = yo.departamento,
                                foto = yo.fotoURL,
                                jerarquia = yo.jerarquia,
                                empleadoID = yo.empleadoID
                            }
                ).ToList();
            int id = (int)misDatos[0].empleadoID;
            System.Diagnostics.Debug.WriteLine($"id: {id}");
            var datosEmpleados = (from empleado in DB.Empleado
                             where empleado.jefeInmediatoID == id
                             select new
                             {
                                 nombre = empleado.nombre,
                                 apellido = empleado.apellido,
                                 sede = empleado.sede,
                                 idPrueba = empleado.empleadoID,
                                 foto = empleado.fotoURL,
                                 tel = empleado.numero,
                                 mail = empleado.mail
                             }
                ).ToList();
            if (datosEmpleados.Count() == 0)
            {
                message += "De momento no tienes registrados empleados";
                await context.PostAsync(message);
                context.Wait(MessageReceived);
                return;
            }
            else
            {
                for (int i = 0; i < datosEmpleados.Count; i++)
                {
                    if (i == 0)
                    {
                        message += $"Tus empleados son: ";
                        await context.PostAsync(message);

                    }
                    List<CardImage> cardImages = new List<CardImage>();
                    HeroCard card = new HeroCard();
                    card.Title = $"{datosEmpleados[i].nombre} {datosEmpleados[i].apellido}";
                    card.Subtitle = "Datos de contacto:";
                    card.Text = $"Teléfono: {datosEmpleados[i].tel}  \n";
                    card.Text += $"Correo: {datosEmpleados[i].mail}";
                    cardImages.Add(new CardImage(datosEmpleados[i].foto));
                    card.Images = cardImages;
                    Attachment att = card.ToAttachment();
                    IMessageActivity reply = context.MakeMessage();
                    reply.Attachments.Add(att);
                    await context.PostAsync(reply);
                }
            }
            
            
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            return;
        }
        [LuisIntent("Saludar")]
        public async Task Saludar(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            Models.NewStartersDBEntities1 DB = new Models.NewStartersDBEntities1();
            message += "¡Hola!";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            return;
        }
        [LuisIntent("QuienEres")]
        public async Task QuienEres(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            Models.NewStartersDBEntities1 DB = new Models.NewStartersDBEntities1();
            message += "Hola, soy New Starter, soy un asistente que te permite ponerte al corriente " +
                "con la empresa, preguntame lo que necesites";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            return;
        }
        [LuisIntent("ComoEstas")]
        public async Task ComoEstas(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            message += "Yo estoy muy bien, ¿tú que tal?";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("TI")]
        public async Task TI(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            Models.NewStartersDBEntities1 DB = new Models.NewStartersDBEntities1();
            //Mis datos es literalmente todo lo de mi mismo (Bruno) se accede de la forma misDatos[0].foto
            var misDatos = (from dept in DB.Empleado_Departamento
                            where dept.departamentoID == 3
                            select new
                            {
                                representate = dept.representanteID,
                                jefe = dept.jefeID,
                                dept = dept.departamentoID
                            }
                ).ToList();
            int id = (int)misDatos[0].jefe;
            var datosJefe = (from jefe in DB.Empleado
                             where jefe.empleadoID == id
                             select new
                             {
                                 nombre = jefe.nombre,
                                 apellido = jefe.apellido,
                                 sede = jefe.sede,
                                 foto = jefe.fotoURL,
                                 mail = jefe.mail,
                                 tel = jefe.numero
                             }
                ).ToList();

            List<CardImage> cardImages = new List<CardImage>();
            HeroCard card = new HeroCard();
            card.Title = $"{datosJefe[0].nombre} {datosJefe[0].apellido}";
            card.Subtitle = "Jefe del departamento de TI";
            card.Text = $"Teléfono: {datosJefe[0].tel}  \n";
            card.Text += $"Correo: {datosJefe[0].mail}";
            cardImages.Add(new CardImage(datosJefe[0].foto));
            card.Images = cardImages;
            Attachment att = card.ToAttachment();
            IMessageActivity reply = context.MakeMessage();
            reply.Attachments.Add(att);
            await context.PostAsync(reply);
            int id2 = (int)misDatos[0].representate;
            var datosRep = (from representante in DB.Empleado
                             where representante.empleadoID == id
                             select new
                             {
                                 nombre = representante.nombre,
                                 apellido = representante.apellido,
                                 sede = representante.sede,
                                 foto = representante.fotoURL,
                                 tel = representante.numero,
                                 mail = representante.mail
                             }
                ).ToList();
            /*List<CardImage> cardImages2 = new List<CardImage>();
            HeroCard card2 = new HeroCard();
            card2.Title = $"{datosRep[0].nombre} {datosRep[0].apellido}";
            card2.Subtitle = "Datos de contacto:";
            card2.Text = $"Teléfono: {datosRep[0].tel}  \n";
            card2.Text += $"Correo: {datosRep[0].mail}";
            cardImages2.Add(new CardImage(datosRep[0].foto));
            card2.Images = cardImages;
            Attachment att2 = card2.ToAttachment();
            IMessageActivity reply2 = context.MakeMessage();
            reply2.Attachments.Add(att2);
            await context.PostAsync(reply2);
            context.Wait(MessageReceived);*/
            return;
        }

        [LuisIntent("Administracion")]
        public async Task Administracion(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            Models.NewStartersDBEntities1 DB = new Models.NewStartersDBEntities1();
            //Mis datos es literalmente todo lo de mi mismo (Bruno) se accede de la forma misDatos[0].foto
            var misDatos = (from dept in DB.Empleado_Departamento
                            where dept.departamentoID == 4
                            select new
                            {
                                representate = dept.representanteID,
                                jefe = dept.jefeID,
                                dept = dept.departamentoID
                            }
                ).ToList();
            int id = (int)misDatos[0].jefe;
            var datosJefe = (from jefe in DB.Empleado
                             where jefe.empleadoID == id
                             select new
                             {
                                 nombre = jefe.nombre,
                                 apellido = jefe.apellido,
                                 sede = jefe.sede,
                                 foto = jefe.fotoURL,
                                 tel = jefe.numero,
                                 mail = jefe.mail
                             }
                ).ToList();
            await context.PostAsync(message);
            int id2 = (int)misDatos[0].representate;
            var datosRep = (from representante in DB.Empleado
                            where representante.empleadoID == id
                            select new
                            {
                                nombre = representante.nombre,
                                apellido = representante.apellido,
                                sede = representante.sede
                            }
                ).ToList();
            List<CardImage> cardImages = new List<CardImage>();
            HeroCard card = new HeroCard();
            card.Title = $"{datosJefe[0].nombre} {datosJefe[0].apellido}";
            card.Subtitle = "Jefe del departamento de administración";
            card.Text = $"Teléfono: {datosJefe[0].tel}  \n";
            card.Text += $"Correo: {datosJefe[0].mail}";
            cardImages.Add(new CardImage(datosJefe[0].foto));
            card.Images = cardImages;
            Attachment att = card.ToAttachment();
            IMessageActivity reply = context.MakeMessage();
            reply.Attachments.Add(att);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
            return;
        }

        [LuisIntent("Finanzas")]
        public async Task Finanazas(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            Models.NewStartersDBEntities1 DB = new Models.NewStartersDBEntities1();
            //Mis datos es literalmente todo lo de mi mismo (Bruno) se accede de la forma misDatos[0].foto
            var misDatos = (from dept in DB.Empleado_Departamento
                            where dept.departamentoID == 1
                            select new
                            {
                                representate = dept.representanteID,
                                jefe = dept.jefeID,
                                dept = dept.departamentoID
                            }
                ).ToList();
            int id = (int)misDatos[0].jefe;
            var datosJefe = (from jefe in DB.Empleado
                             where jefe.empleadoID == id
                             select new
                             {
                                 nombre = jefe.nombre,
                                 apellido = jefe.apellido,
                                 sede = jefe.sede,
                                 foto = jefe.fotoURL,
                                 mail = jefe.mail,
                                 tel = jefe.numero
                             }
                ).ToList();
            int id2 = (int)misDatos[0].representate;
            var datosRep = (from representante in DB.Empleado
                            where representante.empleadoID == id
                            select new
                            {
                                nombre = representante.nombre,
                                apellido = representante.apellido,
                                sede = representante.sede
                            }
                ).ToList();
            List<CardImage> cardImages = new List<CardImage>();
            HeroCard card = new HeroCard();
            card.Title = $"{datosJefe[0].nombre} {datosJefe[0].apellido}";
            card.Subtitle = "Jefe del departamento de finanzas";
            card.Text = $"Teléfono: {datosJefe[0].tel}  \n";
            card.Text += $"Correo: {datosJefe[0].mail}";
            cardImages.Add(new CardImage(datosJefe[0].foto));
            card.Images = cardImages;
            Attachment att = card.ToAttachment();
            IMessageActivity reply = context.MakeMessage();
            reply.Attachments.Add(att);
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
            return;
        }

        /*************************************
            * Métodos para el manejo y filtro de entities
            * **********************************/
        List<String> GetFilteredEntities(LuisResult result, string entityType)
        {
            List<String> res = new List<string>();
            for (int i = 0; i < result.Entities.Count; i++)
            {
                EntityRecommendation current = result.Entities[i];
                if (current.Type == entityType)
                { 
                    res.Add(((List<object>)current.Resolution["values"])[0].ToString());//Esto regresa el valor original en lugar del sinónimo
                }
            }
            return res;
        }
        String ExtractEntities(LuisResult result, string entityType)
        {
            result.TryFindEntity(entityType, out entityRec);
            if (entityRec != null)
            {
                return entityRec.Entity;
            }
            else
            {
                return "notFound";
            }
        }
    }
    /**********************
     * Manejo de forms
     * ********************/
    public enum GroupOptions
    {
        [Describe("Grupo A")]
        [Terms("A")]
        GrupoA = 1,
        [Describe("Grupo B")]
        GrupoB = 2,
        [Describe("Grupo C")]
        GrupoC,
        [Describe("Grupo D")]
        GrupoD,
        [Describe("Grupo E")]
        GrupoE,
        [Describe("Grupo F")]
        GrupoF,
        [Describe("Grupo G")]
        GrupoG,
        [Describe("Grupo H")]
        GrupoH
    };
    [Serializable]
    public class InfoGroup
    {
        public GroupOptions? grupo;

        public static IForm<InfoGroup> BuildForm()
        {
            var form = new FormBuilder<InfoGroup>()
                    .Message("¿De qué grupo quieres saber?")
                    .AddRemainingFields();
            List<string> quitCommands = new List<string>();
            quitCommands.Add("Salir");
            quitCommands.Add("Cancelar");
            quitCommands.Add("No");
            quitCommands.Add("Quiero salir");
            form.Configuration.Commands[FormCommand.Quit].Terms = quitCommands.ToArray();
            return form.Build();
        }
    }
}