-- =====================================================================================
-- Add 2 more reviews to every restaurant (32 restaurants, 64 reviews). Run once (Ctrl+Shift+E).
--
-- Sample data for the project: the reviewers are made-up people (no accounts, like the Project 2
-- reviews: UserID NULL + ReviewerName). A mix of positive, negative and MIXED reviews ("great food
-- but slow service"), in different styles: professional, casual, and one-sentence.
--
-- Review dates are spread over Dec 2025 - Nov 2026, so the rating trend chart has months to show.
-- Sentiment is left NULL on purpose: run HonestPlates.SentimentTrainer afterwards and
-- Review.LabelMissingReviews() labels all 64 with the ML.NET model.
--
-- PriceRating works like the existing data: 1 = cheap ($) ... 5 = very expensive ($$$$$).
-- Text is plain ASCII (VARCHAR column).
--
-- Safe to run again: skipped if the first review below is already in the table.
-- Standard T-SQL, not in the textbooks (Microsoft Docs: BEGIN TRANSACTION, SET XACT_ABORT, INSERT ... VALUES).
-- =====================================================================================

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM TP_Reviews WHERE ReviewerName = 'Nina Alvarez' AND RestaurantID = 3)
BEGIN
    INSERT INTO TP_Reviews (RestaurantID, ReviewerName, ReviewText, FoodRating, ServiceRating, AtmosphereRating, PriceRating, ReviewDate, VisitDateTime)
    VALUES
    -- 3 Buddakan
    (3,  'Nina Alvarez', 'The edamame ravioli lives up to the hype and the room is stunning. Service was a little rushed on a Saturday, but the food more than made up for it.', 5, 3, 5, 4, '2026-03-15 10:12', '2026-03-14 19:30'),
    (3,  'Tom Becker', 'Way too loud, and $39 for fried rice is a stretch.', 3, 3, 2, 4, '2026-08-02 21:05', '2026-08-01 20:00'),

    -- 4 SouthGate
    (4,  'Kevin Lim', 'best korean fried chicken in philly, no contest. kimchi fries are a must', 5, 4, 4, 2, '2025-12-20 23:10', '2025-12-19 21:00'),
    (4,  'Rachel Moore', 'The bibimbap was good and the staff were friendly, but we waited 40 minutes for a table we had reserved.', 4, 2, 3, 2, '2026-06-11 20:30', '2026-06-10 19:00'),

    -- 5 Almyra Restaurant
    (5,  'Sophia Katsaros', 'Grilled octopus was perfect and the baklava tasted homemade. A lovely evening.', 5, 5, 4, 3, '2026-01-25 22:00', '2026-01-24 19:00'),
    (5,  'Brian Walsh', 'Nice place and the food was fine, but nothing really stood out for the price.', 3, 4, 4, 4, '2026-09-13 18:40', '2026-09-12 18:30'),

    -- 6 Dante & Luigi's
    (6,  'Anthony DeMarco', 'Old school South Philly at its best. The gravy is exactly how my grandmother made it.', 5, 5, 4, 2, '2026-02-08 21:15', '2026-02-07 18:00'),
    (6,  'Lauren Chen', 'The veal parm was huge and tasty, but our server forgot our drinks twice and the dining room feels dated.', 4, 2, 2, 2, '2026-07-19 20:20', '2026-07-18 19:30'),

    -- 7 Zahav
    (7,  'Daniel Rosen', 'Worth every penny. The hummus and the lamb shoulder are unforgettable.', 5, 5, 5, 4, '2026-04-05 23:00', '2026-04-04 20:00'),
    (7,  'Megan Foster', 'The food was excellent, but the tasting menu felt rushed and we were out the door in an hour.', 5, 2, 4, 5, '2026-10-18 22:30', '2026-10-17 18:30'),

    -- 8 Cafe Nhan
    (8,  'Linh Tran', 'The bun bo hue is the real deal: spicy, rich, and a huge portion. Will be back.', 5, 4, 3, 1, '2026-01-10 14:00', '2026-01-10 12:15'),
    (8,  'Chris Patel', 'Wings were great.', 4, 4, 3, 1, '2026-05-22 19:45', '2026-05-22 18:00'),

    -- 9 Alif Brew
    (9,  'Hannah Bekele', 'Great Ethiopian coffee and the sambusas are addictive. A cozy spot to work for a few hours.', 5, 5, 4, 1, '2026-02-21 15:30', '2026-02-21 10:00'),
    (9,  'Marcus Reed', 'Coffee was good, but they were out of half the food menu by noon.', 3, 3, 4, 1, '2026-08-29 13:10', '2026-08-29 12:00'),

    -- 10 June BYOB
    (10, 'Elise Martin', 'A special-occasion place done right. The duck was flawless and the souffle was worth the wait.', 5, 5, 5, 4, '2026-03-01 11:00', '2026-02-28 19:00'),
    (10, 'Greg Sullivan', 'Beautiful food, tiny portions, and dinner took nearly three hours. Bring patience along with your wine.', 4, 2, 4, 4, '2026-09-27 10:30', '2026-09-26 18:30'),

    -- 11 White Yak
    (11, 'Tenzin Dorje', 'The momo taste like home. Friendly family and fair prices.', 5, 5, 3, 1, '2025-12-14 20:00', '2025-12-13 18:00'),
    (11, 'Amy Liu', 'The thenthuk was hearty but a bit bland for me, and the place was nearly empty on a Friday night.', 3, 4, 2, 1, '2026-06-27 21:00', '2026-06-26 19:00'),

    -- 12 Sang Kee
    (12, 'Jason Wong', 'Roast duck is still the best in Chinatown. Fast, cheap, and always consistent.', 5, 4, 2, 1, '2026-04-12 13:30', '2026-04-12 12:00'),
    (12, 'Olivia Harris', 'The food was okay, but the tables were sticky and nobody came back to check on us.', 3, 1, 1, 1, '2026-10-04 20:15', '2026-10-03 18:30'),

    -- 13 Tabachoy
    (13, 'Carlo Reyes', 'Longsilog for breakfast is the perfect way to start the day. The ube turon is dangerous.', 5, 5, 4, 2, '2026-02-15 12:00', '2026-02-15 10:30'),
    (13, 'Jess Novak', 'cute spot, adobo was solid, pancit kinda dry', 3, 4, 4, 2, '2026-07-05 16:40', '2026-07-05 13:00'),

    -- 14 Le Virtu
    (14, 'Paolo Ricci', 'The maccheroni alla chitarra with lamb ragu is the best pasta I have had in Philadelphia. Warm, knowledgeable staff.', 5, 5, 5, 4, '2026-05-03 22:45', '2026-05-02 19:30'),
    (14, 'Sarah Kim', 'The garden patio is lovely, but the porchetta was dry and overpriced.', 2, 4, 5, 4, '2026-08-23 21:30', '2026-08-22 19:00'),

    -- 15 Mish Mish
    (15, 'Leila Haddad', 'The hummus was silky and the lamb kofta was full of flavor. Already planning our next visit.', 5, 4, 4, 3, '2026-03-22 20:50', '2026-03-21 18:30'),
    (15, 'Nick Turner', 'Good food, but the music was so loud we could not hear each other.', 4, 3, 1, 3, '2026-11-08 22:00', '2026-11-07 20:00'),

    -- 16 Little Fish
    (16, 'Grace Yoon', 'A tiny BYOB with huge flavor. The scallops were cooked perfectly.', 5, 5, 4, 3, '2026-01-31 21:40', '2026-01-30 19:00'),
    (16, 'Matt Ellis', 'The fish was fresh, but the menu is small and the chairs are uncomfortable.', 4, 3, 2, 3, '2026-06-20 22:10', '2026-06-19 19:30'),

    -- 17 Vernick Fish
    (17, 'Catherine Blake', 'Elegant, polished, and the raw bar is outstanding. A great choice for a business dinner.', 5, 5, 5, 5, '2026-04-26 09:15', '2026-04-24 19:00'),
    (17, 'Ryan Brooks', 'The lobster spaghetti was incredible, but $48 for a small bowl hurt.', 5, 4, 4, 5, '2026-09-06 23:20', '2026-09-05 20:30'),

    -- 18 Le Baja
    (18, 'Carlos Mendez', 'Great tacos and margaritas, with a fun vibe on the weekend.', 4, 4, 5, 2, '2026-05-16 23:50', '2026-05-15 21:00'),
    (18, 'Emily Ross', 'The rooftop is fun, but the food is forgettable and the service was slow.', 2, 2, 4, 3, '2026-10-25 19:30', '2026-10-24 20:00'),

    -- 19 Ogawa
    (19, 'Kenji Watanabe', 'The omakase is some of the best sushi outside New York. Every piece was precise.', 5, 5, 5, 5, '2026-02-28 22:30', '2026-02-27 19:00'),
    (19, 'Diana Lopez', 'Beautiful fish, but the chef barely spoke to us and the whole meal felt rushed.', 5, 2, 3, 5, '2026-07-26 21:00', '2026-07-25 18:00'),

    -- 20 Mayami Wynwood
    (20, 'Luis Ortega', 'Amazing atmosphere and live music, and the guacamole was great.', 4, 4, 5, 3, '2026-01-17 01:10', '2026-01-16 21:30'),
    (20, 'Tara Nguyen', 'More of a party than a restaurant. The drinks were great, the food was just okay.', 3, 3, 4, 3, '2026-08-08 23:45', '2026-08-07 22:00'),

    -- 21 Chimba
    (21, 'Valeria Gomez', 'The arepas and ceviche were fantastic, and the staff made great recommendations.', 5, 5, 4, 3, '2026-03-28 22:00', '2026-03-27 19:30'),
    (21, 'Josh Miller', 'Fun place.', 4, 4, 4, 3, '2026-09-19 20:30', '2026-09-18 20:00'),

    -- 22 Old's Havana Cuban Bar & Cocina
    (22, 'Miguel Fernandez', 'Ropa vieja like in Havana, a live salsa band, and strong mojitos. Loved it.', 5, 4, 5, 2, '2026-04-18 23:30', '2026-04-17 20:00'),
    (22, 'Karen White', 'Tourist trap. A long wait, cold plantains, and the band was so loud we left early.', 2, 2, 2, 3, '2026-11-01 21:15', '2026-10-31 19:00'),

    -- 23 The Arrogant Butcher
    (23, 'Steve Johnson', 'A reliable downtown spot. The burger was great and service was quick before the game.', 4, 5, 4, 3, '2026-05-09 23:00', '2026-05-08 17:30'),
    (23, 'Priya Shah', 'My steak came out overcooked and they were slow to fix it, though the manager was very apologetic.', 2, 3, 4, 4, '2026-10-11 21:40', '2026-10-10 19:00'),

    -- 24 Chelsea's Kitchen
    (24, 'Ashley Turner', 'Love the patio and the green chile dishes. Perfect for brunch.', 5, 4, 5, 3, '2026-02-01 14:20', '2026-02-01 11:00'),
    (24, 'Mark Davis', 'Good food, but Sunday brunch is chaos and we waited over an hour.', 4, 2, 3, 3, '2026-06-14 15:00', '2026-06-14 11:30'),

    -- 25 Second Empire Restaurant & Tavern
    (25, 'Robert Hayes', 'A classic Raleigh fine dining experience. Impeccable service and a great wine list.', 5, 5, 5, 5, '2026-03-08 22:10', '2026-03-07 19:00'),
    (25, 'Julia Price', 'The tavern side is nice and more relaxed, but my entree was lukewarm.', 3, 4, 4, 4, '2026-08-16 20:30', '2026-08-15 18:30'),

    -- 26 State Farmers Market Restaurant
    (26, 'Bill Cooper', 'Biscuits, country ham, and grits the way they should be. Get there early.', 5, 4, 3, 1, '2026-01-04 11:30', '2026-01-04 08:00'),
    (26, 'Natalie Green', 'A huge line and the food was average.', 3, 3, 2, 1, '2026-07-12 12:45', '2026-07-12 09:30'),

    -- 27 Bida Manda Laotian Restaurant And Bar
    (27, 'Sam Vongsa', 'The crispy rice lettuce wraps were incredible. A gem of a restaurant with real heart.', 5, 5, 5, 3, '2026-04-02 22:00', '2026-04-01 19:00'),
    (27, 'Heather Lane', 'Interesting flavors, but a few dishes were much spicier than the menu said.', 4, 4, 4, 3, '2026-09-30 21:20', '2026-09-29 18:30'),

    -- 28 The Chalkboard Kitchen + Bar Tulsa
    (28, 'Ethan Clark', 'A charming little spot with a solid French menu. The mussels were great.', 4, 5, 4, 3, '2026-05-30 21:00', '2026-05-29 18:30'),
    (28, 'Monica Diaz', 'Service was sweet, but the kitchen was out of two dishes we wanted and dessert was dry.', 3, 4, 3, 3, '2026-11-14 20:10', '2026-11-13 19:00'),

    -- 29 Eskina Brazilian Restaurant
    (29, 'Bruno Silva', 'The Saturday feijoada is the real thing. Feels like home.', 5, 4, 3, 2, '2026-02-07 16:00', '2026-02-07 13:00'),
    (29, 'Kelly Adams', 'The food was tasty, but it took forever to get the check.', 4, 2, 3, 2, '2026-07-31 21:30', '2026-07-30 19:30'),

    -- 30 Rasika
    (30, 'Anjali Mehta', 'The crispy spinach palak chaat is a must. One of the best Indian restaurants in the country.', 5, 5, 5, 4, '2026-03-19 23:10', '2026-03-18 19:30'),
    (30, 'David Park', 'Great food, but the tables are packed so close together you hear every conversation.', 5, 4, 2, 4, '2026-10-29 22:00', '2026-10-28 19:00'),

    -- 31 Atomix
    (31, 'Hyun Kim', 'An unforgettable tasting menu. Every course came with a card explaining it, and every bite was thoughtful.', 5, 5, 5, 5, '2026-01-22 23:59', '2026-01-21 20:00'),
    (31, 'Laura Bennett', 'Stunning food, but at this price I expected the pacing to be smoother.', 5, 3, 4, 5, '2026-06-06 23:30', '2026-06-05 20:30'),

    -- 32 Commander's Palace
    (32, 'Beau Thibodeaux', 'Turtle soup, the bread pudding souffle, and 25-cent martinis at lunch. A New Orleans must.', 5, 5, 5, 4, '2026-04-09 16:00', '2026-04-09 12:00'),
    (32, 'Janet Morris', 'A classic room and great service, but the food felt dated for the price.', 3, 5, 4, 5, '2026-09-09 21:00', '2026-09-08 19:00'),

    -- 35 Boba-licious
    (35, 'Tiffany Ho', 'brown sugar milk tea is so good!!', 5, 4, 3, 1, '2026-05-25 17:00', '2026-05-25 15:30'),
    (35, 'Derek Shaw', 'The drinks were too sweet and the tapioca was hard.', 2, 3, 3, 1, '2026-10-20 16:20', '2026-10-20 14:00'),

    -- 36 Chris's Place
    (36, 'Elena Papas', 'A cozy Greek spot with friendly owners. The gyro platter was generous.', 4, 5, 4, 2, '2026-08-11 20:00', '2026-08-10 18:30'),
    (36, 'Mike Russo', 'Decent food, but nothing special.', 3, 3, 3, 2, '2026-11-16 19:30', '2026-11-15 18:00');
END;

COMMIT TRANSACTION;

-- ---------- Check ----------
-- Should show 32 restaurants, each with 2 more reviews than before, and 64 reviews still unlabeled.
SELECT r.RestaurantID, r.RestaurantName, COUNT(v.ReviewID) AS Reviews
FROM TP_Restaurants r
LEFT JOIN TP_Reviews v ON v.RestaurantID = r.RestaurantID
GROUP BY r.RestaurantID, r.RestaurantName
ORDER BY r.RestaurantID;

SELECT COUNT(*) AS NotLabeledYet FROM TP_Reviews WHERE Sentiment IS NULL;
