using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Data;
using System.Web;
using System.Xml;
using System.IO;
using ExpertPdf.HtmlToPdf;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Collections;
using DanFay.SPHelper;

namespace HFA.SPSolutions.WebParts.SiteOverview
{
    public partial class SiteOverviewWebPartUserControl : UserControl
    {
        protected enum Report
        {
            IMV = 0,
            SSV = 1,
            SIV = 2,
            COV = 3,
        }

        private string SiteId
        {
            get
            {
                if (HttpUtility.ParseQueryString(Request.Url.Query).Get("Site") != null)
                    return (HttpUtility.ParseQueryString(Request.Url.Query).Get("Site").ToString());
                else
                    return string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int index = 0;

            BindHeader();

            hplNewSIV.NavigateUrl = string.Format("{0}/SitePages/SIVReport.aspx?Site={1}&ReportId=0", SPContext.Current.Web.Url, SiteId);
            hplNewCOV.NavigateUrl = string.Format("{0}/SitePages/COVReport.aspx?Site={1}&ReportId=0", SPContext.Current.Web.Url, SiteId);
            hplNewIMV.NavigateUrl = string.Format("{0}/SitePages/IMVReport.aspx?Site={1}&ReportId=0", SPContext.Current.Web.Url, SiteId);
            hplNewSSV.NavigateUrl = string.Format("{0}/SitePages/SSVReport.aspx?Site={1}&ReportId=0", SPContext.Current.Web.Url, SiteId);

            //Set tab as per report it came from
            if (Request.UrlReferrer != null)
            {
                if (Request.UrlReferrer.ToString().Contains("COVReport"))
                {
                    index = (int)Report.COV;
                    formMultiView.ActiveViewIndex = index;
                    formMenu.Items[index].Selected = true;
                    BindCOVReport();
                }
                else if (Request.UrlReferrer.ToString().Contains("IMVReport"))
                {
                    index = (int)Report.IMV;
                    formMultiView.ActiveViewIndex = index;
                    formMenu.Items[index].Selected = true;
                    BindIMVReport();
                }
                else if (Request.UrlReferrer.ToString().Contains("SIVReport"))
                {
                    index = (int)Report.SIV;
                    formMultiView.ActiveViewIndex = index;
                    formMenu.Items[index].Selected = true;
                    BindSIVReport();
                }
                else if (Request.UrlReferrer.ToString().Contains("SSVReport"))
                {
                    index = (int)Report.SSV;
                    formMultiView.ActiveViewIndex = index;
                    formMenu.Items[index].Selected = true;
                    BindSSVReport();
                }
                else
                    BindIMVReport(); //Default selection if previous URL is not null
            }
            else
                BindIMVReport(); //Default selection if previous url is null

        }

        protected void BindHeader()
        {
            try
            {
                SPListItem selectedSite = GetSiteData();

                if (selectedSite != null)
                {
                    lblSiteNo.Text = SiteId;
                    lblSiteTitle.Text = selectedSite.Title;
                    lblInvestigatorName.Text = Utilities.GetStringValue(selectedSite["Investigator_x0020_Name"]);
                    //lblContractStatus.Text = selectedSite["Contract Status"].ToString();
                    //lblParticipant.Text = Utilities.GetStringValue(selectedSite["Site_x0020_Participants"]);
                    //lblAuthor.Text = selectedSite["Author"].ToString().Split('#')[1];
                    lblPhoneNo.Text = Utilities.GetStringValue(selectedSite["Phone_x0020_Number"]);
                    lblAddress.Text = Utilities.GetMultiLineTextFieldValue(selectedSite, "Address");
                }
            }

            catch { }
        }

        protected SPListItem GetSiteData()
        {
            SPWeb web = SPContext.Current.Web;
            SPList siteList = web.Lists["Site List"];
            SPQuery oQuery = new SPQuery();
           
            string query = "<Where>";
            query += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Counter'>" + SiteId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //If is match is foud then delete list item
            SPListItemCollection list = siteList.GetItems(oQuery);

            if (list.Count > 0)
                return list[0];
            else
                return null;
        }

        protected void gdvSIV_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            BindSIVReport();
            gdvSIV.PageIndex = e.NewPageIndex;
            gdvSIV.DataBind();
        }

