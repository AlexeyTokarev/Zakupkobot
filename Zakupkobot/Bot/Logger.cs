using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;
using log4net.Config;

namespace Bot
{
    public class Logger
    {
        private static ILog log = LogManager.GetLogger("Zakupkobot");


        /// <summary>
        /// Гетер Логера
        /// </summary>
        public static ILog Log
        {
            get { return log; }
        }

        /// <summary>
        /// Инициализация логера из файла конфигурации
        /// </summary>
        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}