using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModernGod.Utils;
using ModernGod.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.Characters
{
    public class CharacterManager
    {
        public Map Map { get; private set; }

        public int CharacterCount
        {
            get
            {
                return characters.Count;
            }
        }
        private List<Character> characters = new List<Character>();
        private List<Character> pending = new List<Character>();

        public Character CreateCharacter(Vector2Int position, string name)
        {
            Character c = new Character(name, position, this);
            return c;
        }

        public CharacterManager(Map map)
        {
            this.Map = map ?? throw new ArgumentNullException("map", "The map object cannot be null!");
        }

        public void Init()
        {

        }

        public void Register(Character c)
        {
            if (c == null || c.IsDestroyed)
                return;

            if (!pending.Contains(c) && !characters.Contains(c))
            {
                pending.Add(c);
            }
        }

        public void Update()
        {
            pending.RemoveAll(x => x.IsDestroyed);
            characters.AddRange(pending);
            pending.Clear();

            characters.RemoveAll(x => x.IsDestroyed);
            foreach (var c in characters)
            {
                c.Update();
            }
        }

        public void Draw(SpriteBatch spr)
        {
            characters.RemoveAll(x => x.IsDestroyed);
            foreach (var c in characters)
            {
                c.Draw(spr);
            }
        }
    }
}
