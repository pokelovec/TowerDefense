using System.Data;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using Raylib_cs;
enum Gamestate {Welcome, Battle, Pause, Upgrade, Death, Exit, HowToPlay}
public struct Bullet 
{
    public Rectangle Rect;
    public Vector2 Velocity;
}
public struct Enemy
{
    public Rectangle Rect;
    public float enemyLvl;
    public float enemyHp;
    public float enemyDmg;
    public float enemySpd;
}
class Program
{
    static void Main()
    {
        Raylib.InitWindow(1, 1, "emental");
        Raylib.ToggleFullscreen();
        int screenHeight = Raylib.GetScreenHeight();
        int screenWidth = Raylib.GetScreenWidth();
        Color DarkRed = new Color(139, 0, 0, 255);
        Gamestate State = Gamestate.Welcome;

        //variables
        
        //player
        float playerDmg = 10.0f, playerHp = 150.0f, weaponDmg = 0, armor = 0, playerSpeed = 200.0f, bulletSpeed = 600.0f;
        Vector2 playerPos = new Vector2((screenWidth-70)/2, (screenHeight-70)/2);
        Vector2 playerCenter = new Vector2(playerPos.X + (70 / 2), playerPos.Y + (70 / 2));

        //game
        float lvl = 1f, towerHp = 100f;
        int HowToPlayLvl = 1;
        Vector2 cursorPos = Raylib.GetMousePosition();
        List<Bullet> bullets = new List<Bullet>();
        List<Enemy> enemies = new List<Enemy>();
        float textTimer = 0.1f;
        float FrameTime = Raylib.GetFrameTime();

        //code
        while (!Raylib.WindowShouldClose())
        {
            cursorPos = Raylib.GetMousePosition();
            FrameTime = Raylib.GetFrameTime();
            playerCenter = new Vector2(playerPos.X + (70 / 2), playerPos.Y + (70 / 2));

            //Code part
            switch (State)
            {
                case Gamestate.Welcome:
                    break;
                case Gamestate.HowToPlay:
                    movement();
                    if (HowToPlayLvl == 1 && (Raylib.IsKeyPressed(KeyboardKey.W) || Raylib.IsKeyPressed(KeyboardKey.A) || Raylib.IsKeyPressed(KeyboardKey.S) || Raylib.IsKeyPressed(KeyboardKey.D)))
                    {
                        HowToPlayLvl++;
                        textTimer = 1.0f;
                    }
                    if (HowToPlayLvl > 1)
                    {
                        shooting();
                    }
                    break;
            }



            //Draw part
            Raylib.BeginDrawing();
            switch (State)
            {


                //welcome screen
                case Gamestate.Welcome:
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawText("DEFENSIVA DE TOWER", (screenWidth - Raylib.MeasureText("DEFENSIVA DE TOWER", 60))/2, (screenHeight - 60)/2 - 300, 60, Color.SkyBlue);
                    Rectangle Play = new Rectangle((screenWidth-240)/2, (screenHeight-112)/2, 240, 112);
                    if (Raylib.CheckCollisionPointRec(cursorPos, Play))
                    {
                        Raylib.DrawRectangleRec(Play, Color.DarkGreen);
                        Raylib.DrawText("Play", (screenWidth - Raylib.MeasureText("Play", 44))/2, (screenHeight - 44)/2, 44, Color.White);
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            State = Gamestate.Upgrade;
                        }
                    }
                    else 
                    {
                        Raylib.DrawRectangleRec(Play, Color.Green);
                        Raylib.DrawText("Play", (screenWidth - Raylib.MeasureText("Play", 40))/2, (screenHeight - 40)/2, 40, Color.DarkGray);
                    }



                    Rectangle HowToPlay = new Rectangle((screenWidth-240)/2, (screenHeight-112)/2 + 130, 240, 112);
                    if (Raylib.CheckCollisionPointRec(cursorPos, HowToPlay))
                    {
                        Raylib.DrawRectangleRec(HowToPlay, Color.DarkGray);
                        Raylib.DrawText("How to play", (screenWidth - Raylib.MeasureText("How to play", 39))/2, (screenHeight - 39)/2 + 130, 39, Color.White);
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            State = Gamestate.HowToPlay;
                        }
                    }
                    else 
                    {
                        Raylib.DrawRectangleRec(HowToPlay, Color.LightGray);
                        Raylib.DrawText("How to play", (screenWidth - Raylib.MeasureText("How to play", 36))/2, (screenHeight - 36)/2 + 130, 36, Color.Black);
                    }



                    Rectangle Exit = new Rectangle((screenWidth-240)/2, (screenHeight-112)/2 + 260, 240, 112);
                    if (Raylib.CheckCollisionPointRec(cursorPos, Exit))
                    {
                        Raylib.DrawRectangleRec(Exit, DarkRed);
                        Raylib.DrawText("Exit", (screenWidth - Raylib.MeasureText("Exit", 46))/2, (screenHeight - 46)/2 + 260, 46, Color.SkyBlue);
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                        {
                            State = Gamestate.Exit;
                        }
                    }
                    else 
                    {
                        Raylib.DrawRectangleRec(Exit, Color.Red);
                        Raylib.DrawText("Exit", (screenWidth - Raylib.MeasureText("Exit", 40))/2, (screenHeight - 40)/2 + 260, 40, Color.White);
                    }

                    Vector2[] WelcomePath1 = new Vector2[] {new Vector2(150, 0), new Vector2(150, 200), new Vector2(300, 200), new Vector2(300, 520), new Vector2(100, 520), new Vector2(100, 800), new Vector2(200, 800), new Vector2(200, 1080)};
                    Raylib.DrawSplineLinear(WelcomePath1, WelcomePath1.Length, 50.0f, Color.DarkGray);
                    for (int i = 1; i < WelcomePath1.Length - 1; i++)
                    {
                        Raylib.DrawRectangleV(new Vector2(WelcomePath1[i].X - 50.0f / 2, WelcomePath1[i].Y - 50.0f / 2), new Vector2(50.0f, 50.0f), Color.DarkGray);
                    }


                    Vector2[] WelcomePath2 = new Vector2[] {new Vector2(1800, 0), new Vector2(1800, 200), new Vector2(1620, 200), new Vector2(1620, 460), new Vector2(1540, 460), new Vector2(1540, 320), new Vector2(1440, 320), new Vector2(1440, 760), new Vector2(1650, 760), new Vector2(1650, 1080)};
                    Raylib.DrawSplineLinear(WelcomePath2, WelcomePath2.Length, 50.0f, Color.DarkGray);
                    for (int i = 1; i < WelcomePath2.Length - 1; i++)
                    {
                        Raylib.DrawRectangleV(new Vector2(WelcomePath2[i].X - 50.0f / 2, WelcomePath2[i].Y - 50.0f / 2), new Vector2(50.0f, 50.0f), Color.DarkGray);
                    }

                    break;


                //How to play
                case Gamestate.HowToPlay:
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawRectangle((int)playerPos.X, (int)playerPos.Y, 70, 70, Color.Green);
                    foreach (var bullet in bullets)
                    {
                        Raylib.DrawRectangleRec(bullet.Rect, Color.DarkGreen);
                    }
                    switch(HowToPlayLvl)
                    {
                        case 1:
                            Raylib.DrawText("Movement: W, S, A, D", (screenWidth - Raylib.MeasureText("Movement: W, S, A, D", 50))/2, (screenHeight-50)/2 - 420, 50, Color.DarkGray);
                            break;
                        case 2:
                            if (textTimer > 0)
                            {
                               Raylib.DrawText("Movement: W, S, A, D", (screenWidth - Raylib.MeasureText("Movement: W, S, A, D", 50))/2, (screenHeight-50)/2 - 420, 50, Color.DarkGray);
                               textTimer -= FrameTime;
                            }
                            else if (textTimer <= 0)
                            {
                                Raylib.DrawText("Shooting: LMB", (screenWidth - Raylib.MeasureText("Shooting: LMB", 50))/2, (screenHeight-50)/2 - 420, 50, Color.DarkGray);
                            }
                            break;
                    }
                    break;


                //exit
                case Gamestate.Exit:
                    Raylib.CloseWindow();
                    break;


                //Upgrade 
                case Gamestate.Upgrade:
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawText("Not done bro.", (screenWidth - Raylib.MeasureText("Not done bro.", 40))/2, (screenHeight - 40)/2, 40, Color.White);
                    break;

            }
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();




