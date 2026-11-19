-- =====================================================================================
-- Fill in restaurants 1-15 (RestaurantID 3-17): profile fixes + menu items
-- Thimmy's part of "Fill in restaurants 1-15" (issue #70). Run once (Ctrl+Shift+E).
--
-- What it does:
--   1. Fixes 3 profile fields (Tabachoy's name, Cafe Nhan's website, a stray space in Le Virtu's owner).
--   2. Adds 5 menu items to each of the 15 restaurants (75 rows).
--
-- Safe to run again: the menu part is skipped if any of these restaurants already has menu items,
-- and the profile fixes just set the same values again.
--
-- About the menus: the dishes are typical of each restaurant's cuisine and, where known, its
-- signature dishes; prices are approximate. They are sample data for the project, not a copy of
-- each restaurant's current menu. Source = 'Seed' marks them as sample data (reps' own items say 'Rep').
-- Text is plain ASCII on purpose: TP_MenuItems uses VARCHAR, which can't store letters like the
-- Vietnamese "e" in "Hue" (they'd turn into "?").
--
-- Standard T-SQL, not in the textbooks (Microsoft Docs: BEGIN TRANSACTION, SET XACT_ABORT, INSERT ... VALUES).
-- Photos are added separately with the photo loader (3 per restaurant).
-- =====================================================================================

SET XACT_ABORT ON;      -- any error rolls back everything below, so it never half-runs
BEGIN TRANSACTION;

-- ---------- 1. Profile fixes ----------

-- The name was typed "Tabavhoy" in Project 2; its website and reviews say Tabachoy
UPDATE TP_Restaurants SET RestaurantName = 'Tabachoy' WHERE RestaurantID = 13;

-- Cafe Nhan had no website. Its old domain (cafenhanphilly.com) now points to a gambling site,
-- so this uses the restaurant's Philadelphia Inquirer listing instead.
UPDATE TP_Restaurants SET RestaurantWebsite = 'https://www.inquirer.com/food/restaurants/cafe-nhan' WHERE RestaurantID = 8;

-- Le Virtu's owner started with a space
UPDATE TP_Restaurants SET RestaurantOwner = LTRIM(RestaurantOwner) WHERE RestaurantID = 14;

-- ---------- 2. Menu items (5 per restaurant) ----------

IF NOT EXISTS (SELECT 1 FROM TP_MenuItems WHERE RestaurantID BETWEEN 3 AND 17)
BEGIN
    INSERT INTO TP_MenuItems (RestaurantID, Section, ItemName, Description, Price, Source)
    VALUES
    -- 3 Buddakan (Modern Asian)
    (3,  'Starters', 'Edamame Ravioli', 'Edamame dumplings in a shallot-sauternes broth', 19.00, 'Seed'),
    (3,  'Starters', 'Crispy Calamari Salad', 'Fried calamari, greens, miso dressing', 21.00, 'Seed'),
    (3,  'Mains', 'Wasabi Crusted Filet Mignon', 'Filet with wasabi crust and steamed greens', 58.00, 'Seed'),
    (3,  'Mains', 'Lobster Fried Rice', 'Wok-fried rice with lobster, egg and scallion', 39.00, 'Seed'),
    (3,  'Dessert', 'Chocolate Peanut Butter Torte', 'Layered chocolate and peanut butter cake', 15.00, 'Seed'),

    -- 4 SouthGate (Korean gastropub)
    (4,  'Starters', 'Korean Fried Chicken Wings', 'Double-fried wings, soy garlic or spicy glaze', 16.00, 'Seed'),
    (4,  'Starters', 'Kimchi Fries', 'Fries with kimchi, pork belly and gochujang mayo', 13.00, 'Seed'),
    (4,  'Mains', 'Dolsot Bibimbap', 'Rice, vegetables, beef and egg in a hot stone bowl', 22.00, 'Seed'),
    (4,  'Mains', 'Bulgogi Cheesesteak', 'Marinated rib eye, cheese and onions on a roll', 17.00, 'Seed'),
    (4,  'Mains', 'Japchae', 'Sweet potato glass noodles with vegetables', 16.00, 'Seed'),

    -- 5 Almyra Restaurant (Greek)
    (5,  'Starters', 'Spanakopita', 'Spinach and feta in crisp phyllo', 14.00, 'Seed'),
    (5,  'Starters', 'Halloumi Saganaki', 'Pan-seared halloumi with honey and lemon', 16.00, 'Seed'),
    (5,  'Mains', 'Grilled Octopus', 'Charred octopus, fava puree, capers', 26.00, 'Seed'),
    (5,  'Mains', 'Lamb Chops', 'Grilled lamb chops with lemon potatoes', 42.00, 'Seed'),
    (5,  'Dessert', 'Baklava', 'Walnut and honey phyllo pastry', 10.00, 'Seed'),

    -- 6 Dante & Luigi's (Italian)
    (6,  'Starters', 'Fried Calamari', 'With marinara and lemon', 15.00, 'Seed'),
    (6,  'Pasta', 'Gnocchi', 'House potato gnocchi in red gravy', 22.00, 'Seed'),
    (6,  'Mains', 'Veal Parmigiana', 'Breaded veal, marinara, mozzarella, side of pasta', 30.00, 'Seed'),
    (6,  'Mains', 'Braciole', 'Rolled beef braised in gravy', 28.00, 'Seed'),
    (6,  'Dessert', 'Cannoli', 'Crisp shell with sweet ricotta', 9.00, 'Seed'),

    -- 7 Zahav (Israeli, tasting menu: dishes are part of the set price)
    (7,  'Tasting Menu', 'Tasting Menu (per person)', 'Salatim, hummus, mezze and a shared main for the table', 90.00, 'Seed'),
    (7,  'Tasting Menu', 'Hummus Tehina', 'With fresh laffa bread from the taboon oven', NULL, 'Seed'),
    (7,  'Tasting Menu', 'Salatim', 'Six small daily salads', NULL, 'Seed'),
    (7,  'Tasting Menu', 'Pomegranate Lamb Shoulder', 'Smoked and braised, with crispy chickpeas', NULL, 'Seed'),
    (7,  'Tasting Menu', 'Konafi', 'Shredded phyllo with sweet cheese and pistachio', NULL, 'Seed'),

    -- 8 Cafe Nhan (Vietnamese)
    (8,  'Soups', 'Bun Bo Hue', 'Spicy lemongrass beef noodle soup', 17.00, 'Seed'),
    (8,  'Soups', 'Pho Bo', 'Beef broth, rice noodles, herbs', 16.00, 'Seed'),
    (8,  'Sandwiches', 'Banh Mi Thit Nuong', 'Grilled pork, pickled vegetables, cilantro, jalapeno', 11.00, 'Seed'),
    (8,  'Small Plates', 'Fish Sauce Chicken Wings', 'Fish sauce brined and caramelized', 14.00, 'Seed'),
    (8,  'Drinks', 'Ca Phe Sua Da', 'Vietnamese iced coffee with condensed milk', 6.00, 'Seed'),

    -- 9 Alif Brew (Ethiopian cafe)
    (9,  'Coffee', 'Ethiopian Coffee (Buna)', 'Single-origin Ethiopian coffee', 5.00, 'Seed'),
    (9,  'Coffee', 'Spiced Tea (Shai)', 'Black tea with cardamom and cinnamon', 4.00, 'Seed'),
    (9,  'Food', 'Sambusa', 'Crisp pastry filled with spiced lentils', 7.00, 'Seed'),
    (9,  'Food', 'Ful', 'Stewed fava beans with tomato, onion and bread', 13.00, 'Seed'),
    (9,  'Food', 'Chechebsa', 'Torn flatbread with spiced butter and berbere', 12.00, 'Seed'),

    -- 10 June BYOB (French)
    (10, 'Starters', 'Escargots', 'Garlic and parsley butter', 18.00, 'Seed'),
    (10, 'Starters', 'Foie Gras Torchon', 'With brioche and fruit preserves', 24.00, 'Seed'),
    (10, 'Mains', 'Duck Breast', 'Roasted duck, cherry jus, root vegetables', 42.00, 'Seed'),
    (10, 'Mains', 'Pan Roasted Halibut', 'Beurre blanc and seasonal vegetables', 44.00, 'Seed'),
    (10, 'Dessert', 'Chocolate Souffle', 'Baked to order, creme anglaise', 14.00, 'Seed'),

    -- 11 White Yak (Tibetan)
    (11, 'Starters', 'Beef Momo', 'Steamed Tibetan dumplings with chili sauce', 14.00, 'Seed'),
    (11, 'Starters', 'Vegetable Momo', 'Steamed dumplings with cabbage and tofu', 13.00, 'Seed'),
    (11, 'Mains', 'Thenthuk', 'Hand-pulled noodle soup with beef and vegetables', 15.00, 'Seed'),
    (11, 'Mains', 'Shapta', 'Stir-fried beef with peppers and onions', 18.00, 'Seed'),
    (11, 'Drinks', 'Po Cha (Butter Tea)', 'Traditional salted butter tea', 4.00, 'Seed'),

    -- 12 Sang Kee (Cantonese)
    (12, 'Roast Meats', 'Roast Duck (Half)', 'Cantonese-style roast duck', 22.00, 'Seed'),
    (12, 'Roast Meats', 'BBQ Roast Pork', 'Char siu with honey glaze', 15.00, 'Seed'),
    (12, 'Soups', 'Wonton Noodle Soup', 'Shrimp and pork wontons, egg noodles', 12.00, 'Seed'),
    (12, 'Noodles', 'Beef Chow Fun', 'Wide rice noodles stir-fried with beef', 16.00, 'Seed'),
    (12, 'Specials', 'Peking Duck', 'Whole duck with pancakes, scallion and hoisin', 48.00, 'Seed'),

    -- 13 Tabachoy (Filipino)
    (13, 'Silog', 'Longsilog', 'Sweet longganisa sausage, garlic rice, fried egg', 16.00, 'Seed'),
    (13, 'Mains', 'Chicken Adobo', 'Braised in soy, vinegar, garlic and bay leaf', 18.00, 'Seed'),
    (13, 'Mains', 'Pancit Canton', 'Stir-fried egg noodles with vegetables and pork', 16.00, 'Seed'),
    (13, 'Starters', 'Lumpia Shanghai', 'Crispy pork spring rolls, sweet chili sauce', 10.00, 'Seed'),
    (13, 'Dessert', 'Ube Turon', 'Fried banana and ube spring roll', 8.00, 'Seed'),

    -- 14 Le Virtu (Italian, Abruzzo)
    (14, 'Starters', 'Arrosticini', 'Grilled lamb skewers, Abruzzo style', 18.00, 'Seed'),
    (14, 'Starters', 'Scrippelle mbusse', 'Pecorino crepes in chicken broth', 15.00, 'Seed'),
    (14, 'Pasta', 'Maccheroni alla Chitarra', 'Square-cut pasta with lamb ragu', 26.00, 'Seed'),
    (14, 'Mains', 'Porchetta', 'Herb-roasted pork with potatoes', 34.00, 'Seed'),
    (14, 'Dessert', 'Torta di Ricotta', 'Ricotta cheesecake with citrus', 12.00, 'Seed'),

    -- 15 Mish Mish (Mediterranean)
    (15, 'Mezze', 'Hummus', 'Tahini, olive oil, warm pita', 14.00, 'Seed'),
    (15, 'Mezze', 'Fattoush', 'Chopped salad, sumac, crispy pita', 14.00, 'Seed'),
    (15, 'Mains', 'Lamb Kofta', 'Grilled spiced lamb, yogurt, herbs', 26.00, 'Seed'),
    (15, 'Mains', 'Mujadara', 'Lentils and rice with caramelized onions', 18.00, 'Seed'),
    (15, 'Dessert', 'Halva Sundae', 'Tahini halva over vanilla ice cream', 11.00, 'Seed'),

    -- 16 Little Fish (Seafood BYOB)
    (16, 'Starters', 'Tuna Crudo', 'Citrus, chili, olive oil', 18.00, 'Seed'),
    (16, 'Mains', 'Seared Scallops', 'Brown butter and seasonal vegetables', 34.00, 'Seed'),
    (16, 'Mains', 'Whole Roasted Branzino', 'Lemon, herbs, olive oil', 38.00, 'Seed'),
    (16, 'Pasta', 'Linguine with Clams', 'White wine, garlic, parsley', 28.00, 'Seed'),
    (16, 'Dessert', 'Olive Oil Cake', 'With whipped cream and berries', 11.00, 'Seed'),

    -- 17 Vernick Fish (Seafood)
    (17, 'Raw Bar', 'Oysters (Half Dozen)', 'East coast oysters, mignonette', 24.00, 'Seed'),
    (17, 'Raw Bar', 'Yellowtail Crudo', 'Pomelo and serrano', 22.00, 'Seed'),
    (17, 'Mains', 'Roasted Whole Dorade', 'For one, with herbs and lemon', 52.00, 'Seed'),
    (17, 'Mains', 'Lobster Spaghetti', 'Butter-poached lobster, chili, garlic', 48.00, 'Seed'),
    (17, 'Dessert', 'Coconut Cream Pie', 'Toasted coconut, whipped cream', 14.00, 'Seed');
END;

COMMIT TRANSACTION;

-- ---------- Check ----------
-- Should show 15 rows, each with MenuItems = 5, and Tabachoy / the new Cafe Nhan website.
SELECT r.RestaurantID, r.RestaurantName, r.RestaurantWebsite,
       (SELECT COUNT(*) FROM TP_MenuItems m WHERE m.RestaurantID = r.RestaurantID) AS MenuItems
FROM TP_Restaurants r
WHERE r.RestaurantID BETWEEN 3 AND 17
ORDER BY r.RestaurantID;
