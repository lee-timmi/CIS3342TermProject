<%@ Page Title="My Restaurant | Honest Plates" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageRestaurant.aspx.cs" Inherits="RestaurantReview.ManageRestaurant" ValidateRequestMode="Disabled" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

        <main class="container my-5" style="max-width: 900px;">
            <h2 class="mb-1"><asp:Label ID="lblHeading" runat="server" Text="Add Your Restaurant" /></h2>
            <p class="text-muted mb-4">
                <asp:Label ID="lblInstructions" runat="server"
                    Text="Fill in your restaurant's details. Fields marked * are required. You can add photos after you create it." />
                <asp:HyperLink ID="lnkPublicPage" runat="server" Visible="false" Text="View your public page &rarr;" CssClass="ms-2" />
            </p>

            <asp:Label ID="lblMessage" runat="server" Visible="false" />

            <!-- Basic info -->
            <h4 class="mt-4 mb-3">Basic Info</h4>
            <div class="row">
                <div class="col-md-8 mb-3">
                    <label class="form-label fw-bold">Restaurant Name *</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="col-md-4 mb-3">
                    <label class="form-label fw-bold">Cuisine *</label>
                    <asp:DropDownList ID="ddlCuisine" runat="server" CssClass="form-select" />
                </div>
            </div>
            <div class="mb-3">
                <label class="form-label fw-bold">Description <span class="fw-normal text-muted">(what makes your place special, up to 2,000 characters)</span></label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
            </div>
            <div class="mb-3">
                <label class="form-label fw-bold">Owner</label>
                <asp:TextBox ID="txtOwner" runat="server" CssClass="form-control" MaxLength="100" />
            </div>

            <!-- Address -->
            <h4 class="mt-4 mb-3">Address</h4>
            <div class="mb-3">
                <label class="form-label fw-bold">Street *</label>
                <asp:TextBox ID="txtStreet" runat="server" CssClass="form-control" MaxLength="200" placeholder="e.g. 325 Chestnut Street" />
            </div>
            <div class="row">
                <div class="col-md-6 mb-3">
                    <label class="form-label fw-bold">City *</label>
                    <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" MaxLength="100" />
                </div>
                <div class="col-md-3 mb-3">
                    <label class="form-label fw-bold">State *</label>
                    <asp:DropDownList ID="ddlState" runat="server" CssClass="form-select" />
                </div>
                <div class="col-md-3 mb-3">
                    <label class="form-label fw-bold">ZIP *</label>
                    <asp:TextBox ID="txtZip" runat="server" CssClass="form-control" MaxLength="5" placeholder="5 digits" />
                </div>
            </div>

            <!-- Contact -->
            <h4 class="mt-4 mb-3">Contact &amp; Links</h4>
            <div class="row">
                <div class="col-md-4 mb-3">
                    <label class="form-label fw-bold">Phone</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" MaxLength="20" placeholder="e.g. 215-555-0123" />
                </div>
                <div class="col-md-8 mb-3">
                    <label class="form-label fw-bold">Website</label>
                    <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control" MaxLength="200" placeholder="https://..." />
                </div>
            </div>
            <div class="mb-3">
                <label class="form-label fw-bold">Social Media</label>
                <asp:TextBox ID="txtSocialMedia" runat="server" CssClass="form-control" MaxLength="200" placeholder="https://www.instagram.com/..." />
            </div>

            <!-- Hours -->
            <h4 class="mt-4 mb-1">Hours</h4>
            <p class="text-muted small mb-3">Check "Closed" for days you're not open. A closing time earlier than the opening time means you close after midnight.</p>
            <div class="row fw-bold small text-muted mb-1">
                <div class="col-3">Day</div>
                <div class="col-3">Opens</div>
                <div class="col-3">Closes</div>
                <div class="col-3"></div>
            </div>
            <asp:Repeater ID="rptHours" runat="server">
                <ItemTemplate>
                    <div class="row align-items-center mb-2">
                        <div class="col-3 fw-bold"><%# Container.DataItem %></div>
                        <div class="col-3"><asp:DropDownList ID="ddlOpen" runat="server" CssClass="form-select form-select-sm" /></div>
                        <div class="col-3"><asp:DropDownList ID="ddlClose" runat="server" CssClass="form-select form-select-sm" /></div>
                        <div class="col-3"><asp:CheckBox ID="chkClosed" runat="server" Text="&nbsp;Closed" /></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <div class="mt-4">
                <asp:Button ID="btnSave" runat="server" Text="Create Restaurant" CssClass="btn btn-dark" OnClick="btnSave_Click" />
                <a href="Dashboard.aspx" class="btn btn-link">Back to Dashboard</a>
            </div>

            <!-- Photos (only once the restaurant exists) -->
            <asp:Panel ID="pnlPhotos" runat="server" Visible="false">
                <hr class="my-5" />
                <h4 class="mb-1">Photos</h4>
                <p class="text-muted small mb-3">
                    JPG, PNG, or GIF, up to 1 MB each. You get one <b>Logo</b> (shown in search results) and one
                    <b>Profile</b> photo (the big photo on your page). Uploading a new one replaces the old one.
                    Add as many <b>Gallery</b> photos as you like.
                </p>

                <asp:Label ID="lblPhotoMessage" runat="server" Visible="false" />

                <div class="row g-2 align-items-end mb-4">
                    <div class="col-md-4">
                        <label class="form-label fw-bold">Photo</label>
                        <asp:FileUpload ID="fuPhoto" runat="server" CssClass="form-control" accept="image/*" />
                    </div>
                    <div class="col-md-2">
                        <label class="form-label fw-bold">Type</label>
                        <asp:DropDownList ID="ddlPhotoCategory" runat="server" CssClass="form-select">
                            <asp:ListItem Value="Gallery">Gallery</asp:ListItem>
                            <asp:ListItem Value="Logo">Logo</asp:ListItem>
                            <asp:ListItem Value="Profile">Profile</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label fw-bold">Caption <span class="fw-normal text-muted">(optional)</span></label>
                        <asp:TextBox ID="txtCaption" runat="server" CssClass="form-control" MaxLength="200" />
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-dark w-100" OnClick="btnUpload_Click" />
                    </div>
                </div>

                <!-- Photo cards: a Repeater with a Delete button per card (Prof. Pascucci's Repeater Example 3) -->
                <asp:Repeater ID="rptPhotos" runat="server" OnItemCommand="rptPhotos_ItemCommand">
                    <HeaderTemplate>
                        <div class="row g-3">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="col-sm-6 col-md-4">
                            <div class="card h-100">
                                <img src='ImageGrab.aspx?ID=<%# DataBinder.Eval(Container.DataItem, "ImageID") %>'
                                     alt='<%#: DataBinder.Eval(Container.DataItem, "Caption") %>'
                                     class="card-img-top" style="height: 180px; object-fit: cover;" />
                                <div class="card-body">
                                    <span class="badge bg-secondary mb-2"><%# DataBinder.Eval(Container.DataItem, "ImageCategory") %></span>
                                    <p class="card-text small"><%#: DataBinder.Eval(Container.DataItem, "Caption") %></p>
                                    <!-- Hidden: the Delete button's ItemCommand reads the ImageID from here -->
                                    <asp:Label ID="lblImageID" runat="server" Visible="false"
                                        Text='<%# DataBinder.Eval(Container.DataItem, "ImageID") %>' />
                                    <asp:Button ID="btnDeletePhoto" runat="server" Text="Delete" CssClass="btn btn-outline-danger btn-sm"
                                        CommandName="DeletePhoto" OnClientClick="return confirm('Delete this photo?');" />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoPhotos" runat="server" CssClass="text-muted" Visible="false"
                    Text="No photos yet. Start with your logo." />
            </asp:Panel>
        </main>
</asp:Content>
