using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotExample.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {

        static HttpClient client = new HttpClient();

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            Models.Address address = null;
            var url = String.Format("https://viacep.com.br/ws/{0}/json/", activity.Text);


            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString(url);
                JavaScriptSerializer jss = new JavaScriptSerializer();
                address = jss.Deserialize<Models.Address>(json);
            }


            if (address != null)
            {

                await context.PostAsync($"Resultado:" +
                    $" Endreço: {address.Logradouro} \n " +
                    $" Bairro: {address.Bairro} \n" +
                    $" Estado: {address.Localidade} \n" +
                    $" Cep: {address.Cep} ");



            }
            else
            {
                await context.PostAsync($"Nenhum endereço encontrado!");
            }


            context.Wait(MessageReceivedAsync);
        }
    }
}