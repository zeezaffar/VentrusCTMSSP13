<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageSIVReportUserControl.ascx.cs" Inherits="HFA.SPSolutions.WebParts.ManageSIVReport.ManageSIVReportUserControl" %>
<meta http-equiv="X-UA-Compatible" content="IE=8"/>

<SharePoint:CssRegistration ID="defaultCSS" Name="/Style Library/default.css" runat="server" After="corev4.css" /> 
<script type="text/javascript" src="/_layouts/DanFay/jquery-1.7.1.min.js"></script>
<script type="text/javascript" src="/_layouts/DanFay/jquery-ui-1.8.17.custom.min.js"></script>

<script type="text/javascript">
    $(document).ready(function () {

        $('[id$=btnSaveAll]').click(function () {
            var msgLabel = $("#<%=lblMessage.ClientID%>");
            var $msg = "";

            var title = $('[id$=txtTitle]');
            var visitDate = $('[id$=calVisitDate_calVisitDateDate]');
            var siteNo = $('[id$="ddlSites"]');
            var regexp = /^[0-9\\.]+$/;
            var titleRegexp = /^[A-Za-z0-9 _]+$/;

            visitDate.css('border-color', '');
            title.css('border-color', '');
            siteNo.css('border-color', '');

            if (title[0].value.length == 0) {
                $msg = $msg + "<br/>Please enter the report title";
                title.css('border-color', 'red');
            }
            else {
                if (!titleRegexp.test(title[0].value)) {
                    $msg = $msg + "<br/>The title cannot have any special characters";
                    title.css('border-color', 'red');
                }
            }

            if (visitDate[0].value.length == 0) {
                $msg = $msg + "<br/>Please select the visit date";
                visitDate.css('border-color', 'red');
            }

            if (siteNo[0].value == "0") {
                $msg = $msg + "<br/>Please select a site number";
                siteNo.css('border-color', 'red');
            }

            if ($msg.length > 0) {
                msgLabel.html($msg).css("color", "red")
                return false;
            }
            else
                return true;
        });
    });

</script>

 <div style="padding-left:2px">
     <!-- Heading -->
    <table cellpadding="5" cellspacing="5" width="100%">
    <tr>
        <td class="columnHeader">Report Title:</td>
        <td colspan="3"><asp:TextBox ID="txtTitle" runat="server" Width="99%" CssClass="columnText" /></td>
    </tr>
    <tr> 
        <td class="columnHeader">Sponsor:</td><td  style="width:40%"><asp:TextBox ID="txtSponsor" runat="server" Width="98%"  CssClass="columnText" /></td>
        <td class="columnHeader">Protocol:</td><td><asp:TextBox ID="txtProtocol" runat="server" Width="98%" CssClass="columnText" /></td>
    </tr>
    <tr>
        <td class="columnHeader">Visit Date:</td>
        <td ><SharePoint:DateTimeControl ID="calVisitDate" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
         <td class="columnHeader">Monitor:</td>
        <td><SharePoint:PeopleEditor ID="speMonitor" runat="server" MultiSelect="false" BorderWidth="1" AllowEmpty="false" ValidatorEnabled="true" CssClass="columnText"
                                    PlaceButtonsUnderEntityEditor="false" SelectionSet="User" BorderColor="#BEBEBE" Width="99%" /></td>
    </tr>
     <tr>
        <td><label class="columnHeader">Date of Next Visit:</label></td>
        <td colspan="3"><SharePoint:DateTimeControl ID="calNextVisitDate" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
    </tr>
    <tr>
        <td class="columnHeader">Address:</td>
        <td><SharePoint:InputFormTextBox ID="txtAddress" RichText="false" runat="server" TextMode="MultiLine" Rows="3" Width="98%" CssClass="columnText" /></td>
        <td class="columnHeader">Status:</td><td>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="columnText" Width="99%">
                <asp:ListItem Text="Incomplete" Value="Incomplete" />
                <asp:ListItem Text="Ready for Review" Value="Ready for Review" />
                <asp:ListItem Text="Ready for Signatures" Value="Ready for Signatures" />
                <asp:ListItem Text="Complete / Verified" Value="Complete / Verified" />
            </asp:DropDownList></td>
    </tr>
    <tr>
        <td class="columnHeader">Version</td>
        <td><asp:DropDownList id="ddlVersion" runat="server"  CssClass="columnText" /></td>
        <td class="columnHeader">Version Date:</td>
           <td ><SharePoint:DateTimeControl ID="calVersionDate" DateOnly="true" runat="server" CssClassTextBox="columnText" CssClassDescription="columnText" /></td>
     </tr>
     <tr>
        <td class=""><label class="columnHeader">Miscellaneous Comments:</label></td>
        <td><asp:TextBox ID="txtMiscComments" runat="server" TextMode="MultiLine" Rows="2" Width="99%" CssClass="columnText" /></td>
        <td class="columnHeader">Site Number:</td>
        <td><asp:DropDownList ID="ddlSites" runat="server" /></td>
    </tr>
    <tr>
        <td><label class="columnHeader">General Reviewer Comments:</label></td>
        <td colspan="3"><asp:TextBox ID="txtGenRevComments" runat="server" TextMode="MultiLine" Rows="4" Width="99%" CssClass="columnText" /></td>
    </tr>
    <tr>    
         <td style="text-align:right"></td>
         <td style="text-align:right" colspan="3">
