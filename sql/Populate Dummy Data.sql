USE expenses;

-- For now, we'll hard code our names until we have the ability to add them in the application
INSERT INTO payees
	(payee_name)
VALUES
	('Chauncey'), ('Josh');

-- Same with locations
INSERT INTO locations
	(location_name)
VALUES
	('Walmart'), ('Kroger'), ('Sam''s Club'), ('Target'), ('Old Time Pottery'),
    ('Pacific Spice'), ('Total Wine'), ('McDonalds'), ('Wings & Things'),
    ('Chick-Fil-A'), ('TicketMaster'), ('HelloFresh'), ('???');

-- Inserting a few 
INSERT INTO dynamic_expenses
	(payee_id, location_id, `date`, amount)
VALUES
	(1, 1, "2023-03-03", 9.98),
    (2, 1, "2023-03-09", 12.30),
    (2, 2, "2023-03-23", 26.71);