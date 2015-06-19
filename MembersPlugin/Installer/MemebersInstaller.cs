using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using MembersPlugin.Exceptions;

namespace MembersPlugin.Installer
{
     public class MemebersInstaller
    {
         public static string InstallDashboard()
         {
             try
             {
                 string dashboardConfig;
                 string dashboardConfigPath = HttpContext.Current.Server.MapPath("~/config/dashboard.config");
                 using (var streamReader = File.OpenText(dashboardConfigPath))
                     dashboardConfig = streamReader.ReadToEnd();
                 if (string.IsNullOrEmpty(dashboardConfig))
                     throw new Exception("Unable to add dashboard: Couldn't read current ~/config/dashboard.config, permissions issue?");
                 var dashboardDoc = XDocument.Parse(dashboardConfig, LoadOptions.PreserveWhitespace);
                 var dashBoardElement = dashboardDoc.Element("dashBoard");
                 if (dashBoardElement == null)
                     throw new Exception("Unable to add dashboard: dashBoard element not found in ~/config/dashboard.config file");
                 var sectionElements = dashBoardElement.Elements("section").ToList();
                 if (!sectionElements.Any())
                     throw new Exception("Unable to add dashboard: No section elements found in ~/config/dashboard.config file");
                 var startupDashboardSectionElement = sectionElements.SingleOrDefault(x => x.Attribute("alias") != null && x.Attribute("alias").Value == "StartupDashboardSection");
                 if (startupDashboardSectionElement == null)
                     throw new Exception("Unable to add dashboard: StartupDashboardSection not found in ~/config/dashboard.config");

                 var tabs = startupDashboardSectionElement.Elements("tab").ToList();
                 if (!tabs.Any())
                     throw new Exception("Unable to add dashboard: No existing tabs found within the StartupDashboardSection");

                 var membersTabs = tabs.Where(x => x.Attribute("caption").Value == "Members").ToList();
                 if (membersTabs.Any())
                 {
                     if (membersTabs.Select(tab => tab.Elements("control").ToList()).Any(membersTabControls => membersTabControls.Any(x => x.Value == "/app_plugins/Members/memberssashbord.html")))
                     {
                         throw new DashboardAlreadyInstalledException("Dashboard is already installed.");
                     }
                 }

                 var lastTab = tabs.Last();
                 var membersTab = new XElement("tab");
                 membersTab.Add(new XAttribute("caption", "Members"));
                 var membersControl = new XElement("control");
                 membersControl.Add(new XAttribute("addPanel", false), new XAttribute("showOnce", true), new XAttribute("panelCaption", ""));
                 membersControl.SetValue("/app_plugins/Members/memberssashbord.html");
                 membersTab.Add(membersControl);
                 membersControl.AddBeforeSelf(string.Concat(Environment.NewLine, "      "));
                 membersControl.AddAfterSelf(string.Concat(Environment.NewLine, "    "));
                 lastTab.AddAfterSelf(membersTab);
                 lastTab.AddAfterSelf(string.Concat(Environment.NewLine, "    "));
                 dashboardDoc.Save(dashboardConfigPath, SaveOptions.None);
             }
             catch (Exception ex)
             {
                 return HandleException(ex);
             }
             return string.Empty;
         }

        private static string HandleException(Exception ex)
         {
             string exceptionString = "error: ";
             if (ex.InnerException != null)
                 exceptionString += string.Format("{0} ({1})", ex.Message, ex.InnerException.Message);
             else
                 exceptionString += ex.Message;

             var stackTrace = new StackTrace(true);
             exceptionString += " | Stacktrace: " + stackTrace.ToString().Replace("\r", string.Empty).Replace("\n", "<br />");
             return exceptionString;
         }
    }
}
