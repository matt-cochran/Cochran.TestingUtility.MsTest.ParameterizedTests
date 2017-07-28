namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    using System;
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =true)]
    public sealed class ParameterizedTestSourceAttribute : Attribute, ITestParameterSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedTestSourceAttribute"/> class.
        /// </summary>
        /// <param name="parametersSource">The name of the static method providing parameters.</param>
        public ParameterizedTestSourceAttribute(String parametersSource)
        {
            this.ParameterSourceStaticMethodName = parametersSource;
        }

        /// <summary>
        /// Gets or sets the name of the static method providing parameters.
        /// </summary>
        /// <value>
        /// The name of the static method providing parameters.
        /// </value>
        public String ParameterSourceStaticMethodName { get; set; }
    }
}

