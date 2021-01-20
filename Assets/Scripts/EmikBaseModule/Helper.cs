using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

namespace EmikBaseModules
{
    /// <summary>
    /// Helper/extension class for regular and needy modded modules in Keep Talking and Nobody Explodes written by Emik.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Creates an auto-formatted debug log, typically used to display information about the module. Use String.Format to assign variables.
        /// </summary>
        /// <param name="module">The module that called this method, since it needs to access the module's name and id.</param>
        /// <param name="log">The information to log.</param>
        /// <param name="logType"></param>
        internal static void Log(this ModuleScript module, string log, LogType logType = LogType.Log)
        {
            string formattedLog = "[{0} #{1}]: {2}".Format((object)module.ModuleName, module.moduleId, log);

            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(formattedLog);
                    break;
                case LogType.Assert:
                    Debug.LogAssertion(formattedLog);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(formattedLog);
                    break;
                case LogType.Log:
                    Debug.Log(formattedLog);
                    break;
                case LogType.Exception:
                    Debug.LogWarning(formattedLog);
                    break;
                default:
                    throw new NotImplementedException(logType.ToString() + " is not a valid log type.");
            }
        }

        /// <summary>
        /// A method that can play custom/game sounds, and/or shake the bomb. This is a multi-purpose basic enhancement of production value. Null means actions will not be performed.
        /// </summary>
        /// <param name="selectable">The selectable, which is used to call other methods, and use its transform.</param>
        /// <param name="audio">The current instance of KMAudio which is needed to play sound.</param>
        /// <param name="transform">The location of the source, if the selectable isn't being used.</param>
        /// <param name="intensityModifier">Adds bomb movement and controller vibration on interaction, amount is based on the modifier.</param>
        /// <param name="customSound">The custom sound to play, which must be assigned in TestHarness in the editor or mod.bundle in-game for the sound to be heard.</param>
        /// <param name="gameSound">The built-in sound effect to play.</param>
        /// <param name="ignoredCondition">A condition that will cancel this method. The null checks are still performed.</param>
        internal static void Button(this KMSelectable selectable, KMAudio audio = null, Transform transform = null, float? intensityModifier = null, string customSound = null, KMSoundOverride.SoundEffect? gameSound = null, bool ignoredCondition = false)
        {
            if (selectable == null)
                throw new NullReferenceException("Selectable should not be null when calling this method.");
            if (audio == null && (customSound != null || gameSound != null))
                throw new NullReferenceException("Audio should not be null if customSound and gameSound is specified. An instance of KMAudio is required for the sounds to be played.");
            if (ignoredCondition)
                return;
            if (transform == null && selectable != null)
                transform = selectable.transform;
            if (intensityModifier != null)
                selectable.AddInteractionPunch((float)intensityModifier);
            audio.PlaySound(transform, customSound, gameSound);
        }

        /// <summary>
        /// Plays an in-game sound and/or custom sound.
        /// </summary>
        /// <param name="audio">The instance of audio, so that the sound can be played.</param>
        /// <param name="transform">The location of the sound.</param>
        /// <param name="customSound">The custom sound effect, which is ignored if null.</param>
        /// <param name="gameSound">The in-game sound effect, which is ignored if null.</param>
        internal static void PlaySound(this KMAudio audio, Transform transform, string customSound = null, KMSoundOverride.SoundEffect? gameSound = null)
        {
            if (customSound != null)
                audio.PlaySoundAtTransform(customSound, transform);
            if (gameSound != null)
                audio.PlayGameSoundAtTransform((KMSoundOverride.SoundEffect)gameSound, transform);
        }

        /// <summary>
        /// Appends an element to an Enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="source">The enumerable to append with.</param>
        /// <param name="item">The item to append to the enumerable.</param>
        /// <returns>A new instance of the enumerable, with an added last entry being the item.</returns>
        internal static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
        {
            return source.Concat(new T[] { item });
        }

        /// <summary>
        /// Appends an element to an array.
        /// </summary>
        /// <typeparam name="T">The type that both the array and element are.</typeparam>
        /// <param name="array">The array that needs to be appended.</param>
        /// <param name="element">The element to append to the array.</param>
        /// <returns>The new array, consisting of the old array, then the element.</returns>
        internal static T[] Append<T>(this T[] array, T element)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = element;
            return array;
        }

        /// <summary>
        /// Gets multiple random floats between two values. Min is inclusive, max is inclusive.
        /// </summary>
        /// <param name="min">The minimum float (inclusive) to generate.</param>
        /// <param name="max">The maximum float (inclusive) to generate.</param>
        /// <param name="times">The length of the array, or the amount of times the numbers are needed to be generated.</param>
        /// <returns>An array of random floats between min and max, of length times.</returns>
        internal static float[] Ranges(float min, float max, int times)
        {
            float[] vs = new float[times];
            for (int i = 0; i < times; i++)
                vs[i] = Rnd.Range(min, max);
            return vs;
        }

        /// <summary>
        /// Gets multiple random integers between two values. Min is inclusive, max is exclusive.
        /// </summary>
        /// <param name="min">The minimum integer (inclusive) to generate.</param>
        /// <param name="max">The maximum integer (exclusive) to generate.</param>
        /// <param name="times">The length of the array, or the amount of times the numbers are needed to be generated.</param>
        /// <returns>An array of random integers between min and max, of length times.</returns>
        internal static int[] Ranges(int min, int max, int times)
        {
            int[] vs = new int[times];
            for (int i = 0; i < times; i++)
                vs[i] = Rnd.Range(min, max);
            return vs;
        }

        /// <summary>
        /// Modifies an existing color.
        /// </summary>
        /// <param name="color">The color to modify.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        /// <returns>A new instance of the color, with the arguments replacing the values of the color.</returns>
        internal static Color32 Replace(this Color32 color, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color32(
                r == null ? (byte)(r * 255) : color.r,
                g == null ? (byte)(g * 255) : color.g,
                b == null ? (byte)(b * 255) : color.b,
                a == null ? (byte)(a * 255) : color.a);
        }

        /// <summary>
        /// Modifies an existing color.
        /// </summary>
        /// <param name="color">The color to modify.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        /// <returns>A new instance of the color, with the arguments replacing the values of the color.</returns>
        internal static Color32 Replace(this Color32 color, byte? r = null, byte? g = null, byte? b = null, byte? a = null)
        {
            return new Color32(
                r == null ? (byte)r : color.r,
                g == null ? (byte)g : color.g,
                b == null ? (byte)b : color.b,
                a == null ? (byte)a : color.a);
        }

        /// <summary>
        /// Modifies an existing color.
        /// </summary>
        /// <param name="color">The color to modify.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        /// <returns>A new instance of the color, with the arguments replacing the values of the color.</returns>
        internal static Color Replace(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            return new Color(
                r == null ? (float)r : color.r,
                g == null ? (float)g : color.g,
                b == null ? (float)b : color.b,
                a == null ? (float)a : color.a);
        }

        /// <summary>
        /// Modifies an existing color by replacing each component.
        /// </summary>
        /// <param name="color">The color to modify.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        /// <returns>A new instance of the color, with the arguments adding the values of the color.</returns>
        internal static Color Replace(this Color color, byte? r = null, byte? g = null, byte? b = null, byte? a = null)
        {
            return new Color(
                r == null ? (float)r / 255 : color.r,
                g == null ? (float)g / 255 : color.g,
                b == null ? (float)b / 255 : color.b,
                a == null ? (float)a / 255 : color.a);
        }

        /// <summary>
        /// Modifies an existing color by adding each component.
        /// </summary>
        /// <param name="color">The color to modify.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        /// <returns>A new instance of the color, with the arguments replacing the values of the color.</returns>
        internal static Color32 Add(this Color32 color, float r = 0, float g = 0, float b = 0, float a = 0)
        {
            return new Color32(
                (byte)((r * 255) + color.r),
                (byte)((g * 255) + color.g),
                (byte)((b * 255) + color.b),
                (byte)((a * 255) + color.a));
        }

        /// <summary>
        /// Modifies an existing color by adding each component.
        /// </summary>
        /// <param name="color">The color to modify.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        /// <returns>A new instance of the color, with the arguments adding the values of the color.</returns>
        internal static Color32 Add(this Color32 color, byte r = 0, byte g = 0, byte b = 0, byte a = 0)
        {
            return new Color32(
                (byte)(r + color.r),
                (byte)(g + color.g),
                (byte)(b + color.b),
                (byte)(a + color.a));
        }

        /// <summary>
        /// Modifies an existing color by adding each component.
        /// </summary>
        /// <param name="color">The color to modify.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        /// <returns>A new instance of the color, with the arguments adding the values of the color.</returns>
        internal static Color Add(this Color color, float r = 0, float g = 0, float b = 0, float a = 0)
        {
            return new Color(
                r + color.r,
                g + color.g,
                b + color.b,
                a + color.a);
        }

        /// <summary>
        /// Modifies an existing color by adding each component.
        /// </summary>
        /// <param name="color">The color to modify.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="a">The alpha component.</param>
        /// <returns>A new instance of the color, with the arguments adding the values of the color.</returns>
        internal static Color Add(this Color color, byte r = 0, byte g = 0, byte b = 0, byte a = 0)
        {
            return new Color(
                (r / 255) + color.r,
                (g / 255) + color.g,
                (b / 255) + color.b,
                (a / 255) + color.a);
        }

        /// <summary>
        /// Returns true/false if the array has any color that matches the same RGBA components as the other color.
        /// </summary>
        /// <param name="a">The array to scan through and check for equality.</param>
        /// <param name="b">The color to compare equality with.</param>
        /// <returns>True if any in the array match the other color's r, g, b, and a fields.</returns>
        internal static bool IsAnyEqual(this Color32[] a, Color32 b)
        {
            return a.Any(c => c.Equals(b));
        }

        /// <summary>
        /// Returns true/false if the array has any color that matches the same RGBA components as the other color.
        /// </summary>
        /// <param name="a">The array to scan through and check for equality.</param>
        /// <param name="b">The color to compare equality with.</param>
        /// <returns>True if any in the array match the other color's r, g, b, and a fields.</returns>
        internal static bool IsAnyEqual(this Color[] a, Color b)
        {
            return a.Any(c => c.Equals(b));
        }

        /// <summary>
        /// Returns true/false if the colors have the same RGBA components.
        /// </summary>
        /// <param name="a">The first color to compare equality with.</param>
        /// <param name="b">The second color to compare equality with.</param>
        /// <returns>True if both color's r, g, b, and a fields are equal.</returns>
        internal static bool IsEqual(this Color32 a, Color32 b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }

        /// <summary>
        /// Returns true/false if the colors have the same RGBA components.
        /// </summary>
        /// <param name="a">The first color to compare equality with.</param>
        /// <param name="b">The second color to compare equality with.</param>
        /// <returns>True if both color's r, g, b, and a fields are equal.</returns>
        internal static bool IsEqual(this Color a, Color b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }

        /// <summary>
        /// Gets all values from an enum type and returns it as an array.
        /// </summary>
        /// <typeparam name="T">An enum type.</typeparam>
        /// <returns>All values of the type specified.</returns>
        internal static T[] EnumAsArray<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        /// <summary>
        /// Formats and returns the string with the arguments specified. {#} where # is a number is used as placeholders for these variables.
        /// </summary>
        /// <param name="str">The template string.</param>
        /// <param name="args">The variables to assign into the string.</param>
        /// <returns>The string, replacing {#} where # is a number with the corresponding # index in the object array.</returns>
        internal static string Format(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}
