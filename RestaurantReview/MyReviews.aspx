<%@ Page Title="My Reviews | Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyReviews.aspx.cs" Inherits="RestaurantReview.MyReviews" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="style/Profile.css?v=2" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <main class="container my-5" style="max-width: 900px;">
            <h2 class="mb-1">My Reviews</h2>
            <p class="text-muted mb-4">Only you can see this list. To write a new review, <a href="Home.aspx">find a restaurant</a> and click "Write a Review".</p>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />
            <asp:Label ID="lblNoReviews" runat="server" CssClass="no-reviews-msg" Visible="false"
                Text="You haven't written any reviews yet." />

            <!-- One card per review, with Edit + Delete (Prof. Pascucci's Repeater Example 3: button + ItemCommand + FindControl) -->
            <asp:Repeater ID="rptMyReviews" runat="server" OnItemCommand="rptMyReviews_ItemCommand">
                <ItemTemplate>
                    <div class="review-card">
                        <div class="review-header">
                            <a class="review-author" href='RestaurantProfile.aspx?id=<%# DataBinder.Eval(Container.DataItem, "RestaurantID") %>'><%#: DataBinder.Eval(Container.DataItem, "RestaurantName") %></a>
                            <span class="review-date">Posted <%# DataBinder.Eval(Container.DataItem, "ReviewDate", "{0:MMMM d, yyyy}") %></span>
                        </div>

                        <asp:Panel ID="pnlVisit" runat="server" CssClass="review-visit"
                            Visible='<%# DataBinder.Eval(Container.DataItem, "VisitDateTime") != DBNull.Value %>'>
                            Visited <%# DataBinder.Eval(Container.DataItem, "VisitDateTime", "{0:MMMM d, yyyy 'at' h:mm tt}") %>
                        </asp:Panel>

                        <p class="review-text"><%#: DataBinder.Eval(Container.DataItem, "ReviewText") %></p>

                        <asp:Panel ID="pnlPhoto" runat="server"
                            Visible='<%# DataBinder.Eval(Container.DataItem, "HasImage").ToString() == "1" %>'>
                            <img class="review-photo" src='ImageGrab.aspx?ReviewID=<%# DataBinder.Eval(Container.DataItem, "ReviewID") %>' alt="Review photo" />
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

                        <asp:Panel ID="pnlResponse" runat="server" CssClass="review-response"
                            Visible='<%# DataBinder.Eval(Container.DataItem, "ResponseText") != DBNull.Value %>'>
                            <span class="review-response-label">Response from the restaurant</span>
                            <%#: DataBinder.Eval(Container.DataItem, "ResponseText") %>
                        </asp:Panel>

                        <!-- Hidden: the Delete button's ItemCommand reads the ReviewID from here -->
                        <asp:Label ID="lblReviewID" runat="server" Visible="false"
                            Text='<%# DataBinder.Eval(Container.DataItem, "ReviewID") %>' />

                        <div class="review-actions">
                            <a class="btn btn-dark btn-sm" href='ReviewForm.aspx?ReviewID=<%# DataBinder.Eval(Container.DataItem, "ReviewID") %>'>Edit</a>
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="DeleteReview"
                                CssClass="btn btn-outline-danger btn-sm" OnClientClick="return confirm('Delete this review?');" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </main>
</asp:Content>
