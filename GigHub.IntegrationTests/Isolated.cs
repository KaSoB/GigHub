using NUnit.Framework;
using System;
using System.Transactions;

namespace GigHub.IntegrationTests {
    public class Isolated : Attribute, ITestAction {
        private readonly TransactionScope transactionScope;

        public ActionTargets Targets {
            get { return ActionTargets.Test; }
        }

        public void BeforeTest(TestDetails testDetails) {
            throw new NotImplementedException();
        }

        public void AfterTest(TestDetails testDetails) {
            throw new NotImplementedException();
        }


    }
}
