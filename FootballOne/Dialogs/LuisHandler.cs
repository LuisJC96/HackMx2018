﻿using System;
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
            Models.NewStartersDBEntities DB = new Models.NewStartersDBEntities();
            HeroCard card = new HeroCard();
            List<CardImage> cardImages = new List<CardImage>();
            cardImages.Add(new CardImage("http://www.creativefan.com/important/cf/2012/10/black-men-hairstyles/any-way.jpg"));
            card.Images = cardImages;
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
                              sede = jefe.sede
                          }
                ).ToList();
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
            Models.NewStartersDBEntities DB = new Models.NewStartersDBEntities();
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
                                 idPrueba = empleado.empleadoID
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
                        message += $"Tus empleados son:  \n " +
                        $"{datosEmpleados[i].nombre} {datosEmpleados[i].apellido}  \n";
                    }
                    else
                    {
                        message += $"{datosEmpleados[i].nombre} {datosEmpleados[i].apellido}  \n";
                    }

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
            Models.NewStartersDBEntities DB = new Models.NewStartersDBEntities();
            message += "¡Hola!";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            return;
        }
        [LuisIntent("QuienEres")]
        public async Task QuienEres(IDialogContext context, IAwaitable<object> activity, LuisResult result)
        {
            string message = $"";
            Models.NewStartersDBEntities DB = new Models.NewStartersDBEntities();
            message += "Hola, soy New Starter, soy un asistente que te permite ponerte al corriente " +
                "con la empresa, preguntame lo que necesites";
            await context.PostAsync(message);
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