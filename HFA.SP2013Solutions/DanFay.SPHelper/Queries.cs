using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace DanFay.SPHelper
{
    public static class Queries
    {
        public static SPListItemCollection GetActiveIssues(int siteNo)
        {

            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItemCollection listItems = null;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                 {
                     using (SPSite site = new SPSite(siteID))
                     {
                         using (SPWeb web = site.AllWebs[webID])
                         {
                             SPQuery spQuery = new SPQuery();

                             SPList issuesList = web.Lists["Issues List"];
                             string queryText = "<Where><And>";
                             queryText += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Lookup'>" + siteNo + "</Value></Eq>";
                             queryText += "<Eq><FieldRef Name='Issue_x0020_Status' /><Value Type='Choice'>Active</Value></Eq>";
                             queryText += "</And></Where></Query>";

                             spQuery.Query = queryText;
                             listItems = issuesList.GetItems(spQuery);
                         }
                     }
                 });
            }
            catch (Exception ex)
            {
                throw new Exception("GetActiveIssues:" + ex.Message);
            }
            return listItems;
        }

        public static SPListItemCollection GetClosedIssues(int siteNo)
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItemCollection listItems = null;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                 {
                     using (SPSite site = new SPSite(siteID))
                     {
                         using (SPWeb web = site.AllWebs[webID])
                         {
                             SPQuery spQuery = new SPQuery();
                             SPList issuesList = web.Lists["Issues List"];
                             string queryText = "<Where><And>";
                             queryText += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Lookup'>" + siteNo + "</Value></Eq>";
                             queryText += "<Eq><FieldRef Name='Issue_x0020_Status' /><Value Type='Choice'>Closed</Value></Eq>";
                             queryText += "</And></Where></Query>";

                             spQuery.Query = queryText;
                             listItems = issuesList.GetItems(spQuery);
                         }
                     }
                 });
            }
            catch (Exception ex)
            {
                throw new Exception("GetActiveIssues:" + ex.Message);
            }
            return listItems;
        }

        public static SPListItemCollection GetSubjects(int siteNo)
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItemCollection listItems = null;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(siteID))
                    {
                        using (SPWeb web = site.AllWebs[webID])
                        {
                            SPQuery spQuery = new SPQuery();
                            SPList issuesList = web.Lists["Subject List"];
                            string queryText = "<Where>";
                            queryText += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Lookup'>" + siteNo + "</Value></Eq>";
                            queryText += "</Where></Query>";

                            spQuery.Query = queryText;
                            listItems = issuesList.GetItems(spQuery);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("GetSubjects:" + ex.Message);
            }
            return listItems;
        }

        public static SPListItem GetSiteBySiteNo(int siteNo)
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItemCollection listItems = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 using (SPSite site = new SPSite(siteID))
                 {
                     using (SPWeb web = site.AllWebs[webID])
                     {
                         SPQuery spQuery = new SPQuery();
                         SPList siteList = web.Lists["Site List"];

                         string queryText = "<Where>";
                         queryText += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Number'>" + siteNo + "</Value></Eq>";
                         queryText += "</Where>";

                         spQuery.Query = queryText;
                         listItems = siteList.GetItems(spQuery);
                     }
                 }
             });

            if (listItems.Count > 0)
                return listItems[0];
            else
                return null;
        }

        public static int GetActiveSiteBySiteNo(int siteNo)
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItemCollection listItems = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        SPQuery spQuery = new SPQuery();
                        SPList siteList = web.Lists["Site List"];

                        string queryText = "<Where>";
                        queryText += "<And><Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Number'>" + siteNo + "</Value></Eq>";
                        queryText += "<Eq><FieldRef Name='Active' /><Value Type='Boolean'>1</Value></Eq>";
                        queryText += "</And></Where>";

                        spQuery.Query = queryText;
                        listItems = siteList.GetItems(spQuery);
                    }
                }
            });

            return listItems.Count;
        }

        public static SPListItemCollection GetSites()
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItemCollection listItems = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        SPQuery spQuery = new SPQuery();
                        SPList siteList = web.Lists["Site List"];
                        listItems = siteList.Items;
                    }
                }
            });

            if (listItems.Count > 0)
                return listItems;
            else
                return null;
        }

        public static bool ReportExists(string listName, string title)
        {
            SPListItemCollection items = null;
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
             {
                 using (SPSite site = new SPSite(siteID))
                 {
                     using (SPWeb web = site.AllWebs[webID])
                     {
                         SPList siteList = web.Lists[listName];
                         SPQuery spQuery = new SPQuery();
                         string queryText = "<Where>";
                         queryText += "<Eq><FieldRef Name='Title' /><Value Type='Text'>" + title + "</Value></Eq>";
                         queryText += "</Where>";

                         spQuery.Query = queryText;
                         items = siteList.GetItems(spQuery);
                     }
                 }
             });
            return items.Count > 0;
        }

        public static SPListItemCollection GetSeriousAdverseEvents(int siteId)
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItemCollection listItems = null;

            //int subjectId = 123;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        SPQuery spQuery = new SPQuery();
                        SPList siteList = web.Lists["Serious Adverse Events"];
                      
                        string queryText = "<Where>";
                        queryText += "<Eq><FieldRef Name='Site_x0020_Number' LookupId='TRUE' /><Value Type='Lookup'>" + siteId + "</Value></Eq>";
                        //queryText += "<Eq><FieldRef Name='Subject_x0020_ID' /><Value Type='Number'>" + subjectId + "</Value></Eq>";
                       
                        queryText += "</Where>";

                        spQuery.Query = queryText;
                        listItems = siteList.GetItems(spQuery);
                    }
                }
            });

                return listItems;
        }
    }
}
