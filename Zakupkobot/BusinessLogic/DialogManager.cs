using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogic.Service_References.StarService;
using Microsoft.Bot.Connector;
using Presentation;

namespace BusinessLogic
{

    //TODO: Make async?

    public class DialogManager
    {
        private readonly ITenderSearchService starService;
        private BotData userData;

        public DialogManager(ITenderSearchService star, BotData data)
        {
            starService = star;
            userData = data;
        }

        public string ProcessMessage(Activity msg)
        {
            string result = HandleCommand(msg);

            if (String.IsNullOrEmpty(result)) result = GetResultFromStar(msg);

            return result;
        }

        private string GetResultFromStar(Activity msg)
        {
            try
            {
                var request = CreateRequest(msg.Text, GetRegion());

                GetTendersResponse response = starService.GetTenders(request);

                Presenter presenter = PresenterFactory.GetPresenter(msg.ChannelId);

                if (response.Tenders.TotalRowsk__BackingField > 0)
                {
                    
                    presenter.AddFoundInRegionHeader(msg.Text, GetRegion());
                    presenter.BuildResult(response);
                    presenter.BuildSearhLink(response.Tenders.Itemsk__BackingField.Count(), GetRegion());

                    if (String.IsNullOrEmpty(GetRegion()))
                    {
                        presenter.WarningNoRegion();
                    }

                    return presenter.Result;
                }
                else
                {
                    presenter.NotFound(msg.Text);
                    return $"К сожалению, по запросу **{msg.Text}** ничего найти не удалось, попробуй перефразировать запрос";
                }
            }
            catch (Exception exception)
            {
                //TODO: Add error logging
                System.Diagnostics.Trace.TraceError(exception.Message, exception.StackTrace);
                return "Упс, что-то пошло не так... :(";
            }
        }

        private static GetTendersPagedRequest CreateRequest(string msg, string region)
        {
            GetTendersPagedRequest request = new GetTendersPagedRequest();
            request.Keywords = msg;

            request.FilterData = new PagedFilter();
            request.FilterData.PageCount = 1;
            request.FilterData.PageIndex = 1;
            request.FilterData.PageSize = 5;
            request.State = 1;
            request.FilterData.SortingField = SortingField.DatePublished;
            request.FilterData.SortingDirection = SortingDirection.Desc;

            if (!String.IsNullOrWhiteSpace(region))
            {
                request.Region = region + "00000000000|1"; //TODO: Replace magic with something more appropriate
            }

            return request;
        }

        private string HandleCommand(Activity msg)
        {
            if (!LooksLikeACommand(msg.Text)) return null;

            string command = GetCommand(msg.Text).ToLower();

            //TODO: add commands handling code
            switch (command)
            {
                case "start":
                    return "Привет! Напиши мне, какой товар ты хочешь продать, или /? для получения дополнительной информации";
                case "setregion":
                case "регион":
                    return $"Регион поиска установлен в: {ProcessAndSetRegion(msg)}";
                case "getregion":
                case "регион?":
                    return $"Текущий регион поиска: {GetRegion()}";
                case "справка":
                case "помощь":
                case "хелп":
                case "help":
                case "?":
                    return GetHelp();
                default:
                    return "Прости, я не знаю такой команды";
            }
        }

        private static string GetHelp()
        {
            return "Просто напиши название товара для того, чтобы найти закупки по нему. Для того, чтобы установить регион поиска, используй команду /регион NN или *регион NN - вместо NN подставь код интересующего региона. Чтобы сбросить регион - напиши /регион без параметра.";
        }

        private static string GetCommand(string text)
        {
            if (!LooksLikeACommand(text)) return String.Empty; 
            return text.Split(' ')[0].Substring(1);
        }

        private static bool LooksLikeACommand(string text)
        {
            return text.StartsWith("/") || text.StartsWith("*");
        }

        private static string GetParameter(string text)
        {
            string[] result = text.Split(' ');
            return result.Length == 1 ? String.Empty: text.Split(' ')[1]; //return empty string in case of parameter absence
        }

        private string GetRegion()
        {
            //StateClient stateClient = msg.GetStateClient();
            //BotData userData = stateClient.BotState.GetPrivateConversationData(msg.ChannelId, msg.Conversation.Id, msg.From.Id); //TODO: Move to user level?
            return userData.GetProperty<string>("Region");
        }

        private  string ProcessAndSetRegion(Activity msg)
        {
            StateClient stateClient = msg.GetStateClient();
            //BotData userData = stateClient.BotState.GetPrivateConversationData(msg.ChannelId, msg.Conversation.Id, msg.From.Id); //TODO: Move to user level?
            string par = GetParameter(msg.Text);
            userData.SetProperty<string>("Region", par);
            stateClient.BotState.SetPrivateConversationData(msg.ChannelId, msg.Conversation.Id, msg.From.Id, userData);
            return String.IsNullOrEmpty(par) ? "любой." : par+ "   \r\nВернуться к поиску без учета региона можно с помощью команды /регион без параметра.";
        }

    }
}
