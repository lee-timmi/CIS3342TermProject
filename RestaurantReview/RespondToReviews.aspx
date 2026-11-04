<%@ Page Title="Respond to Reviews | Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RespondToReviews.aspx.cs" Inherits="RestaurantReview.RespondToReviews" ValidateRequestMode="Disabled" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="style/Profile.css?v=2" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <main class="container my-5" style="max-width: 900px;">
            <h2 class="mb-1">Respond to Reviews</h2>
            <p class="text-muted mb-4">
                Your response shows under the review on your public page. Reviews you haven't answered yet are listed first.
                To remove a response, clear the box and click Save Response.
                <asp:HyperLink ID="lnkPublicPage" runat="server" Visible="false" Text="View your public page &rarr;" CssClass="ms-1" />
            </p>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />
            <asp:Label ID="lblNoReviews" runat="server" CssClass="no-reviews-msg" Visible="false"
                Text="Your restaurant has no reviews yet." />

            <!-- One card per review with a response box (Prof. Pascucci's Repeater Example 3: button + ItemCommand + FindControl) -->
            <asp:Repeater ID="rptReviews" runat="server" OnItemCommand="rptReviews_ItemCommand">
                <ItemTemplate>
                    <div class="review-card">
                        <div class="review-header">
                            <span class="review-author"><%#: DataBinder.Eval(Container.DataItem, "ReviewerName") %></span>
                            <span class="review-date"><%# DataBinder.Eval(Container.DataItem, "ReviewDate", "{0:MMMM d, yyyy}") %></span>
                        </div>
                        <asp:Panel ID="pnlVisit" runat="server" CssClass="review-visit"
                            Visible='<%# DataBinder.Eval(Container.DataItem, "VisitDateTime") != DBNull.Value %>'>
                            Visited <%# DataBinder.Eval(Container.DataItem, "VisitDateTime", "{0:MMMM d, yyyy 'at' h:mm tt}") %>
                        </asp:Panel>

                        <p class="review-text"><%#: DataBinder.Eval(Container.DataItem, "ReviewText") %></p>

                        <asp:Panel ID="pnlPhoto" runat="server"
                            Visible='<%# DataBinder.Eval(Container.DataItem, "HasImage").ToString() == "1" %>'>
                            <img class="review-photo" src='ImageGrab.aspx?ReviewID=<%# DataBinder.Eval(Container.DataItem, "ReviewID") %>' alt="Photo from this review" />
                        </asp:Panel>

                        <div class="review-ratings">
                            <span class="rating-pill">Food <strong><%# DataBinder.Eval(Container.DataItem, "FoodRating") %></strong></span>
                            <span class="rating-pill">Service <strong><%# DataBinder.Eval(Container.DataItem, "ServiceRating") %></strong></span>
                            <asp:Panel ID="pnlAtmosphere" runat="server" CssClass="rating-pill"
                                Visible='<%# DataBinder.Eval(Container.DataItem, "AtmosphereRating") != DBNull.Value %>'>
                                Atmosphere <strong><%# DataBinder.Eval(Container.DataItem, "AtmosphereRating") %></strong>
                            </asp:Panel>
                            <span class="rating-pill">Price <strong><%# RestaurantReview.Rating.Dollars(Convert.ToInt32(DataBinder.Eval(Container.DataItem, "PriceRating"))) %></strong></span>
                        </div>

                        <div class="review-response">
                            <span class="review-response-label">
                                Your response
                                <%# DataBinder.Eval(Container.DataItem, "ResponseDate") == DBNull.Value ? "(not answered yet)" : DataBinder.Eval(Container.DataItem, "ResponseDate", "(posted {0:MMMM d, yyyy})") %>
                            </span>
                            <asp:TextBox ID="txtResponse" runat="server" CssClass="form-control mb-2" TextMode="MultiLine" Rows="3"
                                Text='<%# DataBinder.Eval(Container.DataItem, "ResponseText") %>' />
                            <!-- Hidden: the button's ItemCommand reads the ReviewID from here -->
                            <asp:Label ID="lblReviewID" runat="server" Visible="false"
                                Text='<%# DataBinder.Eval(Container.DataItem, "ReviewID") %>' />
                            <asp:Button ID="btnSaveResponse" runat="server" Text="Save Response" CommandName="SaveResponse" CssClass="btn btn-dark btn-sm" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </main>
</asp:Content>
