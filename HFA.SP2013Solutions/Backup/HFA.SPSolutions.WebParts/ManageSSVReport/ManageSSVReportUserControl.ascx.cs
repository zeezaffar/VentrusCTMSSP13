using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web;
using Microsoft.SharePoint;
using System.IO;
using ExpertPdf.HtmlToPdf;
using System.Drawing;
using System.Data;
using System.Configuration;
using DanFay.SPHelper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HFA.SPSolutions.WebParts.Libs;

namespace HFA.SPSolutions.WebParts.ManageSSVReport
{
    public partial class ManageSSVReportUserControl : UserControl
    {
        int newReportId = 0;

        protected string PDFDocumentLibrary
        {
            get
            {
                if (ConfigurationManager.AppSettings["PDFDocumentLibrary"] != null)
                    return ConfigurationManager.AppSettings["PDFDocumentLibrary"].ToString();

                else
                    throw new Exception("Missing PDFDocumentLibrary setting in web.config");
            }
        }

        private string ReportId
        {
            get
            {
                if (HttpUtility.ParseQueryString(Request.Url.Query).Get("ReportId") != null)
                    return (HttpUtility.ParseQueryString(Request.Url.Query).Get("ReportId").ToString());
                else
                    return string.Empty;
            }
        }

        private int SiteNo
        {
            get
            {
                if (HttpUtility.ParseQueryString(Request.Url.Query).Get("Site") != null)
                    return ((int.Parse(HttpUtility.ParseQueryString(Request.Url.Query).Get("Site").ToString())));
                else
                    return 0;
            }
        }

        private int SiteId
        {
            get;
            set;
        }

        private string WorkflowName
        {
            get { return ConfigurationManager.AppSettings["WorkflowName"].ToString(); }
        }

        private string UserToImpersonate
        {
            get { return ConfigurationManager.AppSettings["UserToImpersonate"].ToString(); }
        }

        private SiteList SiteInfo
        {
            get;
            set;
        }

        private int PreviousTabIndex
        {
            get
            {
                if (this.ViewState["PreviousTabIndex"] != null)
                    return int.Parse(this.ViewState["PreviousTabIndex"].ToString());
                else
                    return -1;
            }
            set { this.ViewState["PreviousTabIndex"] = value; }
        }

        private bool ShowMessage
        {
            get
            {
                if (SPContext.Current.Web.CurrentUser != null)
                    return Convert.ToBoolean(Application[SPContext.Current.Web.CurrentUser.ID.ToString()]);
                else
                    return false;
            }

            set
            {
                Application[SPContext.Current.Web.CurrentUser.ID.ToString()] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeForm();

                if (ReportId.Equals("0")) { FillSiteData(); return; }

                //Fill report header data
                FillHeaderData();

                //SetStatusField();
                FillVisitAttendees();
                FillBData();
                FillCData();
                FillDData();
                FillEData();
                FillFData();
                FillGData();
                FillHData();
                FillIData();
                FillJData();
                FillKData();
                FillLData();
                FillMData();
                FillNData();
                FillOData();
                FillPData();
                FillQData();

                if (ShowMessage)
                {
                    lblMessage.Text = "Report successfully saved";
                    ShowMessage = false;
                }
            }
        }

        protected void InitializeForm()
        {
            SiteInfo = new SiteList(SiteNo);
            //hplSiteOverView.NavigateUrl = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);
            btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);

            //Bind Sites dropdown
            SPListItemCollection items = Queries.GetSites();
            List<SPListItem> sortedItems = (from SPListItem x in items orderby x["Site_x0020_Number"] select x).ToList();

            foreach (SPListItem t in sortedItems)
            {
                ddlSites.Items.Add(t["Site_x0020_Number"].ToString());
            }
            ddlSites.Items.Insert(0, new ListItem("Select", "0"));

            //Fill Versions dropdown
            ddlVersion.DataSource = Utilities.GetVersions();
            ddlVersion.DataBind();
        }

        protected void FillHeaderData()
        {
            //Fill Header data
            SPListItem sivReport = GetSSVReportData(int.Parse(ReportId));

            if (sivReport != null)
            {
                txtTitle.Text = sivReport.Title;
                txtSponsor.Text = Utilities.GetStringValue(sivReport["Sponsor"]);
                txtProtocol.Text = Utilities.GetStringValue(sivReport["Protocol_x0020__x0023_"]);
                txtAddress.Text = Utilities.GetMultiLineTextFieldValue(sivReport,"Address");
                txtInvestigatorName.Text = Utilities.GetStringValue(sivReport["InvestigatorName"]);

                if (sivReport["Visit_x0020_Date"] != null)
                    calVisitDate.SelectedDate = Convert.ToDateTime(sivReport["Visit_x0020_Date"].ToString());

                if (sivReport["Next_x0020_Visit_x0020_Date"] != null)
                    calNextVisitDate.SelectedDate = Convert.ToDateTime(sivReport["Next_x0020_Visit_x0020_Date"].ToString());

                ddlStatus.SelectedValue = ddlStatus.Items.FindByText(sivReport["Status"].ToString()).Value;

                if (sivReport["Monitor"] != null)
                {
                    try
                    {
                        SPFieldLookupValue monitorLookupValue = new SPFieldLookupValue(sivReport["Monitor"].ToString());
                        if (monitorLookupValue != null)
                        {
                            SPUser user = SPContext.Current.Web.SiteUsers.GetByID(monitorLookupValue.LookupId);
                            Microsoft.SharePoint.WebControls.PickerEntity entity = new Microsoft.SharePoint.WebControls.PickerEntity();
                            entity.Key = user.LoginName;
                            System.Collections.ArrayList entityArrayList = new System.Collections.ArrayList();
                            entityArrayList.Add(entity);
                            speMonitor.UpdateEntities(entityArrayList);
                        }
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = ex.Message;
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                    }
                }

                ddlVersion.SelectedValue = Utilities.GetStringValue(sivReport["VersionNumber"]);
                if (sivReport["VersionDate"] != null)
                    calVersionDate.SelectedDate = Convert.ToDateTime(Utilities.GetShortDateValue(sivReport["VersionDate"]));

                txtMiscComments.Text = Utilities.GetMultiLineTextFieldValue(sivReport, "R_x002e_1_x0020_Miscellaneous_x0");
                txtGenRevComments.Text = Utilities.GetMultiLineTextFieldValue(sivReport, "General_x0020_Reviewer_x0020_Com");

                //Set dropdown to current site
                if (sivReport["Site_x0020_Number"] != null)
                {
                    SPFieldLookupValue siteLookup = new SPFieldLookupValue(sivReport["Site_x0020_Number"].ToString());
                    if (siteLookup.LookupValue != null && siteLookup.LookupValue.Contains("."))
                    {
                        string[] fieldArray = siteLookup.LookupValue.Split('.');
                        ddlSites.Items.FindByText(fieldArray[0]).Selected = true;
                    }
                }
            }
        }

