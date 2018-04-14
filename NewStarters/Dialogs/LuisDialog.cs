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
        subscriptionKey: "a46b97fa8dfc4101b767186ee5e3da34",
        apiVersion:LuisApiVersion.V2,
        domain: "southcentralus.api.cognitive.microsoft.com")]

    public class LuisDialog : LuisDialog<object>
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