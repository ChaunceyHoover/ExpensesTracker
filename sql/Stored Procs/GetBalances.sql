USE expenses;

DROP PROCEDURE IF EXISTS GetBalances;

DELIMITER //

/**
 * Returns a breakdown of all balances paid and owed for a given payee
 */
CREATE PROCEDURE GetBalances(
	IN payee tinyint,
    IN start_range DATE,
    IN end_range DATE
)
BEGIN
	-- For the sake of getting v1, the app is hard coded to work with just us 2. In the future, this will be dynamically split
    -- if we decide to go down the route of adding more users.
	SET @other_payee = IF(payee = 1, 2, 1);
    
    SET @payee_name = (SELECT payee_name FROM payees WHERE payee_id = payee);

	-- "Dynamic Expenses" are expenses shared between us: groceries, concert tickets, food, etc.
	SET @dynamic_expenses =
		IFNULL((
			SELECT SUM(amount) / 2 
            FROM dynamic_expenses 
            WHERE
				payee_id = @other_payee
                AND split = true
                AND (start_range IS NULL OR `date` >= start_range)
                AND (end_range IS NULL OR `date` <= end_range))
		, 0);
    
    -- "Loans" are sums of money given to someone else, usually for bigger events: hotels, vacations, plane tickets, etc.
    SET @loans = 
		IFNULL((
			SELECT SUM(amount)
            FROM dynamic_expenses
            WHERE
				payee_id = @other_payee
                AND split = false
                AND (start_range IS NULL OR `date` >= start_range)
                AND (end_range IS NULL OR `date` <= end_range))
		, 0);
    
    -- "Static Expenses" are repeating expenses that are billed regularly and cover a set time period (billing period).
    -- This includes rent, utilities, etc. and are split between thw two of us.
    SET @static_expenses =
		IFNULL((
			SELECT SUM(amount) / 2
            FROM static_expenses
            WHERE
				(start_range IS NULL OR issue_date >= start_range)
                AND (end_range IS NULL OR issue_date <= end_range))
		, 0);
    
    -- All the recorded payments
    SET @payments =
		IFNULL((
			SELECT SUM(amount)
            FROM payments
            WHERE
				payee_id = payee
                AND (start_range IS NULL OR `date` >= start_range)
                AND (end_range IS NULL OR `date` <= end_range))
		, 0);

	SELECT
		payee as 'payee_id',
		@payee_name as 'payee_name',
		ROUND(@dynamic_expenses, 2) as 'dynamic_expenses',
		ROUND(@loans, 2) as 'loans',
		ROUND(@static_expenses, 2) as 'static_expenses',
		ROUND(@payments, 2) as 'payments',
		ROUND((@dynamic_expenses + @loans + @static_expenses - @payments), 2) as 'balance';
END //

DELIMITER ;