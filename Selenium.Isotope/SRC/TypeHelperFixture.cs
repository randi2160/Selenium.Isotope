using System;
using fit;
using fitlibrary;

namespace Selenium.Isotope
{
    /// <summary>
    /// Most of objects in fitnesse recognised as strings.
    /// This may cause issues when specific types are required.
    /// This fixture could be used to enforce specific base type (int, bool).
    /// Additionaly provides methods to get null and blank values.
    /// </summary>
    public class TypeHelperFixture : DoFixture
    {
        /// <summary>
        /// Get New null Object
        /// |Selenium.Isotope.TypeHelperFixture|
        /// |check|Null Value|>>NULL|
        /// |check|Null Value|<<NULL|
        /// </summary>
        /// <returns></returns>
        public Object NullValue() {
            Object result=null;
            return result;
        }
        /// <summary>
        /// Get New Blank String
        /// |Selenium.Isotope.TypeHelperFixture|
        /// |check|Blank Value|>>BLANK|
        /// |check|Blank Value|<<BLANK|
        /// </summary>
        /// <returns></returns>
        public String BlankValue()
        {
            String result = String.Empty;
            return result;
        }
        /// <summary>
        /// Use to recognise value as int
        /// |Selenium.Isotope.TypeHelperFixture|
        /// |check|Int Value|1|>>IntNumber|
        /// </summary>
        /// <param name="value">int value</param>
        /// <returns></returns>
        public int IntValue(int value){
            return value;
        }
        /// <summary>
        /// Use to recognise value as bool
        /// |Selenium.Isotope.TypeHelperFixture|
        /// |check|bool Value|TRUE|>>BoolTrue|
        /// |check|bool Value|true|<<BoolTrue|
        /// </summary>
        /// <param name="value">TRUE or FALSE</param>
        /// <returns>bool value</returns>
        public bool BoolValue(bool value)
        {
            return value;
        }
    }
}
