<%@ Page Title="Create an Account" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="RestaurantReview.Register" ValidateRequestMode="Disabled" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-5" style="max-width: 600px;">
        <h2 class="mb-4">Create an Account</h2>

        <!-- Account type -->
        <div class="mb-3">
            <label class="form-label fw-bold">I am a...</label>
            <asp:RadioButtonList ID="rblUserType" runat="server" RepeatDirection="Horizontal" CssClass="ms-2">
                <asp:ListItem Value="Reviewer">&nbsp;Reviewer&nbsp;&nbsp;&nbsp;</asp:ListItem>
                <asp:ListItem Value="Representative">&nbsp;Restaurant Representative</asp:ListItem>
            </asp:RadioButtonList>
        </div>

        <!-- Name -->
        <div class="row">
            <div class="col mb-3">
                <label class="form-label fw-bold">First Name</label>
                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="50" />
            </div>
            <div class="col mb-3">
                <label class="form-label fw-bold">Last Name</label>
                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="50" />
            </div>
        </div>

        <!-- Contact -->
        <div class="mb-3">
            <label class="form-label fw-bold">Email</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="100" />
        </div>
        <div class="mb-3">
            <label class="form-label fw-bold">Phone <span class="fw-normal text-muted">(optional)</span></label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="20" placeholder="e.g. 215-555-0123" />
        </div>

        <!-- Login info -->
        <div class="mb-3">
            <label class="form-label fw-bold">Username</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" MaxLength="50" />
        </div>
        <div class="mb-3">
            <label class="form-label fw-bold">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="100" />
        </div>
        <div class="mb-3">
            <label class="form-label fw-bold">Confirm Password</label>
            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="100" />
        </div>

        <asp:Button ID="btnRegister" runat="server" Text="Create Account" CssClass="btn btn-primary" OnClick="btnRegister_Click" />

        <!-- Error / success message -->
        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="d-block mt-3" />

        <p class="mt-3">Already have an account? <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="Login.aspx" Text="Log in" /></p>
    </div>
</asp:Content>
