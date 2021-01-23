using EmikBaseModules;
using System;
using System.Collections;
using UnityEngine;
using Updog;
using IntTuple = EmikBaseModules.Tuple<int, int>;
using OnActivate = KMBombModule.KMModuleActivateEvent;
using OnInteract = KMSelectable.OnInteractHandler;
using Sound = KMSoundOverride.SoundEffect;
using String = System.Text.StringBuilder;

public class UpdogScript : ModuleScript
{
    public KMAudio Audio;
    public KMBombModule Module;
    public KMColorblindMode Colorblind;
    public KMSelectable[] Arrows, Center;
    public Renderer CenterRenderer;
    public TextMesh Text;
    public UpdogTPScript TP;

    internal override KMBombModule KMBombModule { get { return Module; } }
    internal override OnActivate OnActivate { get { return Activate(); } }

    internal override KMSelectable[][] KMSelectable { get { return new[] { Arrows, Center }; } }
    internal override Func<int, OnInteract>[] OnInteract { get { return new Func<int, OnInteract>[] { ArrowsInteract, CenterInteract }; } }

    internal override KMColorblindMode KMColorblindMode { get { return Colorblind; } }

    internal bool IsOnBone { get { return _maze[_position.Item1][_position.Item2] == 'x'; } }
    internal bool[] ValidMoves { get { return _maze.GetValidMovements(ref _position); } }
    internal int OrderOffset { get { return _order[_interactCount % 4] ? 4 : 0; } }

    private IntTuple _position, _initialPosition;
    private String[] _maze, _initialMaze;

    private bool[] _order;
    private int _interactCount;

    private OnActivate Activate()
    {
        return () =>
        {
            var colors = Colors.GetFinal;
            var word = Words.GetRandom;
            new IEnum<Color[], string>(Flash, this).StartCoroutine(colors, word.Key);
            var maze = Mazes.Get(word.Value, colors[2], colors[4]);

            _position = _initialPosition = maze.Find(colors[0]);
            _order = Words.GetOrder(colors[6], word.Value.Item2);
            _initialMaze = maze.InsertBones();

            _maze = new String[_initialMaze.Length];
            _initialMaze.Copy(_maze);

            _order.ToLog(this);
            _maze.ToLog(this, _position);
        };
    }

    private OnInteract CenterInteract(int i)
    {
        return () =>
        {
            if (isSolve)
                return false;

            Center[0].Button(audio: Audio, intensityModifier: 2, gameSound: Sound.BigButtonPress);

            if (!IsOnBone)
            {
                this.Log("The current tile ({0},{1}) does not contain a bone and now the dog is sad, strike!"
                    .Format(_position.Item1.ElevenToFiveIndex(), _position.Item2.ElevenToFiveIndex()));
                Strike();
                return false;
            }

            this.Log("A bone has been picked up! {0} bone{1} remaining..."
                .Format(_maze.CountBones() - 1, _maze.CountBones() - 1 == 1 ? "" : "s"));

            _maze[_position.Item1][_position.Item2] = ' ';

            if (_maze.IsSolved())
                Solve();

            return false;
        };
    }

    private OnInteract ArrowsInteract(int i)
    {
        return () =>
        {
            if (isSolve)
                return false;

            Arrows[i].Button(audio: Audio, intensityModifier: 1, gameSound: Sound.ButtonPress);

            if (_order[_interactCount % 4] ^ i >= 4)
            {
                this.Log("The wrong type of button has been pushed, causing the dog to trip and fall. Strike for being unable to walk correctly!");
                Strike();
            }
            else if (!_maze.IsValidMove(this, ref _position, i % 4))
            { 
                Strike();
            }
            else
            { 
                _interactCount++;
            }

            return false;
        };
    }

    private IEnumerator Flash(Color[] colors, string text)
    {
        const float Time = 0.375f;

        string[] colorblind = colors.AsString(text);

        for (int i = 0; !isSolve; i = i + 1 < colors.Length ? i + 1 : 0)
        {
            UpdateCenter(isColorblind ? colorblind[i] : text, colors[i]);
            yield return new WaitForSecondsRealtime(Time);
        }

        UpdateCenter(Colors.White, Colors.white);
    }

    private void UpdateCenter(string str, Color color)
    {
        var lightColors = new[] { Colors.yellow, Colors.green, Colors.white};

        Text.text = str;
        Text.color = lightColors.IsAnyEqual(color) ? Color.black : Color.white;
        CenterRenderer.material.color = color;
    }

    private void Solve()
    {
        Audio.PlaySound(transform: transform, gameSound: Sound.CorrectChime);
        isSolve = true;
        this.Log("Solved! :)");
        Module.HandlePass();
    }

    private void Strike()
    {
        TP.IsStrike = true;
        _interactCount = 0;
        _position = _initialPosition;

        _initialMaze.Copy(_maze);

        Module.HandleStrike();
    }
}
