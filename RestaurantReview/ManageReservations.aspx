<%@ Page Title="Reservations | Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageReservations.aspx.cs" Inherits="RestaurantReview.ManageReservations" ValidateRequestMode="Disabled" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="style/Profile.css?v=2" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <main class="container my-5" style="max-width: 900px;">
            <h2 class="mb-1">Reservations</h2>
            <p class="text-muted mb-4">Upcoming reservations are listed first. Confirming, changing, or cancelling one emails the customer automatically.</p>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />
            <asp:Label ID="lblNoReservations" runat="server" CssClass="no-reviews-msg" Visible="false"
                Text="No reservations yet." />

            <!-- Edit form: shown when a card's Edit button is clicked -->
            <asp:Panel ID="pnlEdit" runat="server" Visible="false" CssClass="card mb-4">
                <div class="card-body">
                    <h4 class="card-title mb-3">Change <asp:Label ID="lblEditName" runat="server" />'s reservation</h4>
                    <%-- Hidden: which reservation is being edited (the code checks it belongs to this rep again on Save) --%>
                    <asp:Label ID="lblEditID" runat="server" Visible="false" />
                    <div class="row">
                        <div class="col-md-7 mb-3">
                            <label class="form-label fw-bold d-block">Date</label>
                            <asp:Calendar ID="calEditDate" runat="server" CssClass="table table-sm table-bordered bg-white"
                                SelectedDayStyle-BackColor="#c43030" SelectedDayStyle-ForeColor="White" />
                        </div>
                        <div class="col-md-5">
                            <div class="mb-3">
                                <label class="form-label fw-bold">Time</label>
                                <asp:DropDownList ID="ddlEditTime" runat="server" CssClass="form-select" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label fw-bold">Party Size</label>
                                <asp:DropDownList ID="ddlEditPartySize" runat="server" CssClass="form-select" />
                            </div>
                            <div class="mb-3">
                                <label class="form-label fw-bold">Status</label>
                                <asp:RadioButtonList ID="rblEditStatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-choice">
                                    <asp:ListItem Value="Pending">Pending</asp:ListItem>
                                    <asp:ListItem Value="Confirmed">Confirmed</asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    <div class="mb-3">
                        <label class="form-label fw-bold">Special Requests</label>
                        <asp:TextBox ID="txtEditRequests" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                    </div>
                    <asp:Button ID="btnSaveEdit" runat="server" Text="Save and Email Customer" CssClass="btn btn-dark" OnClick="btnSaveEdit_Click" />
                    <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn btn-link" OnClick="btnCancelEdit_Click" />
                </div>
            </asp:Panel>

            <!-- One card per reservation (Prof. Pascucci's Repeater Example 3: buttons + ItemCommand + FindControl) -->
            <asp:Repeater ID="rptReservations" runat="server" OnItemCommand="rptReservations_ItemCommand">
                <ItemTemplate>
                    <div class="review-card">
                        <div class="review-header">
                            <span class="review-author"><%#: DataBinder.Eval(Container.DataItem, "ContactName") %>, party of <%# DataBinder.Eval(Container.DataItem, "PartySize") %></span>
                            <span class='badge <%# DataBinder.Eval(Container.DataItem, "Status").ToString() == "Confirmed" ? "bg-success" : "bg-warning text-dark" %>'><%# DataBinder.Eval(Container.DataItem, "Status") %></span>
                        </div>
                        <p class="review-text mb-1">
                            <b><%# DataBinder.Eval(Container.DataItem, "ReservationDateTime", "{0:dddd, MMMM d, yyyy 'at' h:mm tt}") %></b>
                            <%# DataBinder.Eval(Container.DataItem, "IsUpcoming").ToString() == "1" ? "" : "<span class='text-muted'>(past)</span>" %>
                        </p>
                        <p class="review-visit mb-2">
                            <%#: DataBinder.Eval(Container.DataItem, "ContactPhone") %> &middot; <%#: DataBinder.Eval(Container.DataItem, "ContactEmail") %>
                        </p>
                        <asp:Panel ID="pnlRequests" runat="server" CssClass="review-response"
                            Visible='<%# DataBinder.Eval(Container.DataItem, "SpecialRequests") != DBNull.Value %>'>
                            <span class="review-response-label">Special requests</span>
                            <%#: DataBinder.Eval(Container.DataItem, "SpecialRequests") %>
                        </asp:Panel>

                        <!-- Hidden: the buttons' ItemCommand reads the ReservationID from here -->
                        <asp:Label ID="lblReservationID" runat="server" Visible="false"
                            Text='<%# DataBinder.Eval(Container.DataItem, "ReservationID") %>' />

                        <div class="review-actions">
                            <asp:Button ID="btnConfirm" runat="server" Text="Confirm" CommandName="ConfirmReservation" CssClass="btn btn-success btn-sm"
                                Visible='<%# DataBinder.Eval(Container.DataItem, "Status").ToString() == "Pending" %>' />
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CommandName="EditReservation" CssClass="btn btn-dark btn-sm" />
                            <asp:Button ID="btnDelete" runat="server" Text="Cancel" CommandName="DeleteReservation" CssClass="btn btn-outline-danger btn-sm"
                                OnClientClick="return confirm('Cancel this reservation? The customer will be emailed.');" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </main>
</asp:Content>
