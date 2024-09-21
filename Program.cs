global using static Raylib_cs.Raylib;
using Raylib_cs;
using GNSUsingCS;
using System.Numerics;
using System.Diagnostics;
using GNSUsingCS.Tabs.WorkspaceTab;
using GNSUsingCS.Tabs.ChoiceTab;
using GNSUsingCS.Elements;
using System.IO.Compression;

/*

string zipFile = "compressed_files.zip";
string contentTxt = "someVar = imp 'textFile.txt'";
string textFileTxt = "stuff";

using (FileStream zipToCreate = new FileStream(zipFile, FileMode.Create))
using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
{
    // Create and write to content.txt
    ZipArchiveEntry contentEntry = archive.CreateEntry("content.txt");
    using (StreamWriter writer = new StreamWriter(contentEntry.Open()))
    {
        writer.Write(contentTxt);
    }

    // Create and write to assets/textFile.txt
    ZipArchiveEntry textFileEntry = archive.CreateEntry("assets/textFile.txt");
    using (StreamWriter writer = new StreamWriter(textFileEntry.Open()))
    {
        writer.Write(textFileTxt);
    }
}

using (ZipArchive archive = ZipFile.OpenRead(zipFile))
{
    foreach (ZipArchiveEntry entry in archive.Entries)
    {
        Console.WriteLine($"File: {entry.FullName}");
        using (StreamReader reader = new StreamReader(entry.Open()))
        {
            string content = reader.ReadToEnd();
            Console.WriteLine($"Content: {content}\n");
        }
    }
}

Console.ReadKey();
*/

/*
string a = SaveAndLoadManager.SetupArray(["a", "f"]) + " asd";
var b = SaveAndLoadManager.ParseArray(a);
*/

/*
// settings(of presets) / general saves testing
Button button1 = new Button();

button1.NeedsSave = false;
button1.Label.Text = "haha string boyyy~";
Console.WriteLine(((Button)button1).Label.Text);

ElementSettingsInstance ESI = new ElementSettingsInstance(button1);

Element button2 = ESI.CreateElementFrom();
Console.WriteLine(((Button)button2).Label.Text);

new ElementSettingsInstance(button2).SaveInstance("testFile.save");

ElementSettingsInstance esi3 = ElementSettingsInstance.CreateInstance("testFile.save");

Element button3 = esi3.CreateElementFrom();
Console.WriteLine(((Button)button3).Label.Text);

Console.ReadKey();
*/

SaveAndLoadManager.RelativePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

InitWindow(1200, 800, "raylib [core] example - basic window");
SetTargetFPS(60);
// ToggleFullscreen();
// ToggleBorderlessWindowed();

LuaInterfacer.SetupLuaInterfacer();

ApplicationManager AM = new();

Tab t = SaveAndLoadManager.LoadTab("Origin");

if (t == null)
{
    t = new ChoiceTab();
    t.UUID = "Origin";

    (t as ChoiceTab).SetTempTestThings();
}

AM.AddTab(t);

AM.AddWorkspace();

while (!WindowShouldClose())
{
    // Update
    //stateHandler.UpdateState();
    //TextHandler::Update();

    AM.PreUpdate();

    Vector2 mbPos = GetMousePosition();
    AM.MouseCaptured((int)mbPos.X, (int)mbPos.Y);

    MouseManager.Update();// TODO: should probably replace ^^, but it can stay for now

    InputManager.Update();

    if (IsMouseButtonPressed(MouseButton.Left))
        InputManager.ClearInput();

    AM.Update();

    // Draw
    BeginDrawing();

    ClearBackground(Color.White);

    //stateHandler.DrawState();

    /*
    if (IsMouseButtonPressed(MouseButton.Left))
    {
    }
    */

    /*
    int fontSize = 25;
    DrawTextEx(FontManager.GetFont("Vera", fontSize, 0), "Congrats! You created your first window!", new(190, 200-20), fontSize, 1f, Color.Black);
    DrawText("Congrats! You created your first window!", 190, 200, 20, Color.LightGray);
    */

    AM.Draw();

    EndDrawing();
}

CloseWindow();