        void movement()
        {
            if(Raylib.IsKeyDown(KeyboardKey.W)) {if(playerPos.Y > 0) {playerPos.Y -= playerSpeed * FrameTime;}}
            if(Raylib.IsKeyDown(KeyboardKey.S)) {if(playerPos.Y < screenHeight - 70) {playerPos.Y += playerSpeed * FrameTime;}}
            if(Raylib.IsKeyDown(KeyboardKey.A)) {if(playerPos.X > 0) {playerPos.X -= playerSpeed * FrameTime;}}
            if(Raylib.IsKeyDown(KeyboardKey.D)) {if(playerPos.X < screenWidth - 70) {playerPos.X += playerSpeed * FrameTime;}}
        }
        void shooting()
        {
            if(Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
            Vector2 BulletDirection = Vector2.Normalize(cursorPos - playerCenter);
            bullets.Add(new Bullet {Rect = new Rectangle(playerCenter.X - 5, playerCenter.Y - 5, 10, 10), Velocity = BulletDirection * bulletSpeed});
            }
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                Bullet bullet = bullets[i];
                bullet.Rect.X += bullet.Velocity.X * FrameTime; 
                bullet.Rect.Y += bullet.Velocity.Y * FrameTime;
                bullets[i] = bullet;
            }  
        }
        void enemySpawn()
        {
            Random rand = new Random();
            int spawnChance = rand.Next(0, 100);
            if (spawnChance < 10)
            {
                Enemy newEnemy = new Enemy {Rect = new Rectangle(0, 0, 50, 50), enemyLvl = lvl, enemyHp = 100f * (float)Math.Sqrt(lvl), enemyDmg = 10f * (float)Math.Sqrt(lvl), enemySpd = 200f};
                enemies.Add(newEnemy);
            }
        }
        void pathGeneration()
        {
            Random rand1 = new Random();
            Random rand2 = new Random();
            List<Vector2> path = new List<Vector2>();
            for (int i = 1; i < path.Count - 1; i++)
            {
                Raylib.DrawRectangleV(new Vector2(path[i].X - 50.0f / 2, path[i].Y - 50.0f / 2), new Vector2(50.0f, 50.0f), Color.DarkGray);
            }
        }
    }
}