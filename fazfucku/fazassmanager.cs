using System;
using System.Collections.Generic;
//using System.Linq;

namespace fazfucku
{
    internal class fuckassrunner
    {
        private List<fazassrunner> runnahs = new List<fazassrunner>();
        public void loaduprunner(string[] program, string[] flags)
        {
            runnahs.Add(initfazassrunner(program, flags));
        }
        public bool runhook(string hookname)
        {
            bool catche = false;
            foreach (fazassrunner runner in runnahs)
            {
                runner.caught = false;
                if (runner.hooks.ContainsKey(hookname))
                {
                    runner.counter = runner.hooks[hookname];
                    runner.run();
                }
                if (runner.caught)
                    catche = true;
            }
            return catche;
        }
        static fazassrunner initfazassrunner(string[] program, string[] flags)
        {
            fazassrunner myassisblicky = new fazassrunner();
            myassisblicky.loadprogram(program);
            if (flags.Length > 0)
            {
                for (int i = 0; i < flags.Length; i++)
                {
                    switch (flags[i])
                    {
                        case "runwithdebug":
                        case "ruwd":
                            myassisblicky.debug_runline = true;
                            break;
                        case "typecheck":
                        case "tch":
                            myassisblicky.debug_typecheck = true;
                            break;
                        default:
                            Console.WriteLine(flags[i] + " is not a valid run setting!");
                            break;
                    }
                }
            }
            return myassisblicky;
        }
    }
}