        protected void gdvCOV_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            BindCOVReport();
            gdvCOV.PageIndex = e.NewPageIndex;
            gdvCOV.DataBind();
        }

        protected void gdvIMV_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            BindIMVReport();
            gdvIMV.PageIndex = e.NewPageIndex;
            gdvIMV.DataBind();
        }

        protected void gdvSSV_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            BindSSVReport();
            gdvSSV.PageIndex = e.NewPageIndex;
            gdvSSV.DataBind();
        }

        protected SPListItemCollection GetSIVData()
        {
            SPWeb web = SPContext.Current.Web;
            SPList IMVList = web.Lists["SIV Report"];

            //Crate query to find match on site name and department name
            SPQuery oQuery = new SPQuery();
            string query = "<Where>";
            query += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Lookup'>" + SiteId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //Execute query
            SPListItemCollection selectedItems = IMVList.GetItems(oQuery);

            return selectedItems;
        }

        protected SPListItemCollection GetSSVData()
        {
            SPWeb web = SPContext.Current.Web;
            SPList IMVList = web.Lists["SSV Report"];

            //Crate query to find match on site name and department name
            SPQuery oQuery = new SPQuery();
            string query = "<Where>";
            query += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Lookup'>" + SiteId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //Execute query
            SPListItemCollection selectedIVMData = IMVList.GetItems(oQuery);

            return selectedIVMData;
        }

        protected SPListItemCollection GetCOVData()
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists["COV Report"];

            //Crate query to find match on site name and department name
            SPQuery oQuery = new SPQuery();
            string query = "<Where>";
            query += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Lookup'>" + SiteId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //Execute query
            SPListItemCollection listItems = list.GetItems(oQuery);

            return listItems;
        }

        protected SPListItemCollection GetSiteIMVData()
        {
            if (SiteId.Length <= 0)
                return null;

            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists["IMV Report"];

            //Crate query to find match on site name and department name
            SPQuery oQuery = new SPQuery();
            string query = "<Where>";
            query += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Lookup'>" + SiteId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //Execute query
            SPListItemCollection listItems = list.GetItems(oQuery);

            return listItems;
        }

        protected void BindIMVReport()
        {
            SPListItemCollection items = GetSiteIMVData();
            var sortedItems = from SPListItem item in items orderby item["Modified"] descending select item;

            //Create data table
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Visit Date", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Monitor", typeof(string));

            //Create row for each list item
            DataRow row;
            foreach (SPListItem item in sortedItems)
            {
                row = dt.Rows.Add();
                row["ID"] = item.ID;
                row["Title"] = item.Name;
                row["Visit Date"] = Convert.ToDateTime(item["Visit Date"]).ToShortDateString();
                row["Status"] = item["Status"].ToString();
                row["Monitor"] = item["Monitor"].ToString().Split('#')[1];
            }

            gdvIMV.DataSource = dt.DefaultView;
            gdvIMV.DataBind();
        }

        protected void BindSSVReport()
        {
            SPListItemCollection items = GetSSVData();
            var sortedItems = from SPListItem item in items orderby item["Modified"] descending select item;

            //Create data table
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Visit Date", typeof(string));
            dt.Columns.Add("Monitor", typeof(string));

            //Create row for each list item
            DataRow row;
            foreach (SPListItem item in sortedItems)
            {
                row = dt.Rows.Add();

                row["ID"] = item.ID;
                row["Title"] = item.Name;
                row["Status"] = item["Status"];
                row["Visit Date"] = Utilities.GetShortDateValue(item["Visit Date"]);
                row["Monitor"] = Utilities.GetLookupFieldValue2(item["Monitor"]);
            }

            //Bind data to grid
            gdvSSV.DataSource = dt.DefaultView;
            gdvSSV.DataBind();
        }

        protected void BindSIVReport()
        {
            SPListItemCollection items = GetSIVData();
            var sortedItems = from SPListItem item in items orderby item["Modified"] descending select item;

            //Create data table
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Visit Date", typeof(string));
            dt.Columns.Add("Monitor", typeof(string));
           
            //Create row for each list item
            DataRow row;
            foreach (SPListItem item in sortedItems)
            {
                row = dt.Rows.Add();
                row["ID"] = item.ID;
                row["Title"] = item.Name;
                row["Status"] = item["Status"];
                row["Visit Date"] = Utilities.GetShortDateValue(item["Visit Date"]);
                row["Monitor"] = Utilities.GetLookupFieldValue2(item["Monitor"]);
            }

            //Bind data to grid
            gdvSIV.DataSource = dt.DefaultView;
            gdvSIV.DataBind();
        }

        protected void BindCOVReport()
        {

            SPListItemCollection items = GetCOVData();
            var sortedItems = from SPListItem item in items orderby item["Modified"] descending select item;

            //Create data table
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Visit Date", typeof(string));
            dt.Columns.Add("Monitor", typeof(string));

            //Create row for each list item
            DataRow row;
            foreach (SPListItem item in sortedItems)
            {
                row = dt.Rows.Add();
                row["ID"] = item.ID;
                row["Title"] = item.Name;
                row["Status"] = item["Status"];
                row["Visit Date"] = Utilities.GetShortDateValue(item["Visit Date"]);
                row["Monitor"] = Utilities.GetLookupFieldValue2(item["Monitor"]);
            }

            //Bind data to grid
            gdvCOV.DataSource = dt.DefaultView;
            gdvCOV.DataBind();

        }

        protected void BindIssuesList()
        {
            SPWeb web = SPContext.Current.Web;
            SPList issuesList = web.Lists["Issues List"];
            SPListItemCollection items = issuesList.Items;

            //Create data table
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Issue Status", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Creation Date", typeof(string));
            dt.Columns.Add("Close Date", typeof(string));
            dt.Columns.Add("Description", typeof(string));

            //Crate query to find match on site name and department name
            SPQuery oQuery = new SPQuery();
            string query = "<Where>";
            query += "<Eq><FieldRef Name='SiteNumberValue' /><Value Type='Number'>" + SiteId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //Execute query
            SPListItemCollection selectedItems = issuesList.GetItems(oQuery);

            //Create row for each list item
            DataRow row;
            foreach (SPListItem item in selectedItems)
            {
                row = dt.Rows.Add();
                row["ID"] = item.ID;
                row["Title"] = item.Name;
                row["Issue Status"] = item["Issue Status"];
                row["Category"] = item["Category"];
                row["Creation Date"] = Convert.ToDateTime(item["Creation Date"]).ToShortDateString();
                row["Close Date"] = Convert.ToDateTime(item["Close Date"]).ToShortDateString();
                row["Description"] = item["Description"];
            }
        }

        protected void BindSubjectList()
        {
            SPWeb web = SPContext.Current.Web;
            SPList subjectList = web.Lists["Subject List"];
            SPListItemCollection items = subjectList.Items;
            string[] commentsArray;
            string newComments = string.Empty;

            //Create data table
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("Subject ID", typeof(string));
            dt.Columns.Add("Subject Name", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("Comments", typeof(string));
         
            //Run query to find match on site name and department name
            SPQuery oQuery = new SPQuery();
            string query = "<Where>";
            query += "<Eq><FieldRef Name='Site_x0020_Number' /><Value Type='Lookup'>" + SiteId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //If is match is found then delete list item
            SPListItemCollection selectedItems = subjectList.GetItems(oQuery);

            //Create row for each list item
            DataRow row;
            foreach (SPListItem item in selectedItems)
            {
                row = dt.Rows.Add();
                row["ID"] = item.ID;
                row["Subject ID"] = item.Name;
                row["Subject Name"] = item["Subject Name"];
                row["Status"] = item["Status"];

                //Parse multivalue column field to get comments from XML string
                newComments = item["Comments"].ToString().Replace("<p>", "@");
                newComments = newComments.Replace("</p>","~");
                commentsArray = newComments.Split('@');
                row["Comments"] = commentsArray[1].Split('~')[0];
            }

            //Bind data to grid
            //gdvSubject.DataSource = dt.DefaultView;
            //gdvSubject.DataBind();
        }

        protected void formMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            //Get index
            int idx = int.Parse(e.Item.Value);
            formMultiView.ActiveViewIndex = idx;

            //Get data for current tab
            if (idx == 0)
                BindIMVReport();
            else if (idx == 1)
                BindSSVReport();
            else if (idx == 2)
                BindSIVReport();
            else if (idx == 3)
                BindCOVReport();
            else if (idx == 4)
                BindIssuesList();
            else if (idx == 5)
                BindSubjectList();
        }

        protected void gdvIssues_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HyperLink hlControl = null;

            if (e.Row.RowType != DataControlRowType.Header)
            {
                //Add title for hyperlink that goes to the new form
                hlControl = new HyperLink();
                hlControl.Text = e.Row.Cells[1].Text;
                hlControl.NavigateUrl = string.Format("{0}/Lists/Issues List/EditForm.aspx?ID={1}", SPContext.Current.Web.Url, e.Row.Cells[0].Text);
                e.Row.Cells[1].Controls.Add(hlControl);

                //Left justify Title column
                e.Row.Cells[1].CssClass = "gdvColumnLeft";
            }
        }

        protected void gdvSubject_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HyperLink hlControl = null;

            if (e.Row.RowType != DataControlRowType.Header)
            {
                //Add title for hyperlink that goes to the new form
                hlControl = new HyperLink();
                hlControl.Text = e.Row.Cells[1].Text;
                hlControl.NavigateUrl = string.Format("{0}/Lists/Subject List/EditForm.aspx?ID={1}", SPContext.Current.Web.Url, e.Row.Cells[0].Text);
                e.Row.Cells[1].Controls.Add(hlControl);

                //Left justify Title column
                e.Row.Cells[1].CssClass = "gdvColumnLeft";
            }
        }

        protected void gdvSIV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HyperLink hlControl = null;

            if (e.Row.Cells.Count <= 2) return;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int selectedId = int.Parse(gdvSIV.DataKeys[e.Row.RowIndex].Value.ToString());

                //Add title for hyperlink that goes to the new form
                hlControl = new HyperLink();
                hlControl.Text = e.Row.Cells[1].Text;
                hlControl.NavigateUrl = string.Format("{0}/SitePages/SIVReport.aspx?Site={1}&ReportId={2}", SPContext.Current.Web.Url, SiteId, selectedId);
                e.Row.Cells[1].Controls.Add(hlControl);

                //Add delete confirmation
                LinkButton btnDelete = (LinkButton)e.Row.Cells[5].Controls[0];
                btnDelete.OnClientClick = "if (!confirm('Are you sure you want to delete?')) {return false;}"; 
            }

            //Hide the ID column
            e.Row.Cells[0].Visible = false;
        }

        protected void gdvCOV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HyperLink hlControl = null;

            if (e.Row.Cells.Count <= 2) return;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int selectedId = int.Parse(gdvCOV.DataKeys[e.Row.RowIndex].Value.ToString());

                //Add title for hyperlink that goes to the new form
                hlControl = new HyperLink();
                hlControl.Text = e.Row.Cells[1].Text;
                hlControl.NavigateUrl = string.Format("{0}/SitePages/COVReport.aspx?Site={1}&ReportId={2}", SPContext.Current.Web.Url, SiteId, selectedId);
                e.Row.Cells[1].Controls.Add(hlControl);

                //Add delete confirmation
                LinkButton btnDelete = (LinkButton)e.Row.Cells[5].Controls[0];
                btnDelete.OnClientClick = "if (!confirm('Are you sure you want to delete?')) {return false;}"; 
            }

            //Hide the ID column
            e.Row.Cells[0].Visible = false;
        }

        protected void gdvIMV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HyperLink hlControl = null;

            if (e.Row.Cells.Count <= 2) return;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int selectedId = int.Parse(gdvIMV.DataKeys[e.Row.RowIndex].Value.ToString());

                //Add title for hyperlink that goes to the new form
                hlControl = new HyperLink();
                hlControl.Text = e.Row.Cells[1].Text;
                hlControl.NavigateUrl = string.Format("{0}/SitePages/IMVReport.aspx?Site={1}&ReportId={2}", SPContext.Current.Web.Url, SiteId, selectedId);
                e.Row.Cells[1].Controls.Add(hlControl);

                //Add delete confirmation
                LinkButton btnDelete = (LinkButton)e.Row.Cells[5].Controls[0];
                btnDelete.OnClientClick = "if (!confirm('Are you sure you want to delete?')) {return false;}";
            }

            //Hide the ID column
            e.Row.Cells[0].Visible = false;
        }

        protected void gdvSSV_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HyperLink hlControl = null;

            if (e.Row.Cells.Count <= 2) return;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                int selectedId = int.Parse(gdvSSV.DataKeys[e.Row.RowIndex].Value.ToString());

                //Add title for hyperlink that goes to the new form
                hlControl = new HyperLink();
                hlControl.Text = e.Row.Cells[1].Text;
                hlControl.NavigateUrl = string.Format("{0}/SitePages/SSVReport.aspx?Site={1}&ReportId={2}", SPContext.Current.Web.Url, SiteId, selectedId);
                e.Row.Cells[1].Controls.Add(hlControl);

                //Add delete confirmation
                LinkButton btnDelete = (LinkButton)e.Row.Cells[5].Controls[0];
                btnDelete.OnClientClick = "if (!confirm('Are you sure you want to delete?')) {return false;}";
            }

            //Hide the ID column
            e.Row.Cells[0].Visible = false;
        }

        protected void gdvCOV_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {           
            if (e.CommandName == "Delete")
            {
                int idx = Convert.ToInt16(e.CommandArgument);
                int selectedId = int.Parse(gdvCOV.DataKeys[idx].Value.ToString());

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    Guid siteID = SPContext.Current.Site.ID;
                    Guid webID = SPContext.Current.Web.ID;

                    using (SPSite site = new SPSite(siteID))
                     {
                         using (SPWeb web = site.AllWebs[webID])
                         {
                             SPList siteList = web.Lists["COV Report"];
                             SPQuery oQuery = new SPQuery();
                             SPListItem item = siteList.GetItemById(selectedId);

                             web.AllowUnsafeUpdates = true;
                             item.Delete();
                             web.AllowUnsafeUpdates = false;

                             //Rebind to refresh
                             BindCOVReport();

                         }
                     }
                 });
            }
        }

        protected void gdvSIV_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int idx = Convert.ToInt16(e.CommandArgument);
                int selectedId = int.Parse(gdvSIV.DataKeys[idx].Value.ToString());

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    Guid siteID = SPContext.Current.Site.ID;
                    Guid webID = SPContext.Current.Web.ID;

                    using (SPSite site = new SPSite(siteID))
                    {
                        using (SPWeb web = site.AllWebs[webID])
                        {
                            SPList siteList = web.Lists["SIV Report"];
                            SPQuery oQuery = new SPQuery();
                            SPListItem item = siteList.GetItemById(selectedId);

                            web.AllowUnsafeUpdates = true;
                            item.Delete();
                            web.AllowUnsafeUpdates = false;

                            //Rebind to refresh
                            BindSIVReport();

                        }
                    }
                });
            }
        }

        protected void gdvIMV_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int idx = Convert.ToInt16(e.CommandArgument);
                int selectedId = int.Parse(gdvIMV.DataKeys[idx].Value.ToString());

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    Guid siteID = SPContext.Current.Site.ID;
                    Guid webID = SPContext.Current.Web.ID;

                    using (SPSite site = new SPSite(siteID))
                    {
                        using (SPWeb web = site.AllWebs[webID])
                        {
                            SPList siteList = web.Lists["IMV Report"];
                            SPQuery oQuery = new SPQuery();
                            SPListItem item = siteList.GetItemById(selectedId);

                            web.AllowUnsafeUpdates = true;
                            item.Delete();
                            web.AllowUnsafeUpdates = false;

                            //Rebind to refresh
                            BindIMVReport();

                        }
                    }
                });
            }
        }

        protected void gdvSSV_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                int idx = Convert.ToInt16(e.CommandArgument);
                int selectedId = int.Parse(gdvSSV.DataKeys[idx].Value.ToString());

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    Guid siteID = SPContext.Current.Site.ID;
                    Guid webID = SPContext.Current.Web.ID;

                    using (SPSite site = new SPSite(siteID))
                    {
                        using (SPWeb web = site.AllWebs[webID])
                        {
                            SPList siteList = web.Lists["SSV Report"];
                            SPQuery oQuery = new SPQuery();
                            SPListItem item = siteList.GetItemById(selectedId);

                            web.AllowUnsafeUpdates = true;
                            item.Delete();
                            web.AllowUnsafeUpdates = false;

                            //Rebind to refresh
                            BindSSVReport();

                        }
                    }
                });
            }
        }

        protected void gdvCOV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        { }

        protected void gdvSIV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        { }

        protected void gdvSSV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        { }

        protected void gdvIMV_RowDeleting(object sender, GridViewDeleteEventArgs e)
        { }

        protected string SetFieldValue(object fieldName)
        {
            return (fieldName != null) ? fieldName.ToString() : string.Empty;
        }

        protected SPListItem GetIssueItem(int issueID)
        {
            SPList issuesList = SPContext.Current.Web.Lists["Issues List"];
            IEnumerable<SPListItem> allIssues = issuesList.Items.OfType<SPListItem>();

            var items = (from x in allIssues where x.ID == issueID select x).ToList();

            return items[0];
        }

        protected SPListItemCollection GetActiveIssues(int siteId)
        {
            SPList issuesList = SPContext.Current.Web.Lists["Issues List"];
            SPQuery spQuery = new SPQuery();
            string queryText = "<Where><And>";
            queryText += "<Eq><FieldRef Name='SiteNumberValue' /><Value Type='Number'>" + siteId + "</Value></Eq>";
            queryText += "<Eq><FieldRef Name='Issue_x0020_Status' /><Value Type='Choice'>Active</Value></Eq>";
            queryText += "</And></Where></Query>";

            spQuery.Query = queryText;
            SPListItemCollection issues = issuesList.GetItems(spQuery);

            return issues;
        }

        protected SPListItemCollection GetClosedIssues(int siteId)
        {
            SPList issuesList = SPContext.Current.Web.Lists["Issues List"];
            SPQuery spQuery = new SPQuery();
            string queryText = "<Where><And>";
            queryText += "<Eq><FieldRef Name='SiteNumberValue' /><Value Type='Number'>" + siteId + "</Value></Eq>";
            queryText += "<Eq><FieldRef Name='Issue_x0020_Status' /><Value Type='F'>Closed</Value></Eq>";
            queryText += "</And></Where></Query>";

            spQuery.Query = queryText;
            SPListItemCollection issues = issuesList.GetItems(spQuery);

            return issues;
        }

        protected void ConvertToPDF()
        {
            PdfConverter pdfConverter = GetPdfConverter();
            string templateFileName = "C:\\HFA\\IMVReportTemplate.html";
            string convertedFileName = "C:\\HFA\\IMVReportConverted.html";
            string html = File.ReadAllText(templateFileName);
            html = html.Replace("varSponsor", "Zeeshan Zaffar");
            File.WriteAllText(convertedFileName, html);
        }

        private PdfConverter GetPdfConverter()
        {
            PdfConverter pdfConverter = new PdfConverter();

            //pdfConverter.LicenseKey = "put your license key here";

            // set the HTML page width in pixels
            // the default value is 1024 pixels
           
                pdfConverter.PageWidth = 0; // autodetect the HTML page width
          
            // set if the generated PDF contains selectable text or an embedded image - default value is true
                pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;

            //set the PDF page size 
            pdfConverter.PdfDocumentOptions.PdfPageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize), "A4");
            // set the PDF compression level
            pdfConverter.PdfDocumentOptions.PdfCompressionLevel = (PdfCompressionLevel)Enum.Parse(typeof(PdfCompressionLevel), "Normal");
            // set the PDF page orientation (portrait or landscape)
            pdfConverter.PdfDocumentOptions.PdfPageOrientation = (PDFPageOrientation)Enum.Parse(typeof(PDFPageOrientation), "Portrait");
            //set the PDF standard used to generate the PDF document
            pdfConverter.PdfStandardSubset = GetPdfStandard("PDF");
            // show or hide header and footer
            pdfConverter.PdfDocumentOptions.ShowHeader = false;
            pdfConverter.PdfDocumentOptions.ShowFooter = false;
            //set the PDF document margins
            pdfConverter.PdfDocumentOptions.LeftMargin = 10;// int.Parse(textBoxLeftMargin.Text.Trim());
            pdfConverter.PdfDocumentOptions.RightMargin = 10; //int.Parse(textBoxRightMargin.Text.Trim());
            pdfConverter.PdfDocumentOptions.TopMargin = 10;//int.Parse(textBoxTopMargin.Text.Trim());
            pdfConverter.PdfDocumentOptions.BottomMargin = 10;// int.Parse(textBoxBottomMargin.Text.Trim());
            // set if the HTTP links are enabled in the generated PDF
            pdfConverter.PdfDocumentOptions.LiveUrlsEnabled = true;// cbLiveLinksEnabled.Checked;
            // set if the HTML content is resized if necessary to fit the PDF page width - default is true
            pdfConverter.PdfDocumentOptions.FitWidth = true;// cbFitWidth.Checked;
            // set if the PDF page should be automatically resized to the size of the HTML content when FitWidth is false
            pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;
            // embed the true type fonts in the generated PDF document
            pdfConverter.PdfDocumentOptions.EmbedFonts = false;// cbEmbedFonts.Checked;
            // compress the images in PDF with JPEG to reduce the PDF document size - default is true
            pdfConverter.PdfDocumentOptions.JpegCompressionEnabled = true;// cbJpegCompression.Checked;
            // set if the JavaScript is enabled during conversion 
            pdfConverter.ScriptsEnabled = pdfConverter.ScriptsEnabledInImage = false;// cbScriptsEnabled.Checked;

            // set if the converter should try to avoid breaking the images between PDF pages
            pdfConverter.AvoidImageBreak = false;// cbAvoidImageBreak.Checked;

            pdfConverter.PdfHeaderOptions.HeaderText = "Header Text";// textBoxHeaderText.Text;
            pdfConverter.PdfHeaderOptions.HeaderTextColor = Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), "Black"));
            pdfConverter.PdfHeaderOptions.HeaderSubtitleText = "Subjct Title";// textBoxHeaderSubtitle.Text;
            pdfConverter.PdfHeaderOptions.DrawHeaderLine = true;// cbDrawHeaderLine.Checked;
            pdfConverter.PdfHeaderOptions.HeaderHeight = 50;

            pdfConverter.PdfFooterOptions.FooterText = "Footer Text";// textBoxFooterText.Text;
            pdfConverter.PdfFooterOptions.FooterTextColor = Color.FromKnownColor((KnownColor)Enum.Parse(typeof(KnownColor), "Black"));
            pdfConverter.PdfFooterOptions.DrawFooterLine = true;// cbDrawFooterLine.Checked;
            pdfConverter.PdfFooterOptions.PageNumberText = "Page Number Text";// textBoxPageNmberText.Text;
            pdfConverter.PdfFooterOptions.ShowPageNumber = true;// cbShowPageNumber.Checked;
            pdfConverter.PdfFooterOptions.FooterHeight = 50;

            //pdfConverter.PdfBookmarkOptions.TagNames = cbBookmarks.Checked ? new string[] { "h1", "h2" } : null;

            return pdfConverter;
        }

        private PdfStandardSubset GetPdfStandard(string standardName)
        {
            switch (standardName)
            {
                case "PDF":
                    return PdfStandardSubset.Full;
                case "PDF/A":
                    return PdfStandardSubset.Pdf_A_1b;
                case "PDF/X":
                    return PdfStandardSubset.Pdf_X_1a;
                case "PDF/SiqQA":
                    return PdfStandardSubset.Pdf_SiqQ_a;
                case "PDF/SiqQB":
                    return PdfStandardSubset.Pdf_SiqQ_b;
                default:
                    return PdfStandardSubset.Full;

            }
        }

    }
}
