using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class Conversation
    {
        private List<string> lines = new List<string>(); // párbeszéd sorai
        private int progress = 0; // követi a párbeszéd előrehaladását

        public Conversation(List<string> lines, int progress = 0)
        {
            this.lines = lines;
            this.progress = progress;
        }

        public int GetProgress() => progress; // akt. sor indexe
        public void SetProgress(int value) => progress = value; // akt. sor beállitása
        public void IncrementProgress() => progress++; // sor index növelése
        public int Count => lines.Count; // sorok száma összesen, amit be kell járni
        public List<string> GetLines() => lines; // párbeszéd sorainak lostáját adja vissza
        public string CurrentLine() => lines[progress]; // akt. sor visszaadása
        public bool HasReachedEnd() => progress >= lines.Count; // elértük-e a párbeszéd végét
    }
}