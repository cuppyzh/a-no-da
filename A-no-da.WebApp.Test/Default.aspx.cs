using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace A_no_da.WebApp.Test
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Load config
            //ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            //configMap.ExeConfigFilename = @"D:\Xtremax\_sandbox\Cuppyzh.WebApp\Cuppyzh.WebApp\Custom.config";
            //Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            var config2 = ConfigurationManager.GetSection("sitecore/settings");

            var test = ConfigurationManager.AppSettings["web"];
            var test2 = ConfigurationManager.AppSettings["core"];
            var test3 = ConfigurationManager.AppSettings["default"];
            var test34 = (NameValueCollection)ConfigurationManager.GetSection("secureAppSettings");
            var test5 = test34["test"];

            var connectionString = ConfigurationManager.ConnectionStrings["web"];
            var connectionString2 = ConfigurationManager.ConnectionStrings["core"];
            var connectionString3 = ConfigurationManager.ConnectionStrings["apiKey"];
            var aaaa = ConfigurationManager.ConnectionStrings["web"];

            var test12 = ConfigurationManager.GetSection("sitecore") as ConfigReader;
            var test122 = ConfigurationManager.GetSection("sitecore:settings");
            var asdasdasd = Sitecore.Configuration.Settings.GetSetting("test23");
            var asdasssdasd = Sitecore.Configuration.Settings.GetSetting("test");
        }
    }
}