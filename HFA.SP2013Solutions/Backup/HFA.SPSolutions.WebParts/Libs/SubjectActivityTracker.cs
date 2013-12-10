using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using DanFay.SPHelper;

namespace HFA.SPSolutions.WebParts.Libs
{
    public class SubjectActivityTracker
    {
        public int ID
        { get; set; }

        public SPFieldLookupValue Subject
        { get; set; }

        public string SubjectID
        { get; set; }

        public string ActivityThisVisit
        { get; set; }

        public string DroppedFromStudy
        { get; set; }

        public string ReportType
        { get; set; }

        public int ReportId
        { get; set; }


        public SubjectActivityTracker()
        { }

        public SubjectActivityTracker(int ID)
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        var result = (from SPListItem item in web.Lists["Subject Activity Tracker"].Items where item.ID == ID select item).FirstOrDefault();

                        if (result != null)
                        {
                            this.Subject = new SPFieldLookupValue(result["SubjectID"].ToString());
                            this.SubjectID = this.Subject.LookupValue;
                            this.ActivityThisVisit = Utilities.GetMultiLineTextFieldValue(result, "ActivityThisVisit");
                            this.DroppedFromStudy = Utilities.GetMultiLineTextFieldValue(result, "DroppedFromStudy");
                            this.ReportType = Utilities.GetStringValue(result["ReportType"]);
                            this.ReportId = Convert.ToInt16(result["ReportId"]);
                        }
                    }
                }
            });
        }

        public static Dictionary<string, string> GetSubjects(int siteId)
        {
            Dictionary<string, string> subjects = null;

            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        var result = (from SPListItem item in web.Lists["Subject List"].Items where new SPFieldLookupValue(item["Site Number"] as string).LookupId == siteId select item).ToList();
                        subjects = new Dictionary<string, string>();

                        foreach (var li in result)
                        {
                            subjects.Add(li.ID.ToString(), li.Title);
                        }
                    }
                }
            });

            return subjects;
        }

        public void Save(SubjectActivityTracker sat)
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItem item = null;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                 {
                     using (SPSite site = new SPSite(siteID))
                     {
                         using (SPWeb web = site.AllWebs[webID])
                         {
                             SPList satList = web.Lists["Subject Activity Tracker"];

                             if (sat.ID == 0)
                                 item = satList.AddItem();
                             else
                                 item = (from SPListItem x in satList.Items where x.ID == sat.ID select x).FirstOrDefault();

                             item["SubjectID"] = sat.Subject;
                             item["ActivityThisVisit"] = sat.ActivityThisVisit;
                             //item["DroppedFromStudy"] = sat.DroppedFromStudy;
                             item["ReportType"] = sat.ReportType;
                             item["ReportId"] = sat.ReportId;

                             web.AllowUnsafeUpdates = true;

                             item.Update();

                             web.AllowUnsafeUpdates = false;
                         }
                     }
                 });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static void Delete(int ID)
        {
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPListItem item = null;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(siteID))
                    {
                        using (SPWeb web = site.AllWebs[webID])
                        {
                            SPList satList = web.Lists["Subject Activity Tracker"];

                            item = (from SPListItem x in satList.Items where x.ID == ID select x).FirstOrDefault();

                            web.AllowUnsafeUpdates = true;
                            item.Delete();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<SubjectActivityTracker> GetbyReportId(int reportId)
        {
            List<SubjectActivityTracker> satList = new List<SubjectActivityTracker>();
            SubjectActivityTracker satItem = null;

            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(siteID))
                    {
                        using (SPWeb web = site.AllWebs[webID])
                        {
                            SPList list = web.Lists["Subject Activity Tracker"];

                            var result = (from SPListItem x in list.Items where Convert.ToInt16(x["ReportId"]) == reportId select x).ToList();

                            if (result.Count > 0)
                            {
                                satList = new List<SubjectActivityTracker>();

                                foreach (var item in result)
                                {
                                    satItem = new SubjectActivityTracker();

                                    satItem.ID = item.ID;
                                    satItem.Subject = new SPFieldLookupValue(item["SubjectID"].ToString());
                                    satItem.SubjectID = satItem.Subject.LookupValue;
                                    satItem.ActivityThisVisit = Utilities.GetMultiLineTextFieldValue(item, "ActivityThisVisit");
                                    satItem.DroppedFromStudy = Utilities.GetMultiLineTextFieldValue(item, "DroppedFromStudy");
                                    satItem.ReportType = Utilities.GetStringValue(item["ReportType"]);
                                    satItem.ReportId = Convert.ToInt16(item["ReportId"]);

                                    satList.Add(satItem);
                                }
                            }
                        }
                    }
                });

                return satList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
