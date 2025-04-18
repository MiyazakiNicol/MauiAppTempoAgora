﻿using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade) 
        {
            Tempo? t = null;

            string chave = "e3a21be07af87bf06b61243449764d69";

            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cidade}&untis=metric&appid={chave}";

            using (HttpClient client = new HttpClient()) 
            {

                try
                {
                    HttpResponseMessage resp = await client.GetAsync(url);

                    if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        throw new Exception("Cidade não encontrada. Verifique o nome digitado.");
                    }

                    if (resp.IsSuccessStatusCode)
                    {
                        string json = await resp.Content.ReadAsStringAsync();

                        var rascunho = JObject.Parse(json);

                        DateTime time = new();
                        DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        t = new Tempo
                        {
                            lat = (double)rascunho["coord"]["lat"],
                            lon = (double)rascunho["coord"]["lon"], // <- Corrigido também: antes você repetia "lat"
                            description = (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min = (double)rascunho["main"]["temp_min"],
                            temp_max = (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString(),
                            sunset = sunset.ToString(),
                        };
                    }
                }
                catch (HttpRequestException)
                {
                    throw new Exception("Sem conexão com a internet.");
                }
            }// fecha laço using


            return t;
        }
    }
}
