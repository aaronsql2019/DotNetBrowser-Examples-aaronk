﻿#region Copyright

// Copyright © 2020, TeamDev. All rights reserved.
// 
// Redistribution and use in source and/or binary forms, with or without
// modification, must retain the above copyright notice and the following
// disclaimer.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using DotNetBrowser.Browser;
using DotNetBrowser.Engine;
using DotNetBrowser.Geometry;
using DotNetBrowser.JS;
using DotNetBrowser.Logging;

namespace JavaScriptBridge
{
    internal class Program
    {
        #region Methods

        public static void Main()
        {
            try
            {
                LoggerProvider.Instance.Level = SourceLevels.Information;
                LoggerProvider.Instance.FileLoggingEnabled = true;
                LoggerProvider.Instance.OutputFile = "dnb.log";
                using (IEngine engine = EngineFactory.Create(new EngineOptions.Builder().Build()))
                {
                    Console.WriteLine("Engine created");

                    using (IBrowser browser = engine.CreateBrowser())
                    {
                        Console.WriteLine("Browser created");
                        browser.Size = new Size(700, 500);
                        browser.MainFrame.LoadHtml(@"<html>
                                     <body>
                                        <script type='text/javascript'>
                                            var ShowData = function (a) 
                                            {
                                                 document.title = a.get_FullName() + ' ' + a.get_Age() + '. ' + a.Walk(a.get_Children().get_Item(1))
                                                                + ' ' + a.get_Children().get_Item(1).get_FullName() + ' ' + a.get_Children().get_Item(1).get_Age();
                                            };
                                        </script>
                                     </body>
                                   </html>")
                               .Wait();
                        var person = new Person("Jack", 30, true);
                        person.Children = new Dictionary<double, Person>();
                        person.Children.Add(1.0, new Person("Oliver", 10, true));
                        IJsObject value = browser.MainFrame.ExecuteJavaScript<IJsObject>("window").Result;
                        value.Invoke("ShowData", person);

                        Console.WriteLine($"\tBrowser title: {browser.Title}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Press any key to terminate...");
            Console.ReadKey();
        }

        #endregion


        private class Person
        {
            #region Properties

            public double Age { get; }

            public IDictionary<double, Person> Children { get; set; }
            public string FullName { get; }

            public bool Gender { get; }

            #endregion

            #region Constructors

            public Person(string fullName, int age, bool gender)
            {
                Gender = gender;
                FullName = fullName;
                Age = age;
            }

            #endregion

            #region Methods

            public string Walk(Person withPerson)
                => string.Format("{0} is walking with {1}!", Gender ? "He" : "She", withPerson.FullName);

            #endregion
        }
    }
}