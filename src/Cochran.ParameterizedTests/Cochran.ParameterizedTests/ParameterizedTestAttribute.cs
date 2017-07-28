namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Attribute for parameterized test where test cases can be specified in static
    /// methods with no input parameters that return IEnumerable&lt;Object[]&gt;
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ParameterizedTestAttribute : TestMethodAttribute
    {
        const String _MissingMethodMessage = "the public static method specified in the [ParameterizedTestSource] attribute does not exit.";

        /// <summary>
        /// Find all data rows and execute.
        /// </summary>
        /// <param name="testMethod">
        /// The test Method.
        /// </param>
        /// <returns>
        /// An array of <see cref="TestResult"/>.
        /// </returns>tt
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            ITestParameterSource[] dataSources =
                testMethod
                    .GetAttributes<Attribute>(true)?
                    .Where(a => a is ITestParameterSource)
                    .OfType<ITestParameterSource>()
                    .ToArray();

            if (dataSources == null || dataSources.Length == 0)
            {
                return new TestResult[]
                {
                        new TestResult()
                        {
                            Outcome = UnitTestOutcome.Failed,
                            TestFailureException = new InvalidOperationException("No test data provided")
                        }
                };
            }

            var result = RunParameterizedTests(testMethod, dataSources).ToArray();

            return result;
        }

        private static IEnumerable<TestResult> RunParameterizedTests(ITestMethod testMethod, ITestParameterSource[] dataSources)
        {
            var testMethodParameters = testMethod.MethodInfo.GetParameters();

            var testCaseQuery = 
                from parameterGroup in dataSources
                from parameterList in GetParameters(testMethod, parameterGroup)
                let displayName = testMethod.TestClassName + "." + testMethod.TestMethodName
                select new
                {
                    Parameters   = parameterList,
                    DisplayName  = displayName
                };

            foreach (var testCase in testCaseQuery)
            {
                if (testCase.Parameters.Length != testMethodParameters.Length)
                {
                    TestResult badParamsResult = new TestResult
                    {
                        Outcome              = UnitTestOutcome.Error,
                        TestFailureException = new InvalidOperationException("the number of parameters specified does not match the number of test parameters"),
                        DisplayName          = testCase.DisplayName
                    };

                    yield return badParamsResult;
                }

                if(ReferenceEquals(testMethod, null))
                {
                    var missingMthodResult = BuildMissingMethodResult();
                    yield return missingMthodResult;
                }

                var result = testMethod.Invoke(testCase.Parameters);

                result.DisplayName = $"{testCase.DisplayName} ({String.Join(", ", testCase.Parameters.Select(p => p?.ToString() ?? String.Empty))})";

                yield return result;
            }

            var missingSourceQuery =
                from parameterGroup in dataSources
                let meth = testMethod.MethodInfo.DeclaringType.GetMethod(parameterGroup.ParameterSourceStaticMethodName)
                where ReferenceEquals(meth, null)
                select parameterGroup;


            foreach (var missingSource in missingSourceQuery)
            {
                var result = BuildMissingMethodResult();

                yield return result;
            }
        }

        private static TestResult BuildMissingMethodResult()
        {
            return new TestResult
            {
                Outcome = UnitTestOutcome.Error,
                TestFailureException = new InvalidOperationException(_MissingMethodMessage),
                DisplayName = _MissingMethodMessage
            };
        }

        private static IEnumerable<Object[]> GetParameters(ITestMethod testMethod, ITestParameterSource source)
        {
            var meth = testMethod.MethodInfo.DeclaringType.GetMethod(source.ParameterSourceStaticMethodName);
           
            if (ReferenceEquals(meth, null))
            {
                return Enumerable.Empty<Object[]>();
            }

            var result = meth.Invoke(null, null) as IEnumerable<Object[]>;

            return result;
        }
    }
}

