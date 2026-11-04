<%@ Page Title="Dashboard | Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="RestaurantReview.Dashboard" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <main class="container my-5">
        <h2 class="mb-1">Welcome back, <asp:Label ID="lblFirstName" runat="server" />!</h2>
        <p class="text-muted mb-4">You're logged in as a <asp:Label ID="lblUserType" runat="server" />. Here's what you can do.</p>

        <!-- Reviewer tools -->
        <asp:Panel ID="pnlReviewer" runat="server" Visible="false">
            <div class="row g-4">
                <div class="col-md-6">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">Find a Restaurant</h5>
                            <p class="card-text text-muted">Search by cuisine and location, then write a review.</p>
                            <a href="Home.aspx" class="btn-hp">Search</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">My Reviews</h5>
                            <p class="card-text text-muted">Edit or delete the reviews you've written.</p>
                            <a href="MyReviews.aspx" class="btn btn-dark btn-sm">My reviews</a>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Representative tools -->
        <asp:Panel ID="pnlRepresentative" runat="server" Visible="false">
            <div class="row g-4">
                <div class="col-md-6">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">My Restaurant</h5>
                            <p class="card-text text-muted">Add or edit your restaurant's profile and photos.</p>
                            <a href="ManageRestaurant.aspx" class="btn btn-dark btn-sm">Manage my restaurant</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">Reservations</h5>
                            <p class="card-text text-muted">View, confirm, change, or cancel reservations.</p>
                            <a href="ManageReservations.aspx" class="btn btn-dark btn-sm">View reservations</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">Reviews</h5>
                            <p class="card-text text-muted">Read your restaurant's reviews and post responses.</p>
                            <a href="RespondToReviews.aspx" class="btn btn-dark btn-sm">Respond to reviews</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">Find a Restaurant</h5>
                            <p class="card-text text-muted">Search by cuisine and location, then review another restaurant.</p>
                            <a href="Home.aspx" class="btn-hp">Search</a>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card h-100">
                        <div class="card-body">
                            <h5 class="card-title">My Reviews</h5>
                            <p class="card-text text-muted">Edit or delete the reviews you've written for other restaurants.</p>
                            <a href="MyReviews.aspx" class="btn btn-dark btn-sm">My reviews</a>
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </main>
</asp:Content>
