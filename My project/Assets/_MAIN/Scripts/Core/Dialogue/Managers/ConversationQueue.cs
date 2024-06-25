using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class ConversationQueue
    {
        private Queue<Conversation> conversationQueue = new Queue<Conversation>(); //párbeszédeket tárol
        public Conversation top => conversationQueue.Peek(); // sor elején lévő párbeszéd eltávolitás nélkül
        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation); // hozzáad sor végére
        public void EnqueuePriority(Conversation conversation) // sor elejére tesz
        {
            Queue<Conversation> queue = new Queue<Conversation>(); // új sor létrehozása
            queue.Enqueue(conversation);

            while (conversationQueue.Count > 0)
                queue.Enqueue(conversationQueue.Dequeue());

            conversationQueue = queue;
        }

        public void Dequeue() // sor eleji párbeszéd eltávolitása
        {
            if (conversationQueue.Count > 0)
                conversationQueue.Dequeue();
        }

        public bool IsEmpty() => conversationQueue.Count == 0; // üres-e a sor

        public void Clear() => conversationQueue.Clear(); // sor törlése
    }
}