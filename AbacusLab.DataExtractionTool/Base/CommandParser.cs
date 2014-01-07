using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Command;
using AbacusLab.DataExtractionTool.Interface;

namespace AbacusLab.DataExtractionTool.Base
{
    public class CommandParser
    {
        private readonly IEnumerable<ICommand> _availableCommands;
        private static CommandParser commandParser = null;
        private CommandParser()
        {
            _availableCommands = GetCommands();
        }

        public static CommandParser Parser
        {
            get { return commandParser ?? new CommandParser(); }
        }

        public void Parse(string commandName)
        {
            var command = FindCommand(commandName) ?? new NotFoundCommand();
            command.Execute();
        }

        private ICommand FindCommand(string commandName)
        {
            return _availableCommands.FirstOrDefault(x => x.CommandName.Equals(commandName));
        }

        private IEnumerable<ICommand> GetCommands()
        {
            return new List<ICommand>
            {
                new AmazonCommand(),
                new GoogleCommand(),
                new HtmlParserCommand(),
                new RegexParserCommand(),
                new VCardCommand(),
                new AllabologSeCommand()
            };
        }
    }
}
