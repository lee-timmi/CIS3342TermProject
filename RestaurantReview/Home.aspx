<%@ Page Title="Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="RestaurantReview.Home" ValidateRequestMode="Disabled" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%-- Photo hero: images/hero.jpg behind a dark overlay (a warm gradient shows if there's no photo) --%>
    <section id="search" class="hero hero-photo">
        <%-- Headline --%>
        <p class="hero-eyebrow">Find the best</p>
        <h1 class="hero-headline">Restaurants <span>near you</span></h1>
        <p class="hero-sub">Honest ratings for food, service and atmosphere, from people who ate there.</p>

        <%-- Search form --%>
        <div class="search-box">

            <%-- Cuisine pills (CheckBoxList styled as toggle tags) --%>
            <asp:CheckBoxList ID="cblCuisine" runat="server"
                              RepeatDirection="Horizontal"
                              CssClass="cuisine-pills">
                <%-- populated dynamically in Page_Load --%>
            </asp:CheckBoxList>

            <div class="search-row">
                <asp:TextBox ID="txtLocation" runat="server"
                             CssClass="search-input"
                             placeholder="City, State — e.g. Philadelphia, PA; Miami, FL" />
                <asp:Button ID="btnSearch" runat="server"
                            Text="Search"
                            CssClass="btn-hp"
                            OnClick="btnSearch_Click" />
                <asp:Button ID="btnClear" runat="server"
                            Text="Clear"
                            CssClass="btn-hp btn-hp-outline"
                            OnClick="btnClear_Click" />
            </div>
            <p class="search-hint">Pick one or more cuisines, type a city and/or state, or both. Clear starts over.</p>

            <asp:Label ID="lblError" runat="server"
                       CssClass="search-error"
                       Visible="false" />
        </div>

    </section>

    <%-- Results --%>
    <asp:Panel ID="pnlResults" runat="server" Visible="false" CssClass="results-section">
        <div class="results-header">
            <asp:Label ID="lblResultCount" runat="server" CssClass="results-count" />
        </div>
        <div class="results-grid">
            <asp:Repeater ID="rptResults" runat="server">
                <ItemTemplate>
                    <a class="restaurant-card" href='RestaurantProfile.aspx?id=<%# Eval("RestaurantID") %>'>
                        <div class="card-img-wrap">
                            <img src='<%# Eval("imgFile") %>'
                                 alt='<%# Eval("RestaurantName") %>'
                                 onerror="this.style.visibility='hidden'" />
                        </div>
                        <div class="card-body">
                            <h3 class="card-name"><%# Eval("RestaurantName") %></h3>
                            <p class="card-cuisine"><%# Eval("RestaurantCuisine") %></p>
                            <p class="card-location"><%# Eval("RestaurantCity") %>, <%# Eval("RestaurantState") %></p>
                            <p class="card-rating"><%# Eval("RatingText") %></p>
                        </div>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <%-- Top rated: shown before a search (hidden while search results are on screen) --%>
    <asp:Panel ID="pnlTopRated" runat="server" Visible="false" CssClass="results-section">
        <div class="section-heading">
            <p class="section-eyebrow">Diners' favorites</p>
            <h2 class="section-title">Top rated in Philadelphia</h2>
        </div>
        <div class="results-grid top-rated-grid">
            <asp:Repeater ID="rptTopRated" runat="server">
                <ItemTemplate>
                    <a class="restaurant-card" href='RestaurantProfile.aspx?id=<%# Eval("RestaurantID") %>'>
                        <div class="card-img-wrap">
                            <img src='<%# Eval("imgFile") %>'
                                 alt='<%# Eval("RestaurantName") %>'
                                 onerror="this.style.visibility='hidden'" />
                        </div>
                        <div class="card-body">
                            <h3 class="card-name"><%# Eval("RestaurantName") %></h3>
                            <p class="card-cuisine"><%# Eval("RestaurantCuisine") %></p>
                            <p class="card-location"><%# Eval("RestaurantCity") %>, <%# Eval("RestaurantState") %></p>
                            <p class="card-rating"><%# Eval("RatingText") %></p>
                        </div>
                    </a>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:Panel>

    <%-- How it works: one column per kind of user --%>
    <section class="how-it-works">
        <div class="section-heading">
            <p class="section-eyebrow">How it works</p>
            <h2 class="section-title">Something for everyone at the table</h2>
        </div>
        <div class="how-grid">
            <div class="how-card">
                <span class="how-step">1</span>
                <h3>Diners</h3>
                <p>Search by cuisine and city, read honest reviews, and request a table. No account needed.</p>
                <a href="#search" class="btn-hp btn-hp-outline">Find a restaurant</a>
            </div>
            <div class="how-card">
                <span class="how-step">2</span>
                <h3>Reviewers</h3>
                <p>Rate the food, service, atmosphere and price, add a photo, and edit your reviews any time.</p>
                <asp:HyperLink ID="lnkJoinReviewer" runat="server" NavigateUrl="Register.aspx" CssClass="btn-hp btn-hp-outline" Text="Create an account" />
            </div>
            <div class="how-card">
                <span class="how-step">3</span>
                <h3>Restaurant owners</h3>
                <p>Add your restaurant's profile, hours and photos, manage reservations, and answer reviews.</p>
                <asp:HyperLink ID="lnkJoinRep" runat="server" NavigateUrl="Register.aspx" CssClass="btn-hp btn-hp-outline" Text="Add your restaurant" />
            </div>
        </div>
    </section>
</asp:Content>
