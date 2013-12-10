using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Configuration;
using System.Web;
using System.Data;
using DanFay.SPHelper;
using System.IO;
using ExpertPdf.HtmlToPdf;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using HFA.SPSolutions.WebParts.Libs;

namespace HFA.SPSolutions.WebParts.ManageIMVReport
{
    public partial class ManageIMVReportUserControl : UserControl
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

        private bool ShowMessage
        {
            get
            {
                return Convert.ToBoolean(Application[SPContext.Current.Web.CurrentUser.ID.ToString()]); 
            }

            set
            {
                Application[SPContext.Current.Web.CurrentUser.ID.ToString()] = value;
            }
        }

        private int PreviousTabIndex
        {
            get
            {
                if (this.ViewState["PreviousTabIndex"] != null)
                    return int.Parse(this.ViewState["PreviousTabIndex"].ToString());
                else
                    return 0;
            }
            set { this.ViewState["PreviousTabIndex"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeForm();

                if (ReportId == "0") { FillSiteData(); return; }

                FillHeaderData();

                //SetStatusField();
                FillVisitAttendees();
                FillSectionC();
                FillSectionD();
                FillSectionE();
                FillSectionG();
                FillSectionH();
                FillSectionI();
                FillSectionJ();
                FillSectionK();
                FillSectionL();
                FillSectionM();

                if (ShowMessage)
                {
                    lblMessage.Text = "Report successfully saved";
                    ShowMessage = false;
                }
            }
        }

        protected void InitializeForm()
        {
            //Bind Sites
            SPListItemCollection items = Queries.GetSites();
            List<SPListItem> sortedItems = (from SPListItem x in items orderby x["Site_x0020_Number"] select x).ToList();

            foreach (SPListItem t in sortedItems)
            {
                ddlSites.Items.Add(t["Site_x0020_Number"].ToString());
            }
            ddlSites.Items.Insert(0, new ListItem("Select", "0"));

            //Fill Version
            ddlVersion.DataSource = Utilities.GetVersions();
            ddlVersion.DataBind();

            SiteInfo = new SiteList(SiteNo);
            btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);


            hidSiteId.Value = SiteInfo.ID.ToString();
        }

        protected void FillHeaderData()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                txtTitle.Text = Utilities.GetReportStringValue(item.Title);
                txtSponsor.Text = Utilities.GetStringValue(item["Sponsor"]);
                txtProtocol.Text = Utilities.GetStringValue(item["Protocol_x0020__x0023_"]);
                txtAddress.Text = Utilities.GetMultiLineTextFieldValue(item, "Address");

                if (item["Visit_x0020_Date"] != null)
                    calVisitDate.SelectedDate = Convert.ToDateTime(item["Visit_x0020_Date"].ToString());

                if (item["LastDayOfVisit"] != null)
                    calLastDayOfVisit.SelectedDate = Convert.ToDateTime(item["LastDayOfVisit"].ToString());

                if (item["Next_x0020_Visit_x0020_Date"] != null)
                    calNextVisitDate.SelectedDate = Convert.ToDateTime(item["Next_x0020_Visit_x0020_Date"].ToString());

                if (item["Status"] != null)
                    ddlStatus.SelectedValue = ddlStatus.Items.FindByText(item["Status"].ToString()).Value;

                if (item["Monitor"] != null)
                {
                    try
                    {
                        SPFieldLookupValue monitorLookupValue = new SPFieldLookupValue(item["Monitor"].ToString());
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

                ddlVersion.SelectedValue = Utilities.GetStringValue(item["VersionNumber"]);
                if (item["VersionDate"] != null)
                    calVersionDate.SelectedDate = Convert.ToDateTime(Utilities.GetShortDateValue(item["VersionDate"]));

                txtMiscComments.Text = Utilities.GetMultiLineTextFieldValue(item, "N_x002e__x0020_Miscellaneous_x00");
                txtGenRevComments.Text = Utilities.GetMultiLineTextFieldValue(item, "General_x0020_Reviewer_x0020_Com");

                //Set dropdown to current site
                if (item["Site_x0020_Number"] != null)
                {
                    SPFieldLookupValue siteLookup = new SPFieldLookupValue(item["Site_x0020_Number"].ToString());
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
                txtTitle.Text = string.Format("{0}_IMV_Report_{1}", SiteNo, DateTime.Now.ToString("ddMMMyyyy").ToUpper());
                btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);
            }
        }

