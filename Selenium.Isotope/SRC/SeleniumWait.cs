﻿using System;
using System.Collections.Generic;
using Selenium;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace Selenium.Isotope
{
    /// <summary>
    /// An implementation of the <see cref="IWait&lt;T&gt;"/> interface that may have its timeout and polling interval
    /// configured on the fly.
    /// </summary>
    /// <typeparam name="T">The type of object on which the wait it to be applied.</typeparam>
    public class SeleniumWait<T> : IWait<T>
    {
        private T input;
        private IClock clock;

        private TimeSpan timeout = DefaultSleepTimeout;
        private TimeSpan sleepInterval = DefaultSleepTimeout;
        private string message = string.Empty;

        private List<Type> ignoredExceptions = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumWait&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="input">The input value to pass to the evaluated conditions.</param>
        public SeleniumWait(T input) :
            this(input, new SystemClock())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumWait&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="input">The input value to pass to the evaluated conditions.</param>
        /// <param name="clock">The clock to use when measuring the timeout.</param>
        public SeleniumWait(T input, IClock clock)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input", "input cannot be null");
            }

            if (clock == null)
            {
                throw new ArgumentNullException("clock", "input cannot be null");
            }

            this.input = input;
            this.clock = clock;
        }

        /// <summary>
        /// Gets or sets how long to wait for the evaluated condition to be true. The default timeout is 500 milliseconds.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }

        /// <summary>
        /// Gets or sets how often the condition should be evaluated. The default timeout is 500 milliseconds.
        /// </summary>
        public TimeSpan PollingInterval
        {
            get { return this.sleepInterval; }
            set { this.sleepInterval = value; }
        }

        /// <summary>
        /// Gets or sets the message to be displayed when time expires.
        /// </summary>
        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        private static TimeSpan DefaultSleepTimeout
        {
            get { return TimeSpan.FromMilliseconds(500); }
        }

        /// <summary>
        /// Configures this instance to ignore specific types of exceptions while waiting for a condition.
        /// Any exceptions not whitelisted will be allowed to propagate, terminating the wait.
        /// </summary>
        /// <param name="exceptionTypes">The types of exceptions to ignore.</param>
        public void IgnoreExceptionTypes(params Type[] exceptionTypes)
        {
            if (exceptionTypes == null)
            {
                throw new ArgumentNullException("exceptionTypes", "exceptionTypes cannot be null");
            }

            foreach (Type exceptionType in exceptionTypes)
            {
                if (!typeof(Exception).IsAssignableFrom(exceptionType))
                {
                    throw new ArgumentException("All types to be ignored must derive from System.Exception", "exceptionTypes");
                }
            }

            this.ignoredExceptions.AddRange(exceptionTypes);
        }

        /// <summary>
        /// Repeatedly applies this instance's input value to the given function until one of the following
        /// occurs:
        /// <para>
        /// <list type="bullet">
        /// <item>the function returns neither null nor false</item>
        /// <item>the function throws an exception that is not in the list of ignored exception types</item>
        /// <item>upon timeout method return last achieved result</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <typeparam name="TResult">The delegate's expected return type.</typeparam>
        /// <param name="condition">A delegate taking an object of type T as its parameter, and returning a TResult.</param>
        /// <returns>The delegate's return value.</returns>
        public TResult Until<TResult>(Func<T, TResult> condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException("condition", "condition cannot be null");
            }

            var resultType = typeof(TResult);
            if ((resultType.IsValueType && resultType != typeof(bool)) || !typeof(object).IsAssignableFrom(resultType))
            {
                throw new ArgumentException("Can only wait on an object or boolean response, tried to use type: " + resultType.ToString(), "condition");
            }

            Exception lastException = null;
            var endTime = this.clock.LaterBy(this.timeout);
            while (true)
            {
                try
                {
                    var result = condition(this.input);
                    if (resultType == typeof(bool))
                    {
                        var boolResult = result as bool?;
                        if (boolResult.HasValue && boolResult.Value)
                        {
                            return result;
                        }
                    }
                    else
                    {
                        if (result != null)
                        {
                            return result;
                        }
                    }

                    // Check the timeout after evaluating the function to ensure conditions
                    // with a zero timeout can succeed.
                    if (!this.clock.IsNowBefore(endTime))
                    {
                        return result;
                    }

                }
                catch (Exception e)
                {
                    lastException = this.PropagateExceptionIfNotIgnored(e);
                }
                Thread.Sleep(this.sleepInterval);
            }
        }

        private Exception PropagateExceptionIfNotIgnored(Exception e)
        {
            foreach (Type exceptionType in this.ignoredExceptions)
            {
                if (exceptionType.IsAssignableFrom(e.GetType()))
                {
                    return e;
                }
            }

            throw e;
        }
    }
}
