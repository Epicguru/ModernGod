using Microsoft.Xna.Framework;
using ModernGod.Debugging;
using ModernGod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ModernGod.World
{
    public abstract class Shrub
    {
        public static Dictionary<byte, Shrub> Loaded = new Dictionary<byte, Shrub>();
        public static byte HighestID { get; private set; }
        
        public byte ID { get; private set; }
        public string Name { get; private set; }
        public Color DefaultColour = Color.White;
        public Rectangle TextureBounds = new Rectangle(0, 0, 0, 0);
        
        public Shrub(byte ID, string name)
        {
            this.ID = ID;
            this.Name = name;
        }

        public void SetTexture(int x, int y, int width, int height)
        {
            TextureBounds = new Rectangle(x * 16, y * 16, width * 16, height * 16);
        }

        /// <summary>
        /// Called every frame for every drawn shrub of this type.
        /// It is the colour that it will be rendered in.
        /// </summary>
        /// <returns>The Color to render the placed shrub in.</returns>
        public virtual Color GetColour(int x, int y, Map map)
        {
            return DefaultColour;
        }

        public static void LoadShrubs()
        {
            // Load all Shrubs subclasses, within the current (calling) assembly.

            var a = Assembly.GetCallingAssembly();
            var classes = AttributeHelper.GetTypeSubclasses(a, typeof(Shrub));

            Debug.Log("Loading custom shrubs from assembly '{0}'...".Form(a.GetName().Name), ConsoleColor.Cyan);

            foreach (var t in classes)
            {
                bool found = false;
                foreach (var c in t.GetConstructors())
                {
                    if (c.GetParameters().Length == 0)
                    {
                        found = true;
                        var instance = (c.Invoke(new object[] { }));

                        Shrub s = instance as Shrub;
                        if (!Loaded.ContainsKey(s.ID))
                        {
                            Loaded.Add(s.ID, s);
                            if (HighestID < s.ID)
                                HighestID = s.ID;
                            Debug.Log("  > " + s.ID + " '" + s.Name + "'");
                            break;
                        }
                        else
                        {
                            Debug.LogError("There is already a shrub registered for ID {0}! '{1}' tried to register with the same ID as '{2}'".Form(s.ID, s.Name, Loaded[s.ID].Name));
                            break;
                        }
                    }
                }
                if(!found)
                    Debug.LogError("There is no zero-argument constructor for custom shrub '{0}'! It will not be registred!".Form(t.FullName));
            }
        }

        public static void UnloadShrubs()
        {
            Loaded.Clear();
            HighestID = 0;
        }

        public static bool IsLoaded(byte id)
        {
            return Loaded.ContainsKey(id);
        }

        public static Shrub Get(byte id)
        {
            // 0 is always null, nothing, empty.
            if (id == 0)
                return null;

            if (IsLoaded(id))
                return Loaded[id];
            else
                return null;
        }
    }
}
