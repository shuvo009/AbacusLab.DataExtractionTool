using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.Command
{
    public class NotFoundCommand : ICommand
    {
        public void Execute()
        {
            var messService = DependenceResolver.Resolver.Container.Resolve<IMessageService>();
            messService.ShowMessages("Command Not Found");
        }

        public string CommandName
        {
            get { return "NotFound"; }
        }
    }
}
