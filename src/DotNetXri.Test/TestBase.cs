using System.Reflection;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetXri.Test {
	public class TestBase {
		internal readonly static ILog Logger = LogManager.GetLogger("DotNetXri.Test");

		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		[TestInitialize]
		public virtual void SetUp() {
			log4net.Config.XmlConfigurator.Configure(Assembly.GetExecutingAssembly().GetManifestResourceStream("DotNetXri.Test.Logging.config"));
		}

		[TestCleanup]
		public virtual void Cleanup() {
			log4net.LogManager.Shutdown();
		}
	}
}
