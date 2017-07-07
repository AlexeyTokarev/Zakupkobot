using System.Text;
using BusinessLogic.Service_References.StarService;

namespace Presentation
{
    public class PlainPresenter : Presenter
    {
        
        public override void AddFoundInRegionHeader(string request, string region)
        {
            sb.Append($"По запросу {request} {region} были найдены следующие закупки:   \r\n");
        }

        public override void BuildResult(GetTendersResponse response)
        {
            foreach (var tender in response.Tenders.Itemsk__BackingField)
            {
                string number = string.IsNullOrEmpty(tender.RegistrationNumber)
                    ? tender.Number
                    : tender.RegistrationNumber;

                sb.Append(number);
                sb.Append($"- https://star.fintender.ru/Search/GetActual/l{number}-1   \r\n");

                sb.Append(StripHTML(tender.TradeName));
                sb.Append("  \r\n Заказчик: ");
                sb.Append(StripHTML(tender.OrganizerName));
                sb.Append("  \r\n Цена: ");
                sb.Append($"{tender.Price:#.00}");
                sb.Append(" рублей  \r\n Дата публикации: ");
                sb.Append(tender.DatePublished);
                sb.Append("  \r\n ***  \r\n");
            }
        }

        public override void BuildSearhLink(int numberOfResults, string region)
        {
            sb.Append($"Отображено {numberOfResults} самых свежих закупок, больше закупок можно найти в системе СТАР - https://star.fintender.ru");

        }

        public override void WarningNoRegion()
        {
            sb.Append("  \r\nВНИМАНИЕ, данные результаты найдены без учета региона. Установить регион можно командой /регион NN, где NN - код региона.");
        }

        public override void NotFound(string request)
        {
            sb.Append($"К сожалению, по запросу '{request}' ничего найти не удалось, попробуй перефразировать запрос");
        }
    }
}