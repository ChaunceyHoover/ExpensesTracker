-- Select the database to run commands
USE expenses;

-- We keep this so we can re-run this table setup script as needed while we're in development. Once we hit v1 or "production",
-- we remove this and instead plan out migration scripts to update tables.
DROP TABLES IF EXISTS dynamic_expenses, static_expenses, payees, locations;

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

CREATE TABLE locations (
	-- We use a SMALLINT here because I have no idea how many places we're gonna shop at. It's probably in the hundreds,
    -- so just to be safe, we'll use a SMALLINT.
	location_id SMALLINT UNSIGNED AUTO_INCREMENT,
    
    -- Some restaurants have funny symbols in the name, so we use NVARCHAR here.
    location_name NVARCHAR(64) NOT NULL,
    
    PRIMARY KEY (location_id)
) ENGINE = InnoDB,  COMMENT = "Categories where purchases were made";

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
