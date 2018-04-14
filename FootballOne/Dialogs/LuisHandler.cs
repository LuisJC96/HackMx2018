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
        subscriptionKey: "a46b97fa8dfc4101b767186ee5e3da34")]

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
      
        public string RemoveDiacritics(String s)
        {
            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
        public String checkSingle(int compare)
        {
            if (compare == 1)
            {
                return "";
            }
            else
            {
                return "s";
            }

        }
        public void SimplifyFraction(int upIn, int downIn, out int up, out int down) //Recibe dos número partes de una sola fracción y los simplifica
        {
            int limit;
            up = upIn;
            down = downIn;
            if(upIn > downIn)
            {
                limit = downIn;
                
            }
            else
            {
                limit = upIn;
            }
            for (int x = 1; x <= limit; x++)
            {
                if ( ((downIn % x) == 0 ) && ((upIn % x) == 0))
                {
                    upIn = upIn / x;
                    downIn = downIn / x;
                }
            }
            down = downIn;
            up = upIn;
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