using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DIALOGUE;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;

        public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

        string[] lines = new string[5] {
        "1. This is a random slide. ",
        "2. This is another random slide. ",
        "3. This was my best slide. ",
        "4. This will be good. ",
        "5. This is just a test "
        };

        // Start is called before the first frame update
        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.fade;
            architect.speed = 0.5f;
        }

        // Update is called once per frame
        void Update()
        {
            if (bm != architect.buildMethod)
            {
                architect.buildMethod = bm;
                architect.Stop();
            }

            if (Input.GetKeyDown(KeyCode.S))
                architect.Stop();

            string longLine = "this is a very long line that makes no sense but i am just populating and i want to nknow, if you like the apple, i like the apple, but i dont know if you also like it, so you like it or not, can you tell me, i really want to know";
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();
                }
                else
                    architect.Build(longLine);
                    //architect.Build(lines[UnityEngine.Random.Range(0, lines.Length)]);
            } 
            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(longLine);
                //architect.Append(lines[UnityEngine.Random.Range(0, lines.Length)]);
            }
        }
    }
}