<%--            <asp:HyperLink ID="hplSiteOverView" runat="server" Text="Site Overview" />&nbsp;
--%>            <asp:LinkButton ID="btnSiteOverview" runat="server" Text="Site Overview" onclick="btnSiteOverview_Click"  />&nbsp;
                <asp:LinkButton ID="btnPDFGenerator" runat="server" Text="Generate PDF" onclick="btnPDFGenerator_Click"></asp:LinkButton>
        </td>
     </tr>
      <tr>
        <td colspan="4" style="text-align:right"><asp:Button id="btnSaveAll" runat="server" Text="Save" onclick="btnSaveAll_Click"  /></td>
    </tr>
     <tr>
        <td colspan="4" class="messageLabel"><asp:Label ID="lblMessage" runat="server" ForeColor="Red" /></td>
    </tr>
    </table>
     <br />
     <div style="width:100%">

        <asp:Menu ID="formMenu" OnMenuItemClick="formMenu_MenuItemClick" 

             Orientation="Horizontal" runat="server" Width="100%"
             StaticMenuItemStyle-cssClass="StaticMenuItemStyle"
             StaticMenuStyle-CssClass="StaticMenuStyle"
             DynamicHorizontalOffset="2" > 
           
         <Items>
          <asp:MenuItem Text="Visit Attendees" Value="0" Selected="true"></asp:MenuItem>
         <asp:MenuItem Text="General Information" Value="1"></asp:MenuItem>
         <asp:MenuItem Text="Brochure" Value="2"></asp:MenuItem>
         <asp:MenuItem Text="Protocol" Value="3"></asp:MenuItem>
         <asp:MenuItem Text="Enrollment" Value="4"></asp:MenuItem>
         <asp:MenuItem Text="IRB/ IEC Req." Value="5"></asp:MenuItem>
         <asp:MenuItem Text="Informed Consent" Value="6"></asp:MenuItem>
         <asp:MenuItem Text="Adverse Event" Value="7"></asp:MenuItem>
         <asp:MenuItem Text="Investigator Site File" Value="8"></asp:MenuItem>
         <asp:MenuItem Text="Source Doc." Value="9"></asp:MenuItem>
        </Items>   
        <StaticSelectedStyle CssClass="StaticSelectedStyle" /> 
    </asp:Menu>

        <asp:Menu ID="formMenu2" OnMenuItemClick="formMenu2_MenuItemClick" 
             Orientation="Horizontal" runat="server" Width="100%"
             StaticMenuItemStyle-cssClass="StaticMenuItemStyle"
             StaticMenuStyle-CssClass="StaticMenuStyle"
             DynamicHorizontalOffset="2">
           <Items>  
            <asp:MenuItem Text="Case Report Forms" Value="10"></asp:MenuItem>
            <asp:MenuItem Text=" Facilities" Value="11"></asp:MenuItem>
            <asp:MenuItem Text="Investigational Product" Value="12"></asp:MenuItem>
            <asp:MenuItem Text="Other Trial Material" Value="13"></asp:MenuItem>
            <asp:MenuItem Text="Laboratory" Value="14"></asp:MenuItem>
            <asp:MenuItem Text="Investigator Reponsibilities" Value="15"></asp:MenuItem>
            <asp:MenuItem Text="Ancillary Study Staff" Value="16"></asp:MenuItem>
            <asp:MenuItem Text="Monitoring" Value="17"></asp:MenuItem>
            <asp:MenuItem Text="Site Acceptability" Value="18"></asp:MenuItem>
         </Items>
         <StaticSelectedStyle CssClass="StaticSelectedStyle" /> 
    </asp:Menu>
    </div>

    <asp:MultiView ID="formMultiView" runat="server" ActiveViewIndex="0">
         <!-- Visit Attendees -->
        <asp:View ID="viewVisitAttendees" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%" border="0">
        <tr>
	        <td class="subcategory" colspan="3">CLINICAL SITE PERSONNEL:</td>
        </tr>
        <tr>
	        
	        <td class="columnHeading">Investigator Name</td>
	        <td class="columnHeading">Title</td>
            <td></td>
        </tr>
        <tr>
	        <td style="width:50%"><asp:TextBox ID="txtInvestigatorName" runat="server" Width="100%" CssClass="columnText" /></td>
	        <td colspan="2"><asp:TextBox ID="txtInvestigatorTitle" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
          <tr>
	        
	        <td class="columnHeading">Site Participant Name</td>
	        <td class="columnHeading">Title</td>
            <td></td>
        </tr>
        <tr>
             <td><asp:TextBox ID="txtSitePersonnelName" runat="server" Width="100%" CssClass="columnText" /></td>
             <td colspan="2"><asp:TextBox ID="txtSitePersonnelTitle" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
             <td><asp:TextBox ID="txtSitePersonnelName2" runat="server" Width="100%" CssClass="columnText" /></td>
             <td colspan="2"><asp:TextBox ID="txtSitePersonnelTitle2" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
             <td><asp:TextBox ID="txtSitePersonnelName3" runat="server" Width="100%" CssClass="columnText" /></td>
             <td colspan="2"><asp:TextBox ID="txtSitePersonnelTitle3" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="subcategory" colspan="3">INC RESEARCH MONITORS:</td>
        </tr>
        <tr>
	        <td class="columnHeading">Monitor Name</td>
	        <td class="columnHeading">Title</td>
            <td></td>
        </tr>
        <tr>
	        <td><asp:TextBox ID="txtMonitorName" runat="server" Width="100%" CssClass="columnText" /></td>
	        <td colspan="2"><asp:TextBox ID="txtMonitorTitle" runat="server" Width="100%" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td colspan="2">Were any additional Site Personnel met with at this visit? If so, provide name and role in comments below.</td>
	         <td class="columnText" style="text-align:right">
                <asp:DropDownList id="ddlOtherSitePersonnel" runat="server">
                    <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                    <asp:ListItem Text="N" Value="N"></asp:ListItem>
                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
	        <td colspan="3" class="comments"><asp:TextBox ID="txtOtherSitePersonnelComments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="subcategory" colspan="3">OTHER INC RESEARCH AND/OR SPONSOR PERSONNEL:</td>
        </tr>
        <tr>
	        <td colspan="2" class="question">
	        Were any other INC or Sponsor Personnel present at this visit? If so, provide their name, company affiliation and purpose for attending this visit in comments below:
	        </td>
	        <td class="columnText" style="text-align:right">
                <asp:DropDownList id="ddlOtherPersonnel" runat="server">
                    <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                    <asp:ListItem Text="N" Value="N"></asp:ListItem>
                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
	        <td colspan="3" class="comments"><asp:TextBox ID="txtPersonnelComments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="3" class="columnText"><asp:TextBox ID="txtVAReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>
    </asp:View>

        <!-- C - General Information -->
        <asp:View ID="viewGeneralInfo" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%" >
            <tr>
	            <td class="columnText">Did the CRA confirm that a signed CDA is on file at INC Research?</td>
                <td class="columnText">
                        <asp:DropDownList id="ddlC1" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>
            <tr>
	            <td colspan="2" class="columnText"><asp:TextBox ID="txtC1Comments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
            </tr>
            <tr>
	            <td class="columnText" style="width:97%">Did the CRA receive and review the Feasibility Questionnaire with the Investigator?</td>
	            <td class="columnText">
                        <asp:DropDownList id="ddlC2" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>
            <tr>
	            <td colspan="2" class="columnText"><asp:TextBox ID="txtC2Comments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
            </tr>
            <tr>
                <td colspan="2" class="columnText">Reviewer Comments</td>
            </tr>
            <tr>
	            <td colspan="2" class="columnText"><asp:TextBox ID="txtCReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
            </tr>
        </table>
    </asp:View>

        <!-- D - Brochure -->
        <asp:View ID="viewInvBrochure" runat="server">
          <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Was the Investigator’s Brochure discussed?</td>
	        <td class="columnText"><asp:DropDownList id="ddlD1" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtD1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">Version / Date of Investigator’s Brochure discussed</td>
	        <td class="columnText"> <asp:DropDownList id="ddlD2" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtD2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Did the Investigator and/or Staff raise any questions? If yes, specify questions and answers provided in Comments.
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlD3" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtD3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtDReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
        </table>
    </asp:View>

         <!-- E - Protocol -->
        <asp:View ID="viewProtocol" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Version / Date of the Protocol discussed</td>
	        <td class="columnText"><asp:DropDownList id="ddlE1" runat="server">
                           <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Were study objectives, study design and the procedures to be performed discussed?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE2" runat="server">
                          <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Were Subject selection criteria (inclusion/exclusion) discussed?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlE3" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Were study end-points and criteria of evaluation/measurements discussed?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE4" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	        Were the Subject visit schedule and the Subject discontinuation procedures discussed?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE5" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">Were special equipment needs discussed?</td>
	        <td class="columnText"><asp:DropDownList id="ddlE6" runat="server">
                             <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	       <td colspan="2" class="columnText"><asp:TextBox ID="txtE6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Did the Investigator and/or staff raise any questions? If yes, specify questions and answers provided in Comments.
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE7" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	       <td colspan="2" class="columnText"><asp:TextBox ID="txtE7Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtEReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- F - Enrollment -->
        <asp:View ID="viewEnrollment" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Expected Number of Subjects</td>
	        <td class="columnText"><asp:DropDownList id="ddlF1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtF1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Were enrollment timelines discussed including an estimate of when the 1 st Subject should be enrolled? Enter details
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlF2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtF2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Were Subject recruitment methods /strategies (e.g., advertisements, etc) and the recruitment period discussed? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlF3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtF3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Was the Site's plan for screening subjects discussed including the use of any telephone scripts, call centers, etc? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlF4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtF4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	        Were Subject Screening Logs and Identification Lists discussed? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlF5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtF5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtFReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
     </asp:View>

        <!-- G - IRB / IEC Requirements -->
        <asp:View ID="viewIRB" runat="server">
        <br />
            <table cellpadding="5" cellspacing="5" width="100%">
                <tr>
	        <td class="columnText" style="width:97%">Has unconditional IRB / IEC approval been obtained and is the appropriate documentation present in the ISF?</td>
	        <td class="columnText"><asp:DropDownList id="ddlG1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtG1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                </tr>
                <tr>
	                <td class="columnText">
	                    If no, has approval been received to conduct the SIV without IRB / IEC approval?
	                </td>
	                <td class="columnText"><asp:DropDownList id="ddlG2" runat="server">
                                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                                </asp:DropDownList></td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtG2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                </tr>
                <tr>
	                <td class="columnText">
	                  Was the content of IRB / IEC reports discussed to ensure the Site is aware of the need to report Protocol non-compliance issues, SAEs and IND safety reports
	                </td>
	               <td class="columnText"><asp:DropDownList id="ddlG3" runat="server">
                                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                                </asp:DropDownList></td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtG3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                        </tr>
                <tr>
	                <td class="columnText">
	                  Was the frequency of status reports required by the IRB / IEC (e.g., quarterly, semi-annually or annually) discussed?
	                </td>
	                <td class="columnText"><asp:DropDownList id="ddlG4" runat="server">
                                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                                </asp:DropDownList></td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtG4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                </tr>
                 <tr>
                    <td colspan="2" class="columnText">Reviewer Comments</td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtGReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
                </tr>
        </table>      
        </asp:View>

        <!-- H - Informed Consent -->
        <asp:View ID="viewInformedConsent" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Was the procedure for obtaining informed consent discussed?</td>
	        <td class="columnText"><asp:DropDownList id="ddlH1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtH1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtHReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- I - Adverse Event -->
        <asp:View ID="viewAdverseEvent" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Is the Investigator aware of the definitions of Serious Adverse Events?</td>
	        <td class="columnText"><asp:DropDownList id="ddlI1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">Were Serious and Non Serious Adverse Event reporting and follow-up discussed?</td>
	        <td class="columnText"><asp:DropDownList id="ddlI2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">Was the Serious Adverse Event Form reviewed?</td>
	       <td class="columnText"><asp:DropDownList id="ddlI3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">Were Sponsor and/or Protocol specific AE reporting requirements discussed if any?</td>
	        <td class="columnText"><asp:DropDownList id="ddlI4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI4Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtIReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- J - Investigator Site File -->
        <asp:View ID="viewIvestigateSiteFile" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Was the Investigator Site File reviewed?</td>
	        <td class="columnText"><asp:DropDownList id="ddlJ1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">Are all the documents that are required to initiate this study present on Site? </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">Were the maintenance of required documents and the availability of the Investigator Site File discussed?</td>
	       <td class="columnText"><asp:DropDownList id="ddlJ3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">Was the Site Delegation of Authority Log completed, signed by the PI and a copy sent to INC Research for filing in the TMF?</td>
	        <td class="columnText"><asp:DropDownList id="ddlJ4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ4Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- K - Source Documentation -->
        <asp:View ID="viewSourceDoc" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Is the Investigator aware of the definition of source documents?</td>
	        <td class="columnText"><asp:DropDownList id="ddlK1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	           Has the Investigator been reminded of the responsibility for keeping up to date with Subject progress in the study? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Has Investigator been reminded of the responsibility to ensure that all CRFs and corresponding original source documents are available at each Monitoring Visit? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlK3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	          Was the process for obtaining all original past medical records for each Subject’s records discussed?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK4Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	          Does the Site use electronic source data? Ensure the completed Site Assessment for Electronic / Computer Systems (SAES) Forms have been submitted for filing in the TMF. 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK5Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	          If the Site is using electronic records, has the CRA been provided access with a unique user name and password?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK6" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK6Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	          If no, has the process for providing "certified copies" of electronic records been discussed and agreed to with the Site? Describe process in Comments. 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK7" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK7Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtKReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- L - Case Report Forms -->
        <asp:View ID="viewCaseReportForms" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Were Case Report Form completion instructions discussed?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlL1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Were the data handling procedures discussed? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlL2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         If the study is using EDC, is training and certification on file for all users? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlL3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtLReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
         </asp:View>

        <!-- M - Facilities -->
        <asp:View ID="viewFacilities" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Have there been any changes to the Site’s facilities since the PSSV? If yes, identify in the Comments.
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlM1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtM1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtMReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- N - Investigational Product Requirements -->
        <asp:View ID="view1" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Was IP received, inventoried and does inventory match the shipping invoice? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlN1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Were IP accountability procedures discussed? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlN2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Was the procedure for obtaining re-supply of IP discussed? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         Were IP dispensing and randomization procedures discussed?  
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN4Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         Was the storage and access of the randomization code breaks discussed? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN5Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         Was the randomization code breaking procedure discussed? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN6" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN6Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         Are the storage, space and conditions for IP adequate and meet the needs of the Protocol? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN7" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN7Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         If required are temperature/humidity logs available? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN8" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN8Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	        Are all staff responsible for IP inventory, dispensing and drug accountability appropriately trained and listed on the Site Delegation of Authority Log? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN9" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN9Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtNReviewerComments"  Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- O - Other Trial Material -->
        <asp:View ID="viewOtherTrialMaterial" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Were other trial related materials, e.g., CRFs, inventoried and does inventory match the shipping invoices? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlO1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtO1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td
                 class="columnText" style="width:97%">Was the procedure for obtaining re-supply of other trial-related material discussed?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlO2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtO2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtOReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- P - Laboratory -->
        <asp:View ID="viewLaboratory" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Were the laboratory requirements discussed? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlP1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	             Is a local laboratory to be used? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlP2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         If a local lab to be used, is a copy of the reference / normal ranges (including units) for all lab tests in the ISF? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlP3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         If a local lab to be used, is a copy of the appropriate accreditation certificate(s) in the ISF? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlP4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP4Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         If a central lab is being used, were laboratory supplies received, inventoried and expiration dates on laboratory supplies are current? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlP5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP5Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtPReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- Q - Principal Investigator Reponsibilities -->
        <asp:View ID="viewPrincipalInvestigatorResponsibilities" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Does the Investigator agree to adhere to ICH GCP guidelines and other local regulatory requirements for this study?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlQ1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">Does the Investigator agree to comply with all IRB/IEC requirements?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlQ2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">Does the Investigator agree to conduct the study in accordance with the Protocol?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlQ3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	            Does the Investigator agree to obtain written IC from all Subjects prior to performing study-specific procedures and to provide all Subjects with a signed/dated copy of the IC form?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlQ4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ4Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	        Does the Investigator agree to report all serious adverse events in accordance with ICH GCP, IRB/IEC, Protocol and local requirements?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlQ5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ5Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	            Does the Investigator agree to maintain adequate, accurate and current records related to the conduct of the study? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlQ6" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ6Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	            Does the Investigator agree to inform all associates, colleagues and staff involved in the conduct of this study of their obligations and responsibilities?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlQ7" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ7Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         Does the Investigator agree to provide access to original Subject records/source documents for monitoring, auditing, or inspection by the Sponsor, their Representative or RA?

	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlQ8" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ8Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- R - Ancillary Study Staff -->
        <asp:View ID="view2" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Have all personnel been adequately trained on study-related procedures and appropriate documentation of the training is on file?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlR1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtR1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Have all study personnel signed the Site Delegation of Authority Log and has it been signed by the PI?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlR2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtR2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
          <tr>
	        <td class="columnText">
	     Has a copy of the Site Delegation of Authority Log been sent to INC Research for filing in the TMF? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlR3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtR3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtRReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- S - Monitoring -->
        <asp:View ID="viewMonitoring" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">
                    Was the expectation of who should participate in monitoring visits discussed?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlS1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtS1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Was the expected frequency, scheduling and duration of the monitoring visits discussed? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlS2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtS2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
          <tr>
	        <td class="columnText">
	            Was the tentative date for the first interim monitoring visit discussed? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlS3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtS3Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtSReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- T - Site Acceptability -->
        <asp:View ID="viewSiteAcceptability" runat="server">
        <br />
        <table width="100%" cellpadding="5" cellspacing="5" >
        <tr>
	        <td
                 class="columnText" style="width:97%">
                   Have there been any changes to the Site since the PSSV? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlT1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtT1Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	          Do the Investigator and Staff have the background, qualifications and training to perform study related procedures?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlT2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtT2Comments" runat="server" Width="99.5%" TextMode="MultiLine" CssClass="columnText" Rows="3" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtTReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" runat="server" /></td>
        </tr>
    </table>      
        </asp:View>
        </asp:MultiView>

        <br /><br />
            <div style="width:800px;text-align:center">
           
        </div>
 </div>