        protected void btnSaveAll_Click(object sender, EventArgs e)
        {
            if (speMonitor.ResolvedEntities.Count <= 0)
                return;

            //Save Header
            SPListItem item = null;
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;
            SPList siteList = null;

            SPWeb oWeb = SPContext.Current.Web;
            SPUserToken userToken = oWeb.AllUsers[@UserToImpersonate].UserToken;

            using (SPSite site = new SPSite(siteID, userToken))
            {
                using (SPWeb web = site.AllWebs[webID])
                {
                    if (ReportId.Equals("0"))
                    {
                        if (Queries.ReportExists("IMV Report", txtTitle.Text))
                        {
                            lblMessage.Text = "Report already exists with the same name. Please enter a different name and try again.";
                            return;
                        }

                        siteList = web.Lists["IMV Report"];
                        web.AllowUnsafeUpdates = true;
                        item = siteList.Items.Add();
                        item["Title"] = txtTitle.Text;
                        item.Update();
                        web.AllowUnsafeUpdates = false;

                        //Get new ID
                        newReportId = item.ID;
                        item = GetIMVReportData(web, newReportId);
                    }
                    else
                        item = GetIMVReportData(web, int.Parse(ReportId));

                    item.Web.AllowUnsafeUpdates = true;
                    web.AllowUnsafeUpdates = true;

                    //Get Site info. by siteNo and create site lookup value field
                    SPListItem siteItem = Queries.GetSiteBySiteNo(int.Parse(ddlSites.SelectedItem.Text));
                    SiteInfo = new SiteList(int.Parse(ddlSites.SelectedItem.Text));

                    if (SiteInfo.Exists)
                        btnSiteOverview.CommandArgument = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}&SiteId={2}", SPContext.Current.Web.Url, SiteInfo.SiteNumber, SiteInfo.ID);


                    SPFieldLookupValue siteLookupValue = new SPFieldLookupValue(siteItem.ID, siteItem.Title);
                    item["Site_x0020_Number"] = siteLookupValue.ToString();

                    //Header
                    item["Title"] = txtTitle.Text;
                    item["Sponsor"] = txtSponsor.Text;
                    item["Protocol_x0020__x0023_"] = txtProtocol.Text;

                    if (!calVisitDate.IsDateEmpty)
                        item["Visit_x0020_Date"] = calVisitDate.SelectedDate;
                    else
                        item["Visit_x0020_Date"] = null;

                    if (!calLastDayOfVisit.IsDateEmpty)
                        item["LastDayOfVisit"] = calLastDayOfVisit.SelectedDate;
                    else
                        item["LastDayOfVisit"] = null;

                    if (!calNextVisitDate.IsDateEmpty)
                        item["Next_x0020_Visit_x0020_Date"] = calNextVisitDate.SelectedDate;
                    else
                        item["Next_x0020_Visit_x0020_Date"] = null;

                    item["Status"] = item.Fields["Status"].GetFieldValue(ddlStatus.SelectedItem.Text);
                    item["N_x002e__x0020_Miscellaneous_x00"] = txtMiscComments.Text;
                    item["General_x0020_Reviewer_x0020_Com"] = txtGenRevComments.Text;
                    item["Address"] = txtAddress.Text;
                    item["VersionNumber"] = ddlVersion.SelectedValue;
                    item["VersionDate"] = calVersionDate.SelectedDate;

                    //People picker monitor field
                    SPUser user = SPContext.Current.Web.SiteUsers[speMonitor.CommaSeparatedAccounts];
                    if (user != null)
                        item["Monitor"] = user;

                    //Set hyperlink values
                    SPFieldUrlValue sitePageUrl = new SPFieldUrlValue();
                    SPFieldUrlValue reportTitleUrl = new SPFieldUrlValue();

                    //Site Page hyperlink field
                    sitePageUrl.Url = string.Format("{0}/SitePages/SiteOverview.aspx?Site={1}", SPContext.Current.Web.Url, SiteNo);
                    sitePageUrl.Description = SiteNo.ToString();
                    item["Site_x0020_Page"] = sitePageUrl;

                    //Report Title Page hyperlink field
                    reportTitleUrl.Url = string.Format("{0}/SitePages/IMVReport.aspx?Site={1}&ReportId={2}", SPContext.Current.Web.Url, SiteNo, item.ID);
                    reportTitleUrl.Description = txtTitle.Text;
                    item["Report_x0020_Title"] = reportTitleUrl;

                    //Save the first Visit Attendees section as well.
                    item["InvestigatorName"] = txtInvestigatorName.Text;
                    item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
                    item["SitePersonnelName"] = txtSitePersonnelName.Text;
                    item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;
                    item["SitePersonnelName2"] = txtSitePersonnelName2.Text;
                    item["SitePersonnelTitle2"] = txtSitePersonnelTitle2.Text;
                    item["SitePersonnelName3"] = txtSitePersonnelName3.Text;
                    item["SitePersonnelTitle3"] = txtSitePersonnelTitle3.Text;

                    item["MonitorName"] = txtMonitorName.Text;
                    item["MonitorTitle"] = txtMonitorTitle.Text;

                    //Site Personnel
                    item["Intro_x002e_Clinical_x0020_Site_"] = ddlSitePersonnel.SelectedValue;
                    item["Intro_x002e_Clinical_x0020_Site_0"] = txtSitePersonnelComments.Text;

                    //Personnnel
                    item["Intro_x002e_Other_x0020_Personne0"] = ddlPersonnel.SelectedValue;
                    item["Intro_x002e_Other_x0020_Personne"] = txtPersonnelComments.Text;

                    item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

                    //Save changes
                    item.Update();

                    web.AllowUnsafeUpdates = false;
                    item.Web.AllowUnsafeUpdates = false;

                    //Save current tab
                    switch (int.Parse(formMenu.SelectedValue))
                    {
                        case (int)Utilities.IMV_TABS.Visit_Attendees:
                            SaveVisitAttendees();
                            break;

                        case (int)Utilities.IMV_TABS.Subject_Recruitment:
                            SaveSectionC();
                            break;

                        case (int)Utilities.IMV_TABS.Informed_Consent:
                            SaveSectionD();
                            break;

                        case (int)Utilities.IMV_TABS.Documentation:
                            SaveSectionE();
                            break;

                        case (int)Utilities.IMV_TABS.Adverse_Events:
                            SaveSectionG();
                            break;

                        case (int)Utilities.IMV_TABS.Site_File:
                            SaveSectionH();
                            break;

                        case (int)Utilities.IMV_TABS.Supplies:
                            SaveSectionI();
                            break;

                        case (int)Utilities.IMV_TABS.Laboratory:
                            SaveSectionJ();
                            break;

                        case (int)Utilities.IMV_TABS.SiteStaffChanges:
                            SaveSectionK();
                            break;

                        case (int)Utilities.IMV_TABS.SiteAcceptability:
                            SaveSectionL();
                            break;

                        case (int)Utilities.IMV_TABS.Discussion:
                            SaveSectionM();
                            break;
                    }

                    
                    //Visit Attendees Section
                    //item["InvestigatorName"] = txtInvestigatorName.Text;
                    //item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
                    //item["SitePersonnelName"] = txtSitePersonnelName.Text;
                    //item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;
                    //item["SitePersonnelName2"] = txtSitePersonnelName2.Text;
                    //item["SitePersonnelTitle2"] = txtSitePersonnelTitle2.Text;
                    //item["SitePersonnelName3"] = txtSitePersonnelName3.Text;
                    //item["SitePersonnelTitle3"] = txtSitePersonnelTitle3.Text;

                    //item["MonitorName"] = txtMonitorName.Text;
                    //item["MonitorTitle"] = txtMonitorTitle.Text;

                    ////Site Personnel
                    //item["Intro_x002e_Clinical_x0020_Site_"] = ddlSitePersonnel.SelectedValue;
                    //item["Intro_x002e_Clinical_x0020_Site_0"] = txtSitePersonnelComments.Text;

                    ////Personnnel
                    //item["Intro_x002e_Other_x0020_Personne0"] = ddlPersonnel.SelectedValue;
                    //item["Intro_x002e_Other_x0020_Personne"] = txtPersonnelComments.Text;

                    //item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

                    //Save C Section
                    //item["C_x002e_1"] = ddlC1.SelectedValue;
                    //item["C_x002e_2"] = ddlC2.SelectedValue;
                    //item["C_x002e_3"] = ddlC3.SelectedValue;

                    //item["C_x002e_1_x0020_Comments"] = txtC1Comments.Text;
                    //item["C_x002e_2_x0020_Comments"] = txtC2Comments.Text;
                    //item["C_x002e_3_x0020_Comments"] = txtC3Comments.Text;

                    //item["C_x002e_Reviewer"] = txtCReviewerComments.Text;

                    //item["Number_x0020_Screened"] = txtNoSceened.Text;
                    //item["Number_x0020_Screened_x0020_Fail"] = txtNoScreenedFailed.Text;
                    //item["Number_x0020_Randomized"] = txtNoRandomized.Text;
                    //item["Number_x0020_Active_x0020_Treatm"] = txtNoActiveTreatment.Text;
                    //item["Number_x0020_Completed_x0020_Tre"] = txtNoCompletedTreatment.Text;
                    //item["Number_x0020_Discontinuation"] = txtNoDiscontinuationo.Text;

                    //Save D Section
                    //item["D_x002e_1"] = ddlD1.SelectedValue;
                    //item["D_x002e_1_x0020_Comments"] = txtD1Comments.Text;
                    //item["D_x002e_Reviewer"] = txtDReviewerComments.Text;

                    //Save E Section
                    //item["E_x002e_1"] = ddlE1.SelectedValue;
                    //item["E_x002e_2"] = ddlE2.SelectedValue;
                    //item["E_x002e_3"] = ddlE3.SelectedValue;
                    //item["E_x002e_4"] = ddlE4.SelectedValue;

                    //item["E_x002e_1_x0020_Comments"] = txtE1Comments.Text;
                    //item["E_x002e_2_x0020_Comments"] = txtE2Comments.Text;
                    //item["E_x002e_3_x0020_Comments"] = txtE3Comments.Text;
                    //item["E_x002e_4_x0020_Comments"] = txtE4Comments.Text;

                    //item["E_x002e_Reviewer"] = txtEReviewerComments.Text;

                    //Save G Section
                    //item["G_x002e_1"] = ddlG1.SelectedValue;
                    //item["G_x002e_2"] = ddlG2.SelectedValue;

                    //item["G_x002e_1_x0020_Comments"] = txtG1Comments.Text;
                    //item["G_x002e_2_x0020_Comments"] = txtG2Comments.Text;

                    //item["G_x002e_Reviewer"] = txtGReviewerComments.Text;

                    //Save H Section
                    //item["H_x002e_1"] = ddlH1.SelectedValue;
                    //item["H_x002e_1_x0020_Comments"] = txtH1Comments.Text;
                    //item["H_x002e_Reviewer"] = txtHReviewerComments.Text;

                    ////Site Visit Log Date
                    //if (!calSiteVisitLogDate.IsDateEmpty)
                    //    item["h_x002e_siteVisitLogDate"] = calSiteVisitLogDate.SelectedDate;
                    //else
                    //    item["h_x002e_siteVisitLogDate"] = null;

                    ////Protocol Date
                    //if (!calProtocolDate.IsDateEmpty)
                    //    item["h_x002e_protocolDate"] = calProtocolDate.SelectedDate;
                    //else
                    //    item["h_x002e_protocolDate"] = null;

                    ////Brochure Date
                    //if (!calBrochureDate.IsDateEmpty)
                    //    item["h_x002e_brochureDate"] = calBrochureDate.SelectedDate;
                    //else
                    //    item["h_x002e_brochureDate"] = null;

                    ////Disclosure Date
                    //if (!calDisclosureDate.IsDateEmpty)
                    //    item["h_x002e_disclosureDate"] = calDisclosureDate.SelectedDate;
                    //else
                    //    item["h_x002e_disclosureDate"] = null;

                    ////License Date
                    //if (!calLicenseDate.IsDateEmpty)
                    //    item["h_x002e_licenseDate"] = calLicenseDate.SelectedDate;
                    //else
                    //    item["h_x002e_licenseDate"] = null;

                    ////Regulatory Date
                    //if (!calRegulatoryDate.IsDateEmpty)
                    //    item["h_x002e_regulatoryDate"] = calRegulatoryDate.SelectedDate;
                    //else
                    //    item["h_x002e_regulatoryDate"] = null;

                    ////Versions Date
                    //if (!calVersionsDate.IsDateEmpty)
                    //    item["h_x002e_versionsDate"] = calVersionsDate.SelectedDate;
                    //else
                    //    item["h_x002e_versionsDate"] = null;

                    ////Certification Date
                    //if (!calCertificationDate.IsDateEmpty)
                    //    item["h_x002e_certificationDate"] = calCertificationDate.SelectedDate;
                    //else
                    //    item["h_x002e_certificationDate"] = null;

                    ////Correspondence date
                    //if (!calCorrespondenceDate.IsDateEmpty)
                    //    item["h_x002e_correspondenceDate"] = calCorrespondenceDate.SelectedDate;
                    //else
                    //    item["h_x002e_correspondenceDate"] = null;

                    ////Misc Date                  
                    //if (!calMiscellaneousDate.IsDateEmpty)
                    //    item["h_x002e_miscDate"] = calMiscellaneousDate.SelectedDate;
                    //else
                    //    item["h_x002e_miscDate"] = null;

                    ////Site File Comments Fields
                    //item["h_x002e_siteVisitLogComments"] = txtSiteVisitLogComments.Text;
                    //item["h_x002e_protocolComments"] = txtProtocolComments.Text;
                    //item["h_x002e_disclosureComments"] = txtDisclosureComments.Text;
                    //item["h_x002e_brochureComments"] = txtBrochureComments.Text;
                    //item["h_x002e_licenseComments"] = txtLicenseComments.Text;
                    //item["h_x002e_regulatoryComments"] = txtRegulatoryComments.Text;
                    //item["h_x002e_versionsComments"] = txtVersionsComments.Text;
                    //item["h_x002e_certificationComments"] = txtCertificationComments.Text;
                    //item["h_x002e_correspondenceComments"] = txtCorrespondenceComments.Text;
                    //item["h_x002e_miscComments"] = txtMiscellaneousComments.Text;

                    //Save I Section
                    //item["I_x002e_1"] = ddlI1.SelectedValue;
                    //item["I_x002e_2"] = ddlI2.SelectedValue;
                    //item["I_x002e_3"] = ddlI3.SelectedValue;
                    //item["I_x002e_4"] = ddlI4.SelectedValue;
                    //item["I_x002e_5"] = ddlI5.SelectedValue;
                    //item["I_x002e_6"] = ddlI6.SelectedValue;
                    //item["I_x002e_7"] = ddlI7.SelectedValue;
                    //item["I_x002e_8"] = ddlI8.SelectedValue;
                    //item["I_x002e_9"] = ddlI9.SelectedValue;
                    //item["I_x002e_10"] = ddlI10.SelectedValue;
                    //item["I_x002e_11"] = ddlI11.SelectedValue;
                    //item["I_x002e_12"] = ddlI12.SelectedValue;
                    //item["I_x002e_13"] = ddlI13.SelectedValue;

                    //item["I_x002e_1_x0020_Comments"] = txtI1Comments.Text;
                    //item["I_x002e_2_x0020_Comments"] = txtI2Comments.Text;
                    //item["I_x002e_3_x0020_Comments"] = txtI3Comments.Text;
                    //item["I_x002e_4_x0020_Comments"] = txtI4Comments.Text;

                    //item["I_x002e_5_x0020_Comments"] = txtI5Comments.Text;
                    //item["I_x002e_6_x0020_Comments"] = txtI6Comments.Text;
                    //item["I_x002e_7_x0020_Comments"] = txtI7Comments.Text;
                    //item["I_x002e_8_x0020_Comments"] = txtI8Comments.Text;

                    //item["I_x002e_9_x0020_Comments"] = txtI9Comments.Text;
                    //item["I_x002e_10_x0020_Comments"] = txtI10Comments.Text;
                    //item["I_x002e_11_x0020_Comments"] = txtI11Comments.Text;
                    //item["I_x002e_12_x0020_Comments"] = txtI12Comments.Text;
                    //item["I_x002e_13_x0020_Comments"] = txtI13Comments.Text;

                    //item["I_x002e_Reviewer"] = txtIReviewerComments.Text;

                    //Save J Section
                    //item["J_x002e_1"] = ddlJ1.SelectedValue;
                    //item["J_x002e_2"] = ddlJ2.SelectedValue;
                    //item["J_x002e_3"] = ddlJ3.SelectedValue;

                    //item["J_x002e_1_x0020_Comments"] = txtJ1Comments.Text;
                    //item["J_x002e_2_x0020_Comments"] = txtJ2Comments.Text;
                    //item["J_x002e_3_x0020_Comments"] = txtJ3Comments.Text;

                    //item["J_x002e_Reviewer"] = txtJReviewerComments.Text;

                    //Save K Section
                    //item["K_x002e_1"] = ddlK1.SelectedValue;
                    //item["K_x002e_2"] = ddlK2.SelectedValue;
                    //item["K_x002e_3"] = ddlK3.SelectedValue;
                    //item["K_x002e_4"] = ddlK3.SelectedValue;

                    //item["K_x002e_1_x0020_New_x0020_Site_x"] = txtNSF.Text;
                    //item["K_x002e_1_x0020_Discontinued_x00"] = txtDSP.Text;

                    //item["K_x002e_2_x0020_Comments"] = txtK2Comments.Text;
                    //item["K_x002e_3_x0020_Comments"] = txtK3Comments.Text;
                    //item["K_x002e_4_x0020_Comments"] = txtK4Comments.Text;

                    //item["K_x002e_Reviewer"] = txtKReviewerComments.Text;

                    //Save L Section
                    //item["L_x002e_1"] = ddlL1.SelectedValue;
                    //item["L_x002e_2"] = ddlL2.SelectedValue;

                    //item["L_x002e_1_x0020_Comments"] = txtL1Comments.Text;
                    //item["L_x002e_2_x0020_Comments"] = txtL2Comments.Text;

                    //item["L_x002e_Reviewer"] = txtLReviewerComments.Text;

                    //Save M Section
                    //item["M_x002e_1"] = ddlM1.SelectedValue;

                    //item["M_x002e_1_x0020_Comments"] = txtM1Comments.Text;

                    //item["M_x002e_Reviewer"] = txtMReviewerComments.Text;

                    //Save changes
                    //item.Update();

                    //web.AllowUnsafeUpdates = false;
                    //item.Web.AllowUnsafeUpdates = false;

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
            Guid webID = SPContext.Current.Web.ID;
            Guid siteID = SPContext.Current.Site.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(siteID))
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {

                        string pdfFileName = string.Empty;
                        string mainTemplateFileName = "C:\\HFA\\IMVReportTemplate-Main.html";
                        string signatureTemplateFileName = "C:\\HFA\\SignatureTemplate.html";

                        string convertedFileNameMain = "C:\\HFA\\temp\\IMVReportConverted-Main.html";
                        string convertedFileNameSignature = "C:\\HFA\\temp\\IMVReportConverted-Signature.html";

                        SPListItem report = GetIMVReportData(int.Parse(ReportId));
                        pdfFileName = string.Format("{0}.pdf", report.Title);

                        if (report == null)
                            return;

                        // Read in the contents of the Receipt.htm HTML template file
                        string mainHtml = File.ReadAllText(mainTemplateFileName).Replace("\r\n", string.Empty);

                        #region Tabs
                        //Header
                        mainHtml = mainHtml.Replace("varSponsor", Utilities.GetReportStringValue(report["Sponsor"]));
                        mainHtml = mainHtml.Replace("varHeaderProtocol", Utilities.GetReportStringValue(report["Protocol #"]));
                        mainHtml = mainHtml.Replace("varInc", Utilities.GetReportStringValue(report["Inc #"]));
                        mainHtml = mainHtml.Replace("varSiteNo", ReportId.ToString());
                        mainHtml = mainHtml.Replace("varStudySiteNumber", Utilities.GetLookupFieldValue(report["Site_x0020_Number"]));
                        mainHtml = mainHtml.Replace("varVisitDate", Utilities.GetShortDateValue(report["Visit Date"]));
                        mainHtml = mainHtml.Replace("varLastDayOfVisit", Utilities.GetShortDateValue(report["LastDayOfVisit"]));
                        mainHtml = mainHtml.Replace("varInvestigatorName", Utilities.GetReportStringValue(report["InvestigatorName"]));
                        mainHtml = mainHtml.Replace("varAddress", Utilities.GetReportMultiLineTextFieldValue(report, "Address"));

                        //Visit Attendees
                        //Comments
                        mainHtml = mainHtml.Replace("varSitePersonnelComments", Utilities.GetReportStringValue(report["Intro_x002e_Clinical_x0020_Site_0"]));
                        mainHtml = mainHtml.Replace("varPersonnelComments", Utilities.GetReportStringValue(report["Intro_x002e_Other_x0020_Personne"]));

                        mainHtml = mainHtml.Replace("varInvestigatorTitle", Utilities.GetReportStringValue(report["InvestigatorTitle"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelName1", Utilities.GetReportStringValue(report["SitePersonnelName"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelTitle1", Utilities.GetReportStringValue(report["SitePersonnelTitle"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelTitle2", Utilities.GetReportStringValue(report["SitePersonnelTitle2"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelName2", Utilities.GetReportStringValue(report["SitePersonnelName2"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelTitle3", Utilities.GetReportStringValue(report["SitePersonnelTitle3"]));
                        mainHtml = mainHtml.Replace("varSitePersonnelName3", Utilities.GetReportStringValue(report["SitePersonnelName3"]));

                        mainHtml = mainHtml.Replace("varSitePersonnel", Utilities.GetReportStringValue(report["Intro_x002e_Clinical_x0020_Site_"]));
                        mainHtml = mainHtml.Replace("varPersonnel", Utilities.GetReportStringValue(report["Intro_x002e_Other_x0020_Personne0"]));

                        mainHtml = mainHtml.Replace("varMonitorName", Utilities.GetReportStringValue(report["MonitorName"]));
                        mainHtml = mainHtml.Replace("varMonitorTitle", Utilities.GetReportStringValue(report["MonitorTitle"]));

                        //C Section
                        mainHtml = mainHtml.Replace("varC1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varC2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varC3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "C_x002e_3_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varC1", Utilities.GetDDLStringValue(report["C_x002e_1"]));
                        mainHtml = mainHtml.Replace("varC2", Utilities.GetDDLStringValue(report["C_x002e_2"]));
                        mainHtml = mainHtml.Replace("varC3", Utilities.GetDDLStringValue(report["C_x002e_3"]));

                        mainHtml = mainHtml.Replace("varNumberScreenedFailed", Utilities.GetReportStringValue(report["Number_x0020_Screened_x0020_Fail"]));
                        mainHtml = mainHtml.Replace("varNumberScreened", Utilities.GetReportStringValue(report["Number_x0020_Screened"]));
                        mainHtml = mainHtml.Replace("varNumberRandomized", Utilities.GetReportStringValue(report["Number_x0020_Randomized"]));
                        mainHtml = mainHtml.Replace("varNumberActiveTreatment", Utilities.GetReportStringValue(report["Number_x0020_Active_x0020_Treatm"]));
                        mainHtml = mainHtml.Replace("varNumberCompletedTreatment", Utilities.GetReportStringValue(report["Number_x0020_Completed_x0020_Tre"]));
                        mainHtml = mainHtml.Replace("varNumberDiscontinuation", Utilities.GetReportStringValue(report["Number_x0020_Discontinuation"]));

                        //D Section
                        mainHtml = mainHtml.Replace("varD1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "D_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varD1", Utilities.GetDDLStringValue(report["D_x002e_1"]));

                        //variables for printing lists columns
                        string subjectId = string.Empty;
                        string signedDate = string.Empty;
                        string description = string.Empty;
                        string comments = string.Empty;

                        string subjectsList = string.Empty;

                        //Write active issues
                        SPListItemCollection subjects = Queries.GetSubjects(SiteNo);
                        foreach (SPListItem subject in subjects)
                        {
                            subjectId = Utilities.GetStringValue(subject["Subject ID"]);

                            if (subject["ICF Signed Date"] != null)
                                signedDate = Convert.ToDateTime(subject["ICF Signed Date"]).ToShortDateString();
                            else
                                signedDate = string.Empty;

                            description = (subject["Version_x002f_Description"] != null) ? subject["Version_x002f_Description"].ToString() : string.Empty;
                            comments = Utilities.GetReportMultiLineTextFieldValue(subject, "Comments").Trim();

                            subjectsList += string.Format(Utilities.GetSubjectItemHTML(), subjectId, GetCellValue(signedDate), GetCellValue(description), GetCellValue(comments));
                        }

                        mainHtml = mainHtml.Replace("varSubjects", subjectsList);

                        //E Section
                        mainHtml = mainHtml.Replace("varE1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "E_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "E_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "E_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE4Comments", Utilities.GetMultiLineReportTextFieldValue(report, "E_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varE1", Utilities.GetDDLStringValue(report["E_x002e_1"]));
                        mainHtml = mainHtml.Replace("varE2", Utilities.GetDDLStringValue(report["E_x002e_2"]));
                        mainHtml = mainHtml.Replace("varE3", Utilities.GetDDLStringValue(report["E_x002e_3"]));
                        mainHtml = mainHtml.Replace("varE4", Utilities.GetDDLStringValue(report["E_x002e_4"]));

                        List<SubjectActivityTracker> satItems = SubjectActivityTracker.GetbyReportId(int.Parse(ReportId));
                        string satList = string.Empty;
                        string activityThisVisit = string.Empty;
                        //string droppedFromStudy = string.Empty;

                        foreach (SubjectActivityTracker sat in satItems)
                        {
                            subjectId = sat.SubjectID;
                            activityThisVisit = sat.ActivityThisVisit;
                            //droppedFromStudy = sat.DroppedFromStudy;
                           
                            satList += string.Format(Utilities.GetSATHTML(), GetCellValue(subjectId), GetCellValue(activityThisVisit));
                        }

                        mainHtml = mainHtml.Replace("varSATList", satList);

                        //G Section
                        mainHtml = mainHtml.Replace("varG1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "G_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varG2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "G_x002e_2_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varG1", Utilities.GetDDLStringValue(report["G_x002e_1"]));
                        mainHtml = mainHtml.Replace("varG2", Utilities.GetDDLStringValue(report["G_x002e_2"]));

                        //Additiona variables for printing SAE items
                        string onsetDate = string.Empty;
                        string saeDate = string.Empty;
                        string notifiedDate = string.Empty;
                        string IRBIECNotice = string.Empty;
                        string evetOutcome = string.Empty;

                        string saeList = string.Empty;

                        SiteInfo = new SiteList(SiteNo);
                        SPListItemCollection saeItems = Queries.GetSeriousAdverseEvents(SiteInfo.ID);

                        foreach (SPListItem sae in saeItems)
                        {
                            subjectId = Utilities.GetStringValue(sae["Subject ID"]);
                            description = sae.Title;
                            onsetDate = Utilities.GetShortDateValue(sae["Onset_x0020_Date"]);
                            saeDate = Utilities.GetShortDateValue(sae["SAE_x0020_Identification_x0020_D"]);
                            notifiedDate = Utilities.GetShortDateValue(sae["Date_x0020_Company_x0020_Notifie"]);
                            IRBIECNotice = Utilities.GetStringValue(sae["IRB_x0020__x002f__x0020_IEC_x002"]);
                            evetOutcome = Utilities.GetStringValue(sae["Event_x0020_Outcome"]);

                            saeList += string.Format(Utilities.GetSAEHTML(), GetCellValue(subjectId), GetCellValue(description), GetCellValue(onsetDate), GetCellValue(saeDate), GetCellValue(notifiedDate), GetCellValue(IRBIECNotice), GetCellValue(evetOutcome));
                        }

                        mainHtml = mainHtml.Replace("varSAE", saeList);


                        //H Section
                        mainHtml = mainHtml.Replace("varH1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "H_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varH1", Utilities.GetDDLStringValue(report["H_x002e_1"]));

                        //H Date fields
                        mainHtml = mainHtml.Replace("varSiteVisitLogDate", Utilities.GetShortDateValue(report["h_x002e_siteVisitLogDate"]));
                        mainHtml = mainHtml.Replace("varProtocolDate", Utilities.GetShortDateValue(report["h_x002e_protocolDate"]));
                        mainHtml = mainHtml.Replace("varBrochureDate", Utilities.GetShortDateValue(report["h_x002e_brochureDate"]));
                        mainHtml = mainHtml.Replace("varDisclosureDate", Utilities.GetShortDateValue(report["h_x002e_disclosureDate"]));
                        mainHtml = mainHtml.Replace("varLicenseDate", Utilities.GetShortDateValue(report["h_x002e_licenseDate"]));
                        mainHtml = mainHtml.Replace("varRegulatoryDate", Utilities.GetShortDateValue(report["h_x002e_regulatoryDate"]));
                        mainHtml = mainHtml.Replace("varVersionsDate", Utilities.GetShortDateValue(report["h_x002e_versionsDate"]));
                        mainHtml = mainHtml.Replace("varCertificationDate", Utilities.GetShortDateValue(report["h_x002e_certificationDate"]));
                        mainHtml = mainHtml.Replace("varCorrespondenceDate", Utilities.GetShortDateValue(report["h_x002e_correspondenceDate"]));
                        mainHtml = mainHtml.Replace("varMiscellaneousDate", Utilities.GetShortDateValue(report["h_x002e_miscDate"]));

                        //H Comments fiels
                        mainHtml = mainHtml.Replace("varSiteVisitLogComments", Utilities.GetReportStringValue(report["h_x002e_siteVisitLogComments"]));
                        mainHtml = mainHtml.Replace("varProtocolComments", Utilities.GetReportStringValue(report["h_x002e_protocolComments"]));
                        mainHtml = mainHtml.Replace("varBrochureComments", Utilities.GetReportStringValue(report["h_x002e_brochureComments"]));
                        mainHtml = mainHtml.Replace("varDisclosureComments", Utilities.GetReportStringValue(report["h_x002e_disclosureComments"]));
                        mainHtml = mainHtml.Replace("varLicenseComments", Utilities.GetReportStringValue(report["h_x002e_licenseComments"]));
                        mainHtml = mainHtml.Replace("varRegulatoryComments", Utilities.GetReportStringValue(report["h_x002e_regulatoryComments"]));
                        mainHtml = mainHtml.Replace("varVersionsComments", Utilities.GetReportStringValue(report["h_x002e_versionsComments"]));
                        mainHtml = mainHtml.Replace("varCertificationComments", Utilities.GetReportStringValue(report["h_x002e_certificationComments"]));
                        mainHtml = mainHtml.Replace("varCorrespondenceComments", Utilities.GetReportStringValue(report["h_x002e_correspondenceComments"]));
                        mainHtml = mainHtml.Replace("varMiscellaneousComments", Utilities.GetReportStringValue(report["h_x002e_miscComments"]));

                        //I Section
                        mainHtml = mainHtml.Replace("varI10Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_10_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI11Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_11_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI12Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_12_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI13Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_13_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varI1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI4Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_4_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI5Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_5_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI6Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_6_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI7Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_7_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI8Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_8_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varI9Comments", Utilities.GetMultiLineReportTextFieldValue(report, "I_x002e_9_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varI10", Utilities.GetDDLStringValue(report["I_x002e_10"]));
                        mainHtml = mainHtml.Replace("varI11", Utilities.GetDDLStringValue(report["I_x002e_11"]));
                        mainHtml = mainHtml.Replace("varI12", Utilities.GetDDLStringValue(report["I_x002e_12"]));
                        mainHtml = mainHtml.Replace("varI13", Utilities.GetDDLStringValue(report["I_x002e_13"]));
                        mainHtml = mainHtml.Replace("varI1", Utilities.GetDDLStringValue(report["I_x002e_1"]));
                        mainHtml = mainHtml.Replace("varI2", Utilities.GetDDLStringValue(report["I_x002e_2"]));
                        mainHtml = mainHtml.Replace("varI3", Utilities.GetDDLStringValue(report["I_x002e_3"]));
                        mainHtml = mainHtml.Replace("varI4", Utilities.GetDDLStringValue(report["I_x002e_4"]));
                        mainHtml = mainHtml.Replace("varI5", Utilities.GetDDLStringValue(report["I_x002e_5"]));
                        mainHtml = mainHtml.Replace("varI6", Utilities.GetDDLStringValue(report["I_x002e_6"]));
                        mainHtml = mainHtml.Replace("varI7", Utilities.GetDDLStringValue(report["I_x002e_7"]));
                        mainHtml = mainHtml.Replace("varI8", Utilities.GetDDLStringValue(report["I_x002e_8"]));
                        mainHtml = mainHtml.Replace("varI9", Utilities.GetDDLStringValue(report["I_x002e_9"]));

                        //J Section
                        mainHtml = mainHtml.Replace("varJ1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "J_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varJ2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "J_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varJ3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "J_x002e_3_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varJ1", Utilities.GetDDLStringValue(report["J_x002e_1"]));
                        mainHtml = mainHtml.Replace("varJ2", Utilities.GetDDLStringValue(report["J_x002e_2"]));
                        mainHtml = mainHtml.Replace("varJ3", Utilities.GetDDLStringValue(report["J_x002e_3"]));

                        //K Section
                        mainHtml = mainHtml.Replace("varK1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "K_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varK2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "K_x002e_2_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varK3Comments", Utilities.GetMultiLineReportTextFieldValue(report, "K_x002e_3_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varK4Comments", Utilities.GetMultiLineReportTextFieldValue(report, "K_x002e_4_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varK1", Utilities.GetDDLStringValue(report["K_x002e_1"]));
                        mainHtml = mainHtml.Replace("varK2", Utilities.GetDDLStringValue(report["K_x002e_2"]));
                        mainHtml = mainHtml.Replace("varK3", Utilities.GetDDLStringValue(report["K_x002e_3"]));
                        mainHtml = mainHtml.Replace("varK4", Utilities.GetDDLStringValue(report["K_x002e_4"]));

                        mainHtml = mainHtml.Replace("varK_NSP", Utilities.GetMultiLineReportTextFieldValue(report, "K_x002e_1_x0020_New_x0020_Site_x"));
                        mainHtml = mainHtml.Replace("varK_DSP", Utilities.GetMultiLineReportTextFieldValue(report, "K_x002e_1_x0020_Discontinued_x00"));

                        //L Section
                        mainHtml = mainHtml.Replace("varL1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "L_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varL2Comments", Utilities.GetMultiLineReportTextFieldValue(report, "L_x002e_2_x0020_Comments"));

                        mainHtml = mainHtml.Replace("varL1", Utilities.GetDDLStringValue(report["L_x002e_1"]));
                        mainHtml = mainHtml.Replace("varL2", Utilities.GetDDLStringValue(report["L_x002e_2"]));

                        //M Section
                        mainHtml = mainHtml.Replace("varM1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "M_x002e_1_x0020_Comments"));
                        mainHtml = mainHtml.Replace("varN1Comments", Utilities.GetMultiLineReportTextFieldValue(report, "N_x002e__x0020_Miscellaneous_x00"));

                        mainHtml = mainHtml.Replace("varM1", Utilities.GetDDLStringValue(report["M_x002e_1"]));

                        //Last section
                        mainHtml = mainHtml.Replace("varNextVisit", Utilities.GetShortDateValue(report["Next_x0020_Visit_x0020_Date"]));

                        #endregion

                        //Write main html file to disk
                        File.WriteAllText(convertedFileNameMain, mainHtml);

                        //signature page section
                        string signatureHtml = File.ReadAllText(signatureTemplateFileName).Replace("\r\n", string.Empty);
                        signatureHtml = signatureHtml.Replace("varVersionNumber", Utilities.GetReportStringValue(report["VersionNumber"]));
                        signatureHtml = signatureHtml.Replace("varVersionDate", Utilities.GetShortDateValue(report["VersionDate"]));

                        //Write signature html file to disk
                        File.WriteAllText(convertedFileNameSignature, signatureHtml);

                        string pdfFile = string.Format("C:\\HFA\\temp\\{0}", pdfFileName);

                        PDFConversion.CreateFinalPDF(convertedFileNameMain, convertedFileNameSignature, pdfFile);

                        SPFolder pdfLibrary = web.Folders[PDFDocumentLibrary];

                        //Upload PDF file to document library
                        if (!Utilities.FileExists(pdfLibrary, pdfFileName))
                        {
                            if (Utilities.UplodFileToDocLibrary(web, PDFDocumentLibrary, pdfFile, SiteNo, "IMV", WorkflowName))
                                lblMessage.Text = "PDF Report successfully created";
                        }
                        else
                            lblMessage.Text = "Report with the same name already exists";
                    }
                }
            });
        }

        protected string GetCellValue(string field)
        {
            return field.Length > 0 ? field : "&nbsp;";
        }

        protected void FillVisitAttendees()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            try
            {

                txtInvestigatorName.Text = Utilities.GetStringValue(item["InvestigatorName"]);
                txtInvestigatorTitle.Text = Utilities.GetStringValue(item["InvestigatorTitle"]);
                txtSitePersonnelName.Text = Utilities.GetStringValue(item["SitePersonnelName"]);
                txtSitePersonnelTitle.Text = Utilities.GetStringValue(item["SitePersonnelTitle"]);
                txtSitePersonnelName2.Text = Utilities.GetStringValue(item["SitePersonnelName2"]);
                txtSitePersonnelTitle2.Text = Utilities.GetStringValue(item["SitePersonnelTitle2"]);
                txtSitePersonnelName3.Text = Utilities.GetStringValue(item["SitePersonnelName3"]);
                txtSitePersonnelTitle3.Text = Utilities.GetStringValue(item["SitePersonnelTitle3"]);

                txtMonitorName.Text = Utilities.GetStringValue(item["MonitorName"]);
                txtMonitorTitle.Text = Utilities.GetStringValue(item["MonitorTitle"]);

                //Site Personnel
                ddlSitePersonnel.SelectedValue = Utilities.GetDDLStringValue(item["Intro_x002e_Clinical_x0020_Site_"]);
                txtSitePersonnelComments.Text = Utilities.GetMultiLineTextFieldValue(item, "Intro_x002e_Clinical_x0020_Site_0");

                //Personnnel
                ddlPersonnel.SelectedValue = Utilities.GetDDLStringValue(item["Intro_x002e_Other_x0020_Personne0"]);
                txtPersonnelComments.Text = Utilities.GetMultiLineTextFieldValue(item, "Intro_x002e_Other_x0020_Personne");

                txtVAReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "Visit_x002e_Reviewer");
            }
            catch { }

        }

        protected void FillSectionC()
        {
            //C Section
            if (ReportId.Equals("0")) return;

            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlC1.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_1"]);
                ddlC2.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_2"]);
                ddlC3.SelectedValue = Utilities.GetDDLStringValue(item["C_x002e_3"]);

                txtC1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_1_x0020_Comments");
                txtC2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_2_x0020_Comments");
                txtC3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "C_x002e_3_x0020_Comments");

                txtCReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "C.Reviewer");

                //Number fields
                txtNoSceened.Text = Utilities.GetStringValue(item["Number_x0020_Screened"]);
                txtNoScreenedFailed.Text = Utilities.GetStringValue(item["Number_x0020_Screened_x0020_Fail"]);
                txtNoRandomized.Text = Utilities.GetStringValue(item["Number_x0020_Randomized"]);
                txtNoActiveTreatment.Text = Utilities.GetStringValue(item["Number_x0020_Active_x0020_Treatm"]);
                txtNoCompletedTreatment.Text = Utilities.GetStringValue(item["Number_x0020_Completed_x0020_Tre"]);
                txtNoDiscontinuationo.Text = Utilities.GetStringValue(item["Number_x0020_Discontinuation"]);
            }
        }

        protected void FillSectionD()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlD1.SelectedValue = Utilities.GetDDLStringValue(item["D_x002e_1"]);

                txtD1Comments.Text = Utilities.GetStringValue(item["D_x002e_1_x0020_Comments"]);

                txtDReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "D.Reviewer");

                SPListItemCollection subjectItems = Queries.GetSubjects(SiteNo);

                DataTable dtSubjects = new DataTable();
                dtSubjects.Columns.Add("Subject ID", typeof(string));
                dtSubjects.Columns.Add("ICF Signed Date", typeof(string));
                dtSubjects.Columns.Add("Version Description", typeof(string));
                dtSubjects.Columns.Add("Comments", typeof(string));

                DataRow row;

                if (subjectItems.Count > 0)
                {
                    //Create row for each list item

                    foreach (SPListItem li in subjectItems)
                    {
                        row = dtSubjects.Rows.Add();
                        row["Subject ID"] = li.Title;

                        if (li["ICF Signed Date"] != null)
                            row["ICF Signed Date"] = Convert.ToDateTime(li["ICF Signed Date"]).ToShortDateString();

                        row["Version Description"] = Utilities.GetStringValue(li["Version/Description"]);
                        row["Comments"] = li["Comments"];
                    }

                    //Bind data to grid
                    gdvIC.DataSource = dtSubjects.DefaultView;
                    gdvIC.DataBind();
                }
            }
        }

        protected void FillSectionE()
        {
            if (ReportId.Equals("0")) return;

            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlE1.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_1"]);
                ddlE2.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_2"]);
                ddlE3.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_3"]);
                ddlE4.SelectedValue = Utilities.GetDDLStringValue(item["E_x002e_4"]);

                txtE1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_1_x0020_Comments");
                txtE2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_2_x0020_Comments");
                txtE3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_3_x0020_Comments");
                txtE4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_4_x0020_Comments");

                txtEReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "E_x002e_Reviewer");

                BindSubjectActivityTrackerList();
            }
        }

        //Serious Adverse Events
        protected void FillSectionG()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlG1.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_1"]);
                ddlG2.SelectedValue = Utilities.GetDDLStringValue(item["G_x002e_2"]);

                txtG1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_1_x0020_Comments");
                txtG2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "G_x002e_2_x0020_Comments");


                SPListItemCollection items = Queries.GetSeriousAdverseEvents(SiteInfo.ID);

                //Create data table
                DataTable dt = new DataTable();
                //dt.Columns.Add("ID", typeof(string));
                dt.Columns.Add("Subject ID", typeof(string));
                dt.Columns.Add("Description Comments", typeof(string));
                dt.Columns.Add("Onset Date", typeof(string));
                dt.Columns.Add("Date SAE Identified at Site", typeof(string));
                dt.Columns.Add("Date Company Notified", typeof(string));
                dt.Columns.Add("IRB / IEC Notified", typeof(string));
                dt.Columns.Add("Event Outcome", typeof(string));

                //Create row for each list item
                DataRow row;
                foreach (SPListItem li in items)
                {
                    row = dt.Rows.Add();
                    //row["ID"] = li.ID;
                    row["Subject ID"] = Utilities.GetStringValue(li["Subject_x0020_ID"]);
                    row["Description Comments"] = li.Title;
                    row["Onset Date"] = Utilities.GetShortDateValue(li["Onset_x0020_Date"]);
                    row["Date SAE Identified at Site"] = Utilities.GetShortDateValue(li["SAE_x0020_Identification_x0020_D"]);
                    row["Date Company Notified"] = Utilities.GetShortDateValue(li["Date_x0020_Company_x0020_Notifie"]);
                    row["IRB / IEC Notified"] = li["IRB_x0020__x002f__x0020_IEC_x002"].ToString();
                    row["Event Outcome"] = Utilities.GetStringValue(li["Event_x0020_Outcome"]);
                }

                //Bind data to grid
                gdvSAE.DataSource = dt.DefaultView;
                gdvSAE.DataBind();

                txtGReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "G.Reviewer");
            }
        }

