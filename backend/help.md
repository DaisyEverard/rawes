

context - doesnt' do anything. DONE

repository - sets up queries. Where, if, e.g. applies filters. split based on where stored in database
Where the functions to query the database with SQL Are. Is genericised.
May need different once for each table.



service - calls multiple repositories for different bits of data in different databases.
	split endpoints based on how to interact from frontend.
	each service is the start of an endpoint e.g. users/ products/

controller - handles HTTP requests and sends back errors e.g. bad request or data and 200