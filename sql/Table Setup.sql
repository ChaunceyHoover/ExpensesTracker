-- Select the database to run commands
USE expenses;

-- We keep this so we can re-run this table setup script as needed while we're in development. Once we hit v1 or "production",
-- we remove this and instead plan out migration scripts to update tables.
DROP TABLES IF EXISTS dynamic_expenses, payments, static_expenses, loans, payment_types, payees, locations;

CREATE TABLE payees (
	-- We use a "TINYINT" here because we're not going to have a lot of people using this application - it's just for us.
    -- And an unsigned TINYINT's value range is 0 to 255. We want this over the signed because we're lazy and using
    -- the auto increment feature, which doesn't start at a negative number (-128 to 127).
	payee_id TINYINT UNSIGNED AUTO_INCREMENT,
    
    -- We use an NVARCHAR here in case we want to use funny lil symbols in our name :D
    payee_name NVARCHAR(64) NOT NULL,
    
    -- A "Constriant" is a limiting factor or defining characteristic of a table to place restrictions on a table.
    -- A "Primary Key" is a constriant because it requires a unique value for this column.
    PRIMARY KEY (payee_id)
) ENGINE = InnoDB, COMMENT = 'Stores names of people using the application';

-- For now, hard code us as the only two people in the application
INSERT INTO payees
	(payee_name)
VALUES
	('Chauncey'),
    ('Josh');

CREATE TABLE locations (
	-- We use a SMALLINT here because I have no idea how many places we're gonna shop at. It's probably in the hundreds,
    -- so just to be safe, we'll use a SMALLINT.
	location_id SMALLINT UNSIGNED AUTO_INCREMENT,
    
    -- Some restaurants have funny symbols in the name, so we use NVARCHAR here.
    location_name NVARCHAR(64) NOT NULL,
    
    PRIMARY KEY (location_id)
) ENGINE = InnoDB,  COMMENT = "Categories where purchases were made";

-- Insert some commonly used locations to start with
INSERT INTO locations
	(location_name)
VALUES
	('Walmart'), ('Kroger'), ('Sam''s Club'), ('Target'), 
    ('Total Wine'), ('McDonalds'), ('Chick-Fil-A'), 
    ('HelloFresh');

CREATE TABLE dynamic_expenses (
	de_id INT UNSIGNED AUTO_INCREMENT,
    payee_id TINYINT UNSIGNED NOT NULL,
    location_id SMALLINT UNSIGNED NOT NULL,
    `date` DATETIME NOT NULL,
    amount DECIMAL(13, 2) NOT NULL,
    notes NVARCHAR(512),
    
    PRIMARY KEY (de_id),
	
    -- Here, we're creating another constraint - you can only insert a value in this column IF IT EXISTS in the
    -- table `payees` and has a row with a matching value in the column of `payee_id`. If there isn't a matching
    -- value, it will create an error.
    CONSTRAINT `fk_de_payee`
		FOREIGN KEY (payee_id) REFERENCES payees (payee_id),
	
    -- Same thing here. The value in this column will only be valid if there is a row in the `locations` table.
    CONSTRAINT `fk_de_locations`
		FOREIGN KEY (location_id) REFERENCES locations (location_id)
) ENGINE = InnoDB, COMMENT = "Stores non-regularly occurring payments (groceries, food, etc.)";

CREATE TABLE static_expenses (

	se_id INT UNSIGNED AUTO_INCREMENT,
    se_name VARCHAR(64) NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
	amount DECIMAL(13, 2) NOT NULL,
    notes NVARCHAR(512),

	PRIMARY KEY (se_id)
) ENGINE = InnoDB, COMMENT = "Stores regularly occurring payments (rent, utilities, etc.)";

CREATE TABLE loans (

	loans_id SMALLINT NOT NULL AUTO_INCREMENT,
    loans_name VARCHAR(64) NOT NULL,
    payee_id TINYINT UNSIGNED NOT NULL,
    amount DECIMAL(13, 2) NOT NULL,
    notes NVARCHAR(512),
    
    PRIMARY KEY (loans_id),
    
    CONSTRAINT `fk_loans_payee`
		FOREIGN KEY (payee_id) REFERENCES payees (payee_id)
) ENGINE = InnoDB, COMMENT = "Stores larger sums of borrowed money to be paid over time.";

CREATE TABLE payment_types (

	pt_id TINYINT NOT NULL AUTO_INCREMENT,
    pt_name VARCHAR(64) NOT NULL,
    
    PRIMARY KEY (pt_id)
) ENGINE = InnoDB, COMMENT = "Categorizes the payment (dynamic, static, loan).";

-- These won't change without making other database changes, so it's safe to hard code them in
INSERT INTO payment_types
	(pt_name)
VALUES
	('Dynamic'),
    ('Static'),
    ('Loan');

CREATE TABLE payments (

	payment_id INT NOT NULL AUTO_INCREMENT,
    payee_id TINYINT UNSIGNED NOT NULL,
    payment_name VARCHAR(64) NOT NULL,
    pt_id TINYINT NOT NULL,
    amount DECIMAL(13, 2) NOT NULL,
    `date` DATE NOT NULL,
    notes NVARCHAR(512),
    
	PRIMARY KEY (payment_id),
    
    CONSTRAINT `fk_payment_payee`
		FOREIGN KEY (payee_id) REFERENCES payees (payee_id),
        
	CONSTRAINT `fk_payment_paymenttype`
		FOREIGN KEY (pt_id) REFERENCES payment_types (pt_id)
) ENGINE = InnoDB, COMMENT = "Store payments made by a registered payee.";

-- VIEWS

DROP VIEW IF EXISTS dynamic_expenses_view, loans_view, payments_view;

CREATE VIEW dynamic_expenses_view
AS
    SELECT
		de.de_id, p.payee_id, p.payee_name,
        l.location_id, l.location_name,
        de.`date`, de.amount, de.notes
    FROM dynamic_expenses de
    LEFT JOIN payees p ON de.payee_id = p.payee_id
    LEFT JOIN locations l ON de.location_id = l.location_id;
    
CREATE VIEW loans_view
AS
	SELECT lo.loans_name, p.payee_id, p.payee_name, lo.amount, lo.notes
    FROM loans lo
    LEFT JOIN payees p
    ON lo.payee_id = p.payee_id;
    
CREATE VIEW payments_view
AS
    SELECT pmnt.payment_id, pmnt.payment_name, p.payee_id, p.payee_name, pmnt.`date`, pmnt.amount, pmnt.notes
    FROM payments pmnt
    LEFT JOIN payees p
    ON pmnt.payee_id = p.payee_id
    LEFT JOIN payment_types pt
    ON pmnt.pt_id = pt.pt_id;