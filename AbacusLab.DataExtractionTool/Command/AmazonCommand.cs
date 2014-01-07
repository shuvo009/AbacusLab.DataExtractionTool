using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.View;

namespace AbacusLab.DataExtractionTool.Command
{
    public class AmazonCommand : ICommand
    {
        public void Execute()
        {
            new ContainerWindow(new AmazonSearchView()).Show();
        }

        public string CommandName
        {
            get { return "Amazon"; }
        }
    }
}
