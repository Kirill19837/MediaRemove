using System;
using System.Xml;
using umbraco.interfaces;
using Umbraco.Core.Logging;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.IO;

namespace OSKI.solutions.MediaRemove
{
    public class PackageRegistration : IPackageAction
    {
        public string Alias()
        {
            return "AddDashboard";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {

            LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, String.Format("Start Action {0}: Execute() for package {1}.", this.Alias(), packageName));

            try
            {
                this.AddSectionDashboard("Media Remove", "developer", "Media Remove", "/App_Plugins/Nexu/views/dashboard.html");
                LogHelper.Info(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, String.Format("Complete Action {0}: Execute() for package {1}.", this.Alias(), packageName));

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, String.Format("Error in {0}: Execute() for package {1}.", this.Alias(), packageName), ex);

                return false;
            }
        }

        public void AddSectionDashboard(string sectionAlias, string area, string tabCaption, string src)
        {
            bool saveFile = false;

            //Path to the file resolved
            var dashboardFilePath = IOHelper.MapPath(SystemFiles.DashboardConfig);

            //Load settings.config XML file
            XmlDocument dashboardXml = new XmlDocument();
            dashboardXml.Load(dashboardFilePath);

            // Section Node
            XmlNode findSection = dashboardXml.SelectSingleNode("//section [@alias='" + sectionAlias + "']");

            //Couldn't find it
            if (findSection == null)
            {
                //Let's add the xml
                var xmlToAdd = "<section alias='" + sectionAlias + "'>" +
                                    "<areas>" +
                                        "<area>" + area + "</area>" +
                                    "</areas>" +
                                    "<tab caption='" + tabCaption + "'>" +
                                        "<control addPanel='true' panelCaption=''>" + src + "</control>" +
                                    "</tab>" +
                               "</section>";

                //Get the main root <dashboard> node
                XmlNode dashboardNode = dashboardXml.SelectSingleNode("//dashBoard");

                if (dashboardNode != null)
                {
                    //Load in the XML string above
                    XmlDocument xmlNodeToAdd = new XmlDocument();
                    xmlNodeToAdd.LoadXml(xmlToAdd);

                    var toAdd = xmlNodeToAdd.SelectSingleNode("*");

                    //Prepend the xml above to the dashboard node - so that it will be the first dashboards to show in the backoffice.
                    dashboardNode.PrependChild(dashboardNode.OwnerDocument.ImportNode(toAdd, true));

                    //Save the file flag to true
                    saveFile = true;
                }
            }

            //If saveFile flag is true then save the file
            if (saveFile)
            {
                //Save the XML file
                dashboardXml.Save(dashboardFilePath);
            }
        }


        public XmlNode SampleXml()
        {
            string sample = "<Action runat=\"install\" undo=\"true/false\" alias=\"AddDasboard\"/>";

            return helper.parseStringToXmlNode(sample);
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {
            this.RemoveDashboardTab("Media Remove");

            return true;
        }

        public void RemoveDashboardTab(string sectionAlias)
        {

            string dbConfig = IOHelper.MapPath(SystemFiles.DashboardConfig);
            XmlDocument dashboardFile = new XmlDocument();
            dashboardFile.Load(dbConfig);

            XmlNode section = dashboardFile.SelectSingleNode("//section [@alias = '" + sectionAlias + "']");

            if (section != null)
            {
                dashboardFile.SelectSingleNode("//dashBoard").RemoveChild(section);
                dashboardFile.Save(dbConfig);
            }

        }
    }
}
