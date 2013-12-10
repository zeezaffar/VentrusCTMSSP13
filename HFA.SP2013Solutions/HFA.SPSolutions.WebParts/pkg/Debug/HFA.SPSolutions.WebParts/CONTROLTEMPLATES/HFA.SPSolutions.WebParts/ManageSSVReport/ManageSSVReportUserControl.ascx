<%@ Assembly Name="HFA.SPSolutions.WebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=8c2017e68e4445f2" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManageSSVReportUserControl.ascx.cs" Inherits="HFA.SPSolutions.WebParts.ManageSSVReport.ManageSSVReportUserControl" %>
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
            else 
            {
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
        <td><SharePoint:PeopleEditor ID="speMonitor" runat="server" MultiSelect="false" BorderWidth="1" AllowEmpty="false" ValidatorEnabled="true"
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
          <%--  <asp:HyperLink ID="hplSiteOverView" runat="server" Text="Site Overview" />&nbsp;--%>
            <asp:LinkButton ID="btnSiteOverview" runat="server" Text="Site Overview" onclick="btnSiteOverview_Click"  />&nbsp;
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
         <asp:MenuItem Text="Source Doc." Value="8"></asp:MenuItem>
         <asp:MenuItem Text="Facilities" Value="9"></asp:MenuItem>      
        </Items>   
        <StaticSelectedStyle CssClass="StaticSelectedStyle" /> 

    </asp:Menu>

        <asp:Menu ID="formMenu2" OnMenuItemClick="formMenu2_MenuItemClick" 
             Orientation="Horizontal" runat="server" Width="100%"
             StaticMenuItemStyle-cssClass="StaticMenuItemStyle"
             StaticMenuStyle-CssClass="StaticMenuStyle"
             DynamicHorizontalOffset="2" > 
          
           <Items>  
                <asp:MenuItem Text="IP Storage" Value="10"></asp:MenuItem>
                <asp:MenuItem Text="Laboratory" Value="11"></asp:MenuItem>
                <asp:MenuItem Text="PI Qualifications" Value="12"></asp:MenuItem>
                <asp:MenuItem Text="PI Responsibilities" Value="13"></asp:MenuItem>
                <asp:MenuItem Text="Ancillary Study Staff" Value="14"></asp:MenuItem>
                <asp:MenuItem Text="Financial Discussions" Value="15"></asp:MenuItem>
                <asp:MenuItem Text="Site Acceptability" Value="16"></asp:MenuItem>
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
                <asp:DropDownList id="ddlSitePersonnel" runat="server">
                    <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                    <asp:ListItem Text="N" Value="N"></asp:ListItem>
                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
	        <td colspan="3" class="comments"><asp:TextBox ID="txtSitePersonnelComments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="subcategory" colspan="3">OTHER INC RESEARCH AND/OR SPONSOR PERSONNEL:</td>
        </tr>
        <tr>
	        <td colspan="2" class="question">
	        Were any other INC or Sponsor Personnel present at this visit? If so, provide their name, company affiliation and purpose for attending this visit in comments below:
	        </td>
	        <td class="columnText" style="text-align:right">
                <asp:DropDownList id="ddlPersonnel" runat="server">
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

        <!-- B - General Information -->
        <asp:View ID="viewGeneralInfo" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%" >
            <tr>
	            <td class="columnText">Did the CRA confirm that a signed CDA is on file at Ventrus and the site?</td>
                <td class="columnText">
                        <asp:DropDownList id="ddlB1" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>
            <tr>
	            <td colspan="2" class="columnText"><asp:TextBox ID="txtB1Comments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
            </tr>
            <tr>
	            <td class="columnText" style="width:97%">Was this visit conducted on site or by telephone? </td>
	            <td class="columnText">
                        <asp:DropDownList id="ddlB2" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>
            <tr>
	            <td colspan="2" class="columnText"><asp:TextBox ID="txtB2Comments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
            </tr>
            <tr>
	            <td class="columnText" style="width:97%">Did the CRA receive and review the Feasibility Questionnaire with the Investigator?</td>
	            <td class="columnText">
                        <asp:DropDownList id="ddlB3" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
            </tr>
            <tr>
	            <td colspan="2" class="columnText"><asp:TextBox ID="txtB3Comments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText"/></td>
            </tr>
            <tr>
                <td colspan="2" class="columnText">Reviewer Comments</td>
            </tr>
            <tr>
	            <td colspan="2" class="columnText"><asp:TextBox ID="txtBReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
            </tr>
        </table>
    </asp:View>

        <!-- C - Investigator's Brochure -->
        <asp:View ID="viewInvBrochure" runat="server">
          <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Was the Investigator’s Brochure discussed?</td>
	        <td class="columnText"><asp:DropDownList id="ddlC1" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">Version / Date of Investigator’s Brochure discussed</td>
	        <td class="columnText"> <asp:DropDownList id="ddlC2" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Did the Investigator and/or Staff raise any questions? If yes, specify questions and answers provided in Comments.
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlC3" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtC3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtCReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
        </table>
    </asp:View>

         <!-- D - Protocol -->
        <asp:View ID="viewProtocol" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:96%">Version / Date of the Protocol discussed</td>
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
	        <td class="columnText">
	            Were study objectives, study design and the procedures to be performed discussed?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlD2" runat="server">
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
	            Were Subject selection criteria (inclusion/exclusion) discussed?
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
	        <td class="columnText">
	            Were study end-points and criteria of evaluation/measurements discussed?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlD4" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtD4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	        Were the Subject visit schedule and the Subject discontinuation procedures discussed?
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlD5" runat="server">
                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtD5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">Were special equipment needs discussed?</td>
	        <td class="columnText"><asp:DropDownList id="ddlD6" runat="server">
                             <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	       <td colspan="2" class="columnText"><asp:TextBox ID="txtD6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Did the Investigator and/or staff raise any questions? If yes, specify questions and answers provided in Comments.
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlD7" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	       <td colspan="2" class="columnText"><asp:TextBox ID="txtD7Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtDReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
    </asp:View>

        <!-- E - Enrollment -->
        <asp:View ID="viewEnrollment" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Expected Number of Subjects</td>
	        <td class="columnText"><asp:DropDownList id="ddlE1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	           Were enrollment timelines discussed including an estimate of when the 1 st Subject should be enrolled? Enter details in Comments
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Does the Investigator appear to have an adequate subject population to meet enrollment within the timelines? Discuss the number of subjects seen each month with this indication.  
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlE3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	           What has been the Site’s experience in meeting enrollment numbers and timelines on previous studies?  
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Is the Investigator involved in other competing studies that may adversely affect enrollment in this study? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	            Were Subject recruitment methods /strategies (e.g., advertisements, etc) and the recruitment period discussed? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlE6" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtE6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	            Was the Site's plan for screening subjects discussed including the use of any telephone scripts, call centers, etc? 
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

        <!-- F - IRB / IEC Requirements -->
        <asp:View ID="viewIRB" runat="server">
            <br />
            <table cellpadding="5" cellspacing="5" width="100%">
                <tr>
	                <td class="columnText" style="width:97%">Is a central or local IRB / IEC used by the Site?</td>
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
	                <td class="columnText" style="width:97%">If the Site has an affiliation with a local IRB / IEC, can they request approval to use the Central IRB / IEC for this study?</td>
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
	                    Name of the local IRB / IEC: 
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
	                  Is the IRB registered with OHRP? (Applicable for US IRBs only.) 
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
	                 Approximate time required for local approval: 
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
	                <td class="columnText">Frequency of IRB / IEC meetings: </td>
	                <td class="columnText"><asp:DropDownList id="ddlF6" runat="server">
                                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                                </asp:DropDownList></td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtF6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                </tr>
                 <tr>
	                <td class="columnText">Date of the next meetings: </td>
	                <td class="columnText"><asp:DropDownList id="ddlF7" runat="server">
                                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                                </asp:DropDownList></td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtF7Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                </tr>
                 <tr>
	                <td class="columnText">Anticipated date of submission: </td>
	                <td class="columnText"><asp:DropDownList id="ddlF8" runat="server">
                                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                                </asp:DropDownList></td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtF8Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                </tr>
                <tr>
	                <td class="columnText">Note any documents specifically required for submission by the Local IRB/IEC. </td>
	                <td class="columnText"><asp:DropDownList id="ddlF9" runat="server">
                                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                                </asp:DropDownList></td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtF9Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                </tr>
                 <tr>
	                <td class="columnText">What is the frequency of status reports required by the IRB / IEC (e.g., quarterly, semi-annually or annually)?</td>
	                <td class="columnText"><asp:DropDownList id="ddlF10" runat="server">
                                    <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                                    <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                                    <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                                </asp:DropDownList></td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtF10Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
                </tr>
                 <tr>
                    <td colspan="2" class="columnText">Reviewer Comments</td>
                </tr>
                <tr>
	                <td colspan="2" class="columnText"><asp:TextBox ID="txtFReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
                </tr>
        </table>      
        </asp:View>

        <!-- G - Informed Consent -->
        <asp:View ID="viewInformedConsent" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Was the procedure for obtaining informed consent discussed?</td>
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
	        <td class="columnText" style="width:97%">Are there any special local requirements for informed consent because of where the Site is located e.g., CA Bill of Rights? </td>
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
	        <td class="columnText" style="width:97%">Does the Investigator anticipate any potential subjects from ‘vulnerable populations’? If so, what additional safeguards are in place?</td>
	        <td class="columnText"><asp:DropDownList id="ddlG3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtG3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3"  CssClass="columnText"/></td>
        </tr>
         <tr>
	        <td class="columnText" style="width:97%">Does the Investigator anticipate the need for informed consents in other languages? If yes, please document all languages required in the Comments? </td>
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
	        <td class="columnText" style="width:97%">Is the Investigator aware of the need to submit changes to the informed consent to INC Research and/or Sponsor for review prior to submitting to the IRB / IEC? </td>
	        <td class="columnText"><asp:DropDownList id="ddlG5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtG5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtGReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- H - Adverse Event -->
        <asp:View ID="viewAdverseEvent" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Is the Investigator aware of the definitions of Serious Adverse Events?</td>
	        <td class="columnText"><asp:DropDownList id="ddlH1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtH1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">Was non-serious adverse event reporting and follow-up discussed? </td>
	        <td class="columnText"><asp:DropDownList id="ddlH2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
          <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtH2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtHReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- I - Source Documentation -->
        <asp:View ID="viewSourceDoc" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Does the Investigator agree to provide access to original Subject records/source documents for monitoring, auditing, and/or inspection by the Sponsor, Sponsor Representative or RA? </td>
	        <td class="columnText"><asp:DropDownList id="ddlI1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Was the process for obtaining all past medical records for each Subject discussed? Provide details in comments. 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlI2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	        Does the Site use electronic source data? Ensure the completed Site Assessment for Electronic / Computer Systems (SAES) Forms have been submitted for filing in the TMF.
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlI3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtI3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>  
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtIReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- J - Facilities -->
        <asp:View ID="viewFacilities" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Was a facilities tour conducted?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td
                 class="columnText" style="width:97%">Is emergency medical care and equipment readily available if needed (e.g., crash cart, location of nearest ER, etc.).
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td
                 class="columnText" style="width:97%">Is there adequate space to conduct the study? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td
                 class="columnText" style="width:97%">Is there sufficient space to store non-IP study supplies e.g., CRFs, lab supplies, etc? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr> 
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td
                 class="columnText" style="width:97%">Discuss and comment on the site’s overall safeguards to restrict access and ensure protection for all study materials and documents? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td
                 class="columnText" style="width:97%">Is the required technical equipment adequate and available?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ6" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td
                 class="columnText" style="width:97%">Is there an adequate, confidential and quiet area for monitoring purposes?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlJ7" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJ7Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtJReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- K - IP Storage Equipment -->
        <asp:View ID="viewIPStorageEquipment" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td class="columnText" style="width:97%">Are the storage, space and conditions for IP adequate and meet the needs specified in the Protocol? </td>
	        <td class="columnText"><asp:DropDownList id="ddlK1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Describe the Site’s plan for receipt, storage, dispensing, and return of IP. 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	          Will Pharmacy services be used for this study? If yes, address the following question:
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlK3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	          Were the name, phone number and address of the Pharmacy obtained? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlK4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtK4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtKReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText"/></td>
        </tr>
    </table>      
        </asp:View>

        <!-- L - Laboratory -->
        <asp:View ID="viewLaboratory" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Were the laboratory requirements discussed? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlL1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	             Is a central lab to be used? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlL2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Does the Investigator have access to a local lab, if applicable? If yes, address the following questions: 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlL3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Was the name of the laboratory Director, and the name and address of the laboratory obtained? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlL4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Were reference / normal ranges (with units) requested?  
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlL5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Was a copy of the accreditation certificate(s) requested? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlL6" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtL6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtLReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- M - PI Qualifications -->
        <asp:View ID="viewPIQualifications" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Were current CVs for the Principal Investigator and Sub-Investigator(s) requested? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlM1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtM1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Was previous clinical study experience of the Investigator discussed? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlM2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtM2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Does the Site have previous experience working with INC and/or the Sponsor? Discuss number of studies done previously with either INC and/or the Sponsor.
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlM3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtM3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	         Does the Investigator appear to have sufficient time to conduct the study? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlM4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtM4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtMReviewerComments"  Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- N - Principal Investigator Reponsibilities -->
        <asp:View ID="viewPrincipalInvestigatorResponsibilities" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Does the Investigator agree to adhere to ICH GCP guidelines and other local regulatory requirements for this study?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlN1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	     Does the Investigator agree to comply with all IRB/IEC requirements? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlN2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	      Does the Investigator agree to conduct the study in accordance with the Protocol and Sponsor requirements?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	        Does the Investigator agree to obtain written IC from all Subjects prior to performing study-specific procedures and to provide all Subjects with a signed/dated copy of the IC form?
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	       Does the Investigator agree to report all serious adverse events in accordance with ICH GCP, IRB/IEC, Protocol and local requirements? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN5" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN5Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	            Does the Investigator agree to maintain adequate, accurate and current records related to the conduct of the study? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN6" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN6Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	       Does the Investigator agree to inform all associates, colleagues and staff involved in the conduct of this study of their obligations and responsibilities? 
	        </td>
	       <td class="columnText"><asp:DropDownList id="ddlN7" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtN7Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtNReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- O - Ancillary Study Staff -->
        <asp:View ID="viewAncillaryStudyStaff" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5" width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Does the Staff appear to have sufficient time to conduct the study? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlO1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtO1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	         Discuss the Site Staff’s experience working with the proposed therapeutic indication and their role for this study. 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlO2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtO2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	            Was the delegation of responsibilities within the Investigator’s Team and Ancillary Personnel discussed? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlO3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtO3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td class="columnText">
	            Discuss the Site’s standard protocol for handling turnover of study staff during the course of the study. 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlO4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtO4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtOReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- P - Financial Discussions -->
        <asp:View ID="viewFinancialDiscussions" runat="server">
        <br />
        <table cellpadding="5" cellspacing="5"  width="100%">
        <tr>
	        <td
                 class="columnText" style="width:97%">Have Investigator fees been discussed? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlP1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td
                 class="columnText" style="width:97%">Have the contract procedures been reviewed? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlP2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td
                 class="columnText" style="width:97%">Have financial disclosure reporting requirements been discussed? 
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlP3" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP3Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
	        <td
                 class="columnText" style="width:97%">Does the Investigator foresee any other study-specific costs, such as pharmacy fees, hospital overhead or laboratory costs?
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlP4" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtP4Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtPReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>

        <!-- Q - Site Acceptability -->
        <asp:View ID="viewSiteAcceptability" runat="server">
        <br />
        <table width="100%" cellpadding="5" cellspacing="5" >
        <tr>
	        <td
                 class="columnText" style="width:97%">
                  Has the Site or Investigator been subject to a regulatory inspection? If so, discuss the findings and include the name of the regulatory body that conducted the inspection.
            </td>
	        <td class="columnText"><asp:DropDownList id="ddlQ1" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ1Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
        <tr>
	        <td class="columnText">
	          Do the Investigator and Staff have the background, qualifications and training to perform study related procedures? 
	        </td>
	        <td class="columnText"><asp:DropDownList id="ddlQ2" runat="server">
                            <asp:ListItem Text="Y"  Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N"  Value="N"></asp:ListItem>
                            <asp:ListItem Text="NA" Value="NA"></asp:ListItem>
                        </asp:DropDownList></td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQ2Comments" runat="server" Width="99.5%" TextMode="MultiLine" Rows="3" CssClass="columnText" /></td>
        </tr>
         <tr>
            <td colspan="2" class="columnText">Reviewer Comments</td>
        </tr>
        <tr>
	        <td colspan="2" class="columnText"><asp:TextBox ID="txtQReviewerComments" Width="99.5%" TextMode="MultiLine" Rows="3" runat="server" CssClass="columnText" /></td>
        </tr>
    </table>      
        </asp:View>
        </asp:MultiView>
        <br /><br />
            <div style="width:800px;text-align:center">
        </div>
 </div>
