using System;
using System.Collections.Generic;
using PureMVC.Patterns.Command;

namespace HotFix.Core
{
    /// <summary>
    /// 启动命令
    /// </summary>
    public class StartupCommand : MacroCommand
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeMacroCommand()
        {
            base.InitializeMacroCommand();

            AddSubCommand(() => new BootstrapCommands());
            AddSubCommand(() => new BootstrapProxys());
            AddSubCommand(() => new BootstrapMediators());
        }
    }
}
