using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;

namespace UI_TEST
{
    /// <summary>
    /// https://www.syncfusion.com/kb/3860/how-to-the-release-memory-held-by-automationpeer-in-wpf-components
    /// </summary>
    public class FakeWindowsPeer : WindowAutomationPeer
    {
        public FakeWindowsPeer(Window window)
            : base(window)
        {
        }

        protected override List<AutomationPeer> GetChildrenCore()
        {
            return null;
        }
    }
}