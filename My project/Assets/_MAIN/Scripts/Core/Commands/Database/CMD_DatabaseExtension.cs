using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public abstract class CMD_DatabaseExtension
    {
        public static void Extend(CommandDatabase database) { } //implement�l�s n�lk�li, statikus f�ggv�ny

        public static CommandParameters ConvertDataToParameters(string[] data, int startingIndex = 0) => new CommandParameters(data, startingIndex);
    }
}