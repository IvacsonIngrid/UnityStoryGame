using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    public class CommandParameters
    {
        private const char PARAMETER_IDENTIFIER = '-'; // paraméterek előrésze
        private Dictionary<string, string> parameters = new Dictionary<string, string>(); // cimkézett paramétereket tárol
        private List<string> unlableParameters = new List<string>(); // cimke nélküli paramétereket tárol
        public CommandParameters(string[] parameterArray, int startingIndex = 0) // konstruktor (paraméter tömb, kezdő index a vizsgálathoz)
        {
            for(int i = startingIndex;  i < parameterArray.Length; i ++) // végigmenni a paraméter listán
            {
                if (parameterArray[i].StartsWith(PARAMETER_IDENTIFIER) && !float.TryParse(parameterArray[i], out _)) // ha egyezik a paraméter előrész és nem szám, akkor cimkézett paraméter
                {
                    string pName = parameterArray[i]; // paraméter neve
                    string pValue = "";

                    if(i + 1 < parameterArray.Length && !parameterArray[i + 1].StartsWith(PARAMETER_IDENTIFIER)) // a cimkézett paraméter után egy nem cimkézett érték következik - paraméter értékének veszik
                    {
                        pValue = parameterArray[i + 1]; // érték meghatározása
                        i++;
                    }

                    parameters.Add(pName, pValue); // hozzáadni a cimkézett paraméter listához
                }
                else
                    unlableParameters.Add(parameterArray[i]); // hozzáadni a cimke nélküli paraméter listához
            }
        }

        public bool TryGetValue<T>(string parameterName, out T value, T defaultValue = default(T)) => TryGetValue(new string[] { parameterName }, out value, defaultValue);
        
        public bool TryGetValue<T>(string[] parameterNames, out T value, T defaultValue = default(T)) // megpróbálja lekérni az első megtalálható név alapján
        {
            foreach(string parameterName in parameterNames) // bejárja a cimkézett paraméterek listáját
            {
                if (parameters.TryGetValue(parameterName, out string parameterValue))
                {
                    if (TryCastParameter(parameterValue, out value)) // tipus alakitás
                    {
                        return true;
                    }
                }
            }

            foreach (string parameterName in unlableParameters) // bejárja a cimkézetlen paraméterek listáját
            {
                if (TryCastParameter(parameterName, out value)) // tipusalakitas
                {
                    unlableParameters.Remove(parameterName); 
                    return true;
                }
            }

            value = defaultValue;
            return false;
        }

        private bool TryCastParameter<T>(string parameterValue, out T value) // megpróbálja átalakitani a paraméterek tipusát
        {
            if (typeof(T) == typeof(bool))
            {
                if (bool.TryParse(parameterValue, out bool boolValue))
                {
                    value = (T)(object)boolValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(int))
            {
                if (int.TryParse(parameterValue, out int intValue))
                {
                    value = (T)(object)intValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(float))
            {
                if (float.TryParse(parameterValue, out float floatValue))
                {
                    value = (T)(object)floatValue;
                    return true;
                }
            }
            else if (typeof(T) == typeof(string))
            {
                value = (T)(object)parameterValue;
                return true;
            }

            value = default(T); // sikertelen átalakitás esetén alapértelmezett értéket térit vissza
            return false;
        }
    }
}
