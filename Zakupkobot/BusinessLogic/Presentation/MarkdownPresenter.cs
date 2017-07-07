using System.Text;
using BusinessLogic.Service_References.StarService;

namespace Presentation
{
    public class MarkdownPresenter : Presenter
    {
        public override void AddFoundInRegionHeader(string request, string region)
        {
            string reg = string.IsNullOrEmpty(region) ? string.Empty : $"в регионе {region}";
            sb.Append($"По запросу **{request}** {reg} были найдены следующие закупки:   \r\n");
        }

        public override void BuildResult(GetTendersResponse response)
        {
            foreach (var tender in response.Tenders.Itemsk__BackingField)
            {
                string number = string.IsNullOrEmpty(tender.RegistrationNumber)
                    ? tender.Number
                    : tender.RegistrationNumber;
                sb.Append("[");
                sb.Append(number);
                /*
                                sb.Append(
                                    String.Format(
                                        "](http://zakupki.gov.ru/epz/order/notice/view/common-info.html?regNumber={0})   \r\n",
                                        tender.RegistrationNumber));                
                */
                //sb.Append($"](http://{tender.Url})   \r\n");
                sb.Append($"](https://star.fintender.ru/Search/GetActual/l{number}-1)   \r\n");
                //sb.Append($"](http://b2bpoint.ru/TenderInterests/LogViewAndRedirect?tenderId={tender.ID}&tenderStatus={tender.TenderStatus})  \r\n");


                sb.Append(StripHTML(tender.TradeName));
                sb.Append("  \r\n **Заказчик:** ");
                sb.Append(StripHTML(tender.OrganizerName));
                sb.Append("  \r\n **Цена:** ");
                sb.Append($"{tender.Price:#.00}");
                sb.Append(" рублей  \r\n **Дата публикации:** ");
                sb.Append(tender.DatePublished);
                sb.Append("  \r\n ***  \r\n");
            }
        }

        public override void BuildSearhLink(int numberOfResults, string region)
        {
            string suffix = string.IsNullOrEmpty(region) ? string.Empty : ($"&SearchForm.Region={region}00000000000|1");
            sb.Append(
                $"Отображено {numberOfResults} самых свежих закупок, больше закупок можно найти в системе [СТАР](https://star.fintender.ru)");
            //$"Отображено {numberOfResults} самых свежих закупок, больше закупок можно найти в системе [СТАР](https://star.fintender.ru/Search/?query={msg.Text})";
            //$"Отображено {numberOfResults} самых свежих закупок, больше закупок можно найти на [B2BPoint.ru](http://b2bpoint.ru/?SearchForm.Keywords={msg.Text}{suffix})";

        }

        public override void WarningNoRegion()
        {
            sb.Append("  \r\n**ВНИМАНИЕ, данные результаты найдены без учета региона**. Установить регион можно командой /регион NN, где NN - код региона.");
        }

        public override void NotFound(string request)
        {
            sb.Append($"К сожалению, по запросу **{request}** ничего найти не удалось, попробуй перефразировать запрос");
        }
    }
}