        protected void FillSectionH()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlH1.SelectedValue = Utilities.GetDDLStringValue(item["H_x002e_1"]);

                txtH1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_1_x0020_Comments");

                txtHReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "H_x002e_Reviewer");

                if (item["h_x002e_siteVisitLogDate"] != null)
                    calSiteVisitLogDate.SelectedDate = Convert.ToDateTime(item["h_x002e_siteVisitLogDate"].ToString());
               
                txtSiteVisitLogComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_siteVisitLogComments");

                if (item["h_x002e_protocolDate"] != null)
                    calProtocolDate.SelectedDate = Convert.ToDateTime(item["h_x002e_protocolDate"].ToString());

                txtProtocolComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_protocolComments");

                if (item["h_x002e_brochureDate"] != null)
                    calBrochureDate.SelectedDate = Convert.ToDateTime(item["h_x002e_brochureDate"].ToString());

                txtBrochureComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_brochureComments");

                if (item["h_x002e_disclosureDate"] != null)
                    calDisclosureDate.SelectedDate = Convert.ToDateTime(item["h_x002e_disclosureDate"].ToString());

                txtDisclosureComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_disclosureComments");

                if (item["h_x002e_licenseDate"] != null)
                    calLicenseDate.SelectedDate = Convert.ToDateTime(item["h_x002e_licenseDate"].ToString());

                txtLicenseComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_licenseComments");

                if (item["h_x002e_regulatoryDate"] != null)
                    calRegulatoryDate.SelectedDate = Convert.ToDateTime(item["h_x002e_regulatoryDate"].ToString());

                txtRegulatoryComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_regulatoryComments");

                if (item["h_x002e_versionsDate"] != null)
                    calVersionsDate.SelectedDate = Convert.ToDateTime(item["h_x002e_versionsDate"].ToString());

                txtVersionsComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_versionsComments");

                if (item["h_x002e_certificationDate"] != null)
                    calCertificationDate.SelectedDate = Convert.ToDateTime(item["h_x002e_certificationDate"].ToString());

                txtCertificationComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_certificationComments");

                if (item["h_x002e_correspondenceDate"] != null)
                    calCorrespondenceDate.SelectedDate = Convert.ToDateTime(item["h_x002e_correspondenceDate"].ToString());

                txtCorrespondenceComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_correspondenceComments");

                if (item["h_x002e_miscDate"] != null)
                    calMiscellaneousDate.SelectedDate = Convert.ToDateTime(item["h_x002e_miscDate"].ToString());

                txtMiscellaneousComments.Text = Utilities.GetMultiLineTextFieldValue(item,"h_x002e_miscComments");
            }
        }

        protected void FillSectionI()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlI1.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_1"]);
                ddlI2.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_2"]);
                ddlI3.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_3"]);
                ddlI4.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_4"]);
                ddlI5.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_5"]);
                ddlI6.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_6"]);
                ddlI7.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_7"]);
                ddlI8.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_8"]);
                ddlI9.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_9"]);
                ddlI10.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_10"]);
                ddlI11.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_11"]);
                ddlI12.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_12"]);
                ddlI13.SelectedValue = Utilities.GetDDLStringValue(item["I_x002e_13"]);

                txtI1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_1_x0020_Comments");
                txtI2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_2_x0020_Comments");
                txtI3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_3_x0020_Comments");
                txtI4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_4_x0020_Comments");
                txtI5Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_5_x0020_Comments");
                txtI6Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_6_x0020_Comments");
                txtI7Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_7_x0020_Comments");
                txtI8Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_8_x0020_Comments");
                txtI9Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_9_x0020_Comments");
                txtI10Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_10_x0020_Comments");
                txtI11Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_11_x0020_Comments");
                txtI12Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_12_x0020_Comments");
                txtI13Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_13_x0020_Comments");

                txtIReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "I_x002e_Reviewer");
            }
        }

        protected void FillSectionJ()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlJ1.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_1"]);
                ddlJ2.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_2"]);
                ddlJ3.SelectedValue = Utilities.GetDDLStringValue(item["J_x002e_3"]);

                txtJ1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_1_x0020_Comments");
                txtJ2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_2_x0020_Comments");
                txtJ3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_3_x0020_Comments");

                txtJReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "J_x002e_Reviewer");
            }
        }

        protected void FillSectionK()
        {
            //K Section
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlK1.SelectedValue = Utilities.GetDDLStringValue(item["K_x002e_1"]);
                ddlK2.SelectedValue = Utilities.GetDDLStringValue(item["K_x002e_2"]);
                ddlK3.SelectedValue = Utilities.GetDDLStringValue(item["K_x002e_3"]);
                ddlK4.SelectedValue = Utilities.GetDDLStringValue(item["K_x002e_4"]);

                txtNSF.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_1_x0020_New_x0020_Site_x");
                txtDSP.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_1_x0020_Discontinued_x00");
                txtK2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_2_x0020_Comments");
                txtK3Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_3_x0020_Comments");
                txtK4Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_4_x0020_Comments");

                txtKReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "K_x002e_Reviewer");
            }
        }

        protected void FillSectionL()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlL1.SelectedValue = Utilities.GetDDLStringValue(item["L_x002e_1"]);
                ddlL2.SelectedValue = Utilities.GetDDLStringValue(item["L_x002e_2"]);

                txtL1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_1_x0020_Comments");
                txtL2Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_2_x0020_Comments");

                txtLReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "L_x002e_Reviewer");
            }
        }

        protected void FillSectionM()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                ddlM1.SelectedValue = Utilities.GetDDLStringValue(item["M_x002e_1"]);

                txtM1Comments.Text = Utilities.GetMultiLineTextFieldValue(item, "M_x002e_1_x0020_Comments");

                txtMReviewerComments.Text = Utilities.GetMultiLineTextFieldValue(item, "M_x002e_Reviewer");
            }
        }

        protected void BindSubjectActivityTrackerList()
        {
            List<SubjectActivityTracker> satList = SubjectActivityTracker.GetbyReportId(int.Parse(ReportId));

            //Bind data to grid
            gdvSAT.DataSource = satList;
            gdvSAT.DataBind();

            lblMessage.Text = string.Empty;

            //if (satList.Count == 0)
            //    lblMessage.Text = "No result found that matched the search criteria!";

            BindSubjectDDL();
        }

        protected void BindSubjectDDL()
        {
            ddlSubjectID.Items.Clear();

            Dictionary<string, string> divisions = SubjectActivityTracker.GetSubjects(int.Parse(hidSiteId.Value));

            foreach (var div in divisions)
            {
                ddlSubjectID.Items.Add(new ListItem(div.Value, div.Key));
            }

            ddlSubjectID.DataBind();
            ddlSubjectID.Items.Insert(0,new ListItem(string.Empty, "0"));
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

            if (idx == 5)
                FillSectionH();

            switch (PreviousTabIndex)
            {
                case (int)Utilities.IMV_TABS.Visit_Attendees:
                    SaveVisitAttendees();
                    break;

                case (int)Utilities.IMV_TABS.Subject_Recruitment:
                    SaveSectionC();
                    break;

                case (int)Utilities.IMV_TABS.Informed_Consent:
                    SaveSectionD();
                    break;

                case (int)Utilities.IMV_TABS.Documentation:
                    SaveSectionE();
                    break;

                case (int)Utilities.IMV_TABS.Adverse_Events:
                    SaveSectionG();
                    break;

                case (int)Utilities.IMV_TABS.Site_File:
                    SaveSectionH();
                    break;

                case (int)Utilities.IMV_TABS.Supplies:
                    SaveSectionI();
                    break;

                case (int)Utilities.IMV_TABS.Laboratory:
                    SaveSectionJ();
                    break;

                case (int)Utilities.IMV_TABS.SiteStaffChanges:
                    SaveSectionK();
                    break;

                case (int)Utilities.IMV_TABS.SiteAcceptability:
                    SaveSectionL();
                    break;

                case (int)Utilities.IMV_TABS.Discussion:
                    SaveSectionM();
                    break;
            }

            //Save new PreviousTabIndex
            PreviousTabIndex = idx;
        }

        protected void SaveVisitAttendees()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            if (item != null)
            {
                item.Web.AllowUnsafeUpdates = true;

                //Visit Attendees
                item["InvestigatorName"] = txtInvestigatorName.Text;
                item["InvestigatorTitle"] = txtInvestigatorTitle.Text;
                item["SitePersonnelName"] = txtSitePersonnelName.Text;
                item["SitePersonnelTitle"] = txtSitePersonnelTitle.Text;
                item["SitePersonnelName2"] = txtSitePersonnelName2.Text;
                item["SitePersonnelTitle2"] = txtSitePersonnelTitle2.Text;
                item["SitePersonnelName3"] = txtSitePersonnelName3.Text;
                item["SitePersonnelTitle3"] = txtSitePersonnelTitle3.Text;

                item["MonitorName"] = txtMonitorName.Text;
                item["MonitorTitle"] = txtMonitorTitle.Text;

                //Site Personnel
                item["Intro_x002e_Clinical_x0020_Site_"] = ddlSitePersonnel.SelectedValue;
                item["Intro_x002e_Clinical_x0020_Site_0"] = txtSitePersonnelComments.Text;

                //Personnnel
                item["Intro_x002e_Other_x0020_Personne0"] = ddlPersonnel.SelectedValue;
                item["Intro_x002e_Other_x0020_Personne"] = txtPersonnelComments.Text;

                item["Visit_x002e_Reviewer"] = txtVAReviewerComments.Text;

                item.Update();
                item.Web.AllowUnsafeUpdates = false;
            }
        }

        protected SPListItem GetIMVReportData(int reportId)
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
                        SPList list = web.Lists["IMV Report"];
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

        protected SPListItem GetIMVReportData(SPWeb web, int reportId)
        {
            if (reportId == 0) return null;

            SPListItemCollection listItems = null;

            SPList list = web.Lists["IMV Report"];
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

        protected void SaveSectionM()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item["M_x002e_1"] = ddlM1.SelectedValue;

            item["M_x002e_1_x0020_Comments"] = txtM1Comments.Text;

            item["M_x002e_Reviewer"] = txtMReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionL()
        {

            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item["L_x002e_1"] = ddlL1.SelectedValue;
            item["L_x002e_2"] = ddlL2.SelectedValue;

            item["L_x002e_1_x0020_Comments"] = txtL1Comments.Text;
            item["L_x002e_2_x0020_Comments"] = txtL2Comments.Text;

            item["L_x002e_Reviewer"] = txtLReviewerComments.Text;

            //Save M Section
            item["M_x002e_1"] = ddlM1.SelectedValue;

            item["M_x002e_1_x0020_Comments"] = txtM1Comments.Text;

            item["M_x002e_Reviewer"] = txtMReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionK()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item["K_x002e_1"] = ddlK1.SelectedValue;
            item["K_x002e_2"] = ddlK2.SelectedValue;
            item["K_x002e_3"] = ddlK3.SelectedValue;
            item["K_x002e_4"] = ddlK3.SelectedValue;

            item["K_x002e_1_x0020_New_x0020_Site_x"] = txtNSF.Text;
            item["K_x002e_1_x0020_Discontinued_x00"] = txtDSP.Text;

            item["K_x002e_2_x0020_Comments"] = txtK2Comments.Text;
            item["K_x002e_3_x0020_Comments"] = txtK3Comments.Text;
            item["K_x002e_4_x0020_Comments"] = txtK4Comments.Text;

            item["K_x002e_Reviewer"] = txtKReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionJ()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item["J_x002e_1"] = ddlJ1.SelectedValue;
            item["J_x002e_2"] = ddlJ2.SelectedValue;
            item["J_x002e_3"] = ddlJ3.SelectedValue;

            item["J_x002e_1_x0020_Comments"] = txtJ1Comments.Text;
            item["J_x002e_2_x0020_Comments"] = txtJ2Comments.Text;
            item["J_x002e_3_x0020_Comments"] = txtJ3Comments.Text;

            item["J_x002e_Reviewer"] = txtJReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionI()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item["I_x002e_1"] = ddlI1.SelectedValue;
            item["I_x002e_2"] = ddlI2.SelectedValue;
            item["I_x002e_3"] = ddlI3.SelectedValue;
            item["I_x002e_4"] = ddlI4.SelectedValue;
            item["I_x002e_5"] = ddlI5.SelectedValue;
            item["I_x002e_6"] = ddlI6.SelectedValue;
            item["I_x002e_7"] = ddlI7.SelectedValue;
            item["I_x002e_8"] = ddlI8.SelectedValue;
            item["I_x002e_9"] = ddlI9.SelectedValue;
            item["I_x002e_10"] = ddlI10.SelectedValue;
            item["I_x002e_11"] = ddlI11.SelectedValue;
            item["I_x002e_12"] = ddlI12.SelectedValue;
            item["I_x002e_13"] = ddlI13.SelectedValue;

            item["I_x002e_1_x0020_Comments"] = txtI1Comments.Text;
            item["I_x002e_2_x0020_Comments"] = txtI2Comments.Text;
            item["I_x002e_3_x0020_Comments"] = txtI3Comments.Text;
            item["I_x002e_4_x0020_Comments"] = txtI4Comments.Text;

            item["I_x002e_5_x0020_Comments"] = txtI5Comments.Text;
            item["I_x002e_6_x0020_Comments"] = txtI6Comments.Text;
            item["I_x002e_7_x0020_Comments"] = txtI7Comments.Text;
            item["I_x002e_8_x0020_Comments"] = txtI8Comments.Text;

            item["I_x002e_9_x0020_Comments"] = txtI9Comments.Text;
            item["I_x002e_10_x0020_Comments"] = txtI10Comments.Text;
            item["I_x002e_11_x0020_Comments"] = txtI11Comments.Text;
            item["I_x002e_12_x0020_Comments"] = txtI12Comments.Text;
            item["I_x002e_13_x0020_Comments"] = txtI13Comments.Text;

            item["I_x002e_Reviewer"] = txtIReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionH()
        {
            string errorMsg = string.Empty;

            if (!calSiteVisitLogDate.IsDateEmpty && !calSiteVisitLogDate.IsValid)
            {
                errorMsg += "Please enter a valid Site Visit Log Date" + "<br/>";
            }

            if (!calProtocolDate.IsDateEmpty && !calProtocolDate.IsValid)
            {
                errorMsg += "Please enter a valid Protocol date" + "<br/>";
            }

            if (!calBrochureDate.IsDateEmpty && !calBrochureDate.IsValid)
            {
                errorMsg += "Please enter a valid Brochure date" + "<br/>";
            }

            if (!calDisclosureDate.IsDateEmpty && !calDisclosureDate.IsValid)
            {
                errorMsg += "Please enter a valid Disclosure date" + "<br/>";
            }

            if (!calLicenseDate.IsDateEmpty && !calLicenseDate.IsValid)
            {
                errorMsg += "Please enter a valid License date" + "<br/>";
            }

            if (!calRegulatoryDate.IsDateEmpty && !calRegulatoryDate.IsValid)
            {
                errorMsg += "Please enter a valid Regulatory date" + "<br/>";
            }

            if (!calVersionDate.IsDateEmpty && !calVersionDate.IsValid)
            {
                errorMsg += "Please enter a valid Version date" + "<br/>";
            }

            if (!calCertificationDate.IsDateEmpty && !calCertificationDate.IsValid)
            {
                errorMsg = "Please enter a valid Certification date" + "<br/>";
            }

            if (!calCorrespondenceDate.IsDateEmpty && !calCorrespondenceDate.IsValid)
            {
                errorMsg += "Please enter a valid Correspondence date" + "<br/>";
            }

            if (!calMiscellaneousDate.IsDateEmpty && !calMiscellaneousDate.IsValid)
            {
                errorMsg += "Please enter a valid Miscellaneous date" + "<br/>";
            }

            if (errorMsg.Length > 0)
            {
                lblMessage.Text = errorMsg;
                return;
            }

            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item["H_x002e_1"] = ddlH1.SelectedValue;
            item["H_x002e_1_x0020_Comments"] = txtH1Comments.Text;
            item["H_x002e_Reviewer"] = txtHReviewerComments.Text;

            //Site Visit Log Date
            if (!calSiteVisitLogDate.IsDateEmpty)
                item["h_x002e_siteVisitLogDate"] = calSiteVisitLogDate.SelectedDate;
            else
                item["h_x002e_siteVisitLogDate"] = null;

            //Protocol Date
            if (!calProtocolDate.IsDateEmpty)
                item["h_x002e_protocolDate"] = calProtocolDate.SelectedDate;
            else
                item["h_x002e_protocolDate"] = null;

            //Brochure Date
            if (!calBrochureDate.IsDateEmpty)
                item["h_x002e_brochureDate"] = calBrochureDate.SelectedDate;
            else
                item["h_x002e_brochureDate"] = null;

            //Disclosure Date
            if (!calDisclosureDate.IsDateEmpty)
                item["h_x002e_disclosureDate"] = calDisclosureDate.SelectedDate;
            else
                item["h_x002e_disclosureDate"] = null;

            //License Date
            if (!calLicenseDate.IsDateEmpty)
                item["h_x002e_licenseDate"] = calLicenseDate.SelectedDate;
            else
                item["h_x002e_licenseDate"] = null;

            //Regulatory Date
            if (!calRegulatoryDate.IsDateEmpty)
                item["h_x002e_regulatoryDate"] = calRegulatoryDate.SelectedDate;
            else
                item["h_x002e_regulatoryDate"] = null;

            //Versions Date
            if (!calVersionsDate.IsDateEmpty)
                item["h_x002e_versionsDate"] = calVersionsDate.SelectedDate;
            else
                item["h_x002e_versionsDate"] = null;

            //Certification Date
            if (!calCertificationDate.IsDateEmpty)
                item["h_x002e_certificationDate"] = calCertificationDate.SelectedDate;
            else
                item["h_x002e_certificationDate"] = null;

            //Correspondence date
            if (!calCorrespondenceDate.IsDateEmpty)
                item["h_x002e_correspondenceDate"] = calCorrespondenceDate.SelectedDate;
            else
                item["h_x002e_correspondenceDate"] = null;

            //Misc Date                  
            if (!calMiscellaneousDate.IsDateEmpty)
                item["h_x002e_miscDate"] = calMiscellaneousDate.SelectedDate;
            else
                item["h_x002e_miscDate"] = null;

            //Site File Comments Fields
            item["h_x002e_siteVisitLogComments"] = txtSiteVisitLogComments.Text;
            item["h_x002e_protocolComments"] = txtProtocolComments.Text;
            item["h_x002e_disclosureComments"] = txtDisclosureComments.Text;
            item["h_x002e_brochureComments"] = txtBrochureComments.Text;
            item["h_x002e_licenseComments"] = txtLicenseComments.Text;
            item["h_x002e_regulatoryComments"] = txtRegulatoryComments.Text;
            item["h_x002e_versionsComments"] = txtVersionsComments.Text;
            item["h_x002e_certificationComments"] = txtCertificationComments.Text;
            item["h_x002e_correspondenceComments"] = txtCorrespondenceComments.Text;
            item["h_x002e_miscComments"] = txtMiscellaneousComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionG()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item.Web.AllowUnsafeUpdates = true;

            item["G_x002e_1"] = ddlG1.SelectedValue;
            item["G_x002e_2"] = ddlG2.SelectedValue;

            item["G_x002e_1_x0020_Comments"] = txtG1Comments.Text;
            item["G_x002e_2_x0020_Comments"] = txtG2Comments.Text;

            item["G_x002e_Reviewer"] = txtGReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionE()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item.Web.AllowUnsafeUpdates = true;

            item["E_x002e_1"] = ddlE1.SelectedValue;
            item["E_x002e_2"] = ddlE2.SelectedValue;
            item["E_x002e_3"] = ddlE3.SelectedValue;
            item["E_x002e_4"] = ddlE4.SelectedValue;

            item["E_x002e_1_x0020_Comments"] = txtE1Comments.Text;
            item["E_x002e_2_x0020_Comments"] = txtE2Comments.Text;
            item["E_x002e_3_x0020_Comments"] = txtE3Comments.Text;
            item["E_x002e_4_x0020_Comments"] = txtE4Comments.Text;

            item["E_x002e_Reviewer"] = txtEReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionD()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            item.Web.AllowUnsafeUpdates = true;

            item["D_x002e_1"] = ddlD1.SelectedValue;
            item["D_x002e_1_x0020_Comments"] = txtD1Comments.Text;
            item["D_x002e_Reviewer"] = txtDReviewerComments.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void SaveSectionC()
        {
            SPListItem item = GetIMVReportData(int.Parse(ReportId));

            item.Web.AllowUnsafeUpdates = true;

            //General Info
            item["C_x002e_1"] = ddlC1.SelectedValue;
            item["C_x002e_2"] = ddlC2.SelectedValue;
            item["C_x002e_3"] = ddlC3.SelectedValue;

            item["C_x002e_1_x0020_Comments"] = txtC1Comments.Text;
            item["C_x002e_2_x0020_Comments"] = txtC2Comments.Text;
            item["C_x002e_3_x0020_Comments"] = txtC3Comments.Text;

            item["C_x002e_Reviewer"] = txtCReviewerComments.Text;

            item["Number_x0020_Screened"] = txtNoSceened.Text;
            item["Number_x0020_Screened_x0020_Fail"] = txtNoScreenedFailed.Text;
            item["Number_x0020_Randomized"] = txtNoRandomized.Text;
            item["Number_x0020_Active_x0020_Treatm"] = txtNoActiveTreatment.Text;
            item["Number_x0020_Completed_x0020_Tre"] = txtNoCompletedTreatment.Text;
            item["Number_x0020_Discontinuation"] = txtNoDiscontinuationo.Text;

            item.Update();
            item.Web.AllowUnsafeUpdates = false;
        }

        protected void gdvSAT_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //SubjectActivityTracker satItem = (SubjectActivityTracker)e.Row.DataItem;

                //Add delete confirmation
                LinkButton btnDelete = (LinkButton)e.Row.Cells[4].Controls[0];
                btnDelete.OnClientClick = "if (!confirm('Are you sure you want to delete?')) {return false;}"; 
            }
        }

        protected void gdvSAT_OnRowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Page" || e.CommandName.Length == 0 || e.CommandName == "Sort") return;

            int idx = Convert.ToInt16(e.CommandArgument);
            hidSelectedId.Value = gdvSAT.DataKeys[idx].Value.ToString();

            hidFormMode.Value = e.CommandName;

            if (e.CommandName == "Edit")
            {
                FillSubjectActivityTracker(int.Parse(hidSelectedId.Value));
            }
            else if (e.CommandName == "Delete")
            {
                SubjectActivityTracker.Delete(int.Parse(hidSelectedId.Value));
                BindSubjectActivityTrackerList();
            }
        }

        protected void gdvSAT_RowEditing(object sender, GridViewEditEventArgs e)
        { }

        protected void gdvSAT_RowDeleting(object sender, GridViewDeleteEventArgs e)
        { }

        protected void gdvSAT_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gdvSAT.DataSource = null;
            gdvSAT.DataBind();

            BindSubjectActivityTrackerList();

            gdvSAT.PageIndex = e.NewPageIndex;
            gdvSAT.DataBind();

        }

        protected void FillSubjectActivityTracker(int itemID)
        {
            BindSubjectDDL();

            SubjectActivityTracker sat = new SubjectActivityTracker(itemID);
            ddlSubjectID.SelectedValue = ddlSubjectID.Items.FindByValue(sat.Subject.LookupId.ToString()).Value;
            lblReprtType.Text = sat.ReportType;
            txtActivityThisVisit.Text = sat.ActivityThisVisit;
            //txtDroppedFromStudy.Text = sat.DroppedFromStudy;

            divDetail.Visible = true;
            divGrid.Visible = false;

            lblHeading.Text = "Update Subject Actitivity Tracker";
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            hidFormMode.Value = "Add";
            InitializeSAT();
            divDetail.Visible = true;
            lblHeading.Text = "Add new Subject Activity Tracker";
        }

        protected void btnCancelSAT_Click(object sender, EventArgs e)
        {
            divDetail.Visible = false;
            divGrid.Visible = true;

            InitializeSAT();
        }

        protected void btnSaveSAT_Click(object sender, EventArgs e)
        {
            SubjectActivityTracker sat = null;

            if (hidFormMode.Value == "Edit")
            {
                sat = new SubjectActivityTracker(int.Parse(hidSelectedId.Value));
                sat.ID = int.Parse(hidSelectedId.Value);
            }
            else if (hidFormMode.Value == "Add")
            {
                sat = new SubjectActivityTracker();
                sat.ID = 0;
                sat.ReportId = int.Parse(ReportId);
            }

            sat.Subject = new SPFieldLookupValue(int.Parse(ddlSubjectID.SelectedValue), ddlSubjectID.SelectedItem.Text);
            sat.ReportType = lblReprtType.Text;
            sat.ActivityThisVisit = txtActivityThisVisit.Text;
            //sat.DroppedFromStudy = txtDroppedFromStudy.Text;
           
            sat.Save(sat);

            InitializeSAT();

            divDetail.Visible = false;
            divGrid.Visible = true;
            BindSubjectActivityTrackerList();
        }

        protected void InitializeSAT()
        {
            ddlSubjectID.ClearSelection();
            txtActivityThisVisit.Text = string.Empty;
            //txtDroppedFromStudy.Text = string.Empty;

            lblHeading.Text = string.Empty;
        }

        protected void btnSiteOverview_Click(object sender, EventArgs e)
        {
            //Save all changes
            btnSaveAll_Click(null, EventArgs.Empty);

            //Redirect page
            Response.Redirect(btnSiteOverview.CommandArgument);
        }
    }
}
