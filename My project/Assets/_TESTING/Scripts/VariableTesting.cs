using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class VariableTesting : MonoBehaviour
    {
        public int var_int = 0;
        public float var_float = 0;
        public bool var_bool = false;
        public string var_string = "";

        void Start()
        {
            Variables.CreateDatabase("DB_Links");
            Variables.CreateVariable("DB_Links.L_int", var_int, () => var_int, value => var_int = value);
            Variables.CreateVariable("DB_Links.L_float", var_float, () => var_float, value => var_float = value);
            Variables.CreateVariable("DB_Links.L_bool", var_bool, () => var_bool, value => var_bool = value);
            Variables.CreateVariable("DB_Links.L_string", var_string, () => var_string, value => var_string = value);

            Variables.CreateDatabase("DB_Numbers");
            Variables.CreateDatabase("DB_Booleans");

            Variables.CreateVariable("DB_Numbers.num1", 1);
            Variables.CreateVariable("DB_Numbers.num5", 5);
            Variables.CreateVariable("DB_Booleans.lightIsOn", true);
            Variables.CreateVariable("DB_Numbers.float1", 7.5f);
            Variables.CreateVariable("str1", "Hello ");
            Variables.CreateVariable("str2", "World");

            Variables.PrintAllDatabase();

            Variables.PrintAllVariables();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Variables.PrintAllVariables();
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                string variable = "DB_Numbers.num1";
                Variables.TryGetValue(variable, out object v);
                Variables.TrySetValue(variable, (int)v + 5);
            }

            if (Input.GetKeyDown (KeyCode.S))
            {
                Variables.TryGetValue("DB_Numbers.num1", out object num1);
                Variables.TryGetValue("DB_Numbers.num5", out object num2);

                Debug.Log($"num1 + num2 = {(int)num1 + (int)num2}");
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                if (Variables.TryGetValue("DB_Booleans.lightIsOn", out object lightIsOn) && lightIsOn is bool)
                    Variables.TrySetValue("DB_Booleans.lightIsOn", !(bool)lightIsOn);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                Variables.TryGetValue("str1", out object str_hello);
                Variables.TryGetValue("str2", out object str_world);
                Variables.TrySetValue("str1", (string)str_hello + str_world);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                Variables.TryGetValue("DB_Links.L_int", out object linked_integer);
                Variables.TrySetValue("DB_Links.L_int", (int)linked_integer + 5);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                Variables.RemoveVariable("DB_Links.L_int");
                Variables.RemoveVariable("DB_Booleans.lightIsOn");
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                Variables.RemoveAllVariables();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                Variables.TryGetValue("DB_Links.L_float", out object v);
                Variables.TrySetValue("DB_Links.L_float", (float)v * 5.5f);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                Variables.TryGetValue("DB_Links.L_bool", out object v);
                Variables.TrySetValue("DB_Links.L_bool", !(bool)v);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                Variables.TryGetValue("DB_Links.L_string", out object v);
                Variables.TrySetValue("DB_Links.L_string", (string)v + UnityEngine.Random.Range(1000, 2000));
            }
        }
    }
}