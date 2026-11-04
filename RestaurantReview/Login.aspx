<%@ Page Title="Log In" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RestaurantReview.Login" ValidateRequestMode="Disabled" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container mt-5" style="max-width: 450px;">
        <h2 class="mb-4">Log In</h2>

        <div class="mb-3">
            <label class="form-label fw-bold">Username</label>
            <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" MaxLength="50" />
        </div>
        <div class="mb-3">
            <label class="form-label fw-bold">Password</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="100" />
        </div>

        <asp:Button ID="btnLogin" runat="server" Text="Log In" CssClass="btn btn-primary" OnClick="btnLogin_Click" />

        <!-- Error / success message -->
        <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="d-block mt-3" />

        <p class="mt-3">Don't have an account? <asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="Register.aspx" Text="Create one" /></p>
    </div>
</asp:Content>
