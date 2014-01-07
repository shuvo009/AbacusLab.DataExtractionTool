using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.View;

namespace AbacusLab.DataExtractionTool.Command
{
    class AllabologSeCommand : ICommand
    {
        public void Execute()
        {
            new ContainerWindow(new AllabologSeView()).Show();
        }

        public string CommandName
        {
            get { return "AllabologSe"; }
        }
    }
}