        protected void FillSiteData()
        {
            SiteInfo = new SiteList(SiteNo);

            if (SiteInfo.Exists)
            {
                txtSponsor.Text = "Ventrus Biosciences";
                txtAddress.Text = SiteInfo.Address;
                txtInvestigatorName.Text = SiteInfo.InvestigatorName;
                txtInvestigatorTitle.Text = SiteInfo.InvestigatorTitle;
                ddlSites.SelectedValue = SiteNo.ToString();
                txtProtocol.Text = SiteInfo.Protocol;
                txtTitle.Text = string.Format("{0}_SSV_Report_{1}", SiteNo, DateTime.Now.ToString("ddMMMyyyy").ToUpper());
                btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);
            }
        }

        protected SPListItem GetSSVReportData(int reportId)
        {
            if (reportId == 0) return null;

            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPListItemCollection listItems = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        SPList list = web.Lists["SSV Report"];
                        SPQuery oQuery = new SPQuery();

                        string query = "<Where>";
                        query += "<Eq><FieldRef Name='ID' /><Value Type='Counter'>" + reportId + "</Value></Eq>";
                        query += "</Where>";
                        oQuery.Query = query;

                        //If is match is foud then delete list item
                        listItems = list.GetItems(oQuery);
                    }
                }
            });

            if (listItems.Count > 0)
                return listItems[0];
            else
                return null;
        }

        protected SPListItem GetSSVReportData(SPWeb web, int reportId)
        {
            if (reportId == 0) return null;

            SPListItemCollection listItems = null;

            SPList list = web.Lists["SSV Report"];
            SPQuery oQuery = new SPQuery();

            string query = "<Where>";
            query += "<Eq><FieldRef Name='ID' /><Value Type='Counter'>" + reportId + "</Value></Eq>";
            query += "</Where>";
            oQuery.Query = query;

            //If is match is foud then delete list item
            listItems = list.GetItems(oQuery);

            if (listItems.Count > 0)
                return listItems[0];
            else
                return null;
        }

        protected void formMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            lblMessage.Text = string.Empty;

            if (ReportId.Equals("0"))
            {
                lblMessage.Text = "Please click on the 'Save' button  below to save the report first before adding any other information";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            if (ddlSites.SelectedValue == "0")
            {
                lblMessage.Text = "Please select a site number";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            //Get index
            int idx = int.Parse(e.Item.Value);
            formMultiView.ActiveViewIndex = idx;
            UnselectAllMenuItems(formMenu2);

            switch (PreviousTabIndex)
            {
                case 1:
                    SaveBData();
                    break;

                case 2:
                    SaveCData();
                    break;

                case 3:
                    SaveDData();
                    break;

                case 4:
                    SaveEData();
                    break;

                case 5:
                    SaveFData();
                    break;

                case 6:
                    SaveGData();
                    break;

                case 7:
                    SaveHData();
                    break;

                case 8:
                    SaveIData();
                    break;

                case 9:
                    SaveJData();
                    break;
            }

            //Save new PreviousTabIndex
            PreviousTabIndex = idx;
        }

        protected void formMenu2_MenuItemClick(object sender, MenuEventArgs e)
        {
            lblMessage.Text = string.Empty;

            if (ReportId.Equals("0"))
            {
                lblMessage.Text = "Please click on the 'Save' button  below to save the report first before adding any other information";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            int idx = int.Parse(e.Item.Value);
            formMultiView.ActiveViewIndex = idx;
            UnselectAllMenuItems(formMenu);

            switch (PreviousTabIndex)
            {
                case 10:
                    SaveKData();
                    break;

                case 11:
                    SaveLData();
                    break;

                case 12:
                    SaveMData();
                    break;

                case 13:
                    SaveNData();
                    break;

                case 14:
                    SaveOData();
                    break;

                case 15:
                    SavePData();
                    break;

                case 16:
                    SaveQData();
                    break;
            }

            //Save new PreviousTabIndex
            PreviousTabIndex = idx;
        }

        protected void UnselectAllMenuItems(Menu menu)
        {
            foreach (MenuItem item in menu.Items)
            {
                item.Selected = false;
            }
        }

        protected void FillVisitAttendees()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            //Utilities.GetStringValue(item["Investigator"]);
            txtInvestigatorTitle.Text = Utilities.GetStringValue(item["InvestigatorTitle"]);

            txtSitePersonnelName.Text = Utilities.GetStringValue(item["SitePersonnelName"]);
            txtSitePersonnelTitle.Text = Utilities.GetStringValue(item["SitePersonnelTitle"]);
            txtSitePersonnelName2.Text = Utilities.GetStringValue(item["SitePersonnelName2"]);
            txtSitePersonnelTitle2.Text = Utilities.GetStringValue(item["SitePersonnelTitle2"]);
            txtSitePersonnelName3.Text = Utilities.GetStringValue(item["SitePersonnelName3"]);
            txtSitePersonnelTitle3.Text = Utilities.GetStringValue(item["SitePersonnelTitle3"]);

            txtMonitorName.Text = Utilities.GetStringValue(item["MonitorName"]);
            txtMonitorTitle.Text = Utilities.GetStringValue(item["MonitorTitle"]);

            ddlSitePersonnel.SelectedValue = Utilities.GetStringValue(item["Intro_x002e_Clinical_x0020_Site_"]);
            txtSitePersonnelComments.Text = Utilities.GetStringValue(item["Intro_x002e_Clinical_x0020_Site_0"]);

            ddlPersonnel.SelectedValue = Utilities.GetStringValue(item["Intro_x002e_Other_x0020_Personne"]);
            txtPersonnelComments.Text = Utilities.GetStringValue(item["Intro_x002e_Other_x0020_Personne0"]);

            txtVAReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "Visit_x002e_Reviewer");
        }

        protected void FillBData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            if (item["B_x002e_1"] != null)
                ddlB1.SelectedValue = item["B_x002e_1"].ToString();

            if (item["B_x002e_2"] != null)
                ddlB2.SelectedValue = item["B_x002e_2"].ToString();

            if (item["B_x002e_3"] != null)
                ddlB3.SelectedValue = item["B_x002e_3"].ToString();

            txtB1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "B_x002e_1_x0020_Comments");
            txtB2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "B_x002e_2_x0020_Comments");
            txtB3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "B_x002e_3_x0020_Comments");

            txtBReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "B_x002e_Reviewer");
        }

        protected void FillCData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlC1.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_1"]);
            ddlC2.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_2"]);
            ddlC3.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_3"]);

            txtC1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_1_x0020_Comments");
            txtC2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_2_x0020_Comments");
            txtC3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_3_x0020_Comments");

            txtCReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_Reviewer");
        }

        protected void FillDData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlD1.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_1"]);
            ddlD2.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_2"]);
            ddlD3.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_3"]);
            ddlD4.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_4"]);
            ddlD5.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_5"]);
            ddlD6.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_6"]);
            ddlD7.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_7"]);

            txtD1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "D_x002e_1_x0020_Comments");
            txtD2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "D_x002e_2_x0020_Comments");
            txtD3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "D_x002e_3_x0020_Comments");
            txtD4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "D_x002e_4_x0020_Comments");
            txtD5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "D_x002e_5_x0020_Comments");
            txtD6Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "D_x002e_6_x0020_Comments");
            txtD7Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "D_x002e_7_x0020_Comments");

            txtDReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "D_x002e_Reviewer");
        }

        protected void FillEData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlE1.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_1"]);
            ddlE2.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_2"]);
            ddlE3.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_3"]);
            ddlE4.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_4"]);
            ddlE5.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_5"]);
            ddlE6.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_6"]);
            ddlE7.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_7"]);

            txtE1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_1_x0020_Comments");
            txtE2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_2_x0020_Comments");
            txtE3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_3_x0020_Comments");
            txtE4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_4_x0020_Comments");
            txtE5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_5_x0020_Comments");
            txtE6Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_6_x0020_Comments");
            txtE7Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_7_x0020_Comments");

            txtEReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_Reviewer");
        }

        protected void FillFData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlF1.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_1"]);
            ddlF2.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_2"]);
            ddlF3.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_3"]);
            ddlF4.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_4"]);
            ddlF5.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_5"]);
            ddlF6.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_6"]);
            ddlF7.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_7"]);
            ddlF8.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_8"]);
            ddlF9.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_9"]);
            ddlF10.SelectedValue = Utilities.GetDDLStringValue(item["F_x002e_10"]);

            txtF1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_1_x0020_Comments");
            txtF2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_2_x0020_Comments");
            txtF3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_3_x0020_Comments");
            txtF4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_4_x0020_Comments");
            txtF5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_5_x0020_Comments");
            txtF6Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_6_x0020_Comments");
            txtF7Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_7_x0020_Comments");
            txtF8Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_8_x0020_Comments");
            txtF9Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_9_x0020_Comments");
            txtF10Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_10_x0020_Comments");

            txtFReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "F_x002e_Reviewer");
        }

        protected void FillGData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlG1.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_1"]);
            ddlG2.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_2"]);
            ddlG3.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_3"]);
            ddlG4.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_4"]);
            ddlG5.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_5"]);

            txtG1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_1_x0020_Comments");
            txtG2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_2_x0020_Comments");
            txtG3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_3_x0020_Comments");
            txtG4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_4_x0020_Comments");
            txtG5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_5_x0020_Comments");

            txtGReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_Reviewer");
        }

        protected void FillHData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlH1.SelectedValue = Utilities.GetDDLStringValue(item["H_x002e_1"]);
            ddlH2.SelectedValue = Utilities.GetDDLStringValue(item["H_x002e_2"]);

            txtH1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_1_x0020_Comments");
            txtH2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_2_x0020_Comments");

            txtHReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_Reviewer");
        }

        protected void FillIData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlI1.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_1"]);
            ddlI2.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_2"]);
            ddlI3.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_3"]);

            txtI1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_1_x0020_Comments");
            txtI2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_2_x0020_Comments");
            txtI3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_3_x0020_Comments");

            txtIReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_Reviewer");
        }

        protected void FillJData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlJ1.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_1"]);
            ddlJ2.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_2"]);
            ddlJ3.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_3"]);
            ddlJ4.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_4"]);
            ddlJ5.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_5"]);
            ddlJ6.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_6"]);
            ddlJ7.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_7"]);

            txtJ1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_1_x0020_Comments");
            txtJ2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_2_x0020_Comments");
            txtJ3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_3_x0020_Comments");
            txtJ4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_4_x0020_Comments");
            txtJ5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_5_x0020_Comments");
            txtJ6Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_6_x0020_Comments");
            txtJ7Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_7_x0020_Comments");

            txtJReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_Reviewer");
        }

        protected void FillKData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlK1.SelectedValue = Utilities.GetDDLStringValue(item["K_x002e_1"]);
            ddlK2.SelectedValue = Utilities.GetDDLStringValue(item["K_x002e_2"]);
            ddlK3.SelectedValue = Utilities.GetDDLStringValue(item["K_x002e_3"]);
            ddlK4.SelectedValue = Utilities.GetDDLStringValue(item["K_x002e_4"]);

            txtK1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_1_x0020_Comments");
            txtK2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_2_x0020_Comments");
            txtK3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_3_x0020_Comments");
            txtK4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_4_x0020_Comments");

            txtKReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_Reviewer");
        }

        protected void FillLData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlL1.SelectedValue = Utilities.GetDDLStringValue(item["L_x002e_1"]);
            ddlL2.SelectedValue = Utilities.GetDDLStringValue(item["L_x002e_2"]);
            ddlL3.SelectedValue = Utilities.GetDDLStringValue(item["L_x002e_3"]);
            ddlL4.SelectedValue = Utilities.GetDDLStringValue(item["L_x002e_4"]);
            ddlL5.SelectedValue = Utilities.GetDDLStringValue(item["L_x002e_5"]);
            ddlL6.SelectedValue = Utilities.GetDDLStringValue(item["L_x002e_6"]);

            txtL1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_1_x0020_Comments");
            txtL2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_2_x0020_Comments");
            txtL3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_3_x0020_Comments");
            txtL4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_4_x0020_Comments");
            txtL5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_5_x0020_Comments");
            txtL6Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_6_x0020_Comments");

            txtLReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_Reviewer");
        }

        protected void FillMData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlM1.SelectedValue = Utilities.GetDDLStringValue(item["M_x002e_1"]);
            ddlM2.SelectedValue = Utilities.GetDDLStringValue(item["M_x002e_2"]);
            ddlM3.SelectedValue = Utilities.GetDDLStringValue(item["M_x002e_3"]);
            ddlM4.SelectedValue = Utilities.GetDDLStringValue(item["M_x002e_4"]);

            txtM1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "M_x002e_1_x0020_Comments");
            txtM2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "M_x002e_2_x0020_Comments");
            txtM3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "M_x002e_3_x0020_Comments");
            txtM4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "M_x002e_4_x0020_Comments");

            txtMReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "M_x002e_Reviewer");
        }

        protected void FillNData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlN1.SelectedValue = Utilities.GetDDLStringValue(item["N_x002e_1"]);
            ddlN2.SelectedValue = Utilities.GetDDLStringValue(item["N_x002e_2"]);
            ddlN3.SelectedValue = Utilities.GetDDLStringValue(item["N_x002e_3"]);
            ddlN4.SelectedValue = Utilities.GetDDLStringValue(item["N_x002e_4"]);
            ddlN5.SelectedValue = Utilities.GetDDLStringValue(item["N_x002e_5"]);
            ddlN6.SelectedValue = Utilities.GetDDLStringValue(item["N_x002e_6"]);
            ddlN7.SelectedValue = Utilities.GetDDLStringValue(item["N_x002e_7"]);

            txtN1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e_1_x0020_Comments");
            txtN2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e_2_x0020_Comments");
            txtN3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e_3_x0020_Comments");
            txtN4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e_4_x0020_Comments");
            txtN5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e_5_x0020_Comments");
            txtN6Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e_6_x0020_Comments");
            txtN7Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e_7_x0020_Comments");

            txtNReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e_Reviewer");
        }

        protected void FillOData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlO1.SelectedValue = Utilities.GetDDLStringValue(item["O_x002e_1"]);
            ddlO2.SelectedValue = Utilities.GetDDLStringValue(item["O_x002e_2"]);
            ddlO3.SelectedValue = Utilities.GetDDLStringValue(item["O_x002e_3"]);
            ddlO4.SelectedValue = Utilities.GetDDLStringValue(item["O_x002e_4"]);

            txtO1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "O_x002e_1_x0020_Comments");
            txtO2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "O_x002e_2_x0020_Comments");
            txtO3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "O_x002e_3_x0020_Comments");
            txtO4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "O_x002e_4_x0020_Comments");

            txtOReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "O_x002e_Reviewer");
        }

        protected void FillPData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlP1.SelectedValue = Utilities.GetDDLStringValue(item["P_x002e_1"]);
            ddlP2.SelectedValue = Utilities.GetDDLStringValue(item["P_x002e_2"]);
            ddlP3.SelectedValue = Utilities.GetDDLStringValue(item["P_x002e_3"]);
            ddlP4.SelectedValue = Utilities.GetDDLStringValue(item["P_x002e_4"]);

            txtP1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "P_x002e_1_x0020_Comments");
            txtP2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "P_x002e_2_x0020_Comments");
            txtP3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "P_x002e_3_x0020_Comments");
            txtP4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "P_x002e_4_x0020_Comments");

            txtPReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "P_x002e_Reviewer");
        }

        protected void FillQData()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            ddlQ1.SelectedValue = Utilities.GetDDLStringValue(item["Q_x002e_1"]);
            ddlQ2.SelectedValue = Utilities.GetDDLStringValue(item["Q_x002e_2"]);

            txtQ1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "Q_x002e_1_x0020_Comments");
            txtQ2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "Q_x002e_2_x0020_Comments");

            txtQReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "Q_x002e_Reviewer");
        }

        protected void SaveBData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["B_x002e_1"] = ddlB1.SelectedValue;
            item["B_x002e_2"] = ddlB2.SelectedValue;
            item["B_x002e_3"] = ddlB3.SelectedValue;

            item["B_x002e_1_x0020_Comments"] = txtB1Comments.Text;
            item["B_x002e_2_x0020_Comments"] = txtB2Comments.Text;
            item["B_x002e_3_x0020_Comments"] = txtB3Comments.Text;

            item["B_x002e_Reviewer"] = txtBReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveCData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["C_x002e_1"] = ddlC1.SelectedValue;
            item["C_x002e_2"] = ddlC2.SelectedValue;
            item["C_x002e_3"] = ddlC3.SelectedValue;

            item["C_x002e_1_x0020_Comments"] = txtC1Comments.Text;
            item["C_x002e_2_x0020_Comments"] = txtC2Comments.Text;
            item["C_x002e_3_x0020_Comments"] = txtC3Comments.Text;

            item["C_x002e_Reviewer"] = txtCReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveDData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["D_x002e_1"] = ddlD1.SelectedValue;
            item["D_x002e_2"] = ddlD2.SelectedValue;
            item["D_x002e_3"] = ddlD3.SelectedValue;
            item["D_x002e_4"] = ddlD4.SelectedValue;
            item["D_x002e_5"] = ddlD5.SelectedValue;
            item["D_x002e_6"] = ddlD6.SelectedValue;
            item["D_x002e_7"] = ddlD7.SelectedValue;

            item["D_x002e_1_x0020_Comments"] = txtD1Comments.Text;
            item["D_x002e_2_x0020_Comments"] = txtD2Comments.Text;
            item["D_x002e_3_x0020_Comments"] = txtD3Comments.Text;
            item["D_x002e_4_x0020_Comments"] = txtD4Comments.Text;
            item["D_x002e_5_x0020_Comments"] = txtD5Comments.Text;
            item["D_x002e_6_x0020_Comments"] = txtD6Comments.Text;
            item["D_x002e_7_x0020_Comments"] = txtD7Comments.Text;

            item["D_x002e_Reviewer"] = txtDReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveEData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["E_x002e_1"] = ddlE1.SelectedValue;
            item["E_x002e_2"] = ddlE2.SelectedValue;
            item["E_x002e_3"] = ddlE3.SelectedValue;
            item["E_x002e_4"] = ddlE4.SelectedValue;
            item["E_x002e_5"] = ddlE5.SelectedValue;
            item["E_x002e_6"] = ddlE6.SelectedValue;
            item["E_x002e_7"] = ddlE7.SelectedValue;

            item["E_x002e_1_x0020_Comments"] = txtE1Comments.Text;
            item["E_x002e_2_x0020_Comments"] = txtE2Comments.Text;
            item["E_x002e_3_x0020_Comments"] = txtE3Comments.Text;
            item["E_x002e_4_x0020_Comments"] = txtE4Comments.Text;
            item["E_x002e_5_x0020_Comments"] = txtE5Comments.Text;
            item["E_x002e_6_x0020_Comments"] = txtE6Comments.Text;
            item["E_x002e_7_x0020_Comments"] = txtE7Comments.Text;

            item["E_x002e_Reviewer"] = txtEReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveFData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["F_x002e_1"] = ddlF1.SelectedValue;
            item["F_x002e_2"] = ddlF2.SelectedValue;
            item["F_x002e_3"] = ddlF3.SelectedValue;
            item["F_x002e_4"] = ddlF4.SelectedValue;
            item["F_x002e_5"] = ddlF5.SelectedValue;
            item["F_x002e_6"] = ddlF6.SelectedValue;
            item["F_x002e_7"] = ddlF7.SelectedValue;
            item["F_x002e_8"] = ddlF8.SelectedValue;
            item["F_x002e_9"] = ddlF9.SelectedValue;
            item["F_x002e_10"] = ddlF10.SelectedValue;

            item["F_x002e_1_x0020_Comments"] = txtF1Comments.Text;
            item["F_x002e_2_x0020_Comments"] = txtF2Comments.Text;
            item["F_x002e_3_x0020_Comments"] = txtF3Comments.Text;
            item["F_x002e_4_x0020_Comments"] = txtF4Comments.Text;
            item["F_x002e_5_x0020_Comments"] = txtF5Comments.Text;
            item["F_x002e_6_x0020_Comments"] = txtF6Comments.Text;
            item["F_x002e_7_x0020_Comments"] = txtF7Comments.Text;
            item["F_x002e_8_x0020_Comments"] = txtF8Comments.Text;
            item["F_x002e_9_x0020_Comments"] = txtF9Comments.Text;
            item["F_x002e_10_x0020_Comments"] = txtF10Comments.Text;

            item["F_x002e_Reviewer"] = txtFReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveGData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["G_x002e_1"] = ddlG1.SelectedValue;
            item["G_x002e_2"] = ddlG2.SelectedValue;
            item["G_x002e_3"] = ddlG3.SelectedValue;
            item["G_x002e_4"] = ddlG4.SelectedValue;
            item["G_x002e_5"] = ddlG5.SelectedValue;

            item["G_x002e_1_x0020_Comments"] = txtG1Comments.Text;
            item["G_x002e_2_x0020_Comments"] = txtG2Comments.Text;
            item["G_x002e_3_x0020_Comments"] = txtG3Comments.Text;
            item["G_x002e_4_x0020_Comments"] = txtG4Comments.Text;
            item["G_x002e_5_x0020_Comments"] = txtG5Comments.Text;

            item["G_x002e_Reviewer"] = txtGReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveHData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["H_x002e_1"] = ddlH1.SelectedValue;
            item["H_x002e_2"] = ddlH2.SelectedValue;

            item["H_x002e_1_x0020_Comments"] = txtH1Comments.Text;
            item["H_x002e_2_x0020_Comments"] = txtH2Comments.Text;

            item["H_x002e_Reviewer"] = txtHReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveIData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["I_x002e_1"] = ddlI1.SelectedValue;
            item["I_x002e_2"] = ddlI2.SelectedValue;
            item["I_x002e_3"] = ddlI3.SelectedValue;

            item["I_x002e_1_x0020_Comments"] = txtI1Comments.Text;
            item["I_x002e_2_x0020_Comments"] = txtI2Comments.Text;
            item["I_x002e_3_x0020_Comments"] = txtI3Comments.Text;

            item["I_x002e_Reviewer"] = txtIReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveJData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["J_x002e_1"] = ddlJ1.SelectedValue;
            item["J_x002e_2"] = ddlJ2.SelectedValue;
            item["J_x002e_3"] = ddlJ3.SelectedValue;
            item["J_x002e_4"] = ddlJ4.SelectedValue;
            item["J_x002e_5"] = ddlJ5.SelectedValue;
            item["J_x002e_6"] = ddlJ6.SelectedValue;
            item["J_x002e_7"] = ddlJ7.SelectedValue;

            item["J_x002e_1_x0020_Comments"] = txtJ1Comments.Text;
            item["J_x002e_2_x0020_Comments"] = txtJ2Comments.Text;
            item["J_x002e_3_x0020_Comments"] = txtJ3Comments.Text;
            item["J_x002e_4_x0020_Comments"] = txtJ4Comments.Text;
            item["J_x002e_5_x0020_Comments"] = txtJ5Comments.Text;
            item["J_x002e_6_x0020_Comments"] = txtJ6Comments.Text;
            item["J_x002e_7_x0020_Comments"] = txtJ7Comments.Text;

            item["J_x002e_Reviewer"] = txtJReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveKData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["K_x002e_1"] = ddlK1.SelectedValue;
            item["K_x002e_2"] = ddlK2.SelectedValue;
            item["K_x002e_3"] = ddlK3.SelectedValue;
            item["K_x002e_4"] = ddlK4.SelectedValue;

            item["K_x002e_1_x0020_Comments"] = txtK1Comments.Text;
            item["K_x002e_2_x0020_Comments"] = txtK2Comments.Text;
            item["K_x002e_3_x0020_Comments"] = txtK3Comments.Text;
            item["K_x002e_4_x0020_Comments"] = txtK4Comments.Text;

            item["K_x002e_Reviewer"] = txtKReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveLData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["L_x002e_1"] = ddlL1.SelectedValue;
            item["L_x002e_2"] = ddlL2.SelectedValue;
            item["L_x002e_3"] = ddlL3.SelectedValue;
            item["L_x002e_4"] = ddlL4.SelectedValue;
            item["L_x002e_5"] = ddlL5.SelectedValue;
            item["L_x002e_6"] = ddlL6.SelectedValue;

            item["L_x002e_1_x0020_Comments"] = txtL1Comments.Text;
            item["L_x002e_2_x0020_Comments"] = txtL2Comments.Text;
            item["L_x002e_3_x0020_Comments"] = txtL3Comments.Text;
            item["L_x002e_4_x0020_Comments"] = txtL4Comments.Text;
            item["L_x002e_5_x0020_Comments"] = txtL5Comments.Text;
            item["L_x002e_6_x0020_Comments"] = txtL6Comments.Text;

            item["L_x002e_Reviewer"] = txtLReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveMData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["M_x002e_1"] = ddlM1.SelectedValue;
            item["M_x002e_2"] = ddlM2.SelectedValue;
            item["M_x002e_3"] = ddlM3.SelectedValue;
            item["M_x002e_4"] = ddlM4.SelectedValue;

            item["M_x002e_1_x0020_Comments"] = txtM1Comments.Text;
            item["M_x002e_2_x0020_Comments"] = txtM2Comments.Text;
            item["M_x002e_3_x0020_Comments"] = txtM3Comments.Text;
            item["M_x002e_4_x0020_Comments"] = txtM4Comments.Text;

            item["M_x002e_Reviewer"] = txtMReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveNData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["N_x002e_1"] = ddlN1.SelectedValue;
            item["N_x002e_2"] = ddlN2.SelectedValue;
            item["N_x002e_3"] = ddlN3.SelectedValue;
            item["N_x002e_4"] = ddlN4.SelectedValue;
            item["N_x002e_5"] = ddlN5.SelectedValue;
            item["N_x002e_6"] = ddlN6.SelectedValue;
            item["N_x002e_7"] = ddlN7.SelectedValue;

            item["N_x002e_1_x0020_Comments"] = txtN1Comments.Text;
            item["N_x002e_2_x0020_Comments"] = txtN2Comments.Text;
            item["N_x002e_3_x0020_Comments"] = txtN3Comments.Text;
            item["N_x002e_4_x0020_Comments"] = txtN4Comments.Text;
            item["N_x002e_5_x0020_Comments"] = txtN5Comments.Text;
            item["N_x002e_6_x0020_Comments"] = txtN6Comments.Text;
            item["N_x002e_7_x0020_Comments"] = txtN7Comments.Text;

            item["N_x002e_Reviewer"] = txtNReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveOData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["O_x002e_1"] = ddlO1.SelectedValue;
            item["O_x002e_2"] = ddlO2.SelectedValue;
            item["O_x002e_3"] = ddlO3.SelectedValue;
            item["O_x002e_4"] = ddlO4.SelectedValue;

            item["O_x002e_1_x0020_Comments"] = txtO1Comments.Text;
            item["O_x002e_2_x0020_Comments"] = txtO2Comments.Text;
            item["O_x002e_3_x0020_Comments"] = txtO3Comments.Text;
            item["O_x002e_4_x0020_Comments"] = txtO4Comments.Text;

            item["O_x002e_Reviewer"] = txtOReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SavePData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["P_x002e_1"] = ddlP1.SelectedValue;
            item["P_x002e_2"] = ddlP2.SelectedValue;
            item["P_x002e_3"] = ddlP3.SelectedValue;
            item["P_x002e_4"] = ddlP4.SelectedValue;

            item["P_x002e_1_x0020_Comments"] = txtP1Comments.Text;
            item["P_x002e_2_x0020_Comments"] = txtP2Comments.Text;
            item["P_x002e_3_x0020_Comments"] = txtP3Comments.Text;
            item["P_x002e_4_x0020_Comments"] = txtP4Comments.Text;

            item["P_x002e_Reviewer"] = txtPReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveQData()
        {
            SPListItem item = GetSSVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["Q_x002e_1"] = ddlQ1.SelectedValue;
            item["Q_x002e_2"] = ddlQ2.SelectedValue;

            item["Q_x002e_1_x0020_Comments"] = txtQ1Comments.Text;
            item["Q_x002e_2_x0020_Comments"] = txtQ2Comments.Text;

            item["Q_x002e_Reviewer"] = txtQReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void btnPDFGenerator_Click(object sender, EventArgs e)
        {
            if (ReportId.Equals("0"))
            {
                lblMessage.Text = "Report does not exists. Please save a report first";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            PDFGenerator();
        }

        protected void PDFGenerator()
        {
            string pdfFileName = string.Empty;

            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {

                        string mainTemplateFileName = "C:\\HFA\\SSVReportTemplate-Main.html";
                        string signatureTemplateFileName = "C:\\HFA\\SignatureTemplate.html";

                        string convertedFileNameMain = "C:\\HFA\\temp\\SSVReportConverted-Main.html";
                        string convertedFileNameSignature = "C:\\HFA\\temp\\SSVReportConverted-Signature.html";

                        SPListItem SSVReport = GetSSVReportData(int.Parse(ReportId));
                        pdfFileName = string.Format("{0}.pdf", SSVReport.Title);

                        if (SSVReport == null)
                            return;

                        // Read in the contents of the Receipt.htm HTML template file
                        string mainHtml = File.ReadAllText(mainTemplateFileName).Replace("\r\n", string.Empty);
                        string field = string.Empty;

                        //Replace variable tags
                        mainHtml = mainHtml.Replace("varSponsor", Utilities.GetStringValue(SSVReport["Sponsor"]));
                        mainHtml = mainHtml.Replace("varProtocol", Utilities.GetStringValue(SSVReport["Protocol_x0020__x0023_"]));
                        mainHtml = mainHtml.Replace("varSiteNo", ReportId.ToString());
                        mainHtml = mainHtml.Replace("varStudySiteNumber", Utilities.GetLookupFieldValue(SSVReport["Site_x0020_Number"]));
                        mainHtml = mainHtml.Replace("varVisitDate", Utilities.GetShortDateValue(SSVReport["Visit_x0020_Date"]));
                        mainHtml = mainHtml.Replace("varInvestigatorName", Utilities.GetReportStringValue(SSVReport["InvestigatorName"]));
                        mainHtml = mainHtml.Replace("varAddress", Utilities.GetReportStringValue(SSVReport["Address"]));

                        //Visit Attendees
                        mainHtml = mainHtml.Replace("varInvestigatorTitle", Utilities.GetReportStringValue(SSVReport["InvestigatorTitle"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelName1", Utilities.GetReportStringValue(SSVReport["SitePersonnelName"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelTitle1", Utilities.GetReportStringValue(SSVReport["SitePersonnelTitle"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelTitle2", Utilities.GetReportStringValue(SSVReport["SitePersonnelTitle2"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelName2", Utilities.GetReportStringValue(SSVReport["SitePersonnelName2"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelTitle3", Utilities.GetReportStringValue(SSVReport["SitePersonnelTitle3"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelName3", Utilities.GetReportStringValue(SSVReport["SitePersonnelName3"]));

                        mainHtml = mainHtml.Replace("varSitePersonnelComments", Utilities.GetReportStringValue(SSVReport["Intro_x002e_Clinical_x0020_Site_0"]));
                        mainHtml = mainHtml.Replace("varOtherPersonnelComments", Utilities.GetReportStringValue(SSVReport["Intro_x002e_Other_x0020_Personne0"]));

                        mainHtml = mainHtml.Replace("varMonitorName", Utilities.GetReportStringValue(SSVReport["MonitorName"]));
                        mainHtml = mainHtml.Replace("varMonitorTitle", Utilities.GetReportStringValue(SSVReport["MonitorTitle"]));

                        mainHtml = mainHtml.Replace("varSitePersonnel", Utilities.GetReportStringValue(SSVReport["Intro_x002e_Clinical_x0020_Site_"]));
                        mainHtml = mainHtml.Replace("varOtherPersonnel", Utilities.GetReportStringValue(SSVReport["Intro_x002e_Other_x0020_Personne"]));

                        //B Section
                        mainHtml = mainHtml.Replace("varB1Comments", Utilities.GetReportStringValue(SSVReport["B_x002e_1_x0020_Comments"]));
                        mainHtml = mainHtml.Replace("varB2Comments", Utilities.GetReportStringValue(SSVReport["B_x002e_2_x0020_Comments"]));
                        mainHtml = mainHtml.Replace("varB3Comments", Utilities.GetReportStringValue(SSVReport["B_x002e_3_x0020_Comments"]));
                        mainHtml = mainHtml.Replace("varB1", Utilities.GetReportStringValue(SSVReport["B_x002e_1"]));
                        mainHtml = mainHtml.Replace("varB2", Utilities.GetReportStringValue(SSVReport["B_x002e_2"]));
                        mainHtml = mainHtml.Replace("varB3", Utilities.GetReportStringValue(SSVReport["B_x002e_3"]));

                        //C Section
                        mainHtml = mainHtml.Replace("varC1Comments", Utilities.GetReportStringValue(SSVReport["C_x002e_1_x0020_Comments"]));
                        mainHtml = mainHtml.Replace("varC2Comments", Utilities.GetReportStringValue(SSVReport["C_x002e_2_x0020_Comments"]));
                        mainHtml = mainHtml.Replace("varC3Comments", Utilities.GetReportStringValue(SSVReport["C_x002e_3_x0020_Comments"]));
                        mainHtml = mainHtml.Replace("varC1", Utilities.GetReportStringValue(SSVReport["C_x002e_1"]));
                        mainHtml = mainHtml.Replace("varC2", Utilities.GetReportStringValue(SSVReport["C_x002e_2"]));
                        mainHtml = mainHtml.Replace("varC3", Utilities.GetReportStringValue(SSVReport["C_x002e_3"]));

                        //D Section
                        mainHtml = mainHtml.Replace("varD1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "D_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varD2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "D_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varD3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "D_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varD4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "D_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varD5Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "D_x002e_5_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varD6Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "D_x002e_6_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varD7Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "D_x002e_7_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varD1", Utilities.GetReportStringValue(SSVReport["D_x002e_1"]));
                        mainHtml = mainHtml.Replace("varD2", Utilities.GetReportStringValue(SSVReport["D_x002e_2"]));
                        mainHtml = mainHtml.Replace("varD3", Utilities.GetReportStringValue(SSVReport["D_x002e_3"]));
                        mainHtml = mainHtml.Replace("varD4", Utilities.GetReportStringValue(SSVReport["D_x002e_4"]));
                        mainHtml = mainHtml.Replace("varD5", Utilities.GetReportStringValue(SSVReport["D_x002e_5"]));
                        mainHtml = mainHtml.Replace("varD6", Utilities.GetReportStringValue(SSVReport["D_x002e_6"]));
                        mainHtml = mainHtml.Replace("varD7", Utilities.GetReportStringValue(SSVReport["D_x002e_7"]));

                        //E Section
                        mainHtml = mainHtml.Replace("varE1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "E_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "E_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "E_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "E_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE5Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "E_x002e_5_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE6Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "E_x002e_6_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE7Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "E_x002e_7_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varE2", Utilities.GetReportStringValue(SSVReport["E_x002e_2"]));
                        mainHtml = mainHtml.Replace("varE1", Utilities.GetReportStringValue(SSVReport["E_x002e_1"]));
                        mainHtml = mainHtml.Replace("varE3", Utilities.GetReportStringValue(SSVReport["E_x002e_3"]));
                        mainHtml = mainHtml.Replace("varE4", Utilities.GetReportStringValue(SSVReport["E_x002e_4"]));
                        mainHtml = mainHtml.Replace("varE5", Utilities.GetReportStringValue(SSVReport["E_x002e_5"]));
                        mainHtml = mainHtml.Replace("varE6", Utilities.GetReportStringValue(SSVReport["E_x002e_6"]));
                        mainHtml = mainHtml.Replace("varE7", Utilities.GetReportStringValue(SSVReport["E_x002e_7"]));

                        //F Section
                        mainHtml = mainHtml.Replace("varF10Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_10_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF5Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_5_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF6Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_6_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF7Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_7_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF8Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_8_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varF9Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "F_x002e_9_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varF10", Utilities.GetReportStringValue(SSVReport["F_x002e_10"]));
                        mainHtml = mainHtml.Replace("varF1", Utilities.GetReportStringValue(SSVReport["F_x002e_1"]));
                        mainHtml = mainHtml.Replace("varF2", Utilities.GetReportStringValue(SSVReport["F_x002e_2"]));
                        mainHtml = mainHtml.Replace("varF3", Utilities.GetReportStringValue(SSVReport["F_x002e_3"]));
                        mainHtml = mainHtml.Replace("varF4", Utilities.GetReportStringValue(SSVReport["F_x002e_4"]));
                        mainHtml = mainHtml.Replace("varF5", Utilities.GetReportStringValue(SSVReport["F_x002e_5"]));
                        mainHtml = mainHtml.Replace("varF6", Utilities.GetReportStringValue(SSVReport["F_x002e_6"]));
                        mainHtml = mainHtml.Replace("varF7", Utilities.GetReportStringValue(SSVReport["F_x002e_7"]));
                        mainHtml = mainHtml.Replace("varF8", Utilities.GetReportStringValue(SSVReport["F_x002e_8"]));
                        mainHtml = mainHtml.Replace("varF9", Utilities.GetReportStringValue(SSVReport["F_x002e_9"]));
                     

                        //G Section
                        mainHtml = mainHtml.Replace("varG1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "G_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varG2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "G_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varG3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "G_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varG4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "G_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varG5Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "G_x002e_5_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varG1", Utilities.GetReportStringValue(SSVReport["G_x002e_1"]));
                        mainHtml = mainHtml.Replace("varG2", Utilities.GetReportStringValue(SSVReport["G_x002e_2"]));
                        mainHtml = mainHtml.Replace("varG3", Utilities.GetReportStringValue(SSVReport["G_x002e_3"]));
                        mainHtml = mainHtml.Replace("varG4", Utilities.GetReportStringValue(SSVReport["G_x002e_4"]));
                        mainHtml = mainHtml.Replace("varG5", Utilities.GetReportStringValue(SSVReport["G_x002e_5"]));

                        //H Section
                        mainHtml = mainHtml.Replace("varH1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "H_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varH2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "H_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varH1", Utilities.GetReportStringValue(SSVReport["H_x002e_1"]));
                        mainHtml = mainHtml.Replace("varH2", Utilities.GetReportStringValue(SSVReport["H_x002e_2"]));

                        //I Section
                        mainHtml = mainHtml.Replace("varI1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "I_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "I_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "I_x002e_3_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varI1", Utilities.GetReportStringValue(SSVReport["I_x002e_1"]));
                        mainHtml = mainHtml.Replace("varI2", Utilities.GetReportStringValue(SSVReport["I_x002e_2"]));
                        mainHtml = mainHtml.Replace("varI3", Utilities.GetReportStringValue(SSVReport["I_x002e_3"]));

                        //J Section
                        mainHtml = mainHtml.Replace("varJ1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "J_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varJ2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "J_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varJ3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "J_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varJ4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "J_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varJ5Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "J_x002e_5_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varJ6Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "J_x002e_6_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varJ7Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "J_x002e_7_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varJ1", Utilities.GetReportStringValue(SSVReport["J_x002e_1"]));
                        mainHtml = mainHtml.Replace("varJ2", Utilities.GetReportStringValue(SSVReport["J_x002e_2"]));
                        mainHtml = mainHtml.Replace("varJ3", Utilities.GetReportStringValue(SSVReport["J_x002e_3"]));
                        mainHtml = mainHtml.Replace("varJ4", Utilities.GetReportStringValue(SSVReport["J_x002e_4"]));
                        mainHtml = mainHtml.Replace("varJ5", Utilities.GetReportStringValue(SSVReport["J_x002e_5"]));
                        mainHtml = mainHtml.Replace("varJ6", Utilities.GetReportStringValue(SSVReport["J_x002e_6"]));
                        mainHtml = mainHtml.Replace("varJ7", Utilities.GetReportStringValue(SSVReport["J_x002e_7"]));

                        //K Section
                        mainHtml = mainHtml.Replace("varK1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "K_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varK2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "K_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varK3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "K_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varK4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "K_x002e_4_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varK1", Utilities.GetReportStringValue(SSVReport["K_x002e_3"]));
                        mainHtml = mainHtml.Replace("varK2", Utilities.GetReportStringValue(SSVReport["K_x002e_1"]));
                        mainHtml = mainHtml.Replace("varK3", Utilities.GetReportStringValue(SSVReport["K_x002e_2"]));
                        mainHtml = mainHtml.Replace("varK4", Utilities.GetReportStringValue(SSVReport["K_x002e_4"]));

                        //L Section
                        mainHtml = mainHtml.Replace("varL1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "L_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varL2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "L_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varL3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "L_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varL4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "L_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varL5Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "L_x002e_5_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varL6Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "L_x002e_6_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varL1", Utilities.GetReportStringValue(SSVReport["L_x002e_1"]));
                        mainHtml = mainHtml.Replace("varL2", Utilities.GetReportStringValue(SSVReport["L_x002e_2"]));
                        mainHtml = mainHtml.Replace("varL3", Utilities.GetReportStringValue(SSVReport["L_x002e_3"]));
                        mainHtml = mainHtml.Replace("varL4", Utilities.GetReportStringValue(SSVReport["L_x002e_4"]));
                        mainHtml = mainHtml.Replace("varL5", Utilities.GetReportStringValue(SSVReport["L_x002e_5"]));
                        mainHtml = mainHtml.Replace("varL6", Utilities.GetReportStringValue(SSVReport["L_x002e_6"]));

                        //M Section
                        mainHtml = mainHtml.Replace("varM1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "M_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varM2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "M_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varM3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "M_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varM4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "M_x002e_4_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varM1", Utilities.GetReportStringValue(SSVReport["M_x002e_1"]));
                        mainHtml = mainHtml.Replace("varM2", Utilities.GetReportStringValue(SSVReport["M_x002e_2"]));
                        mainHtml = mainHtml.Replace("varM3", Utilities.GetReportStringValue(SSVReport["M_x002e_3"]));
                        mainHtml = mainHtml.Replace("varM4", Utilities.GetReportStringValue(SSVReport["M_x002e_4"]));

                        //N Section
                        mainHtml = mainHtml.Replace("varN1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "N_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varN2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "N_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varN3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "N_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varN4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "N_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varN5Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "N_x002e_5_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varN6Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "N_x002e_6_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varN7Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "N_x002e_7_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varN1", Utilities.GetReportStringValue(SSVReport["N_x002e_1"]));
                        mainHtml = mainHtml.Replace("varN2", Utilities.GetReportStringValue(SSVReport["N_x002e_2"]));
                        mainHtml = mainHtml.Replace("varN3", Utilities.GetReportStringValue(SSVReport["N_x002e_3"]));
                        mainHtml = mainHtml.Replace("varN4", Utilities.GetReportStringValue(SSVReport["N_x002e_4"]));
                        mainHtml = mainHtml.Replace("varN5", Utilities.GetReportStringValue(SSVReport["N_x002e_5"]));
                        mainHtml = mainHtml.Replace("varN6", Utilities.GetReportStringValue(SSVReport["N_x002e_6"]));
                        mainHtml = mainHtml.Replace("varN7", Utilities.GetReportStringValue(SSVReport["N_x002e_7"]));
                       
                        //O Section
                        mainHtml = mainHtml.Replace("varO1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "O_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varO2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "O_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varO3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "O_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varO4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "O_x002e_4_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varO1", Utilities.GetReportStringValue(SSVReport["O_x002e_1"]));
                        mainHtml = mainHtml.Replace("varO2", Utilities.GetReportStringValue(SSVReport["O_x002e_2"]));
                        mainHtml = mainHtml.Replace("varO3", Utilities.GetReportStringValue(SSVReport["O_x002e_3"]));
                        mainHtml = mainHtml.Replace("varO4", Utilities.GetReportStringValue(SSVReport["O_x002e_4"]));

                        //P Section
                        mainHtml = mainHtml.Replace("varP1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "P_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varP2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "P_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varP3Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "P_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varP4Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "P_x002e_4_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varP1", Utilities.GetReportStringValue(SSVReport["P_x002e_1"]));
                        mainHtml = mainHtml.Replace("varP2", Utilities.GetReportStringValue(SSVReport["P_x002e_2"]));
                        mainHtml = mainHtml.Replace("varP3", Utilities.GetReportStringValue(SSVReport["P_x002e_3"]));
                        mainHtml = mainHtml.Replace("varP4", Utilities.GetReportStringValue(SSVReport["P_x002e_4"]));
                        
                        //Q Section
                        mainHtml = mainHtml.Replace("varQ1Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "Q_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varQ2Comments", Utilities.GetMultiLineReportTextFieldValue(SSVReport, "Q_x002e_2_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varQ1", Utilities.GetReportStringValue(SSVReport["Q_x002e_1"]));
                        mainHtml = mainHtml.Replace("varQ2", Utilities.GetReportStringValue(SSVReport["Q_x002e_2"]));

                       
                        //Last section
                        mainHtml = mainHtml.Replace("varNextVisit", Utilities.GetShortDateValue(SSVReport["Next_x0020_Visit_x0020_Date"]));
                        mainHtml = mainHtml.Replace("varMiscComments", Utilities.GetReportStringValue(SSVReport["R_x002e_1_x0020_Miscellaneous_x0"])); 

                        //string issueId = string.Empty;
                        //string creationDate = string.Empty;
                        //string closeDate = string.Empty;
                        //string description = string.Empty;
                        //string category = string.Empty;
                        //string action = string.Empty;
                        //string subjectId = string.Empty;

                        //string issuesList = string.Empty;


                        //Write active issues
                        //Write active issues
                        //SPListItemCollection activeIssues = Queries.GetActiveIssues(SiteNo);
                        //foreach (SPListItem issue in activeIssues)
                        //{
                        //    issueId = issue.ID.ToString();
                        //    subjectId = Utilities.GetStringValue(issue["SubjectId"]);
                        //    creationDate = (issue["Issue_x0020_Creation_x0020_Date"] != null) ? Convert.ToDateTime(issue["Issue_x0020_Creation_x0020_Date"]).ToShortDateString() : string.Empty;
                        //    closeDate = (issue["Issue_x0020_Close_x0020_Date"] != null) ? Convert.ToDateTime(issue["Issue_x0020_Close_x0020_Date"]).ToShortDateString() : string.Empty;
                        //    description = Utilities.GetStringValue(issue["Description"]);
                        //    category = Utilities.GetStringValue(issue["Category"]);
                        //    action = Utilities.GetStringValue(issue["Action_x0020_Required"]);

                        //    issuesList += string.Format(Utilities.GetIssueItemHTML(), issueId, GetCellValue(creationDate), GetCellValue(closeDate), GetCellValue(category), GetCellValue(subjectId), GetCellValue(description), GetCellValue(action));
                        //}

                        //mainHtml = mainHtml.Replace("varOpenIssues", issuesList);
                        //issuesList = string.Empty;

                        //Write closed issues
                        //SPListItemCollection closedIssues = Queries.GetClosedIssues(SiteNo);
                        //foreach (SPListItem issue in closedIssues)
                        //{
                        //    issueId = issue.ID.ToString();
                        //    subjectId = Utilities.GetStringValue(issue["SubjectId"]);
                        //    creationDate = (issue["Issue_x0020_Creation_x0020_Date"] != null) ? Convert.ToDateTime(issue["Issue_x0020_Creation_x0020_Date"]).ToShortDateString() : string.Empty;
                        //    closeDate = (issue["Issue_x0020_Close_x0020_Date"] != null) ? Convert.ToDateTime(issue["Issue_x0020_Close_x0020_Date"]).ToShortDateString() : string.Empty;
                        //    description = Utilities.GetStringValue(issue["Description"]);
                        //    category = Utilities.GetStringValue(issue["Category"]);
                        //    action = Utilities.GetStringValue(issue["Action_x0020_Required"]);

                        //    issuesList += string.Format(Utilities.GetIssueItemHTML(), issueId, GetCellValue(creationDate), GetCellValue(closeDate), GetCellValue(category), GetCellValue(subjectId), GetCellValue(description), GetCellValue(action));
                        //}

                        //mainHtml = mainHtml.Replace("varClosedIssues", issuesList);

                        //Write main html file to disk
                        File.WriteAllText(convertedFileNameMain, mainHtml);

                        //signature page section
                        string signatureHtml = File.ReadAllText(signatureTemplateFileName).Replace("\r\n", string.Empty);
                        signatureHtml = signatureHtml.Replace("varVersionNumber", Utilities.GetReportStringValue(SSVReport["VersionNumber"]));
                        signatureHtml = signatureHtml.Replace("varVersionDate", Utilities.GetShortDateValue(SSVReport["VersionDate"]));

                        //Write signature html file to disk
                        File.WriteAllText(convertedFileNameSignature, signatureHtml);

                        string pdfFile = string.Format("C:\\HFA\\temp\\{0}", pdfFileName);

                        PDFConversion.CreateFinalPDF(convertedFileNameMain, convertedFileNameSignature, pdfFile);

                        SPFolder pdfLibrary = web.Folders[PDFDocumentLibrary];

                        //Upload PDF file to document library
                        if (!Utilities.FileExists(pdfLibrary, pdfFileName))
                        {
                            if (Utilities.UplodFileToDocLibrary(web, PDFDocumentLibrary, pdfFile, SiteNo, "SSV", WorkflowName))
                            {
                                lblMessage.Text = "PDF Report successfully created";
                            }
                        }
                        else
                            lblMessage.Text = "Report with the same name already exists";
                    }
                }
            });
        }

        protected void btnSaveAll_Click(object sender, EventArgs e)
        {
            SPListItem item = null;
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPList siteList = null;

            if (speMonitor.ResolvedEntities.Count <= 0)
                return;

            SPWeb oWeb = SPContext.Current.Web;
            SPUserToken userToken = oWeb.AllUsers[@UserToImpersonate].UserToken;

            using (SPSite site = new SPSite(siteID, userToken))
            {
                using (SPWeb web = site.AllWebs[webID])
                {

                    if (ReportId.Equals("0"))
                    {
                        if (Queries.ReportExists("SSV Report", txtTitle.Text))
                        {
                            lblMessage.Text = "Report already exists with the same name. Please enter a different name and try again.";
                            return;
                        }

                        siteList = web.Lists["SSV Report"];

                        web.AllowUnsafeUpdates = true;
                        item = siteList.Items.Add();
                        item["Title"] = txtTitle.Text;
                        item.Update();
                        web.AllowUnsafeUpdates = false;

                        //Get new ID
                        newReportId = item.ID;
                        item = GetSSVReportData(web, newReportId);
                    }
                    else
                        item = GetSSVReportData(web, int.Parse(ReportId));


                    item.Web.AllowUnsafeUpdates = true;
                    web.AllowUnsafeUpdates = true;

                    //Get Site info. by siteNo and create site lookup value field
                    SPListItem siteItem = Queries.GetSiteBySiteNo(int.Parse(ddlSites.SelectedItem.Text));
                    SiteInfo = new SiteList(int.Parse(ddlSites.SelectedItem.Text));

                    if (SiteInfo.Exists)
                        btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);

                    SPFieldLookupValue siteLookupValue = new SPFieldLookupValue(siteItem.ID, siteItem.Title);
                    item["Site_x0020_Number"] = siteLookupValue.ToString();

                    item["Title"] = txtTitle.Text;
                    item["InvestigatorName"] = txtInvestigatorName.Text;
                    item["Sponsor"] = txtSponsor.Text;
                    item["Protocol_x0020__x0023_"] = txtProtocol.Text;
                    //item["Visit_x0020_Date"] = calVisitDate.SelectedDate;
                    //item["Next_x0020_Visit_x0020_Date"] = calNextVisitDate.SelectedDate;
                    if (!calVisitDate.IsDateEmpty)
                        item["Visit_x0020_Date"] = calVisitDate.SelectedDate;
                    else
                        item["Visit_x0020_Date"] = null;

                    if (!calNextVisitDate.IsDateEmpty)
                        item["Next_x0020_Visit_x0020_Date"] = calNextVisitDate.SelectedDate;
                    else
                        item["Next_x0020_Visit_x0020_Date"] = null;

                    item["Address"] = txtAddress.Text;
                    item["Status"] = item.Fields["Status"].GetFieldValue(ddlStatus.SelectedItem.Text);
                    item["SiteNumberValue"] = SiteNo;
                    item["R_x002e_1_x0020_Miscellaneous_x0"] = txtMiscComments.Text;
                    item["General_x0020_Reviewer_x0020_Com"] = txtGenRevComments.Text;
                    item["VersionNumber"] = ddlVersion.SelectedValue;
                    item["VersionDate"] = calVersionDate.SelectedDate;

                    //Set hyperlink values
                    SPFieldUrlValue sitePageUrl = new SPFieldUrlValue();
                    SPFieldUrlValue reportTitleUrl = new SPFieldUrlValue();

                    //Site Page hyperlink field
                    sitePageUrl.Url = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}", SPContext.Current.Web.Url, SiteNo);
                    sitePageUrl.Description = SiteNo.ToString();
                    item["Site_x0020_Page"] = sitePageUrl;

                    //Report Title Page hyperlink field
                    reportTitleUrl.Url = string.Format("{0}/SitePages/SSVReport.aspx?Site={1}&ReportId={2}", SPContext.Current.Web.Url, SiteNo, item.ID);
                    reportTitleUrl.Description = txtTitle.Text;
                    item["Report_x0020_Title"] = reportTitleUrl;

                    //People picker monitor field                
                    SPUser user = SPContext.Current.Web.SiteUsers[speMonitor.CommaSeparatedAccounts];
                    if (user != null)
                        item["Monitor"] = user;


                    //Investigate Attendees
                    item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
                    item["SitePersonnelName"] = txtSitePersonnelName.Text;
                    item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;
                    item["SitePersonnelName2"] = txtSitePersonnelName2.Text;
                    item["SitePersonnelTitle2"] = txtSitePersonnelTitle2.Text;
                    item["SitePersonnelName3"] = txtSitePersonnelName3.Text;
                    item["SitePersonnelTitle3"] = txtSitePersonnelTitle3.Text;

                    item["MonitorName"] = txtMonitorName.Text;
                    item["MonitorTitle"] = txtMonitorTitle.Text;

                    //Site
                    item["Intro_x002e_Clinical_x0020_Site_"] = ddlSitePersonnel.SelectedValue;
                    item["Intro_x002e_Clinical_x0020_Site_0"] = txtSitePersonnelComments.Text;

                    //Personnel
                    item["Intro_x002e_Other_x0020_Personne"] = ddlPersonnel.SelectedValue;
                    item["Intro_x002e_Other_x0020_Personne0"] = txtPersonnelComments.Text;

                    item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

                    //Save B Section
                    item["B_x002e_1"] = ddlB1.SelectedValue;
                    item["B_x002e_2"] = ddlB2.SelectedValue;
                    item["B_x002e_3"] = ddlB3.SelectedValue;

                    item["B_x002e_1_x0020_Comments"] = txtB1Comments.Text;
                    item["B_x002e_2_x0020_Comments"] = txtB2Comments.Text;
                    item["B_x002e_3_x0020_Comments"] = txtB3Comments.Text;

                    item["B_x002e_Reviewer"] = txtBReviewerComments.Text;

                    //Save C Section
                    item["C_x002e_1"] = ddlC1.SelectedValue;
                    item["C_x002e_2"] = ddlC2.SelectedValue;
                    item["C_x002e_3"] = ddlC3.SelectedValue;

                    item["C_x002e_1_x0020_Comments"] = txtC1Comments.Text;
                    item["C_x002e_2_x0020_Comments"] = txtC2Comments.Text;
                    item["C_x002e_3_x0020_Comments"] = txtC3Comments.Text;

                    item["C_x002e_Reviewer"] = txtCReviewerComments.Text;

                    //Save D Section
                    item["D_x002e_1"] = ddlD1.SelectedValue;
                    item["D_x002e_2"] = ddlD2.SelectedValue;
                    item["D_x002e_3"] = ddlD3.SelectedValue;
                    item["D_x002e_4"] = ddlD4.SelectedValue;
                    item["D_x002e_5"] = ddlD5.SelectedValue;
                    item["D_x002e_6"] = ddlD6.SelectedValue;
                    item["D_x002e_7"] = ddlD7.SelectedValue;

                    item["D_x002e_1_x0020_Comments"] = txtD1Comments.Text;
                    item["D_x002e_2_x0020_Comments"] = txtD2Comments.Text;
                    item["D_x002e_3_x0020_Comments"] = txtD3Comments.Text;
                    item["D_x002e_4_x0020_Comments"] = txtD4Comments.Text;
                    item["D_x002e_5_x0020_Comments"] = txtD5Comments.Text;
                    item["D_x002e_6_x0020_Comments"] = txtD6Comments.Text;
                    item["D_x002e_7_x0020_Comments"] = txtD7Comments.Text;

                    item["D_x002e_Reviewer"] = txtDReviewerComments.Text;

                    //Save E Section
                    item["E_x002e_1"] = ddlE1.SelectedValue;
                    item["E_x002e_2"] = ddlE2.SelectedValue;
                    item["E_x002e_3"] = ddlE3.SelectedValue;
                    item["E_x002e_4"] = ddlE4.SelectedValue;
                    item["E_x002e_5"] = ddlE5.SelectedValue;
                    item["E_x002e_6"] = ddlE6.SelectedValue;
                    item["E_x002e_7"] = ddlE7.SelectedValue;

                    item["E_x002e_1_x0020_Comments"] = txtE1Comments.Text;
                    item["E_x002e_2_x0020_Comments"] = txtE2Comments.Text;
                    item["E_x002e_3_x0020_Comments"] = txtE3Comments.Text;
                    item["E_x002e_4_x0020_Comments"] = txtE4Comments.Text;
                    item["E_x002e_5_x0020_Comments"] = txtE5Comments.Text;
                    item["E_x002e_6_x0020_Comments"] = txtE6Comments.Text;
                    item["E_x002e_7_x0020_Comments"] = txtE7Comments.Text;

                    item["E_x002e_Reviewer"] = txtEReviewerComments.Text;

                    //Save F Section
                    item["F_x002e_1"] = ddlF1.SelectedValue;
                    item["F_x002e_2"] = ddlF2.SelectedValue;
                    item["F_x002e_3"] = ddlF3.SelectedValue;
                    item["F_x002e_4"] = ddlF4.SelectedValue;
                    item["F_x002e_5"] = ddlF5.SelectedValue;
                    item["F_x002e_6"] = ddlF6.SelectedValue;
                    item["F_x002e_7"] = ddlF7.SelectedValue;
                    item["F_x002e_8"] = ddlF8.SelectedValue;
                    item["F_x002e_9"] = ddlF9.SelectedValue;
                    item["F_x002e_10"] = ddlF10.SelectedValue;

                    item["F_x002e_1_x0020_Comments"] = txtF1Comments.Text;
                    item["F_x002e_2_x0020_Comments"] = txtF2Comments.Text;
                    item["F_x002e_3_x0020_Comments"] = txtF3Comments.Text;
                    item["F_x002e_4_x0020_Comments"] = txtF4Comments.Text;
                    item["F_x002e_5_x0020_Comments"] = txtF5Comments.Text;
                    item["F_x002e_6_x0020_Comments"] = txtF6Comments.Text;
                    item["F_x002e_7_x0020_Comments"] = txtF7Comments.Text;
                    item["F_x002e_8_x0020_Comments"] = txtF8Comments.Text;
                    item["F_x002e_9_x0020_Comments"] = txtF9Comments.Text;
                    item["F_x002e_10_x0020_Comments"] = txtF10Comments.Text;

                    item["F_x002e_Reviewer"] = txtFReviewerComments.Text;

                    //Save G Section
                    item["G_x002e_1"] = ddlG1.SelectedValue;
                    item["G_x002e_2"] = ddlG2.SelectedValue;
                    item["G_x002e_3"] = ddlG3.SelectedValue;
                    item["G_x002e_4"] = ddlG4.SelectedValue;
                    item["G_x002e_5"] = ddlG5.SelectedValue;

                    item["G_x002e_1_x0020_Comments"] = txtG1Comments.Text;
                    item["G_x002e_2_x0020_Comments"] = txtG2Comments.Text;
                    item["G_x002e_3_x0020_Comments"] = txtG3Comments.Text;
                    item["G_x002e_4_x0020_Comments"] = txtG4Comments.Text;
                    item["G_x002e_5_x0020_Comments"] = txtG5Comments.Text;

                    item["G_x002e_Reviewer"] = txtGReviewerComments.Text;

                    //Save H Section
                    item["H_x002e_1"] = ddlH1.SelectedValue;
                    item["H_x002e_2"] = ddlH2.SelectedValue;

                    item["H_x002e_1_x0020_Comments"] = txtH1Comments.Text;
                    item["H_x002e_2_x0020_Comments"] = txtH2Comments.Text;

                    item["H_x002e_Reviewer"] = txtHReviewerComments.Text;

                    //Save I Section
                    item["I_x002e_1"] = ddlI1.SelectedValue;
                    item["I_x002e_2"] = ddlI2.SelectedValue;
                    item["I_x002e_3"] = ddlI3.SelectedValue;

                    item["I_x002e_1_x0020_Comments"] = txtI1Comments.Text;
                    item["I_x002e_2_x0020_Comments"] = txtI2Comments.Text;
                    item["I_x002e_3_x0020_Comments"] = txtI3Comments.Text;

                    item["I_x002e_Reviewer"] = txtIReviewerComments.Text;

                    //Save J Section
                    item["J_x002e_1"] = ddlJ1.SelectedValue;
                    item["J_x002e_2"] = ddlJ2.SelectedValue;
                    item["J_x002e_3"] = ddlJ3.SelectedValue;
                    item["J_x002e_4"] = ddlJ4.SelectedValue;
                    item["J_x002e_5"] = ddlJ5.SelectedValue;
                    item["J_x002e_6"] = ddlJ6.SelectedValue;
                    item["J_x002e_7"] = ddlJ7.SelectedValue;

                    item["J_x002e_1_x0020_Comments"] = txtJ1Comments.Text;
                    item["J_x002e_2_x0020_Comments"] = txtJ2Comments.Text;
                    item["J_x002e_3_x0020_Comments"] = txtJ3Comments.Text;
                    item["J_x002e_4_x0020_Comments"] = txtJ4Comments.Text;
                    item["J_x002e_5_x0020_Comments"] = txtJ5Comments.Text;
                    item["J_x002e_6_x0020_Comments"] = txtJ6Comments.Text;
                    item["J_x002e_7_x0020_Comments"] = txtJ7Comments.Text;

                    item["J_x002e_Reviewer"] = txtJReviewerComments.Text;

                    //Save K Section
                    item["K_x002e_1"] = ddlK1.SelectedValue;
                    item["K_x002e_2"] = ddlK2.SelectedValue;
                    item["K_x002e_3"] = ddlK3.SelectedValue;
                    item["K_x002e_4"] = ddlK4.SelectedValue;

                    item["K_x002e_1_x0020_Comments"] = txtK1Comments.Text;
                    item["K_x002e_2_x0020_Comments"] = txtK2Comments.Text;
                    item["K_x002e_3_x0020_Comments"] = txtK3Comments.Text;
                    item["K_x002e_4_x0020_Comments"] = txtK4Comments.Text;

                    item["K_x002e_Reviewer"] = txtKReviewerComments.Text;

                    //Save L Section
                    item["L_x002e_1"] = ddlL1.SelectedValue;
                    item["L_x002e_2"] = ddlL2.SelectedValue;
                    item["L_x002e_3"] = ddlL3.SelectedValue;
                    item["L_x002e_4"] = ddlL4.SelectedValue;
                    item["L_x002e_5"] = ddlL5.SelectedValue;
                    item["L_x002e_6"] = ddlL6.SelectedValue;

                    item["L_x002e_1_x0020_Comments"] = txtL1Comments.Text;
                    item["L_x002e_2_x0020_Comments"] = txtL2Comments.Text;
                    item["L_x002e_3_x0020_Comments"] = txtL3Comments.Text;
                    item["L_x002e_4_x0020_Comments"] = txtL4Comments.Text;
                    item["L_x002e_5_x0020_Comments"] = txtL5Comments.Text;
                    item["L_x002e_6_x0020_Comments"] = txtL6Comments.Text;

                    item["L_x002e_Reviewer"] = txtLReviewerComments.Text;

                    //Save M Section
                    item["M_x002e_1"] = ddlM1.SelectedValue;
                    item["M_x002e_2"] = ddlM2.SelectedValue;
                    item["M_x002e_3"] = ddlM3.SelectedValue;
                    item["M_x002e_4"] = ddlM4.SelectedValue;

                    item["M_x002e_1_x0020_Comments"] = txtM1Comments.Text;
                    item["M_x002e_2_x0020_Comments"] = txtM2Comments.Text;
                    item["M_x002e_3_x0020_Comments"] = txtM3Comments.Text;
                    item["M_x002e_4_x0020_Comments"] = txtM4Comments.Text;

                    item["M_x002e_Reviewer"] = txtMReviewerComments.Text;

                    //Save N Section

                    item["N_x002e_1"] = ddlN1.SelectedValue;
                    item["N_x002e_2"] = ddlN2.SelectedValue;
                    item["N_x002e_3"] = ddlN3.SelectedValue;
                    item["N_x002e_4"] = ddlN4.SelectedValue;
                    item["N_x002e_5"] = ddlN5.SelectedValue;
                    item["N_x002e_6"] = ddlN6.SelectedValue;
                    item["N_x002e_7"] = ddlN7.SelectedValue;

                    item["N_x002e_1_x0020_Comments"] = txtN1Comments.Text;
                    item["N_x002e_2_x0020_Comments"] = txtN2Comments.Text;
                    item["N_x002e_3_x0020_Comments"] = txtN3Comments.Text;
                    item["N_x002e_4_x0020_Comments"] = txtN4Comments.Text;
                    item["N_x002e_5_x0020_Comments"] = txtN5Comments.Text;
                    item["N_x002e_6_x0020_Comments"] = txtN6Comments.Text;
                    item["N_x002e_7_x0020_Comments"] = txtN7Comments.Text;

                    item["N_x002e_Reviewer"] = txtNReviewerComments.Text;

                    //Save O Section
                    item["O_x002e_1"] = ddlO1.SelectedValue;
                    item["O_x002e_2"] = ddlO2.SelectedValue;
                    item["O_x002e_3"] = ddlO3.SelectedValue;
                    item["O_x002e_4"] = ddlO4.SelectedValue;

                    item["O_x002e_1_x0020_Comments"] = txtO1Comments.Text;
                    item["O_x002e_2_x0020_Comments"] = txtO2Comments.Text;
                    item["O_x002e_3_x0020_Comments"] = txtO3Comments.Text;
                    item["O_x002e_4_x0020_Comments"] = txtO4Comments.Text;

                    item["O_x002e_Reviewer"] = txtOReviewerComments.Text;

                    //Save P Section
                    item["P_x002e_1"] = ddlP1.SelectedValue;
                    item["P_x002e_2"] = ddlP2.SelectedValue;
                    item["P_x002e_3"] = ddlP3.SelectedValue;
                    item["P_x002e_4"] = ddlP4.SelectedValue;

                    item["P_x002e_1_x0020_Comments"] = txtP1Comments.Text;
                    item["P_x002e_2_x0020_Comments"] = txtP2Comments.Text;
                    item["P_x002e_3_x0020_Comments"] = txtP3Comments.Text;
                    item["P_x002e_4_x0020_Comments"] = txtP4Comments.Text;

                    item["P_x002e_Reviewer"] = txtPReviewerComments.Text;

                    //Save Q Section
                    item["Q_x002e_1"] = ddlQ1.SelectedValue;
                    item["Q_x002e_2"] = ddlQ2.SelectedValue;

                    item["Q_x002e_1_x0020_Comments"] = txtQ1Comments.Text;
                    item["Q_x002e_2_x0020_Comments"] = txtQ2Comments.Text;

                    item["Q_x002e_Reviewer"] = txtQReviewerComments.Text;

                    //Save Changes
                    item.Update();

                    web.AllowUnsafeUpdates = false;
                    item.Web.AllowUnsafeUpdates = false;

                    //If new report, redirect to current page to get updated report id in the querystring
                    if (ReportId.Equals("0"))
                    {
                        ShowMessage = true;
                        Response.Redirect(reportTitleUrl.Url);
                    }

                }
            }

            lblMessage.Text = "Report successfully saved";
        }

        protected void btnSiteOverview_Click(object sender, EventArgs e)
        {
            //Save all changes
            btnSaveAll_Click(null, EventArgs.Empty);

            //Redirect page
            Response.Redirect(btnSiteOverview.CommandArgument);
            //hplSiteOverView.NavigateUrl = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);

        }
    }
}
