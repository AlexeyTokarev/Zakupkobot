using System;
using System.Text;
using System.Text.RegularExpressions;
using BusinessLogic.Service_References.StarService;

namespace Presentation
{
    public abstract class Presenter
    {

        protected StringBuilder sb = new StringBuilder();

        public string Result
        {
            get { return sb.ToString(); }
        }

        protected static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public abstract void AddFoundInRegionHeader(string request, string region);
        public abstract void BuildResult(GetTendersResponse response);
        public abstract void BuildSearhLink(int number, string region);
        public abstract void WarningNoRegion();
        public abstract void NotFound(string request);
    }
}
