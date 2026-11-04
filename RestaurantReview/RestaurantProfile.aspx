<%@ Page Title="Restaurant Profile — Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RestaurantProfile.aspx.cs" Inherits="RestaurantReview.RestaurantProfile" %>

<%-- Profile styles (Bootstrap, fonts and Home.css come from Site.Master) --%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="style/Profile.css?v=2" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%-- Error label (shown if invalid ID) --%>
    <asp:Label ID="lblError" runat="server" CssClass="text-danger d-block text-center mt-4" Visible="false"></asp:Label>

    <%-- Restaurant Hero --%>
    <div class="profile-hero">
        <asp:Image ID="imgRestaurant" runat="server" CssClass="profile-hero-img" />
        <div class="profile-hero-overlay">
            <%-- Logo photo (only when the rep uploaded one) --%>
            <asp:Image ID="imgLogo" runat="server" CssClass="profile-logo" Visible="false" />
            <span class="profile-cuisine-tag">
                <asp:Label ID="lblCuisine" runat="server"></asp:Label>
            </span>
            <h1 class="profile-name">
                <asp:Label ID="lblName" runat="server"></asp:Label>
            </h1>
        </div>
    </div>

    <%-- Main content --%>
    <div class="profile-body">

        <%-- Info grid --%>
        <div class="profile-info-grid">
            <div class="profile-about">
                <h2 class="section-heading">About</h2>
                <p class="profile-description">
                    <asp:Label ID="lblDescription" runat="server"></asp:Label>
                </p>
            </div>

            <aside class="profile-details">
                <div class="detail-item">
                    <span class="detail-label">Address</span>
                    <span class="detail-value">
                        <asp:Label ID="lblAddress" runat="server"></asp:Label>
                    </span>
                </div>
                <asp:Panel ID="pnlPhone" runat="server" CssClass="detail-item">
                    <span class="detail-label">Phone</span>
                    <span class="detail-value">
                        <asp:Label ID="lblPhone" runat="server"></asp:Label>
                    </span>
                </asp:Panel>
                <div class="detail-item">
                    <span class="detail-label">Hours</span>
                    <span class="detail-value">
                        <asp:Literal ID="litHours" runat="server" />
                    </span>
                </div>
                <asp:Panel ID="pnlOwner" runat="server" CssClass="detail-item">
                    <span class="detail-label">Owner</span>
                    <span class="detail-value">
                        <asp:Label ID="lblOwner" runat="server"></asp:Label>
                    </span>
                </asp:Panel>
                <%-- Anyone can reserve, even without an account (Session 10) --%>
                <div class="detail-item">
                    <asp:HyperLink ID="lnkReserve" runat="server" CssClass="btn btn-dark btn-sm w-100" Text="Make a Reservation" />
                </div>
                <%-- Links (each one hidden when the restaurant doesn't have it) --%>
                <asp:Panel ID="pnlWebsite" runat="server" CssClass="detail-item" Visible="false">
                    <span class="detail-label">Website</span>
                    <span class="detail-value">
                        <asp:HyperLink ID="lnkWebsite" runat="server" Target="_blank" Text="Visit website &rarr;" />
                    </span>
                </asp:Panel>
                <asp:Panel ID="pnlSocialMedia" runat="server" CssClass="detail-item" Visible="false">
                    <span class="detail-label">Social Media</span>
                    <span class="detail-value">
                        <asp:HyperLink ID="lnkSocialMedia" runat="server" Target="_blank" Text="Follow us &rarr;" />
                    </span>
                </asp:Panel>
            </aside>
        </div>

        <%-- Photo gallery: the HTML is built in C# (Prof. Pascucci's Dynamic Display Example 1) --%>
        <asp:Panel ID="pnlGallery" runat="server" Visible="false">
            <hr class="profile-divider" />
            <h2 class="section-heading">Photos</h2>
            <div id="divGallery" runat="server" class="gallery-grid"></div>
        </asp:Panel>

        <hr class="profile-divider" />

        <%-- Reviews --%>
        <h2 class="section-heading">Reviews</h2>

        <%-- Write a Review: Reviewers get the button, visitors get a log-in link, reps see nothing --%>
        <asp:Panel ID="pnlWriteReview" runat="server" CssClass="write-review" Visible="false">
            <asp:HyperLink ID="lnkWriteReview" runat="server" CssClass="btn btn-dark btn-sm" Text="Write a Review" />
        </asp:Panel>
        <asp:Panel ID="pnlLoginToReview" runat="server" CssClass="write-review" Visible="false">
            <asp:HyperLink ID="lnkLoginToReview" runat="server" Text="Log in" /> or
            <asp:HyperLink ID="lnkRegisterToReview" runat="server" Text="create an account" /> to write a review.
        </asp:Panel>

        <%-- Average ratings summary --%>
        <asp:Label ID="lblNoReviews" runat="server"
            CssClass="no-reviews-msg"
            Visible="false"
            Text="No reviews yet — be the first!" />

        <asp:Panel ID="pnlRatingSummary" runat="server" Visible="false" CssClass="rating-summary">
            <div class="avg-row">
                <div class="avg-item">
                    <span class="avg-label">Overall</span>
                    <span class="avg-value"><asp:Label ID="lblAvgOverall" runat="server" /></span>
                    <span class="avg-max">/5</span>
                </div>
                <div class="avg-item">
                    <span class="avg-label">Food</span>
                    <span class="avg-value"><asp:Label ID="lblAvgFood" runat="server" /></span>
                    <span class="avg-max">/5</span>
                </div>
                <div class="avg-item">
                    <span class="avg-label">Service</span>
                    <span class="avg-value"><asp:Label ID="lblAvgService" runat="server" /></span>
                    <span class="avg-max">/5</span>
                </div>
                <div class="avg-item">
                    <span class="avg-label">Atmosphere</span>
                    <span class="avg-value"><asp:Label ID="lblAvgAtmosphere" runat="server" /></span>
                    <span class="avg-max">/5</span>
                </div>
                <div class="avg-item">
                    <span class="avg-label">Price</span>
                    <span class="avg-value"><asp:Label ID="lblAvgPrice" runat="server" /></span>
                </div>
            </div>
            <p class="avg-count">Based on <asp:Label ID="lblReviewCount" runat="server" /> review(s)</p>
        </asp:Panel>

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

                    <%-- Review photo (Session 9): sent by ImageGrab.aspx?ReviewID=... --%>
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

                    <%-- The rep's public response (written in Session 11) --%>
                    <asp:Panel ID="pnlResponse" runat="server" CssClass="review-response"
                        Visible='<%# DataBinder.Eval(Container.DataItem, "ResponseText") != DBNull.Value %>'>
                        <span class="review-response-label">Response from the restaurant</span>
                        <%#: DataBinder.Eval(Container.DataItem, "ResponseText") %>
                    </asp:Panel>

                    <%-- Hidden review ID for ItemCommand --%>
                    <asp:Label ID="lblReviewID" runat="server" Visible="false"
                        Text='<%# DataBinder.Eval(Container.DataItem, "ReviewID") %>'>
                    </asp:Label>

                    <div class="review-actions">
                        <asp:Button ID="btnHelpful" runat="server" CommandName="Helpful"
                            Text='<%# "Helpful (" + DataBinder.Eval(Container.DataItem, "HelpfulCount") + ")" %>'
                            CssClass="btn-vote btn-helpful" />
                        <asp:Button ID="btnNotHelpful" runat="server" CommandName="NotHelpful"
                            Text='<%# "Not Helpful (" + DataBinder.Eval(Container.DataItem, "NotHelpfulCount") + ")" %>'
                            CssClass="btn-vote btn-not-helpful" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </div>
</asp:Content>
