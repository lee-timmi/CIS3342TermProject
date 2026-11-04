<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PhillyForm.aspx.cs" Inherits="LabDemo2.PhillyForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Please Take a Moment To Fill Out Our Survey</title>
    <style type="text/css">
        .auto-style2 {
            height: 111px;
        }
        .auto-style3 {
            height: 27px;
            width: 636px;
        }
        .auto-style4 {
            width: 745px;
        }
        .auto-style5 {
        }
        .auto-style7 {
            height: 23px;
        }
        .auto-style8 {
            height: 23px;
            width: 636px;
        }
    </style>
</head>
<body align="center" style="background-color: indianred">
    <form id="form1" runat="server">
        <div>
            <table align="center" style="width:786px" margin-bottom="0px">
                <tr align="center">
                    <td class="auto-style2" colspan="2"><h2/>Take a Moment To Fill Out Our Philly Survey</td>
                </tr>
                <tr>
                    <td class="auto-style3">Full Name</td>
                    <td class="auto-style4">
                        <asp:TextBox ID="txtName" runat="server" Width="300px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style5">Favorite Philly Team</td>
                    <td>
                        <asp:DropDownList ID="ddlTeam" runat="server" Width="189px">
                            <asp:ListItem>Eagles</asp:ListItem>
                            <asp:ListItem>Flyers</asp:ListItem>
                            <asp:ListItem>Phillies</asp:ListItem>
                            <asp:ListItem>Sixers</asp:ListItem>
                            <asp:ListItem>Union</asp:ListItem>
                            <asp:ListItem>Temple Sports</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style8">Check to Join our Mailing List</td>
                    <td class="auto-style7">
                        <asp:CheckBox ID="chkJoin" runat="server" Text="Thank You" />
                    </td>
                </tr>
                <tr>
                    <td class="auto-style5">&nbsp;</td>
                    <td>
                        <asp:Button ID="btnSend" runat="server" Text="Send" OnClick="btnSend_Click" />
                    </td>
                </tr>
                <tr align="center">
                    <td class="auto-style5" colspan="2">
                        <asp:Label ID="lblDisplay" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
