using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random;
public class updogScript : MonoBehaviour 
{
	public KMAudio Audio;
	public KMBombInfo Bomb;
	public KMBombModule Module;

	public KMSelectable Doge;
	public KMSelectable[] Dogs;
	public KMSelectable[] Regs;
    public TextMesh text;
    public Renderer DogeColor;
    string sequence = "";
    int indexOfSequence = 0;
    List<Color32> colorsCycle = new List<Color32>();
    string[] a = { "DAWG", "DOG", "DOGE", "DAG", "DOGG", "DAGE", "dog", "dawg", "dag", "doge", "dogg", "dage"};
    List<Color32> COL14 = new List<Color32>() { new Color32(255, 0, 0, 255), new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 255), new Color32(125, 0, 125, 255), new Color32(255, 125, 0, 255), new Color32(255, 255, 0, 255) };
    List<Color32> COL23 = new List<Color32>() { new Color32(255, 0, 0, 255), new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 255) };
    List<Color32> oCOL14 = new List<Color32>() { new Color32(255, 0, 0, 255), new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 255), new Color32(125, 0, 125, 255), new Color32(255, 125, 0, 255), new Color32(255, 255, 0, 255) };
    List<Color32> pCOL23 = new List<Color32>() { new Color32(255, 0, 0, 255), new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 255) };

    string[,] maze;
    int[] positionOfPlayer = {0, 0};
    int[] bonePosition1 = {};
    int bones;
    string[] letterOfColor = {"R", "G", "B", "P", "O", "Y"};
    //Logging
    static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;
    // Custom Objects
    struct bonePositions{
        public int x;
        public int y;
    }

    List<bonePositions> positions = new List<bonePositions>(){
        
    };
	void Awake()
	{
		moduleId = moduleIdCounter++;
		foreach (KMSelectable Dog in Dogs)
		{
			KMSelectable pressedDog = Dog;
			Dog.OnInteract += delegate () {pressedDog.AddInteractionPunch(); DogsPress(pressedDog); return false; };
		} 

		foreach (KMSelectable Reg in Regs)
		{
			KMSelectable pressedReg = Reg;
			Reg.OnInteract += delegate () {pressedReg.AddInteractionPunch(); RegsPress(pressedReg); return false; };
		} 

		Doge.OnInteract += delegate () {PressDoge(); return false; };
	}
    void Start () 
	{
        COL14.Shuffle();
        COL23.Shuffle();
        for (int i = 0; i < 4; i++)
        {
            if (i != 1 && i != 2)
                colorsCycle.Add(COL14[i]);
            else
                colorsCycle.Add(COL23[i]);
        }
        StartCoroutine(cycling());
        text.text = a[Rnd.Range(0, a.Length)];
        sequenceGet();
        Debug.LogFormat("updog #{0}] Text is: {1}", moduleId, text.text);   
        mazeStuff();
        getPlayerPosition();
        getBonePositions();
	}

    void mazeStuff(){
        switch(text.text.ToLowerInvariant()){
            case "dog":
                maze = new string[5, 5]{{"R", ".", ".", ".", "."},
                                        {".", ".", ".", "Y", "."},
                                        {"O", ".", "G", ".", "B"},
                                        {".", ".", ".", ".", "."},
                                        {"P", ".", ".", ".", "."}};
                break;
            case "dawg":
                maze = new string[5, 5]{{".", ".", ".", ".", "B"},
                                        {".", "R", ".", "Y", "."},
                                        {".", ".", ".", ".", "."},
                                        {".", "O", ".", "G", "."},
                                        {"P", ".", ".", ".", "."}};
                break;
            case "doge":
                maze = new string[5, 5]{{"R", ".", ".", ".", "B"},
                                        {".", ".", ".", ".", "."},
                                        {".", ".", "O", "G", "."},
                                        {".", ".", ".", ".", "."},
                                        {"P", ".", ".", ".", "Y"}};
                break;
            case "dag":
                maze = new string[5, 5]{{"G", ".", "R", ".", "B"},
                                        {"O", ".", ".", ".", "."},
                                        {".", ".", ".", ".", "Y"},
                                        {".", ".", ".", ".", "."},
                                        {"P", ".", ".", ".", "."}};
                break;
            case "dogg":
                maze = new string[5, 5]{{"R", ".", ".", ".", "."},
                                        {"Y", ".", ".", ".", "."},
                                        {".", ".", "O", ".", "."},
                                        {".", ".", ".", ".", "B"},
                                        {"G", ".", ".", ".", "P"}};
                break;
            case "dage":
                maze = new string[5, 5]{{".", ".", ".", "R", "."},
                                        {".", "Y", ".", ".", "."},
                                        {".", ".", ".", "O", "."},
                                        {".", "G", ".", ".", "B"},
                                        {".", ".", ".", ".", "P"}};
                break;
        }
        Debug.LogFormat("[Updog #{0}] Maze used: {1}", moduleId, text.text);
    }
    void getPlayerPosition(){
        int indexOfColor = oCOL14.IndexOf(colorsCycle[0]);
        for (int i = 1; i < 6; i++){
            for (int j = 1; j < 6; j++){
                if (maze[i-1, j-1] == letterOfColor[indexOfColor]){
                    positionOfPlayer[0] = j;
                    positionOfPlayer[1] = i;
                }
            }
        }
        Debug.LogFormat("[Updog #{0} Initial player position is {1}, {2}", moduleId, positionOfPlayer[0], positionOfPlayer[1]);
    }
    void getBonePositions(){
        if (colorsCycle[1].Equals(COL23[0]) && colorsCycle[2].Equals(COL23[0])){
            positions.Add(new bonePositions{x = 1, y = 2});
            positions.Add(new bonePositions{x = 4, y = 3});
            positions.Add(new bonePositions{x = 3, y = 4});
            positions.Add(new bonePositions{x = 1, y = 5});
            positions.Add(new bonePositions{x = 5, y = 5});
        }
        else if (colorsCycle[1].Equals(COL23[0]) && colorsCycle[2].Equals(COL23[2])){
            positions.Add(new bonePositions{x = 1, y = 1});
            positions.Add(new bonePositions{x = 3, y = 1});
            positions.Add(new bonePositions{x = 4, y = 2});
            positions.Add(new bonePositions{x = 5, y = 2});
            positions.Add(new bonePositions{x = 2, y = 3});
        }
        else if (colorsCycle[1].Equals(COL23[0]) && colorsCycle[2].Equals(COL23[1])){
            positions.Add(new bonePositions{x = 1, y = 2});
            positions.Add(new bonePositions{x = 2, y = 2});
            positions.Add(new bonePositions{x = 2, y = 3});
            positions.Add(new bonePositions{x = 1, y = 4});
            positions.Add(new bonePositions{x = 3, y = 4});
        }
        else if (colorsCycle[1].Equals(COL23[2]) && colorsCycle[2].Equals(COL23[0])){
            positions.Add(new bonePositions{x = 1, y = 1});
            positions.Add(new bonePositions{x = 2, y = 3});
            positions.Add(new bonePositions{x = 4, y = 1});
            positions.Add(new bonePositions{x = 5, y = 1});
            positions.Add(new bonePositions{x = 5, y = 2});
        }
        else if (colorsCycle[1].Equals(COL23[2]) && colorsCycle[2].Equals(COL23[2])){
            positions.Add(new bonePositions{x = 2, y = 2});
            positions.Add(new bonePositions{x = 1, y = 4});
            positions.Add(new bonePositions{x = 1, y = 5});
            positions.Add(new bonePositions{x = 3, y = 4});
            positions.Add(new bonePositions{x = 5, y = 4});
        }
        else if (colorsCycle[1].Equals(COL23[2]) && colorsCycle[2].Equals(COL23[1])){
            positions.Add(new bonePositions{x = 3, y = 1});
            positions.Add(new bonePositions{x = 3, y = 2});
            positions.Add(new bonePositions{x = 2, y = 3});
            positions.Add(new bonePositions{x = 3, y = 5});
            positions.Add(new bonePositions{x = 5, y = 3});
        }
        else if (colorsCycle[1].Equals(COL23[1]) && colorsCycle[2].Equals(COL23[0])){
            positions.Add(new bonePositions{x = 5, y = 1});
            positions.Add(new bonePositions{x = 5, y = 2});
            positions.Add(new bonePositions{x = 2, y = 3});
            positions.Add(new bonePositions{x = 5, y = 4});
            positions.Add(new bonePositions{x = 2, y = 5});
        }
        else if (colorsCycle[1].Equals(COL23[1]) && colorsCycle[2].Equals(COL23[2])){
            positions.Add(new bonePositions{x = 1, y = 1});
            positions.Add(new bonePositions{x = 1, y = 3});
            positions.Add(new bonePositions{x = 3, y = 3});
            positions.Add(new bonePositions{x = 2, y = 5});
            positions.Add(new bonePositions{x = 4, y = 5});
        }
        else{
            positions.Add(new bonePositions{x = 1, y = 2});
            positions.Add(new bonePositions{x = 2, y = 1});
            positions.Add(new bonePositions{x = 2, y = 2});
            positions.Add(new bonePositions{x = 3, y = 4});
            positions.Add(new bonePositions{x = 4, y = 2});
        }
    }
    IEnumerator cycling()
    {
        yield return null;
        int a = 0;
        while (!moduleSolved)
        {
            if (a <= 3)
            {
                DogeColor.material.color = colorsCycle[a];
                DogeColor.gameObject.GetComponentInChildren<Light>().color = colorsCycle[a];
                a++;
                yield return new WaitForSeconds(.3f);
            }
            else
            {
                DogeColor.material.color = new Color32(0, 0, 0, 255);
                DogeColor.gameObject.GetComponentInChildren<Light>().color = new Color32(0, 0, 0, 255);
                a = 0;
                yield return new WaitForSeconds(.6f);
            }            
        }
    }
	void sequenceGet()
    {
        Debug.LogFormat("[updog #{0}] 4th color is: {1}", moduleId, colorsCycle[3]);
        if (text.text == "DAWG" || text.text == "DOG" || text.text == "DOGE" || text.text == "DAG" || text.text == "DOGG" || text.text == "DAGE")
        {
            if (colorsCycle[3].Equals(new Color32(255, 0, 0, 255)))
            {
                sequence = "DDDD";
            }
            else if (colorsCycle[3].Equals(new Color32(255, 255, 0, 255)))
            {
                sequence = "DDDN";
            }
            else if (colorsCycle[3].Equals(new Color32(255, 125, 0, 255))) 
            {
                sequence = "DDND";
            }
            else if (colorsCycle[3].Equals(new Color32(0, 255, 0, 255)))
            {
                sequence = "DDNN";
            }
            else if (colorsCycle[3].Equals(new Color32(0, 0, 255, 255))) 
            {
                sequence = "DNDD";
            }
            else if (colorsCycle[3].Equals(new Color32(125, 0, 125, 255)))
            {
                sequence = "DNND";
            }
        }
        else
        {
            if (colorsCycle[3].Equals(new Color32(255, 0, 0, 255)))
            {
                sequence = "NNNN";
            }
            else if (colorsCycle[3].Equals(new Color32(255, 255, 0, 255)))
            {
                sequence = "NNND";
            }
            else if (colorsCycle[3].Equals(new Color32(255, 125, 0, 255))) 
            {
                sequence = "NNDN";
            }
            else if (colorsCycle[3].Equals(new Color32(0, 255, 0, 255)))
            {
                sequence = "NNDD";
            }
            else if (colorsCycle[3].Equals(new Color32(0, 0, 255, 255))) 
            {
                sequence = "NDNN";
            }
            else if (colorsCycle[3].Equals(new Color32(125, 0, 125, 255)))
            {
                sequence = "NDDN";
            }
        }
        Debug.LogFormat("[updog #{0}] Sequence: {1}", moduleId, sequence);
    }
	void DogsPress(KMSelectable Dog)
	{
        if (sequence[indexOfSequence] == 'N'){
            if (Dog.name == "UpReg"){
                positionOfPlayer[1]--;
                if (positionOfPlayer[1] < 0){
                    Module.HandleStrike();
                    positionOfPlayer[1]++;
                }
                else {
                    indexOfSequence++;
                    if (indexOfSequence == 4){
                        indexOfSequence = 0;
                    }
                }
            }
            else if (Dog.name == "DownReg"){
                positionOfPlayer[1]++;
                if (positionOfPlayer[1] > 5){
                    Module.HandleStrike();
                    positionOfPlayer[1]--;
                }
                else {
                    indexOfSequence++;
                    if (indexOfSequence == 4){
                        indexOfSequence = 0;
                    }
                }
            }
            else if (Dog.name == "LeftReg"){
                positionOfPlayer[0]--;
                if (positionOfPlayer[0] < 0){
                    Module.HandleStrike();
                    positionOfPlayer[0]++;
                }
                else {
                    indexOfSequence++;
                    if (indexOfSequence == 4){
                        indexOfSequence = 0;
                    }
                }
            }
            else{
                positionOfPlayer[0]++;
                if (positionOfPlayer[1] > 0){
                    Module.HandleStrike();
                    positionOfPlayer[1]--;
                }
                else {
                    indexOfSequence++;
                    if (indexOfSequence == 4){
                        indexOfSequence = 0;
                    }
                }
            }
            Debug.Log("A dog button has been pressed.");
            Debug.LogFormat("Player position: ({0}, {1})", positionOfPlayer[0], positionOfPlayer[1]);
        }
        else{
            Module.HandleStrike();
            bones = 0;
        }
	}

	void RegsPress(KMSelectable Reg)
	{
        if (sequence[indexOfSequence] == 'N'){
            if (Reg.name == "UpReg"){
                positionOfPlayer[1]--;
                if (positionOfPlayer[1] < 0){
                    Module.HandleStrike();
                    positionOfPlayer[1]++;
                }
                else {
                    indexOfSequence++;
                    if (indexOfSequence == 4){
                        indexOfSequence = 0;
                    }
                }
            }
            else if (Reg.name == "DownReg"){
                positionOfPlayer[1]++;
                if (positionOfPlayer[1] > 5){
                    Module.HandleStrike();
                    positionOfPlayer[1]--;
                }
                else {
                    indexOfSequence++;
                    if (indexOfSequence == 4){
                        indexOfSequence = 0;
                    }
                }
            }
            else if (Reg.name == "LeftReg"){
                positionOfPlayer[0]--;
                if (positionOfPlayer[0] < 0){
                    Module.HandleStrike();
                    positionOfPlayer[0]++;
                }
                else {
                    indexOfSequence++;
                    if (indexOfSequence == 4){
                        indexOfSequence = 0;
                    }
                }
            }
            else{
                positionOfPlayer[0]++;
                if (positionOfPlayer[1] > 0){
                    Module.HandleStrike();
                    positionOfPlayer[1]--;
                }
                else {
                    indexOfSequence++;
                    if (indexOfSequence == 4){
                        indexOfSequence = 0;
                    }
                }
            }
            Debug.Log("A regular button has been pressed.");
            Debug.LogFormat("Player position: ({0}, {1})", positionOfPlayer[0], positionOfPlayer[1]);
        }
        else{
            Module.HandleStrike();
            bones = 0;
        }
	}

	void PressDoge()
	{
        bool found = false;
        for (int i = 0; i < 5; i++){
            if (positionOfPlayer[0] == positions[i].x && positionOfPlayer[1] == positions[i].y){
                found = true;
            }
        }
        if (found){
            bones++;
            Debug.LogFormat("[Updog #{0}] A bone has been collected!", moduleId);
            found = false;
        }
        else{
            Module.HandleStrike();
            bones = 0;
        }
        if (bones == 5){
            moduleSolved = true;
            foreach (KMSelectable Dog in Dogs)
		    {
                Dog.gameObject.SetActive(false);
		    } 
		    foreach (KMSelectable Reg in Regs)
		    {
                Reg.gameObject.SetActive(false);
		    } 
            Doge.gameObject.SetActive(false);
            Module.HandlePass();
        }
	}
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} Dog [U/R/D/L] to press a Dog button of that direction. | !{0} Nor [U/R/D/L] to press a Normal button of that direction. | !{0} Updog to press the middle button.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*Dog\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {   
            yield return null;
            if (parameters.Length < 2)
            {
                yield return "sendtochaterror Please specify what number you would like to press!";
            }
            else if (parameters.Length > 3)
            {
                yield return "sendtochaterror Too many arguements!";
            }
            else if (Regex.IsMatch(parameters[1], @"^\s*L\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
                Dogs[1].OnInteract();
            }
            else if (Regex.IsMatch(parameters[1], @"^\s*R\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
                Dogs[2].OnInteract();
            }
            else if (Regex.IsMatch(parameters[1], @"^\s*U\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
                Dogs[0].OnInteract();
            }
            else if (Regex.IsMatch(parameters[1], @"^\s*D\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
                Dogs[3].OnInteract();
            }
        }
        else if (Regex.IsMatch(parameters[0], @"^\s*Nor\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {   
            yield return null;
            if (parameters.Length < 2)
            {
                yield return "sendtochaterror Please specify what number you would like to press!";
            }
            else if (parameters.Length > 3)
            {
                yield return "sendtochaterror Too many arguements!";
            }
            else if (Regex.IsMatch(parameters[1], @"^\s*L\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
                Regs[2].OnInteract();
            }
            else if (Regex.IsMatch(parameters[1], @"^\s*R\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
                Regs[1].OnInteract();
            }
            else if (Regex.IsMatch(parameters[1], @"^\s*U\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
                Regs[0].OnInteract();
            }
            else if (Regex.IsMatch(parameters[1], @"^\s*D\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
                Regs[3].OnInteract();
            }
        }
        else if (Regex.IsMatch(parameters[0], @"^\s*Updog\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)){
            yield return null;
            Doge.OnInteract();
        }
        else{
            yield return "sendtochaterror Please give a valid command.";
        }
    }
}