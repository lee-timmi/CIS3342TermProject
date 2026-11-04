<%@ Page Title="Write a Review | Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReviewForm.aspx.cs" Inherits="RestaurantReview.ReviewForm" ValidateRequestMode="Disabled" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <main class="container my-5" style="max-width: 800px;">
            <h2 class="mb-1"><asp:Label ID="lblHeading" runat="server" Text="Write a Review" /></h2>
            <p class="text-muted mb-4">
                <asp:HyperLink ID="lnkRestaurant" runat="server" CssClass="fw-bold" />
                <asp:Label ID="lblLocation" runat="server" />
            </p>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />

            <asp:Panel ID="pnlForm" runat="server">
                <p class="text-muted small">Rate each part of your visit, tell other diners about it, and add a photo if you like. Fields marked * are required.</p>

                <!-- Ratings -->
                <h4 class="mt-4 mb-3">Your Ratings</h4>
                <div class="row">
                    <div class="col-md-6 mb-3">
                        <label class="form-label fw-bold d-block">Food *</label>
                        <asp:RadioButtonList ID="rblFood" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-choice" />
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label fw-bold d-block">Service *</label>
                        <asp:RadioButtonList ID="rblService" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-choice" />
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label fw-bold d-block">Atmosphere *</label>
                        <asp:RadioButtonList ID="rblAtmosphere" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-choice" />
                    </div>
                    <div class="col-md-6 mb-3">
                        <label class="form-label fw-bold d-block">Price * <span class="fw-normal text-muted">($ = cheap, $$$$$ = most expensive)</span></label>
                        <asp:RadioButtonList ID="rblPrice" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" CssClass="rating-choice" />
                    </div>
                </div>

                <!-- Visit -->
                <h4 class="mt-4 mb-3">Your Visit</h4>
                <div class="row">
                    <div class="col-md-7 mb-3">
                        <label class="form-label fw-bold d-block">Date of your visit *</label>
                        <asp:Calendar ID="calVisit" runat="server" CssClass="table table-sm table-bordered bg-white"
                            SelectedDayStyle-BackColor="#c43030" SelectedDayStyle-ForeColor="White" />
                    </div>
                    <div class="col-md-5 mb-3">
                        <label class="form-label fw-bold">Time *</label>
                        <asp:DropDownList ID="ddlVisitTime" runat="server" CssClass="form-select" />
                    </div>
                </div>

                <!-- Comments -->
                <div class="mb-3">
                    <label class="form-label fw-bold">Your Review * <span class="fw-normal text-muted">(up to 4,000 characters)</span></label>
                    <asp:TextBox ID="txtReviewText" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" />
                </div>

                <!-- Photo -->
                <h4 class="mt-4 mb-1">Photo <span class="fs-6 fw-normal text-muted">(optional)</span></h4>
                <p class="text-muted small mb-2">JPG, PNG, or GIF, up to 1 MB. Choosing a new photo replaces the old one.</p>
                <asp:Panel ID="pnlCurrentPhoto" runat="server" Visible="false" CssClass="mb-2">
                    <asp:Image ID="imgCurrentPhoto" runat="server" CssClass="rounded border" Style="max-height: 160px;" />
                    <div class="mt-1"><asp:CheckBox ID="chkRemovePhoto" runat="server" Text="&nbsp;Remove this photo" /></div>
                </asp:Panel>
                <asp:FileUpload ID="fuPhoto" runat="server" CssClass="form-control" accept="image/*" />

                <div class="mt-4">
                    <asp:Button ID="btnSave" runat="server" Text="Post Review" CssClass="btn btn-dark" OnClick="btnSave_Click" />
                    <a href="MyReviews.aspx" class="btn btn-link">Cancel</a>
                </div>
            </asp:Panel>
        </main>
</asp:Content>
