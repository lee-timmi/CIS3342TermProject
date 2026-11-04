<%@ Page Title="Make a Reservation | Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MakeReservation.aspx.cs" Inherits="RestaurantReview.MakeReservation" ValidateRequestMode="Disabled" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <main class="container my-5" style="max-width: 800px;">
            <h2 class="mb-1">Make a Reservation</h2>
            <p class="text-muted mb-4">
                <asp:HyperLink ID="lnkRestaurant" runat="server" CssClass="fw-bold" />
                <asp:Label ID="lblLocation" runat="server" />
            </p>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />

            <asp:Panel ID="pnlForm" runat="server">
                <p class="text-muted small">No account needed. The restaurant will get your request by email and confirm it with you. Fields marked * are required.</p>

                <h4 class="mt-4 mb-3">Your Details</h4>
                <div class="mb-3">
                    <label class="form-label fw-bold">Name *</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="row">
                    <div class="col-md-5 mb-3">
                        <label class="form-label fw-bold">Phone *</label>
                        <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="20" placeholder="e.g. 215-555-0123" />
                    </div>
                    <div class="col-md-7 mb-3">
                        <label class="form-label fw-bold">Email *</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" MaxLength="100" TextMode="Email" placeholder="you@example.com" />
                    </div>
                </div>

                <h4 class="mt-4 mb-3">Your Reservation</h4>
                <div class="row">
                    <div class="col-md-7 mb-3">
                        <label class="form-label fw-bold d-block">Date *</label>
                        <asp:Calendar ID="calDate" runat="server" CssClass="table table-sm table-bordered bg-white"
                            SelectedDayStyle-BackColor="#c43030" SelectedDayStyle-ForeColor="White"
                            OnSelectionChanged="calDate_SelectionChanged" />
                    </div>
                    <div class="col-md-5">
                        <div class="mb-3">
                            <label class="form-label fw-bold">Time *</label>
                            <asp:DropDownList ID="ddlTime" runat="server" CssClass="form-select"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlTime_SelectedIndexChanged" />
                            <!-- That day's hours; turns into a warning when the time is outside them -->
                            <asp:Label ID="lblHours" runat="server" Visible="false" />
                        </div>
                        <div class="mb-3">
                            <label class="form-label fw-bold">Party Size *</label>
                            <asp:DropDownList ID="ddlPartySize" runat="server" CssClass="form-select" />
                        </div>
                    </div>
                </div>
                <div class="mb-3">
                    <label class="form-label fw-bold">Special Requests <span class="fw-normal text-muted">(optional, up to 500 characters)</span></label>
                    <asp:TextBox ID="txtRequests" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Allergies, a birthday, a high chair..." />
                </div>

                <div class="mt-4">
                    <asp:Button ID="btnReserve" runat="server" Text="Request Reservation" CssClass="btn btn-dark" OnClick="btnReserve_Click" />
                    <asp:HyperLink ID="lnkCancel" runat="server" CssClass="btn btn-link" Text="Cancel" />
                </div>
            </asp:Panel>

            <!-- Shown after a successful request -->
            <asp:Panel ID="pnlDone" runat="server" Visible="false" CssClass="card">
                <div class="card-body">
                    <h4 class="card-title">Request sent</h4>
                    <asp:Literal ID="litSummary" runat="server" />
                    <asp:HyperLink ID="lnkBack" runat="server" CssClass="btn btn-dark btn-sm mt-2" Text="Back to the restaurant" />
                </div>
            </asp:Panel>
        </main>
</asp:Content>
