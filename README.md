# APM - Dev report

# Pre-requisites notes

  - Built with visual Studio 2017 and SQL Server 2017
  - Any issues opening the solution in VS2013 (if it is the only available visual studio), please open the .sln file and change the line (Microsoft Visual Studio Solution File, Format Version 12.00) to (Microsoft Visual Studio Solution File, Format Version 10.00)
  - I've got some windows security issues attaching the database directly from the connectionstring, as i was losing too much time handling/researching windows security issues, i decided to manually attach the mdf file to sql server.

# General notes
- I found the behavior of creating a blank product when productid < 0 in Get inside the ProductController a bit strange, by my understanding this affects the single responsability principle so i decided to remove the possibility of using negative ids to get a blank product returned in get.


# Requirement 1
- Handled with Regex pattern in Angular
 
# Requirement 2
- Decided to use Autofac as IoC container, is not the most performatic one, but it is fast and easy to setup.

For Reference:
http://www.palmmedia.de/blog/2011/8/30/ioc-container-benchmark-performance-comparison
    
# Requirement 3
- Decided to use Xunit to tests + FakeitEasy to handle mocks. I could also build some postman tests to validate the web api routing, however i sticked to the scope noted in the history. In order to the Visual Studio IDE find the tests, the solution needs to be built. Also i isolated the tests in a new project.

- There are some postman test scripts in the same project used to test api routing and currency routing mechanism.

# Requirement 4
- Created a single readonly field in Angular showing the new model property

# Requirement 5
- Never worked with multi-currency applications, so i did some  research and find out about a "Money Pattern" described by Martin-Fowler. By using it, we avoid errors of mixing currencies arithmetics and we "could" gain speed and minimize other developers errors. However IDK the impacts and performance issues of this pattern, or code complexity increase and this would require some community research, benchmark and profiling testing.

    For reference: 
    https://martinfowler.com/eaaCatalog/money.html
    https://code.tutsplus.com/tutorials/money-pattern-the-right-way-to-represent-value-unit-pairs--net-35509
    https://www.codeproject.com/Articles/837791/Money-pattern
    https://github.com/zpbappi/money
    
- Decided to build the mechanism described bellow due to cost x time benefit:

1- All values in backend will be handled and inputted in USD (this may change in the futured, but i tried to stay in the requirement scope);  
2- Created a Domain Service called CurrencyConversionService;  
3- All responses that should have currency shown in other coin than USD should call this service (only get products by now);  
4 - Created a CurrencyRepository that will consist of an in-memory dictionary mechanism that will handle currency conversions based on the ISO Currency symbols;  
5- CurrencyRepository will receive its data from a JSON file similar to the product file, this will easy up future integrations with daily exchange ratio providers and/or allow other services to provide this info to this one;  
6- It seems that from SEO perspective is not a good practice to have same URL with different contents (this mechanism can also be used in the future to multi-language implementations), so i created an optional webapi route and created a LocalizationAttribute. This mechanism will isolate the dependency between browser localization and backend localization, the backend will only send responses to what he is asked to. By this way, for example, a customer from Argentina can have his frontend show currencies in euros if we build this properly in frontend.  
  
By checking the culture in the route we can return different currency symbols and conversions according to the routes called. So, assuming we have the currencies and ratios in the JSON file, they would be automatically converted to the required output. Cases that are not in the  JSON file would return the default currencies/values in USD.  
  
Some examples:  
http://localhost:7918/api/fr/products (returns prices in EUR)  
http://localhost:7918/api/de/products (returns prices in EUR)  
http://localhost:7918/api/de-LU/products (returns prices in EUR)  
http://localhost:7918/api/en-US/products (returns prices in USD)  
http://localhost:7918/api/en/products (returns prices in USD)  
http://localhost:7918/api/products (returns prices in USD)  
http://localhost:7918/api/ar/products (returns prices in USD - we ONLY have USD-EUR conversion in the database)  
  
  
for reference about #6:  
https://support.google.com/webmasters/answer/182192?hl=en&topic=2370587&ctx=topic  