global using static Raylib_cs.Raylib;
using Raylib_cs;
using GNSUsingCS;
using System.Numerics;

/*
string a = SaveAndLoadManager.SetupArray(["a", "f"]) + " asd";
var b = SaveAndLoadManager.ParseArray(a);
*/

InitWindow(1200, 800, "raylib [core] example - basic window");
SetTargetFPS(60);
// ToggleFullscreen();
// ToggleBorderlessWindowed();

LuaInterfacer.SetupLuaInterfacer();

ApplicationManager AM = new();

AM.AddWorkspace();
AM.ChoiceTab();
AM.ChoiceTab();

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