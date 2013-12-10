using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Collections;
using System.Linq;

namespace HFA.SPSolutions.EventReceivers.EventReceiver1
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class EventReceiver1 : SPItemEventReceiver
    {
       /// <summary>
       /// An item was added.
       /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);

            SPWeb web = properties.OpenWeb();

            string reportType = string.Empty;
            string reportName = string.Empty;
            string reportStatus = string.Empty;
            SPListItem reportItem = null;
            string listName = string.Empty;

            try
            {
                SPListItem item = properties.ListItem;
                reportType = item["Report_x0020_Type"].ToString();
                reportName = item["Name"].ToString().Replace(".pdf", string.Empty);
                reportStatus = item["Status"].ToString();

                if (reportStatus.Contains("Completed"))
                {
                    //if (reportType == "IMV")
                    //{
                    reportItem = (from SPListItem li in properties.Web.Lists[reportType + " Report"].Items
                                  where li.Title == reportName
                                  select li).FirstOrDefault();


                    if (reportItem != null)
                    {
                        reportItem["Status"] = "Complete / Verified";
                        reportItem.Update();
                    }
                    //}
                }
            }
            catch { }

            finally
            {
                web.Close();
            }
        }

    }
}
