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
        
        public byte ID;
        public string Name;
        public Color DefaultColour = Color.White;
        
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
                foreach (var c in t.GetConstructors())
                {
                    if (c.GetParameters().Length == 0)
                    {
                        var instance = (c.Invoke(new object[] { }));
                        if (instance is Shrub)
                        {
                            Shrub s = instance as Shrub;
                            if (!Loaded.ContainsKey(s.ID))
                            {
                                Loaded.Add(s.ID, s);
                                Debug.Log("  >" + s.ID + " '" + s.Name + "'");
                                continue;
                            }
                            else
                            {
                                Debug.LogError("There is already a shrub registered for ID {0}! '{1}' tried to register with the same ID as '{2}'".Form(s.ID, s.Name, Loaded[s.ID].Name));
                                continue;
                            }
                        }
                        else
                        {
                            Debug.LogError("Type '{0}' is not a subclass of Shrub, cannot have the CustomShrub attribute!".Form(t.FullName));
                            continue;
                        }
                    }
                }
                Debug.LogError("There is no zero-argument constructor for custom shrub '{0}'! It will not be registred!".Form(t.FullName));
                continue;
            }
        }

        public static void UnloadShrubs()
        {
            Loaded.Clear();
        }

        public static bool IsLoaded(byte id)
        {
            return Loaded.ContainsKey(id);
        }

        public static Shrub Get(byte id)
        {
            if (IsLoaded(id))
                return Loaded[id];
            else
                return null;
        }
    }
}
