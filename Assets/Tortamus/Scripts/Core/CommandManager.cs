using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

public class CommandArgs : EventArgs {}

public delegate bool CommandCanExecuteHandler(object sender, CommandArgs args);

public delegate void CommandExecuteHandler(object sender, CommandArgs args);

public static class CommandManager
{
    private readonly static Dictionary<string,
       KeyValuePair<CommandCanExecuteHandler, CommandExecuteHandler>> RegisteredCommands;

    public static void RegisterCommandHandlers(string commandName, CommandCanExecuteHandler canExecuteHandler,
        CommandExecuteHandler executeHandler)
    {
		return;
        if (RegisteredCommands.ContainsKey(commandName))
            throw new InvalidEnumArgumentException(string.Format("The command {0} yet was registered.", commandName));

        RegisteredCommands.Add(commandName, new KeyValuePair<CommandCanExecuteHandler, CommandExecuteHandler>(canExecuteHandler, executeHandler));
    }

    public static bool CanExecute(string command, CommandArgs args, object sender = null)
    {
		return false;
        if (RegisteredCommands.ContainsKey(command))
            return RegisteredCommands[command].Key(sender, args);
   
        return true;
    }

    public static void Execute(string command, CommandArgs args, object sender = null)
    {
		return;
        if (!RegisteredCommands.ContainsKey(command))
            return;

        RegisteredCommands[command].Value(sender, args);
    }
}