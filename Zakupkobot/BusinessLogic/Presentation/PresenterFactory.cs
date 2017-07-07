using System.Text;

namespace Presentation
{
    public static class PresenterFactory
    {
        public static Presenter GetPresenter(string channelId)
        {
            switch (channelId) //TODO: find out correct IDs
            {
                case "facebook": return new PlainPresenter();
                case "groupme": return new PlainPresenter();
                case "kik": return new PlainPresenter();
                case "twilio": return new PlainPresenter();
                case "telegram": return new MarkdownPresenter();
                case "skype": return new MarkdownPresenter();
                case "slack": return new MarkdownPresenter();
                case "webchat": return new MarkdownPresenter();
                case "email": return new MarkdownPresenter();
                case "emulator": return new MarkdownPresenter();
                default: return new PlainPresenter();
            }
        }
    }
}