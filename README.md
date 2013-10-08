## Selenium.Isotope is a yet another Selenium fixture for FitNesse.

Implemented only for FitNesse.Net (sorry SLIM users).

## Currently supports 
* DefaultSelenium (AKA Selenium.RC, AKA Selenium v1, AKA Selenium.Core)
* WebDriverBackedSelenium (local driver)
* WebDriverBackedSelenium using RemoteWebDriver (Selenium HUB)

Fixture uses SystemUnderTest object so most of the method calls redirected to the Selenium itself.
For more details on SystemUnder test see http://fitnesse.org/FitNesse.UserGuide.FixtureGallery.ImportantConcepts.SystemUnderTest.

All Selenium API methods are available for this fixture via Reflection.
As result fixture has very few methods implemented by itself (for example pause, store and WaifForText).
For the full list of available methods use selenium documentation: http://selenium.googlecode.com/git/docs/api/dotnet/index.html

Fixture includes SeleniumSymbolHandler which allows combyning few sysnonins into a single string.
Note: Selenium is using the same notation as fitnesse ddoes for markup variables therefore Selenium variables would have to be escaped by !-${VARIABLE}-!
for example 

```
!|Import                |
|Selenium.Isotope       |
|OpenQA.Selenium        |
|OpenQA.Selenium.Firefox|

!|Cell Handler Loader               |
|load|SymbolSaveHandler  |FitLibrary|
|load|SymbolRecallHandler|FitLibrary|
|load|Selenium Recall Handler           |
|load|Selenium Recall Handler|FitLibrary|

!|SeleniumSUT                                                            |
|check|New WebDriver Backed Selenium|FirefoxDriver||http://www.google.com|>>SUT|
|open |http://google.com                                                 |
|check     |get Location    |>>URL                                       |
|ignore    |or your can combine you values using !-${SYMBOL}-!           |
|check     |echo            |Something                      |>>SEARCH    |
|check     |open            |!-${URL}-!?q=!-${SEARCH}-!    |             |
|check     |Wait for text   |<<SEARCH | | 3000             |true         |
|Close|
|Stop|

```

Feedback is appreciated!
