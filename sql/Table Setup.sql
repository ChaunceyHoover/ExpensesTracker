-- Select the database to run commands
USE expenses;

DROP TABLES IF EXISTS payments, dynamic_expenses, static_expenses, payees, vendors;

/** TABLES **/

CREATE TABLE payees (
	payee_id TINYINT UNSIGNED AUTO_INCREMENT,
    payee_name NVARCHAR(64) NOT NULL,
    
    PRIMARY KEY (payee_id)
) ENGINE = InnoDB, COMMENT = 'Stores names of people using the application';

CREATE TABLE vendors (
	vendor_id SMALLINT UNSIGNED AUTO_INCREMENT,
    vendor_name NVARCHAR(64) NOT NULL,
    
    PRIMARY KEY (vendor_id)
) ENGINE = InnoDB,  COMMENT = "Categories where purchases were made";

CREATE TABLE dynamic_expenses (
	de_id INT UNSIGNED AUTO_INCREMENT,
    payee_id TINYINT UNSIGNED NOT NULL,
    vendor_id SMALLINT UNSIGNED NOT NULL,
    `date` DATETIME NOT NULL,
    amount DECIMAL(13, 2) NOT NULL,
    split BOOLEAN DEFAULT TRUE,
    notes NVARCHAR(512),
    
    PRIMARY KEY (de_id),
	
    CONSTRAINT `fk_de_payee`
		FOREIGN KEY (payee_id) REFERENCES payees (payee_id),
	
    CONSTRAINT `fk_de_vendors`
		FOREIGN KEY (vendor_id) REFERENCES vendors (vendor_id)
) ENGINE = InnoDB, COMMENT = "Stores non-regularly occurring payments (groceries, food, etc.)";

CREATE TABLE static_expenses (
	se_id INT UNSIGNED AUTO_INCREMENT,
    se_name VARCHAR(64) NOT NULL,
    issue_date DATE NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
	amount DECIMAL(13, 2) NOT NULL,
    notes NVARCHAR(512),

	PRIMARY KEY (se_id)
) ENGINE = InnoDB, COMMENT = "Stores regularly occurring payments (rent, utilities, etc.)";

CREATE TABLE payments (
	payment_id INT NOT NULL AUTO_INCREMENT,
    payee_id TINYINT UNSIGNED NOT NULL,
    payment_name VARCHAR(64) NOT NULL,
    amount DECIMAL(13, 2) NOT NULL,
    `date` DATE NOT NULL,
    notes NVARCHAR(512),
    
	PRIMARY KEY (payment_id),
    
    CONSTRAINT `fk_payment_payee`
		FOREIGN KEY (payee_id) REFERENCES payees (payee_id)
) ENGINE = InnoDB, COMMENT = "Store payments made by a registered payee.";



/** VIEWS **/

DROP VIEW IF EXISTS dynamic_expenses_view, loans_view, payments_view;

CREATE VIEW dynamic_expenses_view
AS
    SELECT
		de.de_id, de.`date`, de.amount, de.notes, de.amount / 2 as 'split',
        p.payee_id, p.payee_name,
        v.vendor_id, v.vendor_name
    FROM dynamic_expenses de
    JOIN payees p ON de.payee_id = p.payee_id AND de.split = true
    JOIN vendors v ON de.vendor_id = v.vendor_id;

CREATE VIEW loans_view
AS
	SELECT
		de.de_id, de.`date`, de.amount, de.notes,
		p.payee_id, p.payee_name,
        v.vendor_id, v.vendor_name
	FROM dynamic_expenses de
    JOIN payees p ON de.payee_id = p.payee_id AND de.split = false
    JOIN vendors v ON de.vendor_id = v.vendor_id;

CREATE VIEW payments_view
AS
    SELECT
		pmnt.payment_id, pmnt.payment_name, pmnt.amount, pmnt.notes,
        p.payee_id, p.payee_name, pmnt.`date`
    FROM payments pmnt
    LEFT JOIN payees p
    ON pmnt.payee_id = p.payee_id;

/** DUMMY DATA **/
-- Insert us into database
INSERT INTO payees
	(payee_name)
VALUES
	('Chauncey'),
    ('Josh');

-- Pre-populate some places we've sent money to each other for
INSERT INTO vendors
	(vendor_name)
VALUES
	('Walmart'), ('Kroger'), ('Sam''s Club'), ('Target'), 
    ('Total Wine'), ('McDonalds'), ('Chick-Fil-A'), 
    ('HelloFresh'), ('American Airlines'), ('AirBnB'),
    ('Royal Carribbean'), ('Old Time Pottery'), ('Pacific Spice'),
    ('TicketMaster'), ('Wings&Tings'), ('Home Depot'), ('Publix'),
    ('Sprouts'), ('Canyons'), ('Knuckies Hoagies'), ('DragonCon');

SELECT * FROM vendors;