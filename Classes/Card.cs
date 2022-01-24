using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MTCGClassLib
{
    /*
     * TB_Card
     * UID
     * Cardname nvarchar(50)
     * CardType int
     * Element int
     * MonsterRace int
     */
    public class Card
    {
        [JsonProperty("guid")]
        public Guid guid { get; set; }
        [JsonProperty("CardName")]
        public string CardName { get; private set; } // Primary Key
        [JsonProperty("Atk")]
        public int Atk { get; private set; }
        [JsonProperty("CardType")]
        public CardType CardType { get; private set; }
        [JsonProperty("CardElement")]
        public Element CardElement { get; private set; }
        [JsonProperty("Race")]
        public MonsterRace Race { get; private set; }
        public bool IsDeck { get; set; }

        public Card()
        { 
        }
        public Card(string name, int atk, CardType ctype, Element element, MonsterRace race, int IsDeck = 0)
        {
            guid = new Guid();
            CardName = name;
            Atk = atk;
            CardType = ctype;
            CardElement = element;
            Race = race;
            if (IsDeck == 0)
            {
                this.IsDeck = false;
            }
            else
            {
                this.IsDeck = true;
            }
        }
        public void DisplayNameElement()
        {
            Console.WriteLine(this.CardElement + " " + this.CardName);
        }
        public Card(Guid guid, string CardName, int Atk, int CardType, int CardElement, int Race, int IsDeck)
        {
            this.guid = guid;
            this.CardName = CardName;
            this.Atk = Atk;
            this.CardType = (CardType)CardType;
            this.CardElement = (Element)CardElement;
            this.Race = (MonsterRace)Race;
            if(IsDeck == 0)
            {
                this.IsDeck = false;
            }
            else
            {
                this.IsDeck = true;
            }
            
        }
    }
}